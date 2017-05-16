﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class LGYLOGWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(LGYLOG), typeof(GAMELOG));
		L.RegFunction("Log", Log);
		L.RegFunction("New", _CreateLGYLOG);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLGYLOG(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				LGYLOG obj = new LGYLOG();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: LGYLOG.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Log(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1 && TypeChecker.CheckTypes(L, 1, typeof(string)))
			{
				string arg0 = ToLua.ToString(L, 1);
				LGYLOG.Log(arg0);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes(L, 1, typeof(string), typeof(int)))
			{
				string arg0 = ToLua.ToString(L, 1);
				int arg1 = (int)LuaDLL.lua_tonumber(L, 2);
				LGYLOG.Log(arg0, arg1);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: LGYLOG.Log");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
