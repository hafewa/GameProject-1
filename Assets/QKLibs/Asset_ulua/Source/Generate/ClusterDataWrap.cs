﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class ClusterDataWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(ClusterData), typeof(PositionObject));
		L.RegFunction("SetDataValue", SetDataValue);
		L.RegFunction("PushTarget", PushTarget);
		L.RegFunction("PushTargetList", PushTargetList);
		L.RegFunction("PopTarget", PopTarget);
		L.RegFunction("ClearTarget", ClearTarget);
        //L.RegFunction("Update", Update);
		L.RegFunction("Destory", Destory);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("State", get_State, set_State);
		L.RegVar("RotateSpeed", get_RotateSpeed, set_RotateSpeed);
		L.RegVar("RotateWeight", get_RotateWeight, set_RotateWeight);
		L.RegVar("TargetPos", get_TargetPos, set_TargetPos);
		L.RegVar("Moveing", get_Moveing, set_Moveing);
		L.RegVar("Wait", get_Wait, set_Wait);
		L.RegVar("Complete", get_Complete, set_Complete);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetDataValue(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			ClusterData obj = (ClusterData)ToLua.CheckObject(L, 1, typeof(ClusterData));
			VOBase arg0 = (VOBase)ToLua.CheckObject(L, 2, typeof(VOBase));
			obj.SetDataValue(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PushTarget(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			ClusterData obj = (ClusterData)ToLua.CheckObject(L, 1, typeof(ClusterData));
			UnityEngine.Vector3 arg0 = ToLua.ToVector3(L, 2);
			obj.PushTarget(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PushTargetList(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			ClusterData obj = (ClusterData)ToLua.CheckObject(L, 1, typeof(ClusterData));
			System.Collections.Generic.List<UnityEngine.Vector3> arg0 = (System.Collections.Generic.List<UnityEngine.Vector3>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<UnityEngine.Vector3>));
			obj.PushTargetList(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PopTarget(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClusterData obj = (ClusterData)ToLua.CheckObject(L, 1, typeof(ClusterData));
			bool o = obj.PopTarget();
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearTarget(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClusterData obj = (ClusterData)ToLua.CheckObject(L, 1, typeof(ClusterData));
			obj.ClearTarget();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

    //[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    //static int Update(IntPtr L)
    //{
    //    try
    //    {
    //        ToLua.CheckArgsCount(L, 1);
    //        ClusterData obj = (ClusterData)ToLua.CheckObject(L, 1, typeof(ClusterData));
    //        obj.Update();
    //        return 0;
    //    }
    //    catch(Exception e)
    //    {
    //        return LuaDLL.toluaL_exception(L, e);
    //    }
    //}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Destory(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClusterData obj = (ClusterData)ToLua.CheckObject(L, 1, typeof(ClusterData));
			obj.Destory();
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_State(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			SchoolItemState ret = obj.State;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index State on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RotateSpeed(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			float ret = obj.RotateSpeed;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index RotateSpeed on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RotateWeight(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			float ret = obj.RotateWeight;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index RotateWeight on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_TargetPos(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			UnityEngine.Vector3 ret = obj.TargetPos;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index TargetPos on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Moveing(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			System.Action<UnityEngine.GameObject> ret = obj.Moveing;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Moveing on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Wait(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			System.Action<UnityEngine.GameObject> ret = obj.Wait;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Wait on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Complete(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			System.Action<UnityEngine.GameObject> ret = obj.Complete;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Complete on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_State(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			SchoolItemState arg0 = (SchoolItemState)ToLua.CheckObject(L, 2, typeof(SchoolItemState));
			obj.State = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index State on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_RotateSpeed(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.RotateSpeed = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index RotateSpeed on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_RotateWeight(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.RotateWeight = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index RotateWeight on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_TargetPos(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			UnityEngine.Vector3 arg0 = ToLua.ToVector3(L, 2);
			obj.TargetPos = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index TargetPos on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Moveing(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			System.Action<UnityEngine.GameObject> arg0 = null;
			LuaTypes funcType2 = LuaDLL.lua_type(L, 2);

			if (funcType2 != LuaTypes.LUA_TFUNCTION)
			{
				 arg0 = (System.Action<UnityEngine.GameObject>)ToLua.CheckObject(L, 2, typeof(System.Action<UnityEngine.GameObject>));
			}
			else
			{
				LuaFunction func = ToLua.ToLuaFunction(L, 2);
				arg0 = DelegateFactory.CreateDelegate(typeof(System.Action<UnityEngine.GameObject>), func) as System.Action<UnityEngine.GameObject>;
			}

			obj.Moveing = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Moveing on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Wait(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			System.Action<UnityEngine.GameObject> arg0 = null;
			LuaTypes funcType2 = LuaDLL.lua_type(L, 2);

			if (funcType2 != LuaTypes.LUA_TFUNCTION)
			{
				 arg0 = (System.Action<UnityEngine.GameObject>)ToLua.CheckObject(L, 2, typeof(System.Action<UnityEngine.GameObject>));
			}
			else
			{
				LuaFunction func = ToLua.ToLuaFunction(L, 2);
				arg0 = DelegateFactory.CreateDelegate(typeof(System.Action<UnityEngine.GameObject>), func) as System.Action<UnityEngine.GameObject>;
			}

			obj.Wait = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Wait on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Complete(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClusterData obj = (ClusterData)o;
			System.Action<UnityEngine.GameObject> arg0 = null;
			LuaTypes funcType2 = LuaDLL.lua_type(L, 2);

			if (funcType2 != LuaTypes.LUA_TFUNCTION)
			{
				 arg0 = (System.Action<UnityEngine.GameObject>)ToLua.CheckObject(L, 2, typeof(System.Action<UnityEngine.GameObject>));
			}
			else
			{
				LuaFunction func = ToLua.ToLuaFunction(L, 2);
				arg0 = DelegateFactory.CreateDelegate(typeof(System.Action<UnityEngine.GameObject>), func) as System.Action<UnityEngine.GameObject>;
			}

			obj.Complete = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Complete on a nil value" : e.Message);
		}
	}
}
