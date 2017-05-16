﻿using System;
using MonoEX;
using UnityEngine;


/// <summary>
/// 点对对象飞行特效行为构建器
/// </summary>
public class PointToObjFormulaItem : AbstractFormulaItem
{

    /// <summary>
    /// 特效key(或者路径)
    /// </summary>
    public string EffectKey { get; private set; }

    /// <summary>
    /// 飞行速度
    /// </summary>
    public float Speed { get; private set; }

    /// <summary>
    /// 飞行轨迹
    /// </summary>
    public TrajectoryAlgorithmType FlyType { get; private set; }


    /// <summary>
    /// 缩放
    /// </summary>
    private float[] scale = new[] { 1f, 1f, 1f };

    /// <summary>
    /// 初始化行为链生成器
    /// </summary>
    /// <param name="formulaType">行为节点类型</param>
    /// <param name="effectKey">特效Key(或path)</param>
    /// <param name="speed">飞行速度</param>
    /// <param name="flyType">飞行方式(0抛物线, 1直线, 2 sin线</param>
    /// <param name="scale">缩放</param>
    public PointToObjFormulaItem(int formulaType, string effectKey, float speed, TrajectoryAlgorithmType flyType, float[] scale = null)
    {
        FormulaType = formulaType;
        EffectKey = effectKey;
        Speed = speed;
        FlyType = flyType;
        if (scale != null)
        {
            this.scale = scale;
        }
    }

    /// <summary>
    /// 初始化行为链生成器
    /// </summary>
    /// <param name="array">数据数组</param>
    /// 
    /// 0>行为节点类型
    /// 1>特效Key(或path)
    /// 2>飞行速度
    /// 3>飞行方式(0抛物线, 1直线, 2 sin线
    /// 456>缩放
    public PointToObjFormulaItem(string[] array)
    {
        if (array == null)
        {
            throw new Exception("数据列表为空");
        }
        var argsCount = 7;
        // 解析参数
        if (array.Length < argsCount)
        {
            throw new Exception("参数数量错误.需求参数数量:" + argsCount + " 实际数量:" + array.Length);
        }
        // 是否等待完成,特效Key,速度,飞行轨迹
        var formulaType = Convert.ToInt32(array[0]);
        var effectKey = array[1];
        var speed = Convert.ToSingle(array[2]);
        var flyType = (TrajectoryAlgorithmType)Enum.Parse(typeof(TrajectoryAlgorithmType), array[3]);

        float[] scale = new float[3];
        scale[0] = Convert.ToSingle(array[4]);
        scale[1] = Convert.ToSingle(array[5]);
        scale[2] = Convert.ToSingle(array[6]);

        FormulaType = formulaType;
        EffectKey = effectKey;
        Speed = speed;
        FlyType = flyType;
        this.scale = scale;
    }


    /// <summary>
    /// 获取行为构建器
    /// </summary>
    /// <returns>构建完成的单个行为</returns>
    public override IFormula GetFormula(FormulaParamsPacker paramsPacker)
    {
        //UnityEngine.Debug.Log("点对对象");
        // 验证数据正确, 如果有问题直接抛错误
        string errorMsg = null;
        if (paramsPacker == null)
        {
            errorMsg = "调用参数 paramsPacker 为空.";
        }
        else if (EffectKey == null)
        {
            errorMsg = "特效Key(或路径)为空.";
        }
        else if (Speed <= 0)
        {
            errorMsg = "物体飞行速度不合法, <=0";
        }
        else if (paramsPacker.ReceiverMenber == null)
        {
            errorMsg = "弹道特效目标对象为空.";
        }

        if (!string.IsNullOrEmpty(errorMsg))
        {
            throw new Exception(errorMsg);
        }

        // 数据本地化
        var myFormulaType = FormulaType;
        var myEffectKey = EffectKey;
        var mySpeed = Speed;
        var myFlyType = FlyType;
        var myScale = scale;

        IFormula result = new Formula((callback) =>
        {
            // 判断发射与接收位置
            // TODO 父级暂时没有
            EffectsFactory.Single.CreatePointToObjEffect(myEffectKey, null, paramsPacker.StartPos,
                                paramsPacker.ReceiverMenber.GameObj, new Vector3(myScale[0], myScale[1], myScale[2]), mySpeed, myFlyType, callback).Begin();
        }, myFormulaType);

        return result;
    }
}