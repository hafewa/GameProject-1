﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

class DesplayController_OverheadPanel : I_DesplayController
{
    static int dyDepth = 5000000;
    public DesplayController_OverheadPanel(
        Camera eyeCamera,
        Camera uiCamera,
        PosTrack2DManage m_PosTrack2DManage,
        OverheadPanelResources res, 
        GameObject gameObject
        )
    {
        this.gameObject = gameObject;

        m_PosTrack2D = new PosTrack2D();
        m_PosTrack2D.Obj2D = gameObject;

        float dirx; Transform tr; DP_BattlefieldDraw.Single.GetActorTransfrom(res.HeroActorID, out tr, out dirx);
        m_PosTrack2D.Obj3D = tr.gameObject;
        m_PosTrack2D.YOffset3D = 20f;
        m_PosTrack2D.Camera3D = eyeCamera;
        m_PosTrack2D.Camera2D = uiCamera;
        m_PosTrack2DManage.Add(m_PosTrack2D);

        deadObj = gameObject.transform.FindChild("dead").gameObject;



        jian_num_sObj = gameObject.transform.FindChild("jian_num_s").gameObject;

        jian_numObj = gameObject.transform.FindChild("jian_num").gameObject;
        jia_numObj = gameObject.transform.FindChild("jia_num").gameObject;

        shoudongTxt = gameObject.transform.FindChild("shoudong_txt").gameObject;
        texingTxt = gameObject.transform.FindChild("texing_txt").gameObject;
        bishaTxt = gameObject.transform.FindChild("bisha_txt").gameObject;

        shoudongTxtTweens = shoudongTxt.GetComponents<UITweener>();
        texingTxtTweens = texingTxt.GetComponents<UITweener>();
        bishaTxtTweens = bishaTxt.GetComponents<UITweener>();
        shoudongTxtLabel = shoudongTxt.GetComponent<UILabel>();
        texingTxtLabel = texingTxt.GetComponent<UILabel>();
        bishaTxtLabel = bishaTxt.GetComponent<UILabel>();


        m_HeadObj = gameObject.transform.FindChild("head").gameObject;
        m_JtObj = gameObject.transform.FindChild("JT").gameObject;

        //设置头像
        //var packFight = PacketManage.Single.GetPacket("ui_fight");
        //var atlasGObj = packFight.Load("EagleEyeAtlas.prefab") as GameObject;
        //headSprite = m_HeadObj.GetComponent<UISprite>();
        //LuaHeroInfoLibs.SetHeroIconCirclemask(headSprite, SData_Hero.Get(res.DataID));
        //headSprite.depth = dyDepth++;
        /*

        headSprite.atlas = atlasGObj.GetComponent<UIAtlas>();
        headSprite.spriteName = SData_Hero.Get(res.DataID).HeroFace;
        headSprite.depth = dyDepth++;
        */
        //设置箭头外观
        jtSprite = m_JtObj.GetComponent<UISprite>();
        jtSprite.spriteName = res.IsAttack ? "QL" : "QR";
        //jtSprite.depth = dyDepth++;

        jian_numObj.SetActive(false);
        jia_numObj.SetActive(false);
        jian_num_sObj.SetActive(false);

        shoudongTxt.SetActive(false);
        texingTxt.SetActive(false);
        bishaTxt.SetActive(false);


        //临时屏蔽头像和箭头
        /*{
            m_HeadObj.SetActive(false);
            m_JtObj.SetActive(false);
        }
        */




        m_Res = res;

        DepthOffset = DP_BattlefieldDraw.Single.m_UIDepthOffset;
        DP_BattlefieldDraw.Single.m_UIDepthOffset += 2;


        GameObject hpObj = gameObject.transform.FindChild("hp").gameObject;
        GameObject hpEnemy = gameObject.transform.FindChild("hp_enemy").gameObject;
        hpObj.SetActive(m_Res.IsAttack);
        hpEnemy.SetActive(!m_Res.IsAttack);

        hpBar = hpObj.GetComponent<UIProgressBar>();
        var hpEnemyBar = hpEnemy.GetComponent<UIProgressBar>();
        hpBar.alpha = 1;
        hpEnemyBar.alpha = 1;
        GameObject hp_mpObj;

        if (m_Res.IsAttack)
        { 
            wnd_fight.Single.LeftHeroPanel.CreateHero(m_Res.Fid, m_Res.DataID, m_Res.HeroActorID, m_Res.xj);

            deadTweeners = hpObj.GetComponents<UITweener>();


            hp_mpObj = hpObj.transform.FindChild("mg").gameObject;



        }
        else
        {
            hpBar = hpEnemyBar; 
            wnd_fight.Single.RightHeroPanel.CreateHero(m_Res.Fid, m_Res.DataID, m_Res.HeroActorID);

            deadTweeners = hpEnemy.GetComponents<UITweener>();

            hp_mpObj = hpEnemy.transform.FindChild("mg").gameObject;
        }

        hpmpBar = hp_mpObj.GetComponent<UIProgressBar>();
        hpmpBar.alpha = 1;
        HeadDeadTweeners = m_HeadObj.GetComponents<UITweener>();

        {
            var widgets = hpBar.GetComponentsInChildren<UIWidget>();
            var depth = hpBar.backgroundWidget.depth;
            foreach (var curr in widgets)
                hpWidgets.Add(new KeyValuePair<UIWidget, int>(curr, curr.depth - depth));
        }

        {
            var widgets = m_HeadObj.GetComponentsInChildren<UIWidget>();
            var depth = headSprite.depth;
            foreach (var curr in widgets)
                hpWidgets.Add(new KeyValuePair<UIWidget, int>(curr, curr.depth - depth));
        }
    }

    List<KeyValuePair<UIWidget, int>> hpWidgets = new List<KeyValuePair<UIWidget,int>>();
 
   public void Update(float lostTime)
   {
       /*
    if (HPHideTime > 0)
    {
        HPHideTime -= lostTime;
        if (HPHideTime <= 0)
        {
            //隐藏血条
            if (tw != null) tw.Kill();
            tw = DOTween.To(() => hpBar.alpha, (a) => hpmpBar.alpha=  hpBar.alpha = a, 0, 0.3f)
                .SetEase(Ease.Linear)//线性变化
                .SetAutoKill(true)//渐隐
                .OnKill(() => tw = null);
        }
    } */
   }

    public void OnEnable() { }
    public void SetBrightness(float v) { }

    int DepthOffset;
    public void OnUISet3DPos(Vector3 pos3d)
    {
        float z = pos3d.z;
        //if (!QKMath.Equals(z, lastz))
        {
            //lastz = z;
            var distance = Vector3.Distance(DP_BattlefieldDraw.Single.EyeCamera.transform.position, pos3d);
            int dp = (int)(1000000f - distance * 10f + DepthOffset);
            //var dp = (int)(1000000 - z * 10 + DepthOffset);
            //headSprite.depth = dp;//头像深度

            //控件深度
            {
                var len = hpWidgets.Count;
                for (int i = 0; i < len; i++)
                {
                    var kv = hpWidgets[i];
                    kv.Key.depth = dp + kv.Value;
                }
            }

            //头像箭头深度
            //jtSprite.depth = dp + 1;
        }

        //m_PosTrack2D.Pos = pos3d;

        //抛出英雄移动事件
        //DP_FightEvent.PostHeroMove(m_Res.IsAttack, m_Res.DataID, pos3d);
    }

    public void OnKeyChanged(DP_Key newKey)
    {

        switch (newKey.KeyType)
        {
            case DPKeyType.ShoudongIcon://手动技能图标激活
                {
                    var hKey = newKey as DPKey_ShoudongIcon;
                    if (m_Res.IsAttack) wnd_fight.Single.LeftHeroPanel.ActiveShoudongFID(  m_Res.Fid);
                    m_ActiveShoudongCount++;

                    //抛出手动激活事件
                    DP_FightEvent.PostShoudongActive(m_Res.DataID, m_ActiveShoudongCount);
                }
                break;
            case DPKeyType.CD://CD自增
                {
                    var hKey = newKey as DPKey_CD;
                     if (m_Res.IsAttack)
                         wnd_fight.Single.LeftHeroPanel.CDChangeFID(  m_Res.Fid, hKey.StartBfb, hKey.len);
                    else
                         wnd_fight.Single.RightHeroPanel.CDChangeFID(m_Res.Fid, hKey.StartBfb, hKey.len);

                }
                break;
            case DPKeyType.SoldiersCountChange://士兵数量变化
                {
                    var hKey = newKey as DPKey_SoldiersCountChange;

                    //更新界面中的血量
                    if (m_Res.IsAttack)
                        wnd_fight.Single.LeftHeroPanel.SoldiersCountChangeFID(  m_Res.Fid, hKey.num);
                    else
                        wnd_fight.Single.RightHeroPanel.SoldiersCountChangeFID(m_Res.Fid, hKey.num);

                    //twSoldiersCount
                }
                break;
            case DPKeyType.HPChange://生命改变
                {
                    const float time = 1.5f;
                    const float scale = 1f;
                    DPKey_HPChange hKey = newKey as DPKey_HPChange;

                    if (hKey.lostHP > 0) //减血
                    {
                        var iobj = hKey.isHeroHit ? jian_numObj : jian_num_sObj;

                        GameObject newObj = GameObject.Instantiate(iobj);
                        var label = newObj.GetComponent<UILabel>();
                        label.text = "-" + hKey.lostHP.ToString();
                        label.depth = dyDepth++;

                        newObj.transform.parent = iobj.transform.parent;
                        newObj.transform.localPosition = iobj.transform.localPosition;
                        newObj.transform.localRotation = iobj.transform.localRotation;
                        newObj.transform.localScale = iobj.transform.localScale;// new Vector3(0.5f, 0.5f, 0.5f);

                        newObj.SetActive(true);
                        NGUITools.MarkParentAsChanged(newObj);
                        GameObject.Destroy(newObj, 2);
                        //渐变消失
                        /*
                        newObj.transform.DOLocalMoveY(newObj.transform.localPosition.y + label.height, time).SetAutoKill(true).SetEase(Ease.OutQuart);
                        newObj.transform.DOScale(scale, time).SetAutoKill(true);
                        DOTween.To(() => label.alpha, (a) => label.alpha = a, 0, time).SetAutoKill(true).SetEase(Ease.Linear)
                            .OnKill(() =>
                            {
                                GameObject.Destroy(newObj);
                            });*/
                    }
                    else if (hKey.lostHP < 0) //加血
                    {
                        GameObject newObj = GameObject.Instantiate(jia_numObj);
                        var label = newObj.GetComponent<UILabel>();
                        label.text = "+" + (-hKey.lostHP).ToString();
                        label.depth = dyDepth++;

                        newObj.transform.parent = jia_numObj.transform.parent;
                        newObj.transform.localPosition = jia_numObj.transform.localPosition;
                        newObj.transform.localRotation = jia_numObj.transform.localRotation;
                        newObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        //newObj.transform.parent = gameObject.transform.parent;

                        newObj.SetActive(true);
                        NGUITools.MarkParentAsChanged(newObj);

                        GameObject.Destroy(newObj, 2);
                        /*
                        //渐变消失
                        newObj.transform.DOLocalMoveY(newObj.transform.localPosition.y + label.height, time).SetAutoKill(true).SetEase(Ease.OutQuart);
                        newObj.transform.DOScale(scale, time).SetAutoKill(true);
                        DOTween.To(() => label.alpha, (a) => label.alpha = a, 0, time).SetAutoKill(true).SetEase(Ease.Linear)
                            .OnKill(() => GameObject.Destroy(newObj));*/
                    }

                    //更新界面中的血量
                    if (m_Res.IsAttack)
                        wnd_fight.Single.LeftHeroPanel.HPChangeFID(m_Res.Fid, hKey.hpBfb);
                    else
                        wnd_fight.Single.RightHeroPanel.HPChangeFID(m_Res.Fid, hKey.hpBfb);

                    if (hKey.hpBfb < QKMath.PRECISION)//血量已经没有了
                    {
                        //隐藏头顶箭头和头顶头像
                        //m_HeadObj.SetActive(false);
                        //m_JtObj.SetActive(false);
                        //gameObject.SetActive(false);//隐藏整个面板 飘血效果也会被隐藏

                        hpBar.gameObject.SetActive(false);
                        hpmpBar.gameObject.SetActive(false);
                        if (tw != null) tw.Kill();
                        HPHideTime = 0;

                        //处理死亡表现
                        {
                            deadObj.SetActive(true);
                            foreach (var tweener in HeadDeadTweeners)
                                tweener.PlayForward();

                            foreach (var tweener in deadTweeners)
                                tweener.PlayForward();
                        }

                        //播放杀敌界面特效
                        if(!m_Res.IsAttack) wnd_fight.Single.AddKillHero();
                        

                        //取消对本角色的镜头追踪
                        DP_CameraTrackObjectManage.Single.CancelTrackActor(m_Res.HeroActorID);

                        //抛出英雄死亡事件
                        DP_FightEvent.PostHeroDie(m_Res.DataID, m_Res.IsAttack ? ArmyFlag.Attacker : ArmyFlag.Defender);
                    }


                    //更改头顶血槽值
                    if (twValue != null) twValue.Kill();
                    twValue = DOTween.To(() => hpBar.value, (a) => hpBar.value = a, hKey.hpBfb, 0.3f)
                       .SetEase(Ease.OutQuart)
                       .SetAutoKill(true)
                       .OnKill(() => twValue = null);

                    /*
                    if (hKey.lostHP != 0)
                    {
                        //显示头顶血条
                        if (tw != null) tw.Kill();
                        tw = DOTween.To(() => hpBar.alpha, (a) => hpmpBar.alpha = hpBar.alpha = a, 1, 0.3f)
                            .SetEase(Ease.Linear)//线性变化
                            .SetAutoKill(true)//渐显
                            .OnKill(() => tw = null);

                        //更新头顶血条显示时间
                        HPHideTime = SData_FightKeyValue.Single.BloodSpan;
                    }*/
                }
                break;
            case DPKeyType.PopupText://弹出文本
                {
                    var hKey = newKey as DPKey_PopupText;

                    GameObject txtObj;
                    UITweener[] tweens;
                    UILabel label;

                    if(hKey.txtType == DPKey_PopupText.textType.Bisha)
                    {
                        txtObj = bishaTxt;
                        tweens = bishaTxtTweens;
                        label = bishaTxtLabel;
                    }else if(hKey.txtType == DPKey_PopupText.textType.Texing)
                    {
                        txtObj = texingTxt;
                        tweens = texingTxtTweens;
                        label = texingTxtLabel;
                    }
                    else
                    {
                        txtObj = shoudongTxt;
                        tweens = shoudongTxtTweens;
                        label = shoudongTxtLabel;
                    }

                    label.text = hKey.text;
                    txtObj.SetActive(true);
                    foreach(var curr in tweens)
                    {
                        curr.ResetToBeginning();
                        curr.PlayForward();
                    } 
                }
                break;
            case DPKeyType.BuffIcon://buff状态图标变化
                {
                    var hKey = newKey as DPKey_BuffIcon;

                    if (m_Res.IsAttack)
                        wnd_fight.Single.LeftHeroPanel.BuffIconChangeFID(m_Res.Fid,hKey.bid ,hKey.icon);
                    else
                        wnd_fight.Single.RightHeroPanel.BuffIconChangeFID(m_Res.Fid, hKey.bid, hKey.icon);
                }
                break;
        }

    }

    public void SetAlpha(float v) { hpBar.alpha = v; hpmpBar.alpha = v; }
    public float DirX { get { return 0; } }
    public void Destroy()
    {
        if (tw != null) { tw.Kill(); tw = null; }
        if (twValue != null) { twValue.Kill(); twValue = null; }
        // if (twSoldiersCount != null) { twSoldiersCount.Kill(); twSoldiersCount = null; }
    }

    //float lastz = -999999999;
    
    GameObject gameObject;
    UIProgressBar hpBar, hpmpBar;
    GameObject jian_numObj;
    GameObject jian_num_sObj;
    GameObject jia_numObj;
    GameObject deadObj;
    UITweener[] deadTweeners;
    UITweener[] HeadDeadTweeners;
    PosTrack2D m_PosTrack2D;

    UITweener[] shoudongTxtTweens;
    UITweener[] texingTxtTweens;
    UITweener[] bishaTxtTweens;
    GameObject shoudongTxt;
    GameObject  texingTxt;
    GameObject  bishaTxt;
    UILabel shoudongTxtLabel; 
    UILabel  texingTxtLabel ; 
    UILabel bishaTxtLabel ;

    float HPHideTime = 0;
    Tweener tw = null;
    Tweener twValue = null;
    OverheadPanelResources m_Res;
    UISprite jtSprite;
    UISprite headSprite;
    GameObject m_HeadObj;
    GameObject m_JtObj;
    int m_ActiveShoudongCount = 0;//手动激活总次数
}