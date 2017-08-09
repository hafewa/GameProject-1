using System;
using System.Collections.Generic;
using LuaInterface;

public class SData_maininterface_c : MonoEX.Singleton<SData_maininterface_c>
{
    public void setData(LuaTable table1, LuaTable table2)
    {
        var head = new string[table1.Length];
        SDataUtils.dealTable(table1, (Object o1, Object o2) =>
        {
            head[(int)(double)o1 - 1] = (string)o2;
        });
        SDataUtils.dealTable(table2, (Object o1, Object o2) =>
        {
            maininterface_cInfo dif = new maininterface_cInfo();
            SDataUtils.dealTable((LuaTable)o2, (Object o11, Object o22) =>
            {
                switch (head[(int)(double)o11 - 1])
				{
					case "ID": dif.ID = (short)(double)o22; break;
					case "Name": dif.Name = (string)o22; break;
					case "Type": dif.Type = (short)(double)o22; break;
					case "UnlockLevel": dif.UnlockLevel = (short)(double)o22; break;
					case "UnlockEvent": dif.UnlockEvent = (string)o22; break;
					case "Icon": dif.Icon = (string)o22; break;
                }
            });
            if (Data.ContainsKey(dif.ID))
                MonoEX.Debug.Logout(MonoEX.LOG_TYPE.LT_ERROR, "重复的ID：" + dif.ID.ToString());
            Data.Add(dif.ID, dif);
        });
    }

    public maininterface_cInfo GetDataOfID(int Id)
    {
        if (!Data.ContainsKey(Id)) throw new Exception(String.Format("maininterface_cInfo::GetDataOfID() not found data  Id:{0}", Id));
        return Data[Id];
    }

    internal Dictionary<int, maininterface_cInfo> Data = new Dictionary<int, maininterface_cInfo>();
}


public struct maininterface_cInfo
{
	 /// <summary>
	 ///ID排序用
	 /// </summary>
	public short ID;
	 /// <summary>
	 ///名称
	 /// </summary>
	public string Name;
	 /// <summary>
	 ///类型
	 /// </summary>
	public short Type;
	 /// <summary>
	 ///解锁等级
	 /// </summary>
	public short UnlockLevel;
	 /// <summary>
	 ///解锁条件
	 /// </summary>
	public string UnlockEvent;
	 /// <summary>
	 ///图标资源
	 /// </summary>
	public string Icon;
}