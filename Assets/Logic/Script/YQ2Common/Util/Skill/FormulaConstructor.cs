﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


/// <summary>
/// 行为链构造器
/// </summary>
public static class FormulaConstructor
{

    /// <summary>
    /// 已注册行为链生成器的字典
    /// 新增构造器请在此处添加
    /// </summary>
    private static Dictionary<string, Type> registerFormulaItem = new Dictionary<string, Type>()
    {
        {"PointToPoint", typeof(PointToPointFormulaItem)},
        {"PointToObj", typeof(PointToObjFormulaItem)},
        {"Point", typeof(PointFormulaItem)},
        {"CollisionDetection", typeof(CollisionDetectionFormulaItem)},
        {"SlideCollisionDetection", typeof(SlideCollisionDetectionFormulaItem)},
        {"Skill", typeof(SkillFormulaItem)},
        {"Pause", typeof(PauseFormulaItem)},
        {"Move", typeof(MoveFormulaItem)},
    };


    /// <summary>
    /// 构建行为链
    /// </summary>
    /// <param name="info">字符串数据源</param>
    /// <returns>构建完成的行为链</returns>
    public static SkillInfo Constructor(string info)
    {
        // 技能信息
        SkillInfo skillInfo = null;

        if (info != null)
        {
            // 技能ID
            var skillId = 0;
            // 大括号标记
            var braket = false;
            var stackLevel = 0;
            // 创建临时堆栈, 存储不同层级的行为链
            var stack = new Stack<IFormulaItem>();
            IFormulaItem tmpItem = null;

            // 解析字符串
            // 根据对应行为列表创建Formula
            var infoLines = info.Split('\n');
            for (var i = 0; i < infoLines.Length; i++)
            {
                var line = infoLines[i];
                // 跳过空行
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                // 消除空格
                line = line.Trim();
                // 跳过注释行
                if (line.StartsWith("//"))
                {
                    continue;
                }

                // 如果是技能描述开始
                if (line.StartsWith("SkillNum"))
                {
                    // 读取技能号
                    var start = line.IndexOf("(", StringComparison.Ordinal);
                    var end = line.IndexOf(")", StringComparison.Ordinal);
                    if (start < 0 || end < 0)
                    {
                        throw new Exception("转换行为链失败: ()符号不完整, 行数:" + (i + 1));
                    }
                    // 编号长度
                    var length = end - start - 1;
                    if (length <= 0)
                    {
                        throw new Exception("转换行为链失败: ()顺序错误, 行数:" + (i + 1));
                    }

                    // 读取技能ID
                    var strSkillId = line.Substring(start + 1, length);
                    skillId = Convert.ToInt32(strSkillId);
                    //// 创建新技能
                    //skillInfo = new SkillInfo(skillId);
                }
                else if (line.StartsWith("{"))
                {
                    // 开始括号内容
                    stackLevel++;
                    // 创建行为链

                    // 将上级FormulaItem push进堆栈
                    stack.Push(tmpItem);
                    tmpItem = null;
                }
                else if (line.StartsWith("}"))
                {
                    // 关闭括号内容
                    stackLevel--;
                    // 将上级FormulaItem pop出来
                    var prvLevelItem = stack.Pop();
                    if (prvLevelItem != null && tmpItem != null)
                    {
                        prvLevelItem.AddSubFormulaItem(tmpItem.GetFirst());
                        tmpItem = prvLevelItem;
                    }
                }
                else
                {
                    // 解析内容
                    if (stackLevel > 0)
                    {
                        // TODO 判断Formula的stack等级是否与当前stack等级一直? 不一致则新建将其加入上一级formula的子集
                        // 参数列表内容
                        var start = line.IndexOf("(", StringComparison.Ordinal);
                        var end = line.IndexOf(")", StringComparison.Ordinal);

                        if (start < 0 || end < 0)
                        {
                            throw new Exception("转换行为链失败: ()符号不完整, 行数:" + (i + 1));
                        }
                        // 编号长度
                        var length = end - start - 1;
                        if (length <= 0)
                        {
                            throw new Exception("转换行为链失败: ()顺序错误, 行数:" + (i + 1));
                        }

                        // 行为类型
                        var type = line.Substring(0, start);
                        // 行为参数
                        var args = line.Substring(start + 1, length);
                        // 消除参数空格
                        args = args.Replace(" ", "");
                        // 使用参数+名称获取IFormula
                        var item = GetFormula(type, args);
                        // formula加入暂停item
                        var pauseItem = GetFormula("Pause", "1");

                        if (tmpItem == null)
                        {
                            tmpItem = pauseItem;
                        }
                        else
                        {
                            tmpItem = tmpItem.After(pauseItem);
                        }
                        tmpItem = tmpItem.After(item);
                        //skillInfo.AddFormulaItem(pauseItem);

                        //skillInfo.AddFormulaItem(item);
                    }
                    else
                    {
                        Debug.Log("泄漏! 泄漏内容:" + line);
                    }
                }
            }
            if (tmpItem != null)
            {
                // 获得行为链生成器的head
                tmpItem = tmpItem.GetFirst();
                skillInfo = new SkillInfo(skillId);
                skillInfo.AddFormulaItem(tmpItem);
            }
        }

        return skillInfo;
    }

    /// <summary>
    /// 获取行为链
    /// </summary>
    /// <param name="type">行为类型名称</param>
    /// <param name="args">行为</param>
    /// <param name="startPos">施法者位置</param>
    /// <param name="targetPos">目标位置</param>
    /// TODO 封装施法者与目标对象
    /// <returns></returns>
    public static IFormulaItem GetFormula(string type, string args)
    {

        IFormulaItem result = null;
        // 错误消息
        string errorMsg = null;
        if (string.IsNullOrEmpty(type))
        {
            errorMsg = "函数类型为空";
        }
        if (string.IsNullOrEmpty(errorMsg))
        {
            // 分割数据
            var argsArray = args.Split(',');
            // 注册列表中是否包含该类型
            if (registerFormulaItem.ContainsKey(type))
            {
                // 获取该行为的Type
                var formulaItemType = registerFormulaItem[type];
                // 反射方式实例化行为链构造器(因为实例化的是构造器, 所以不用担心效率问题, 实际技能是使用构造器生产产生)
                result = (IFormulaItem)formulaItemType.InvokeMember("", BindingFlags.Public | BindingFlags.CreateInstance,
                    null, null, new object[] { argsArray });
            }
            else
            {
                throw new Exception("未知行为类型: " + type);
            }
        }
        // 如果错误信息不为空则抛出错误
        if (!string.IsNullOrEmpty(errorMsg))
        {
            throw new Exception(errorMsg);
        }
        return result;
    }

    // 结构例子
    /*
     SkillNum(10000)
     {
        PointToPoint(1,key,0,1,10,1,1),     // 需要等待其结束, 特效key(对应key,或特效path), 释放位置, 命中位置, 速度10, 飞行轨迹类型
        Point(0,key,1,0,3),                // 不需要等待其结束, 特效key(对应key,或特效path), 释放位置, 播放速度, 持续3秒
        CollisionDetection(1, 1, 10, 0, 10001)
        {
            Skill(1, 10002, 1)
        }
     }
     
     */
    // -----------------特效-------------------- 
    // PointToPoint 点对点特效        参数 是否等待完成,特效Key,释放位置(0放技能方, 1目标方),命中位置(0放技能方, 1目标方),速度,飞行轨迹,缩放(三位)
    // PointToObj 点对对象特效        参数 是否等待完成,特效Key,速度,飞行轨迹,缩放(三位)
    // Point 点特效                   参数 是否等待完成,特效Key,速度,持续时间,缩放(三位)
    // Scope 范围特效                 参数 是否等待完成,特效Key,释放位置(0放技能方, 1目标方),持续时间,范围半径

    // --------------目标选择方式---------------
    // CollisionDetection 碰撞检测    参数 是否等待完成, 目标数量, 检测位置(0放技能方, 1目标方), 检测范围形状(0圆, 1方), 
    // 目标阵营(-1:都触发, 0: 己方, 1: 非己方),碰撞单位被释放技能ID范围大小(方 第一个宽, 第二个长, 第三个旋转角度, 圆的就取第一个值当半径, 扇形第一个半径, 第二个开口角度, 第三个旋转角度有更多的参数都放进来)
    //{
    //  功能
    //}
    // SlideCollisionDetection 滑动碰撞检测   参数 是否等待完成, 滑动速度, 检测宽度, 检测总长度, 目标阵营(-1:都触发, 0: 己方, 1: 非己方)
    //{
    //  功能
    //}
    // Skill 释放技能                 参数 是否等待完成,被释放技能,技能接收方(0释放者,1被释放者)
    // -----------------音效--------------------
    // Audio 音效                     参数 是否等待完成,点音,持续音,持续时间

    // -----------------buff--------------------
    // Buff buff                      参数 是否等待完成,buffID

    // -----------------结算--------------------
    // Calculate 结算                 参数 是否等待完成,伤害或治疗(0,1),技能编号

    // -----------------技能--------------------
    // Skill 释放技能                 参数 是否等待完成,技能编号

    // -----------------位置--------------------
    // Move 位置移动                  参数 是否等待完成,移动速度,是否瞬移(0: 否, 1: 是(如果是瞬移则速度无效))


}

/// <summary>
/// 技能信息
/// </summary>
public class SkillInfo
{
    /// <summary>
    /// 技能ID
    /// </summary>
    public int SkillNum { get; private set; }

    /// <summary>
    /// 技能行为单元
    /// </summary>
    private IFormulaItem formulaItem = null;

    /// <summary>
    /// 构造技能信息
    /// </summary>
    /// <param name="skillNum">技能ID</param>
    public SkillInfo(int skillNum)
    {
        SkillNum = skillNum;
    }

    /// <summary>
    /// 添加行为生成器
    /// </summary>
    /// <param name="formulaItem">行为单元生成器</param>
    public void AddFormulaItem(IFormulaItem formulaItem)
    {
        if (formulaItem == null)
        {
            throw new Exception("行为节点为空");
        }
        if (this.formulaItem == null)
        {
            this.formulaItem = formulaItem;
        }
        else
        {
            formulaItem.After(formulaItem);
        }
    }

    /// <summary>
    /// 获取行为链
    /// </summary>
    /// <param name="paramsPacker">参数封装</param>
    /// <returns>行为链</returns>
    public IFormula GetFormula(FormulaParamsPacker paramsPacker)
    {
        if (paramsPacker == null)
        {
            throw new Exception("参数封装为空.");
        }
        IFormula result = null;

        if (formulaItem == null)
        {
            return null;
        }
        // 循环构建行为链构造器
        var tmpItem = formulaItem;
        while (tmpItem != null)
        {
            if (result != null)
            {
                result = result.After(tmpItem.GetFormula(paramsPacker));
            }
            else
            {
                result = tmpItem.GetFormula(paramsPacker);
            }
            tmpItem = tmpItem.NextFormulaItem;
        }

        // 构造器不为空
        if (result != null)
        {
            // 获取构造器链head
            result = result.GetFirst();
        }
        return result;
    }
}