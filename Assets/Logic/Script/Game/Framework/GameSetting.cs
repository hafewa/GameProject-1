﻿using UnityEngine;
using System.Collections;

public class GameSetting
{
    // 开发模式
    public static bool DevelopMode = true;
    public static bool UpdateMode = false;

    public const string WebUrl = "http://192.168.1.222:6688/";      //测试更新地址

    public const string AppName = "UnityHello";               //应用程序名称

    public const int GameFrameRate = 30;

    public const string AssetDir = "StreamingAssets";           //素材目录 
    //AB素材扩展名
    public const string ExtName = ".assetbundle";

    public static bool LuaByteMode = false;                     //Lua字节码模式-默认关闭 
    public static bool LuaBundleMode = false;                    //Lua代码AssetBundle模式

    public static int SocketPort = 10010;                           //Socket服务器端口
    public static string SocketAddress = "106.75.36.113";          //Socket服务器地址

    public static bool EnableLuaDebug = false;

    public static string AssetPath
    {
        get
        {
            return Application.streamingAssetsPath;
        }
    }

    public static string TransAssetPath
    {
        get
        {
            string protocl = "file://";
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                protocl = "file:///";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                protocl = "";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                protocl = "";
            }
            return protocl + AssetPath;
        }
    }
}