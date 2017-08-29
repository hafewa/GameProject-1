﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class AbilityBaseWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(AbilityBase), typeof(System.Object));
		L.RegFunction("AddActionFormulaItem", AddActionFormulaItem);
		L.RegFunction("AddAttachFormulaItem", AddAttachFormulaItem);
		L.RegFunction("AddDetachFormulaItem", AddDetachFormulaItem);
		L.RegFunction("GetActionFormulaItem", GetActionFormulaItem);
		L.RegFunction("GetAttachFormulaItem", GetAttachFormulaItem);
		L.RegFunction("GetDetachFormulaItem", GetDetachFormulaItem);
		L.RegFunction("GetAttachFormula", GetAttachFormula);
		L.RegFunction("GetDetachFormula", GetDetachFormula);
		L.RegFunction("GetActionFormula", GetActionFormula);
		L.RegFunction("ReplaceData", ReplaceData);
		L.RegFunction("New", _CreateAbilityBase);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("DataList", get_DataList, set_DataList);
		L.RegVar("DataScope", get_DataScope, set_DataScope);
		L.RegVar("Level", get_Level, set_Level);
		L.RegVar("ReplaceSourceDataDic", get_ReplaceSourceDataDic, set_ReplaceSourceDataDic);
		L.RegVar("ShareData", get_ShareData, set_ShareData);
		L.RegVar("Num", get_Num, null);
		L.RegVar("AddtionId", get_AddtionId, null);
		L.RegVar("ReleaseMember", get_ReleaseMember, set_ReleaseMember);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateAbilityBase(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				AbilityBase obj = new AbilityBase();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: AbilityBase.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddActionFormulaItem(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			IFormulaItem arg0 = (IFormulaItem)ToLua.CheckObject(L, 2, typeof(IFormulaItem));
			obj.AddActionFormulaItem(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddAttachFormulaItem(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			IFormulaItem arg0 = (IFormulaItem)ToLua.CheckObject(L, 2, typeof(IFormulaItem));
			obj.AddAttachFormulaItem(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddDetachFormulaItem(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			IFormulaItem arg0 = (IFormulaItem)ToLua.CheckObject(L, 2, typeof(IFormulaItem));
			obj.AddDetachFormulaItem(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetActionFormulaItem(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			IFormulaItem o = obj.GetActionFormulaItem();
			ToLua.PushObject(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAttachFormulaItem(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			IFormulaItem o = obj.GetAttachFormulaItem();
			ToLua.PushObject(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetDetachFormulaItem(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			IFormulaItem o = obj.GetDetachFormulaItem();
			ToLua.PushObject(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAttachFormula(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			FormulaParamsPacker arg0 = (FormulaParamsPacker)ToLua.CheckObject(L, 2, typeof(FormulaParamsPacker));
			IFormula o = obj.GetAttachFormula(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetDetachFormula(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			FormulaParamsPacker arg0 = (FormulaParamsPacker)ToLua.CheckObject(L, 2, typeof(FormulaParamsPacker));
			IFormula o = obj.GetDetachFormula(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetActionFormula(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			FormulaParamsPacker arg0 = (FormulaParamsPacker)ToLua.CheckObject(L, 2, typeof(FormulaParamsPacker));
			IFormula o = obj.GetActionFormula(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReplaceData(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AbilityBase obj = (AbilityBase)ToLua.CheckObject(L, 1, typeof(AbilityBase));
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.ReplaceData(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DataList(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			System.Collections.Generic.List<System.Collections.Generic.List<string>> ret = obj.DataList;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index DataList on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DataScope(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			DataScope ret = obj.DataScope;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index DataScope on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Level(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			int ret = obj.Level;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Level on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ReplaceSourceDataDic(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			System.Collections.Generic.Dictionary<string,string> ret = obj.ReplaceSourceDataDic;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index ReplaceSourceDataDic on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ShareData(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			System.Collections.Generic.Dictionary<string,string> ret = obj.ShareData;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index ShareData on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Num(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			int ret = obj.Num;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Num on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_AddtionId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			long ret = obj.AddtionId;
			LuaDLL.tolua_pushint64(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index AddtionId on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ReleaseMember(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			DisplayOwner ret = obj.ReleaseMember;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index ReleaseMember on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_DataList(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			System.Collections.Generic.List<System.Collections.Generic.List<string>> arg0 = (System.Collections.Generic.List<System.Collections.Generic.List<string>>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<System.Collections.Generic.List<string>>));
			obj.DataList = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index DataList on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_DataScope(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			DataScope arg0 = (DataScope)ToLua.CheckObject(L, 2, typeof(DataScope));
			obj.DataScope = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index DataScope on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Level(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.Level = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index Level on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ReplaceSourceDataDic(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			System.Collections.Generic.Dictionary<string,string> arg0 = (System.Collections.Generic.Dictionary<string,string>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.Dictionary<string,string>));
			obj.ReplaceSourceDataDic = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index ReplaceSourceDataDic on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ShareData(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			System.Collections.Generic.Dictionary<string,string> arg0 = (System.Collections.Generic.Dictionary<string,string>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.Dictionary<string,string>));
			obj.ShareData = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index ShareData on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ReleaseMember(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AbilityBase obj = (AbilityBase)o;
			DisplayOwner arg0 = (DisplayOwner)ToLua.CheckObject(L, 2, typeof(DisplayOwner));
			obj.ReleaseMember = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index ReleaseMember on a nil value" : e.Message);
		}
	}
}
