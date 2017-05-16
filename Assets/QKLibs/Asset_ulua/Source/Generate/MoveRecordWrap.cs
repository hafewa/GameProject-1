﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class MoveRecordWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(MoveRecord), typeof(System.Object));
		L.RegFunction("Clear", Clear);
		L.RegFunction("JumpTo", JumpTo);
		L.RegFunction("GetInertiaSpeed", GetInertiaSpeed);
		L.RegFunction("New", _CreateMoveRecord);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("CurrentPos", get_CurrentPos, set_CurrentPos);
		L.RegVar("TotalOffset", get_TotalOffset, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateMoveRecord(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				MoveRecord obj = new MoveRecord();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: MoveRecord.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Clear(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MoveRecord obj = (MoveRecord)ToLua.CheckObject(L, 1, typeof(MoveRecord));
			UnityEngine.Vector3 arg0 = ToLua.ToVector3(L, 2);
			obj.Clear(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int JumpTo(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MoveRecord obj = (MoveRecord)ToLua.CheckObject(L, 1, typeof(MoveRecord));
			UnityEngine.Vector3 arg0 = ToLua.ToVector3(L, 2);
			obj.JumpTo(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetInertiaSpeed(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MoveRecord obj = (MoveRecord)ToLua.CheckObject(L, 1, typeof(MoveRecord));
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.Vector3 o = obj.GetInertiaSpeed(arg0);
			ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_CurrentPos(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			MoveRecord obj = (MoveRecord)o;
			UnityEngine.Vector3 ret = obj.CurrentPos;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index CurrentPos on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_TotalOffset(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			MoveRecord obj = (MoveRecord)o;
			UnityEngine.Vector3 ret = obj.TotalOffset;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index TotalOffset on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_CurrentPos(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			MoveRecord obj = (MoveRecord)o;
			UnityEngine.Vector3 arg0 = ToLua.ToVector3(L, 2);
			obj.CurrentPos = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index CurrentPos on a nil value" : e.Message);
		}
	}
}
