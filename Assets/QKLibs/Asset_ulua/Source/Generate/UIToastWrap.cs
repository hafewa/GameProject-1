﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UIToastWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(UIToast), typeof(System.Object));
		L.RegFunction("ShowWithCallback", ShowWithCallback);
		L.RegFunction("Show", Show);
		L.RegFunction("New", _CreateUIToast);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUIToast(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				UIToast obj = new UIToast();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: UIToast.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ShowWithCallback(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			string arg0 = ToLua.CheckString(L, 1);
			DG.Tweening.TweenCallback arg1 = null;
			LuaTypes funcType2 = LuaDLL.lua_type(L, 2);

			if (funcType2 != LuaTypes.LUA_TFUNCTION)
			{
				 arg1 = (DG.Tweening.TweenCallback)ToLua.CheckObject(L, 2, typeof(DG.Tweening.TweenCallback));
			}
			else
			{
				LuaFunction func = ToLua.ToLuaFunction(L, 2);
				arg1 = DelegateFactory.CreateDelegate(typeof(DG.Tweening.TweenCallback), func) as DG.Tweening.TweenCallback;
			}

			UIToast.ShowType arg2 = (UIToast.ShowType)ToLua.CheckObject(L, 3, typeof(UIToast.ShowType));
			UIToast.ShowWithCallback(arg0, arg1, arg2);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Show(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			UIToast.Show(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
