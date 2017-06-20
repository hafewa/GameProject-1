﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 数据持有类 隶属于ClusterData, 所有数据放入该类
/// </summary>
public class AllData : ISelectWeightDataHolder
{
    /// <summary>
    /// 目标筛选数据
    /// </summary>
    public VOBase MemberData { get; set; }

    /// <summary>
    /// 目标选择权重数据
    /// </summary>
    public SelectWeightData SelectWeightData { get; set; }

    /// <summary>
    /// 单位AOE数据
    /// </summary>
    public ArmyAOEData AOEData { get; set; }



}