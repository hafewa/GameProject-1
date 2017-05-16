﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface   AICmd
{
    AICmdType CmdType { get; }
}

/// <summary>
/// 释放手动技能
/// </summary>
public class AICmd_SDSkill : AICmd
{
    public AICmdType CmdType { get { return AICmdType.SDSkill; } }
    public int fid;//英雄阵位ID
    public float CurrTime;//命令执行时间点
}


/// <summary>
/// 冲锋
/// </summary>
public class AICmd_Charge : AICmd
{
    public AICmdType CmdType { get { return AICmdType.Charge; } } 
    public float CurrTime;//命令执行时间点
}

/// <summary>
/// 直接设置胜负 用于作弊命令
/// </summary>
public class AICmd_SetResult : AICmd
{
    public AICmdType CmdType { get { return AICmdType.SetResult; } }
    public float CurrTime;//命令执行时间点
    public bool IsWin;//进攻方是否胜利
}