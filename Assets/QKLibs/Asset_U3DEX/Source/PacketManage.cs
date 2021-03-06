﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections;
/*
public class UObject  {
    public UObject(UnityEngine.Object obj)
    {
        m_Obj = obj;
    }

    public override bool Equals(object o)
    {
        UObject otherObj = o as UObject;
        if (otherObj != null)
        {
            return otherObj.m_Obj == m_Obj;
        }

        return (object)m_Obj == o;
    }

    public override int GetHashCode()
    {
        return m_Obj.GetHashCode();
    }

    public static implicit operator UnityEngine.Object(UObject obj)
    {
        return obj == null ? null : obj.m_Obj;
    }

    public static bool operator !=(UObject x, UObject y)
    {
        if ((System.Object)x == null) { return (System.Object)y != null; }
        else if (y == null) return true;
        return x.m_Obj != y.m_Obj;
    }

    public static bool operator ==(UObject x, UObject y)
    {
        if ((System.Object)x == null) { return (System.Object)y == null; }
        else if (y == null) return false;
        return x.m_Obj == y.m_Obj;
    }

    UnityEngine.Object m_Obj;
}
*/


//包类型
public enum PackType
{
    Res = 0,//资源
    Script = 1//脚本
}



//抽象资源包
public interface IPacket
{
    //从包里加载资源
    UnityEngine.Object Load(string path);

    //卸载
    void UnLoad();

    //资源包所在位置
    FileSystem.RES_LOCATION ResLocation { get; }
}

//资源包
public class Packet_Bundle : IPacket
{
    public Packet_Bundle(string name, AssetBundle bundle, FileSystem.RES_LOCATION resLocation)
    {
        /*
        using (StreamWriter sw = new StreamWriter("d:/qkwork/tmp.log", true, Encoding.UTF8))
        {
            sw.Write(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + "装完资源包 " + name + "\r\n");
        }
        */

        m_Name = name;
        m_bundle = bundle;
        m_ResLocation = resLocation;


    }

    public UnityEngine.Object Load(string path)
    {
        return m_bundle.LoadAsset(path);
    }

    public void UnLoad()
    {
        /*
        using (StreamWriter sw = new StreamWriter("d:/qkwork/tmp.log", true, Encoding.UTF8))
        {
            sw.Write(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + "卸载资源包 " + m_Name+"\r\n");
        }*/


        m_bundle.Unload(true);
    }

    public FileSystem.RES_LOCATION ResLocation { get { return m_ResLocation; } }
    AssetBundle m_bundle;
    FileSystem.RES_LOCATION m_ResLocation;
    string m_Name;
}


public class BinAsset : UnityEngine.Object
{
    public BinAsset(byte[] bytes)
    {
        m_bytes = bytes;
    }


    public override int GetHashCode()
    {
        return bytes.GetHashCode();
    }

    public static bool operator ==(BinAsset a, BinAsset b)
    {
        return _Equals(a, b);
    }
    public static bool operator !=(BinAsset a, BinAsset b)
    {
        return !_Equals(a, b);
    }

    public override bool Equals(object otherObj)
    {
        return _Equals(this, otherObj);
    }

    static bool _Equals(object a, object b)
    {
        if ((object)a == null) { return b == null; }
        if (b == null) { return a == null; }

        return a == b;
    }


    public byte[] bytes { get { return m_bytes; } }
    byte[] m_bytes;
}

//文件夹
public class Packet_Dir : IPacket
{
    public Packet_Dir(string path)
    {
        m_path = path;

        //遍历文件夹，并按文件名生成索引
        DirectoryInfo root = new DirectoryInfo(m_path);
        BuildIndex(root);
    }

    public FileSystem.RES_LOCATION ResLocation { get { return FileSystem.RES_LOCATION.fileSystem; } }

    public UnityEngine.Object Load(string name)
    {
        string fullPath;
        if (m_fileName2path.ContainsKey(name))
        {
            fullPath = m_fileName2path[name].FullName;//Path.Combine( Path.Combine(m_path, path),name);
        }
        else if (m_fileNameAndExt2path.ContainsKey(name))
            fullPath = m_fileNameAndExt2path[name].FullName;
        else
            return null;



        using (BufferedStream myBS = new BufferedStream(new FileStream(fullPath, FileMode.Open)))
        {
            int len = (int)myBS.Length;
            byte[] bytes = new byte[len];

            myBS.Read(bytes, 0, len);
            myBS.Close();

            BinAsset re = new BinAsset(bytes);

            return re;
        }
    }

    public void BuildIndex(DirectoryInfo dir)
    {
        //int cl = dir.FullName.Length - m_path.Length;
        //string ph = cl <= 0 ? "" : dir.FullName.Substring(m_path.Length+1);
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo curr in files)
        {
            if (FileSystem.IsMetaTypeFile(curr.Extension)) continue;

            int exlen = curr.Extension.Length;
            string name = curr.Name.Substring(0, curr.Name.Length - exlen);
            if (m_fileName2path.ContainsKey(name))
            {
                Debug.LogError(
                    String.Format("存在重名文件 名:{0} 路径1:{1} 路径2:{2}", name, curr.FullName, m_fileName2path[name])
                    );
            }
            else
            {
                m_fileName2path.Add(name, curr);
                m_fileNameAndExt2path.Add(curr.Name, curr);
            }
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        foreach (DirectoryInfo curr in dirs)
            BuildIndex(curr);
    }

    public void UnLoad()
    {
        m_fileName2path.Clear();
        m_fileNameAndExt2path.Clear();
    }

    //文件名，文件信息
    Dictionary<string, FileInfo> m_fileName2path = new Dictionary<string, FileInfo>();
    Dictionary<string, FileInfo> m_fileNameAndExt2path = new Dictionary<string, FileInfo>();
    string m_path;
}

/// <summary>
/// 包路由
/// </summary>
public class PacketRouting
{
    public void Add(IPacket packet)
    {
        m_packets.Add(packet);
    }

    public UnityEngine.Object Load(string path, FileSystem.RES_LOCATION resLocation = FileSystem.RES_LOCATION.auto)
    {
        foreach (IPacket curr in m_packets)
        {
            if (resLocation != FileSystem.RES_LOCATION.auto && curr.ResLocation != resLocation) continue;//资源所在的域不对

            UnityEngine.Object re = curr.Load(path);
            BinAsset binAsset = re as BinAsset;
            if (binAsset != null || re != null)
                return re;

        }
        return null;
    }

    public byte[] LoadBytes(string path, FileSystem.RES_LOCATION resLocation = FileSystem.RES_LOCATION.auto)
    {
        UnityEngine.Object obj = Load(path, resLocation);
        return GetBytesFromPacketFile(obj);
    }

    public string LoadString(string path, FileSystem.RES_LOCATION resLocation = FileSystem.RES_LOCATION.auto)
    {
        byte[] bytes = LoadBytes(path, resLocation);
        if (bytes == null) return null;
        return FileSystem.byte2string(bytes);
    }


    byte[] GetBytesFromPacketFile(UnityEngine.Object file)
    {
        TextAsset textAsset = file as TextAsset;
        if (textAsset != null)
            return textAsset.bytes;
        else
        {
            BinAsset bin = (BinAsset)file;
            if (bin == null)
                return null;

            return bin.bytes;
        }
    }

    public void UnLoad()
    {
        foreach (IPacket curr in m_packets)
            curr.UnLoad();

        m_packets.Clear();
    }

    List<IPacket> m_packets = new List<IPacket>();
}
//密封在发布程序中的包
/*
public class Packet_Sealed : IPacket
{
    public Packet_Sealed(string packName)
    {
        m_packName = packName + "/";
    }

    public UnityEngine.Object Load(string path)
    {
        return Resources.Load(m_packName + path); 
    }

    string m_packName;
}*/

//仅工作在编辑器模式下的一种包
public class Packet_ResourcesEditor : IPacket
{
    public Packet_ResourcesEditor(string packName)
    {
        string resourcesPath = Application.dataPath + "/Resources";
        DirectoryInfo dirInfo = new DirectoryInfo(resourcesPath);
        if (dirInfo.Exists)//编辑器中
        {
            packName = "@" + packName;

            m_PackPath = FindPack("", packName, dirInfo);

            if (m_PackPath != null)
                Debug.Log("Packet_ResourcesEditor#2" + m_PackPath);
            else
                Debug.Log("Packet_ResourcesEditor#2  null");
        }
        else //发布版
        {
            packName = "@" + packName;
            m_PackPath = packName;
        }

    }

    string FindPack(string path, string packName, DirectoryInfo dirInfo)
    {
        DirectoryInfo[] childs = dirInfo.GetDirectories();
        foreach (DirectoryInfo info in childs)
        {
            string currPath = string.IsNullOrEmpty(path) ? info.Name : (path + "/" + info.Name);
            if (info.Name == packName)
                return currPath;
            string rePath = FindPack(currPath, packName, info);
            if (rePath != null) return rePath;
        }
        return null;
    }

    public UnityEngine.Object Load(string path)
    {
        if (m_PackPath == null) return null;

        string fullpath = m_PackPath + "/" + path;
        Debug.Log("Packet_ResourcesEditor.Load#1 " + fullpath);
        return Resources.Load(fullpath);
    }

    public void UnLoad()
    {
        m_PackPath = null;
    }


    public FileSystem.RES_LOCATION ResLocation { get { return FileSystem.RES_LOCATION.fileSystem; } }
    string m_PackPath = null;
}


/// <summary>
/// 资源包管理器
/// </summary>
public class PacketManage : MonoBehaviour
{
    const string packetSuffix = "";//.assetbundle

    static PacketManage _Single = null;

    public bool WorkInResourcesEditor = false;//是否工作在资源编辑器中

    public static PacketManage Single
    {
        get
        {
            if (_Single == null)
            {
                // 尝试寻找该类的实例。此处不能用GameObject.Find，因为MonoBehaviour继承自Component。
                _Single = UnityEngine.Object.FindObjectOfType(typeof(PacketManage)) as PacketManage;
                if (_Single == null)  // 如果没有找到
                    _Single = UROMSystem.Single.AddComponent<PacketManage>();
            }
            return _Single;
        }
    }


    void OnEnable() { CoroutineManage.Single.RegComponentUpdate(IUpdate); }

    void OnDestroy()
    {
        CoroutineManage.Single.UnRegComponentUpdate(IUpdate);
    }

    /*
     只在编辑器下有效
     Object cloneSrc = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Sphere.prefab", typeof(GameObject));
     */


    PacketRouting AddPacket(string packName, IPacket packetObj)
    {
        if (!m_DonePacks.ContainsKey(packName))
            m_DonePacks.Add(packName, new PacketRouting());

        PacketRouting re = m_DonePacks[packName];
        re.Add(packetObj);
        return re;
    }

    /// <summary>
    /// 装载包
    /// </summary>
    /// <param name="pkType"></param>
    /// <param name="packName"></param>
    /// <param name="OnPacketLoadDone">包装载完成通知 包名,包路由对象</param>
    /// <param name="OnPacketLoadingProgress">包装载进度通知 包名,进度</param>
    public void LoadPacket(
        PackType pkType, string packName,
        Action<string, PacketRouting> OnPacketLoadDone,
        Action<string, float> OnPacketLoadingProgress,
        bool FoceLoadInternalPack = false//强制装内部包
        )
    {

        //已经装载完成，直接返回装载结果
        if (m_DonePacks.ContainsKey(packName))
        {
            OnPacketLoadDone(packName, m_DonePacks[packName]);
            return;
        }

        //当前处于下载中
        if (m_DownloadPacks.ContainsKey(packName))
        {
            DownloadInfo info = m_DownloadPacks[packName];
            if (OnPacketLoadDone != null && !info.listener_OnPacketLoadDone.Contains(OnPacketLoadDone))
                info.listener_OnPacketLoadDone.Add(OnPacketLoadDone);

            if (OnPacketLoadingProgress != null && !info.listener_OnPacketLoadingProgress.Contains(OnPacketLoadingProgress))
                info.listener_OnPacketLoadingProgress.Add(OnPacketLoadingProgress);
            return;
        }

        if (WorkInResourcesEditor)//当前处于资源编辑器中
        {
            Packet_ResourcesEditor bundle = new Packet_ResourcesEditor(packName);
            PacketRouting pk = AddPacket(packName, bundle);
            OnPacketLoadDone(packName, pk);
            return;
        }

        FileSystem.RES_LOCATION srcType;
        string devPackDir;
        string url = FileSystem.RrelativePath2Absolute_Packet(pkType == PackType.Res ? "pack_res" : "pack_script", packName + packetSuffix, false, out srcType, out devPackDir);

        //存在研发调试资源
        if (!string.IsNullOrEmpty(devPackDir))
        {
            Packet_Dir bundle = new Packet_Dir(devPackDir);
            AddPacket(packName, bundle);
        }

        switch (srcType)
        {
            case FileSystem.RES_LOCATION.fileSystem://文件系统
                {
                    Packet_Dir bundle = new Packet_Dir(url);

                    PacketRouting pk = AddPacket(packName, bundle);
                    OnPacketLoadDone(packName, pk);
                }
                break;
            case FileSystem.RES_LOCATION.externalPack://外部包
                {
                    //##############################################解密包数据############################################
                    //DateTime startTime = DateTime.Now;
                    long FileSize;
                    Packet_Bundle bundle;
                    {
                        byte[] encryptedData;

                        using (FileStream kk = new FileStream(url, FileMode.Open))
                        {
                            FileSize = kk.Length;
                            encryptedData = new byte[FileSize];
                            kk.Read(encryptedData, 0, (int)kk.Length);
                        }

                        if (DefsDate.bundleJudge(encryptedData))
                        {
                            bool li = DefsDate.SupDecToStream(encryptedData);
                            //Debug.Log("解密是否成功 = " + li);
                        }
                        //else
                        //{
                        //    Debug.Log("解密不了的包或非加密包");
                        //}
                        AssetBundle assetBundle = AssetBundle.LoadFromMemory(encryptedData);
                        bundle = new Packet_Bundle(packName, assetBundle, FileSystem.RES_LOCATION.internalPack);
                    }

                    AutoGC(FileSize);

                    //Debug.Log("加载时间 = " + (DateTime.Now - startTime).ToString());
                    //##############################################解密包数据############################################

                    //AssetBundle assetBundle = AssetBundle.LoadFromFile(url);

                    //Packet_Bundle bundle = new Packet_Bundle(packName,assetBundle, FileSystem.RES_LOCATION.externalPack);
                    PacketRouting pk = AddPacket(packName, bundle);
                    if (FoceLoadInternalPack)
                    {
                        url = FileSystem.RrelativePath2Absolute_Packet(pkType == PackType.Res ? "pack_res" : "pack_script", packName + packetSuffix, false, out srcType, out devPackDir, true);
                        LoadInternalPack(url, packName, OnPacketLoadDone, OnPacketLoadingProgress);
                    }
                    else
                        OnPacketLoadDone(packName, pk);

                }
                break;
            case FileSystem.RES_LOCATION.internalPack://内部包
                {

                    LoadInternalPack(url, packName, OnPacketLoadDone, OnPacketLoadingProgress);
                }
                break;

        }
    }

    public void AutoGC(long addsize)
    {
        m_AutoGC_SizeCount += addsize;
        if (m_AutoGC_SizeCount > GCMAXSIZE)
        {
            m_AutoGC_SizeCount = 0;
            Resources.UnloadUnusedAssets();
        }
    }


    public void LoadInternalPack(
        string url, string packName,
        Action<string, PacketRouting> OnPacketLoadDone,
        Action<string, float> OnPacketLoadingProgress
        )
    {
        WWW www = new WWW(url);

        DownloadInfo info = new DownloadInfo();
        info.www = www;

        if (OnPacketLoadDone != null && !info.listener_OnPacketLoadDone.Contains(OnPacketLoadDone))
            info.listener_OnPacketLoadDone.Add(OnPacketLoadDone);

        if (OnPacketLoadingProgress != null && !info.listener_OnPacketLoadingProgress.Contains(OnPacketLoadingProgress))
            info.listener_OnPacketLoadingProgress.Add(OnPacketLoadingProgress);


        m_DownloadPacks.Add(packName, info);
    }



    public PacketRouting GetPacket(string packName)
    {
        return (m_DonePacks.ContainsKey(packName)) ? m_DonePacks[packName] : null;
    }

    public void UnLoadPacket(string packName)
    {
        if (!m_DonePacks.ContainsKey(packName))
        {
            Debug.LogWarning(string.Format("资源包 {0} 不存在，不能卸载", packName));
            return;
        }

        PacketRouting pkr = m_DonePacks[packName];
        pkr.UnLoad();
        m_DonePacks.Remove(packName);
    }

    void IUpdate()
    {
        needRemoveDownloading.Clear();
        foreach (KeyValuePair<string, DownloadInfo> curr in m_DownloadPacks)
        {
            DownloadInfo info = curr.Value;
            string packName = curr.Key;
            if (info.www.error != null)
            {
                /*
                //加载失败时读取包内资源
                Packet_Sealed packet = new Packet_Sealed(packName);
                m_DonePacks.Add(packName, packet);

                foreach (IPacketLoadingListener listener in info.listeners)
                    listener.OnPacketLoadDone(packName, packet); 
        
                needRemoveDownloading.Add(packName);
*/
                if (m_DonePacks.ContainsKey(packName))//存在研发目录
                {
                    PacketRouting pk = m_DonePacks[packName];
                    foreach (var OnPacketLoadDone in info.listener_OnPacketLoadDone)
                        OnPacketLoadDone(packName, pk);

                }
                else
                {
                    Debug.LogError("包装载失败 " + packName);
                    Debug.Log("包装载失败路径 " + info.www.url);
                    foreach (var OnPacketLoadDone in info.listener_OnPacketLoadDone)
                        OnPacketLoadDone(packName, null);
                }
                needRemoveDownloading.Add(packName);
            }
            else if (info.www.isDone)
            {
                //Debug.Log("内部包加载完成" + packName);

                //##############################################解密包数据############################################
                byte[] encryptedData = info.www.bytes;
                //DateTime startTime = DateTime.Now;
                Packet_Bundle bundle;
                if (DefsDate.bundleJudge(encryptedData))
                {
                    bool li = DefsDate.SupDecToStream(encryptedData);
                    //Debug.Log("解密是否成功 = " + li);
                    AssetBundle assetBundle = AssetBundle.LoadFromMemory(encryptedData);
                    bundle = new Packet_Bundle(packName, assetBundle, FileSystem.RES_LOCATION.internalPack);
                }
                else
                {
                    //Debug.Log("解密不了的包或非加密包");
                    bundle = new Packet_Bundle(packName, info.www.assetBundle, FileSystem.RES_LOCATION.internalPack);
                }
                //Debug.Log("加载时间 = " + (DateTime.Now - startTime).ToString());
                //##############################################解密包数据############################################

                PacketRouting pk = AddPacket(packName, bundle);

                foreach (var OnPacketLoadDone in info.listener_OnPacketLoadDone)
                    OnPacketLoadDone(packName, pk);

                needRemoveDownloading.Add(packName);
            }
            else
            {
                foreach (var OnPacketLoadingProgress in info.listener_OnPacketLoadingProgress)
                    OnPacketLoadingProgress(packName, info.www.progress);
            }
        }

        if (needRemoveDownloading.Count > 0)
        {
            int size = 0;
            foreach (string packName in needRemoveDownloading)
            {
                size += m_DownloadPacks[packName].www.bytes.Length;
                m_DownloadPacks[packName].www.Dispose();
                m_DownloadPacks.Remove(packName);
            }

            AutoGC(size);
        }

    }


    class DownloadInfo
    {
        public WWW www;
        public HashSet<Action<string, PacketRouting>> listener_OnPacketLoadDone = new HashSet<Action<string, PacketRouting>>();
        public HashSet<Action<string, float>> listener_OnPacketLoadingProgress = new HashSet<Action<string, float>>();
    }



    List<string> needRemoveDownloading = new List<string>();


    //包名,下载对象
    Dictionary<string, DownloadInfo> m_DownloadPacks = new Dictionary<string, DownloadInfo>();

    //装载完成的包
    Dictionary<string, PacketRouting> m_DonePacks = new Dictionary<string, PacketRouting>();


    const long GCMAXSIZE = 1024 * 1024 * 10;

    long m_AutoGC_SizeCount = 0;
}