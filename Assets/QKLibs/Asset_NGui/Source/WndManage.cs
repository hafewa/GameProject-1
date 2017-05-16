﻿using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine;

public class wndInfo : ICloneable
{
    public string name;
    public List<string> dependPackets = null;
    public WndFadeMode fade;
    public WndAnimationMode animaMode;
    public bool isVisible = false;//当前是否处于显示状态
    public int cacheTime;
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public class wndShowHideInfo
{
    public string name;
    //public bool needVisible = false;//逻辑当前是希望它显示还是隐藏
    public WShowType needVisible = WShowType.hide;//逻辑当前是希望它显示还是隐藏,还是预加载
    public float duration;//延迟
    public bool isWithBg;
}
/*
class SafeShowing:IDisposable 
{
    public SafeShowing(string wndName)
    {
        m_wndName = wndName;
        WndManage.Single.__showing.Add(wndName);
          
    }

    public void Dispose()
    { 
        WndManage.Single.__showing.Remove(m_wndName); 
    }

    string m_wndName;
}*/
public enum WShowType
{
    hide,
    show,
    preLoad,
    destroy,
}
/// <summary>
/// 窗体的管理类 当前实现了窗体注册、卸载、获取依赖等功能点
/// </summary>
public class WndManage
{

    static WndManage _Single = null;
    // public HashSet<string> __showing = new HashSet<string>();//正在显示过程中的窗体
    Dictionary<string, DateTime> m_wndLastHideTime = new Dictionary<string, DateTime>();

    public delegate void Evt_WndDestroy(Wnd wndObj);
    public Evt_WndDestroy OnWndDestroy = null;
    public QKEvent OnWndOpen = new QKEvent();
    public static WndManage Single
    {
        get
        {
            if (_Single == null) _Single = new WndManage();
            return _Single;
        }
    }

    public readonly GameObject UIRootObj = null;

    //强制清除已经被隐藏的窗体，以立即回收内存
    public void DestroyHideWnds()
    {
        foreach (KeyValuePair<string, DateTime> curr in m_wndLastHideTime)
        {
            string wndname = curr.Key;
            Wnd wnd = m_wndInstances[wndname];
            DestroyWnd(wnd);
        }
        m_wndLastHideTime.Clear();
    }

    private WndManage()
    {
        UIRootObj = UnityEngine.GameObject.Find("/UIRoot");


        // QKEvent Event_CheckRedundantUI = new QKEvent();
        //  Event_CheckRedundantUI.AddCallback(new SharpEventCallback(CheckRedundantUI_CallBack));

        //定时检查并卸载时间过长未使用的ui
        new MonoEX.Timer(15.0f).Play().OnComplete(CheckRedundantUI_CallBack);
    }

    void CheckRedundantUI_CallBack()
    {
        DateTime now = DateTime.Now;
        //检查冗余窗体
        Dictionary<string, DateTime> newList = new Dictionary<string, DateTime>();
        foreach (KeyValuePair<string, DateTime> curr in m_wndLastHideTime)
        {
            string wndname = curr.Key;
            Wnd wnd = m_wndInstances[wndname];
            wndInfo wInfo = m_wndInfos[wndname];
            if (
                (now - curr.Value).TotalSeconds > wInfo.cacheTime//超时
                )
                DestroyWnd(wnd);
            else
                newList.Add(curr.Key, curr.Value);
        }
        m_wndLastHideTime = newList;

        //继续定时
        {
            new MonoEX.Timer(15.0f).Play().OnComplete(CheckRedundantUI_CallBack);
        }
    }


    public void DestroyWnd(Wnd wnd)
    {
        if (OnWndDestroy != null)
        {
            try
            {
                OnWndDestroy(wnd);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        wnd.Dispose();//卸载窗体对象
        m_wndInstances.Remove(wnd.Name);//从管理器移除引用

        //引用计数减少
        {
            wndInfo wInfo = m_wndInfos[wnd.Name];
            foreach (string packet in wInfo.dependPackets)
                ResourceRefManage.Single.SubRef(packet);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="wndName"></param>
    /// <param name="dependPackets"></param>
    /// <param name="sort">显示排序</param>
    /// <param name="cacheTime">缓存时间，从隐藏开始算，单位秒，如果值小于1表示常注</param>
    /// <param name="fade"></param>
    public void RegWnd(string wndName, string dependPackets, int cacheTime, WndFadeMode fade, WndAnimationMode animaMode)
    {
        if (m_wndInfos.ContainsKey(wndName))
            m_wndInfos.Remove(wndName);

        List<string> packlist = new List<string>();
        string[] packs = dependPackets.Split(';');

        packlist.Add("packets");
        foreach (string curr in packs) packlist.Add(curr);
        m_wndInfos.Add(
            wndName,
            new wndInfo() { name = wndName, dependPackets = packlist, fade = fade, animaMode = animaMode, cacheTime = cacheTime }
        );
    }

    public void RegWnd1(string wndName, string dependPackets, int cacheTime, int fade, int animaMode)
    {
        RegWnd(wndName, dependPackets, cacheTime, (WndFadeMode)fade, (WndAnimationMode)animaMode);
    }


    /// <summary>
    /// 获取窗体依赖包
    /// </summary>
    /// <param name="wndName"></param>
    /// <returns></returns>
    public HashSet<string> GetDependPackets(List<string> wndNames)
    {
        HashSet<string> re = new HashSet<string>();
        foreach (string wndName in wndNames)
        {
            if (!m_wndInfos.ContainsKey(wndName)) continue;

            wndInfo wInfo = m_wndInfos[wndName];
            foreach (string packet in wInfo.dependPackets)
            {
                if (!re.Contains(packet)) re.Add(packet);
            }
        }
        return re;
    }

    IEnumerator LoadDepend(string wndName)
    {
        if (!m_wndInfos.ContainsKey(wndName)) throw new Exception();

        wndInfo wInfo = m_wndInfos[wndName];
        //加载界面所需资源包
        PacketLoader packloader = new PacketLoader();
        packloader.Start(PackType.Res, wInfo.dependPackets, null);

        //等待包装载完成
        while (packloader.Wait().MoveNext()) { yield return null; }

        //引用计数增加
        foreach (string packet in wInfo.dependPackets)
        {
            ResourceRefManage.Single.AddRef(packet);
        }

    }

    public void ShowWnd(string wndName, float duration, bool isWithBg)
    {
        var wInfo = new wndShowHideInfo();
        wInfo.name = wndName;
        wInfo.needVisible = WShowType.show;
        wInfo.duration = duration;
        wInfo.isWithBg = isWithBg;
        DoCmd(wInfo);
    }

    public void HideWnd(string wndName, float duration)
    {
        var wInfo = new wndShowHideInfo();
        wInfo.name = wndName;
        wInfo.needVisible = WShowType.hide;
        wInfo.duration = duration;

        DoCmd(wInfo);
    }
    public void DestroyWnd(string wndName, float duration)
    {
        var wInfo = new wndShowHideInfo();
        wInfo.name = wndName;
        wInfo.needVisible = WShowType.destroy;
        wInfo.duration = duration;

        DoCmd(wInfo);
    }
    public void PreLoadDepend(string wndName)
    {
        var wInfo = new wndShowHideInfo();
        wInfo.name = wndName;
        wInfo.needVisible = WShowType.preLoad;

        DoCmd(wInfo);
    }
    void DoCmd(wndShowHideInfo wInfo)
    {
        m_Cmds.Add(wInfo);
        if (!m_coIsRuning)
        {
            m_coIsRuning = true;
            MonoEX.CoroutineManage.Single.StartCoroutine(coDoCmd());
        }
    }

    IEnumerator coDoCmd()
    {
        while (m_Cmds.Count > 0)
        {
            var aInfo = m_Cmds[0];
            m_Cmds.RemoveAt(0);

            var wndName = aInfo.name;



            var subID = "";
            string[] sArray = aInfo.name.Split('&');
            if (sArray.Length == 2)
            {
                wndName = sArray[0];
                subID = '&' + sArray[1];
            }
            if (!m_wndInfos.ContainsKey(aInfo.name))
            {
                if (sArray.Length == 2)
                {
                    wndInfo wInfo2 = (wndInfo)m_wndInfos[wndName].Clone();
                    wInfo2.name = aInfo.name;
                    m_wndInfos.Add(wInfo2.name, wInfo2);
                }
                else
                {
                    Debug.LogError("窗体注册信息不存在 " + wndName);
                    continue;
                }
            }

            if (aInfo.needVisible != WShowType.hide && aInfo.needVisible != WShowType.destroy)
            {

                //从最近隐藏记录中清除
                if (m_wndLastHideTime.ContainsKey(wndName + subID))
                    m_wndLastHideTime.Remove(wndName + subID);

                //窗体不存在，则创建
                if (!m_wndInstances.ContainsKey(wndName + subID))
                {
                    IEnumerator it = LoadDepend(wndName);

                    while (it.MoveNext()) yield return null;

                    wndInfo wInfo = m_wndInfos[wndName + subID];

                    //创建一个uipanel
                    GameObject uipanel = new GameObject(wInfo.name + "_panel");
                    uipanel.transform.parent = UIRootObj.transform;
                    uipanel.transform.localScale = new Vector3(1, 1, 1);
                    uipanel.transform.localRotation = new Quaternion(0, 0, 0, 1);
                    uipanel.transform.localPosition = new Vector3(0, 0, 0);
                    uipanel.layer = LayerMask.NameToLayer("UI");//设置层


                    UIPanel cmpanel = uipanel.AddComponent<UIPanel>();
                    GameObject wnd_Obj = GameObjectExtension.InstantiateFromPacket(wInfo.dependPackets[1], wndName + ".prefab", uipanel);
                    if (wnd_Obj == null)
                    {
                        //删除刚创建的uipanel
                        GameObject.Destroy(uipanel);
                        Debug.LogError(String.Format("实例化窗体错误， packet:{0} wndName:{1}", wInfo.dependPackets[1], wInfo.name));
                        throw new Exception();
                    }


                    //设置新创建的panel锚点
                    {
                        UIRect rectCM = uipanel.GetComponent<UIRect>();
                        rectCM.SetAnchor(UIRootObj, 0, 0, 0, 0);
                        rectCM.updateAnchors = UIRect.AnchorUpdate.OnStart;
                    }

                    //设置预置锚点
                    const int safev = 1;
                    {
                        UIRect rectCM = wnd_Obj.GetComponent<UIRect>();
                        rectCM.SetAnchor(uipanel, -safev, -safev, safev, safev);
                        rectCM.updateAnchors = UIRect.AnchorUpdate.OnStart;
                    }

                    //创建挡板

                    GameObject uibaffle;
                    {
                        uibaffle = new GameObject(wInfo.name + "_baffle");
                        uibaffle.layer = LayerMask.NameToLayer("UI");//设置层 
                        uibaffle.transform.parent = uipanel.transform;
                        uibaffle.transform.localScale = Vector3.one;
                        uibaffle.transform.localRotation = Quaternion.identity;
                        uibaffle.transform.localPosition = Vector3.zero;



                        //增加碰撞盒
                        var cl = uibaffle.AddComponent<BoxCollider>();

                        //增加UIWidget组件
                        var cmBaffleWidget = uibaffle.AddComponent<UIWidget>();
                        cl.isTrigger = true;
                        cmBaffleWidget.autoResizeBoxCollider = true;
                        cmBaffleWidget.updateAnchors = UIRect.AnchorUpdate.OnStart;
                        cmBaffleWidget.SetAnchor(uipanel, -safev, -safev, safev, safev);//设置锚点  
                        cmBaffleWidget.depth = -99;

                    }

                    wnd_Obj.name = wndName + subID;
                    wnd_Obj.SetActive(true);
                    uipanel.SetActive(false);
                    m_wndInstances.Add(wndName + subID, new Wnd(uipanel, uibaffle, wInfo));
                    if (aInfo.isWithBg)//再加一个gameobject的原因是如果做动画底板需要层级高于预制，底板会压在预制上
                    {
                        //TODODO 图片放到了Resources目录
                        UITexture ut = NGUITools.AddWidget<UITexture>(uipanel, -99);
                        Texture texure = Resources.Load<Texture>("zanting_jiashenceng");
                        ut.mainTexture = texure;
                        UIStretch stretch = ut.gameObject.AddComponent<UIStretch>();
                        stretch.style = UIStretch.Style.Both;
                        // set relative size bigger
                        stretch.relativeSize = new Vector2(3, 3);
                    }
                }

                //显示
                Wnd wnd = m_wndInstances[wndName + subID];
                if (aInfo.needVisible == WShowType.preLoad)
                {
                    yield return null;
                    Wnd.OnPreLoadFinish.Call(wnd);
                }
                else if (aInfo.needVisible == WShowType.show)
                {
                    wnd._Show(aInfo.duration);
                    //等待窗体组件准备就绪
                    yield return null;
                    Wnd.OnShowFinish.Call(wnd);
                    if (OnWndOpen != null)
                        OnWndOpen.Call(wndName);
                }
            }
            else
            {
                if (m_wndInstances.ContainsKey(wndName + subID))
                {
                    //从最近隐藏记录中清除
                    if (aInfo.needVisible == WShowType.destroy && m_wndLastHideTime.ContainsKey(wndName + subID))
                        m_wndLastHideTime.Remove(wndName + subID);
                    Wnd wnd = m_wndInstances[wndName + subID];
                    wnd._Hide(aInfo.duration, aInfo.needVisible);
                    Wnd.OnDestroyFinish.Call(wnd);
                }

            }
            yield return null;
        }

        m_coIsRuning = false;
    }

    bool m_coIsRuning = false;
    List<wndShowHideInfo> m_Cmds = new List<wndShowHideInfo>();
    public Wnd _GetWnd(string wndName)
    {
        if (!m_wndInstances.ContainsKey(wndName)) return null;

        return m_wndInstances[wndName];
    }

    public void _OnWndHide(string wndName)
    {
        if (m_wndInfos[wndName].cacheTime > 0)//这是一个需要定时回收资源的界面
        {
            //记录最近隐藏时间
            if (m_wndLastHideTime.ContainsKey(wndName))
                m_wndLastHideTime[wndName] = DateTime.Now;
            else
                m_wndLastHideTime.Add(wndName, DateTime.Now);
        }
    }

    public static int LoadMainBaseActors()
    {
        //in FightParameter[,talkRecallClass,talkRecallFunc]
        //if (FightLoading) return 0;

        //var n = lua.GetTop();
        FightParameter param = new FightParameter();

        param.QixiSquare = null;
        param.sceneID = 1;
        param.tuiguan_zhang = 1;
        param.tuiguan_jie = 1;
        param.fightType = FightType.Tuiguan;

        param.Squares = new List<ArmySquareInfo>();
        CoroutineManage.Single.StartCoroutine(LoadMainSceneActor(param));
        return 0;
    }
    private static IEnumerator LoadMainSceneActor(FightParameter param)
    {
        ////显示战斗装载界面
        //wnd_prefight.Single.Show(Wnd.DefaultDuration);

        //停顿一段时间，等界面显示完全
        {
            float t = Wnd.DefaultDuration;
            while (t > 0)
            {
                yield return null;
                t -= Time.deltaTime;
            }
        }


        //立即回收被隐藏的窗口
        WndManage.Single.DestroyHideWnds();

        //清理战场
        DP_Battlefield.Single.Reset();

        //卸掉场景
        DP_Battlefield.Single.SwapScene(0, null, null);

        //立即垃圾回收
        //GC.Collect();
        Resources.UnloadUnusedAssets();

        //装载资源包
        List<string> dyPacks = AI_Single.Single.Battlefield.GeneratePackList();

        ////显示战斗界面(初始化所有控件，并没有真正显示出来)
        //wnd_fight.Single.Show(1); //Wnd.DefaultDuration
        //wnd_scene.Single.Show(1);

        //while (
        //    !wnd_fight.Single.IsInitd || //等待鹰眼图初始化完毕
        //    !wnd_scene.Single.IsInitd //等待场景中的界面初始化完成
        //    ) yield return null;


        //装载场景
        bool loadDone = false;
        DP_Battlefield.Single.SwapScene(param.sceneID, dyPacks, () => loadDone = true);
        while (!loadDone) yield return null; //等待场景装载完成


        //重新装载3D物体预置
        DP_FightPrefabManage.ReLoad3DObjects();


        //隐藏loading
        //wnd_prefight.Single.Hide(Wnd.DefaultDuration);
        //BackgroundMusic = param.Music;

        LuaFunction main = LuaClient.GetMainState().GetFunction("Onfs");
        main.Call();
        main.Dispose();
        main = null;
    }


    public void LogicInit_Go()
    {
        CoroutineManage.Single.StartCoroutine(LogicInit.InitLogic());
    }
    public float LogicInit_GetInitProgress()
    {
        return LogicInit.LogicInitProgress;
    }



    Dictionary<string, Wnd> m_wndInstances = new Dictionary<string, Wnd>();
    Dictionary<string, wndInfo> m_wndInfos = new Dictionary<string, wndInfo>();
}
