﻿using System;
using System.ComponentModel;
using UnityEngine;


/// <summary>
/// 目标选择单位数据
/// </summary>
public class Member : ISelectWeightData, IBaseMember//, IGraphical<Rectangle>
{
    // ----------------------------暴露接口-------------------------------

    /// <summary>
    /// 单位数据
    /// </summary>
    public VOBase MemberData { get; set; }
    
    ///// <summary>
    ///// 扫描范围类型
    ///// </summary>
    //public GraphicType ScanType
    //{
    //    get { return scanType; }
    //    set { scanType = value; }
    //}

    /// <summary>
    /// 当前位置X
    /// </summary>
    public float X
    {
        get { return x; }
        set { x = value; }
    }

    /// <summary>
    /// 当前位置Y
    /// </summary>
    public float Y
    {
        get { return y; }
        set { y = value; }
    }

    /// <summary>
    /// 方向
    /// </summary>
    public Vector3 Direction { get; set; }
    
    /// <summary>
    /// 单位名称
    /// </summary>
    public string Name { get; set; }


    // ----------------------------权重选择 Level1-----------------------------
    /// <summary>
    /// 选择地面单位权重
    /// </summary>
    public float SurfaceWeight { get; set; }

    /// <summary>
    /// 选择天空单位权重
    /// </summary>
    public float AirWeight { get; set; }

    /// <summary>
    /// 选择建筑权重
    /// </summary>
    public float BuildWeight { get; set; }


    // ----------------------------权重选择 Level2-----------------------------

    /// <summary>
    /// 选择坦克权重
    /// </summary>
    public float TankWeight { get; set; }

    /// <summary>
    /// 选择轻型载具权重
    /// </summary>
    public float LVWeight { get; set; }

    /// <summary>
    /// 选择火炮权重
    /// </summary>
    public float CannonWeight { get; set; }

    /// <summary>
    /// 选择飞行器权重
    /// </summary>
    public float AirCraftWeight { get; set; }

    /// <summary>
    /// 选择步兵权重
    /// </summary>
    public float SoldierWeight { get; set; }


    // ----------------------------权重选择 Level3-----------------------------
    /// <summary>
    /// 选择隐形单位权重
    /// </summary>
    public float HideWeight { get; set; }

    /// <summary>
    /// 选择嘲讽权重(这个值应该很大, 除非有反嘲讽效果的单位)
    /// </summary>
    public float TauntWeight { get; set; }


    // ----------------------------权重选择 Level4-----------------------------


    /// <summary>
    /// 低生命权重
    /// </summary>
    public float HealthMinWeight { get; set; }

    /// <summary>
    /// 高生命权重
    /// </summary>
    public float HealthMaxWeight { get; set; }

    /// <summary>
    /// 近位置权重
    /// </summary>
    public float DistanceMinWeight { get; set; }

    /// <summary>
    /// 远位置权重
    /// </summary>
    public float DistanceMaxWeight { get; set; }

    /// <summary>
    /// 角度权重
    /// </summary>
    public float AngleWeight { get; set; }



    ///// <summary>
    ///// 精准度
    ///// </summary>
    //public float Accuracy { get; set; }
    ///// <summary>
    ///// 散射半径
    ///// </summary>
    //public float ScatteringRadius { get; set; }


    // -------------------------------私有属性--------------------------------------

    /// <summary>
    /// 攻击范围形状
    /// </summary>
    private GraphicType scanType = GraphicType.Circle;
    
    /// <summary>
    /// 当前位置x
    /// </summary>
    private float x = 0;

    /// <summary>
    /// 当前位置y
    /// </summary>
    private float y = 0;

    /// <summary>
    /// 目标点
    /// </summary>
    private Vector3 direction;


    private float healthWeight = 100;

    private float distanceWeight = 0.2f;

    private float angleWeight = 1;

    private float typeWeight;

    private float levelWeight;




    /// <summary>
    /// 单位矩形占位
    /// </summary>
    //private Rectangle _rect = null;

    private float _hisX = 0;

    private float _hisY = 0;

    private float _hisDimeter = 0;
    


    // ------------------------------公有方法-------------------------------------

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="memberData"></param>
    public Member(VOBase memberData)
    {
        if (memberData == null)
        {
            throw new Exception("单位数据对象不能为空.");
        }
        MemberData = memberData;
    }

    /// <summary>
    /// 获得单位矩形占位
    /// </summary>
    /// <returns></returns>
    //public Rectangle GetGraphical()
    //{
    //    // 当rect不存在或位置大小发生变更时创建新Rect
    //    if (Math.Abs(_hisDimeter - MemberData.SpaceSet) > Utils.ApproachZero || Math.Abs(_hisX - X) > 0.0001f || Math.Abs(_hisY - Y) > 0.0001f || _rect == null)
    //    {
    //        _hisX = X;
    //        _hisY = Y;
    //        _hisDimeter = MemberData.SpaceSet;
    //        _rect = new Rectangle(X, Y, MemberData.SpaceSet, MemberData.SpaceSet);
    //    }
    //    return _rect;
    //}
}


/// <summary>
/// Mamber基础接口
/// </summary>
public interface IBaseMember
{
    // ----------------------------------暴露接口--------------------------------------

    /// <summary>
    /// 持有数据对象
    /// </summary>
    VOBase MemberData { get; }
    
    ///// <summary>
    ///// 扫描范围形状类型
    ///// </summary>
    //GraphicType ScanType { get; set; }

    /// <summary>
    /// 位置X
    /// </summary>
    float X { get; set; }

    /// <summary>
    /// 位置Y
    /// </summary>
    float Y { get; set; }

    /// <summary>
    /// 方向
    /// </summary>
    Vector3 Direction { get; set; }
    // ----------------------------------暴露接口--------------------------------------
}

///// <summary>
///// 部队常规属性
///// </summary>
//public enum GeneralType
//{
//    Surface = 1,
//    Air = 2,
//    Building = 3
//}