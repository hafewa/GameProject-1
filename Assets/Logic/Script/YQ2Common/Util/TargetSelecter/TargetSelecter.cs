﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 目标选择器
/// </summary>
public class TargetSelecter
{
    /// <summary>
    /// 单例对象
    /// </summary>
    public static TargetSelecter Single
    {
        get
        {
            if (single == null)
            {
                single = new TargetSelecter();
            }
            return single;
        }
    }

    /// <summary>
    /// 单例对象
    /// </summary>
    private static TargetSelecter single = null;


    /// <summary>
    /// 筛选对象
    /// TODO 优化
    /// TODO 选择己方目标不用去掉自己
    /// </summary>
    /// <typeparam name="T">对象类型. 必须继承</typeparam>
    /// <param name="searchObj">搜索对象</param>
    /// <param name="quadTree">四叉树</param>
    /// <returns></returns>
    public IList<T> TargetFilter<T>(T searchObj, QuadTree<T> quadTree) where T : IBaseMember, IGraphicsHolder
    {
        IList<T> result = null;
        if (searchObj != null && searchObj.AllData.MemberData != null && quadTree != null)
        {
            // 取出范围内的单位
            var range = searchObj.AllData.MemberData.AttackRange;
            //var halfRange = range;
            var inScope =
                quadTree.GetScope(new RectGraphics(new Vector2(searchObj.X, searchObj.Y), range, range, 0));


            result = TargetFilter(searchObj, inScope).Where(targetItem => targetItem != null).ToList();

            // TODO 不放在这里 散射效果
            //if (searchObj.MemberData.SpreadRange > Utils.ApproachZero && result.Count > 0)
            //{
            //    result = Scottering(result[0], searchObj, quadTree);
            //}
        }

        return result;
    }

    /// <summary>
    /// 筛选对象
    /// </summary>
    /// <typeparam name="T">对象类型. 必须继承</typeparam>
    /// <param name="searchObj">搜索对象</param>
    /// <param name="dataList">搜索列表</param>
    /// <returns></returns>
    public IList<T> TargetFilter<T>(T searchObj, IList<T> dataList) where T : IAllDataHolder, IBaseMember, IGraphicsHolder
    {
        IList<T> result = null;
        if (searchObj != null && searchObj.AllData.MemberData != null && dataList != null)
        {


            // 单位数量
            var targetCount = searchObj.AllData.MemberData.MultiAimMax;
            // 目标列表Array
            var targetArray = new T[targetCount];
            // 目标权重值
            var weightKeyArray = new float[targetCount];

            for (var i = 0; i < dataList.Count; i++)
            {
                var item = dataList[i];
                if (item.Equals(searchObj))
                {
                    continue;
                }
                var sumWeight = 0f;


                // 从列表中找到几项权重值最高的目标个数个单位
                // 将各项值标准化, 然后乘以权重求和, 得到最高值

                // -------------------------Level1-----------------------------
                // 排除不可攻击单位
                if (!CouldSelectTarget(item, searchObj))
                {
                    continue;
                }
                // 计算空地属性权重
                switch (item.AllData.MemberData.GeneralType)
                {
                    case Utils.GeneralTypeAir:
                        sumWeight += searchObj.AllData.SelectWeightData.AirWeight;
                        break;
                    case Utils.GeneralTypeBuilding:
                        sumWeight += searchObj.AllData.SelectWeightData.BuildWeight;
                        break;
                    case Utils.GeneralTypeSurface:
                        sumWeight += searchObj.AllData.SelectWeightData.SurfaceWeight;
                        break;
                }

                // -------------------------Level2-----------------------------
                // 计算单位类型权重
                switch (item.AllData.MemberData.ArmyType)
                {
                    case Utils.MemberItemTypeHuman:
                        sumWeight += searchObj.AllData.SelectWeightData.HumanWeight;
                        break;
                    case Utils.MemberItemTypeOrc:
                        sumWeight += searchObj.AllData.SelectWeightData.OrcWeight;
                        break;
                    case Utils.MemberItemTypeOmnic:
                        sumWeight += searchObj.AllData.SelectWeightData.OmnicWeight;
                        break;
                    //case Utils.MemberItemTypeTank:
                    //    sumWeight += searchObj.SelectWeightData.TankWeight;
                    //    break;
                    //case Utils.MemberItemTypeLV:
                    //    sumWeight += searchObj.SelectWeightData.LVWeight;
                    //    break;
                    //case Utils.MemberItemTypeCannon:
                    //    sumWeight += searchObj.SelectWeightData.CannonWeight;
                    //    break;
                    //case Utils.MemberItemTypeAircraft:
                    //    sumWeight += searchObj.SelectWeightData.AirCraftWeight;
                    //    break;
                    //case Utils.MemberItemTypeSoldier:
                    //    sumWeight += searchObj.SelectWeightData.SoldierWeight;
                    //    break;
                }

                // -------------------------Level3-----------------------------
                // 隐形单位
                if (item.AllData.MemberData.IsHide)
                {
                    if (searchObj.AllData.SelectWeightData.HideWeight < 0)
                    {
                        continue;
                    }
                    sumWeight += searchObj.AllData.SelectWeightData.HideWeight;
                }
                // 嘲讽单位
                //if (item.IsTaunt)
                //{
                //    if (searchObj.TauntWeight < 0)
                //    {
                //        continue;
                //    }
                //    sumWeight += searchObj.TauntWeight;
                //}

                // -------------------------Level4-----------------------------
                // 小生命权重, 血越少权重越高
                if (searchObj.AllData.SelectWeightData.HealthMaxWeight > 0)
                {
                    // 血量 (最大血量 - 当前血量)/最大血量 * 生命权重
                    sumWeight += searchObj.AllData.SelectWeightData.HealthMaxWeight * (item.AllData.MemberData.TotalHp - item.AllData.MemberData.CurrentHP) / item.AllData.MemberData.TotalHp;
                }

                // 大生命权重, 生命值越多权重越高
                if (searchObj.AllData.SelectWeightData.HealthMinWeight > 0)
                {
                    // 血量 当前血量/最大血量 * 生命权重
                    sumWeight += searchObj.AllData.SelectWeightData.HealthMinWeight * item.AllData.MemberData.CurrentHP / item.AllData.MemberData.TotalHp;
                }

                //// 角度权重, 角度越大权重越小
                //if (searchObj.SelectWeightData.AngleWeight > 0)
                //{
                //    sumWeight += searchObj.SelectWeightData.AngleWeight * (180 - Vector3.Angle(searchObj.Direction, new Vector3(item.X - searchObj.X, 0, item.Y - searchObj.Y))) / 180;
                //}

                var distance = Utils.GetTwoPointDistance2D(searchObj.X, searchObj.Y, item.X, item.Y);
                // 长距离权重, 距离越远权重越大
                if (searchObj.AllData.SelectWeightData.DistanceMinWeight > 0)
                {
                    sumWeight += searchObj.AllData.SelectWeightData.DistanceMinWeight *
                                 (searchObj.AllData.MemberData.AttackRange - distance) /
                                 searchObj.AllData.MemberData.AttackRange;
                }

                // 短距离权重, 距离越远权重越小
                if (searchObj.AllData.SelectWeightData.DistanceMaxWeight > 0)
                {
                    sumWeight += searchObj.AllData.SelectWeightData.DistanceMaxWeight * distance / searchObj.AllData.MemberData.AttackRange;
                }

                // TODO 各项为插入式结构
                // 比对列表中的值, 大于其中某项值则将其替换位置并讲其后元素向后推一位.
                for (var j = 0; j < weightKeyArray.Length; j++)
                {
                    if (sumWeight > weightKeyArray[j])
                    {
                        for (var k = weightKeyArray.Length - 1; k > j; k--)
                        {
                            weightKeyArray[k] = weightKeyArray[k - 1];
                            targetArray[k] = targetArray[k - 1];
                        }
                        weightKeyArray[j] = sumWeight;
                        targetArray[j] = item;
                        break;
                    }
                }
            }

            result = targetArray.Where(targetItem => targetItem != null).ToList();
        }

        return result;
    }

    /// <summary>
    /// 散射效果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="targetObj">目标单位</param>
    /// <param name="searchObj">搜索单位</param>
    /// <param name="quadTree">对象二叉树</param>
    /// <returns>被散射目标</returns>
    public static List<T> Scottering<T>(T targetObj, T searchObj, QuadTree<T> quadTree) where T : ISelectWeightDataHolder, IBaseMember, IGraphicsHolder//, IGraphical<Rectangle>
    {
        List<T> result = null;
        // 散射, 按照距离搜索周围的单位, 并将其中一个作为本次的目标
        // 如果散射参数有值, 并且筛选列表中存在目标, 则取第一个单位作为筛选目标散射周围单位
        if (searchObj != null && searchObj.AllData.MemberData != null && searchObj.AllData.MemberData.SpreadRange > Utils.ApproachZero && targetObj != null)
        {
            var memberData = searchObj.AllData.MemberData;
            result = new List<T>();
            var target = targetObj;
            // TODO 圆形 取单位周围单位
            var rect = new RectGraphics(new Vector2(target.X, target.Y),
                memberData.SpreadRange * 2, memberData.SpreadRange * 2, 0);
           
            // 距离太近向后推
            var searchPos = new Vector3(searchObj.X, 0, searchObj.Y);
            var targetPos = new Vector3(target.X, 0, target.Y);
            var diffPos = targetPos - searchPos;
            var distance = diffPos.magnitude;
            // 距离小于极限距离
            if (distance < memberData.SpreadRange * 1.2f)
            {
                // 将目标位置向后推
                diffPos = diffPos.normalized * memberData.SpreadRange;
                targetPos = searchPos + diffPos;
                rect.Postion = new Vector2(targetPos.x, targetPos.z);
            }
            Utils.DrawGraphics(rect, Color.red);
            Debug.DrawLine(searchPos, targetPos, Color.red);
            // 散射范围内的单位
            var scatteringList = quadTree.GetScope(rect);
            // 计算命中
            var hitRate = 0f;
            var sumVolume = 0f;
            foreach (var scatteringItem in scatteringList)
            {
                sumVolume += scatteringItem.AllData.MemberData.SpaceSet * scatteringItem.AllData.MemberData.SpaceSet;
            }
            hitRate = (1 - (float)Math.Pow(1 - memberData.Accuracy, sumVolume)) * 100;
            // 先随机命中, 如果命中则返回一个对象
            // 如果没命中则返回空对象
            // 随机一个值从对象中
            var random = new System.Random(DateTime.Now.Millisecond);
            var randomNum = random.Next(100);
            
            if (randomNum <= hitRate)
            {
                // 命中
                result.Add(scatteringList[random.Next(scatteringList.Count)]);
            }
        }
        return result;
    }

    ///// <summary>
    ///// 查找符合条件的对向列表
    ///// </summary>
    ///// <typeparam name="T">对象类型</typeparam>
    ///// <param name="mine">当前对象</param>
    ///// <param name="list">目标对象列表</param>
    ///// <param name="func">判断方法</param>
    ///// <returns>符合条件的对象列表</returns>
    //// 数据符合合并类型, 选择具体功能外抛
    //private static IList<T> Search<T>(T mine, IList<T> list, Func<T, T, bool> func) where T : ISelectWeightData
    //{
    //    List<T> result = null;
    //    if (list != null && func != null && mine != null)
    //    {
    //        result = new List<T>();
    //        T item;
    //        for (var i = 0; i < list.Count; i++)
    //        {
    //            item = list[i];
    //            if (func(mine, item))
    //            {
    //                result.Add(item);
    //            }
    //        }
    //    }
    //    return result;
    //}

    /// <summary>
    /// 判断圆形(扇形),圆形是否碰撞
    /// </summary>
    /// <param name="item">被搜索单位</param>
    /// <param name="scanerX">搜索单位位置X</param>
    /// <param name="scanerY">搜索单位位置Y</param>
    /// <param name="radius">搜索半径</param>
    /// <param name="openAngle">扇形开合角度</param>
    /// <param name="rotate">扇形转动角度</param>
    /// <returns>是否碰撞</returns>
    public static bool IsCollisionItem(IBaseMember item, float scanerX, float scanerY, float radius, float openAngle = 361f, float rotate = 0f)
    {
        if (item == null || item.AllData.MemberData == null)
        {
            return false;
        }

        var memberDta = item.AllData.MemberData;
        // 求距离
        var xOff = item.X - scanerX;
        var yOff = item.Y - scanerY;
        var distanceSquare = xOff * xOff + yOff * yOff;

        var radiusSum = memberDta.SpaceSet * 0.5f + radius;

        // 距离超过半径和不会相交
        if (distanceSquare > radiusSum * radiusSum)
        {
            return false;
        }

        if (openAngle >= 360f)
        {
            // 判断与圆形碰撞
            return true;
        }
        else
        {
            // 判断与扇形碰撞
            var halfOpenAngle = openAngle * 0.5f;
            // 两个点相对圆心方向
            var pointForCorner1 = new Vector2((float)Math.Sin(rotate + halfOpenAngle),
                (float)Math.Cos(rotate + halfOpenAngle));
            var pointForCorner2 = new Vector2((float)Math.Sin(rotate - halfOpenAngle),
                (float)Math.Cos(rotate - halfOpenAngle));
            var circlePosition = new Vector2(item.X, item.Y);
            var sectorPosition = new Vector2(scanerX, scanerY);

            // 判断圆心到扇形两条边的距离
            var distance1 = CollisionGraphics.EvaluatePointToLine(circlePosition, pointForCorner1, sectorPosition);
            var distance2 = CollisionGraphics.EvaluatePointToLine(circlePosition, sectorPosition, pointForCorner2);
            if (distance1 >= 0 && distance2 >= 0)
            {
                // 圆心在扇形开口角度内
                return true;
            }
            // 如果与两线相交则相交
            var circleRadius = memberDta.SpaceSet * 0.5f;
            if (CollisionGraphics.CheckCircleAndLine(circlePosition, circleRadius, sectorPosition, pointForCorner1) ||
                CollisionGraphics.CheckCircleAndLine(circlePosition, circleRadius, sectorPosition, pointForCorner2))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="itemList">被检测列表</param>
    /// <param name="scanerX">搜索范围位置X</param>
    /// <param name="scanerY">搜索范围位置Y</param>
    /// <param name="radius">搜索范围半径</param>
    /// <param name="openAngle">搜索扇形范围开合角度[可选]</param>
    /// <param name="rotate">搜索扇形范围旋转角度[可选]</param>
    /// <returns>范围内有碰撞的单位列表</returns>
    public static IList<PositionObject> GetCollisionItemList(IList<PositionObject> itemList, float scanerX, float scanerY, float radius,
        float openAngle = 361f, float rotate = 0f)
    {
        IList<PositionObject> result = new List<PositionObject>();

        foreach (var item in itemList)
        {
            if (IsCollisionItem(item, scanerX, scanerY, radius, openAngle, rotate))
            {
                result.Add(item);
            }
        }

        return result;
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="ownerList">外层持有类列表</param>
    /// <param name="scanerX">搜索范围位置X</param>
    /// <param name="scanerY">搜索范围位置Y</param>
    /// <param name="radius">搜索范围半径</param>
    /// <param name="openAngle">搜索扇形范围开合角度[可选]</param>
    /// <param name="rotate">搜索扇形范围旋转角度[可选]</param>
    /// <returns>范围内有碰撞的外层持有类列表</returns>
    public static IList<DisplayOwner> GetCollisionItemList(IList<DisplayOwner> ownerList, float scanerX,
        float scanerY, float radius,
        float openAngle = 361f, float rotate = 0f)
    {
        if (ownerList == null)
        {
            return null;
        }
        IList<DisplayOwner> result = new List<DisplayOwner>();

        foreach (var item in ownerList)
        {
            if (IsCollisionItem(item.ClusterData, scanerX, scanerY, radius, openAngle, rotate))
            {
                result.Add(item);
            }
        }

        return result;
    }


    /// <summary>
    /// 是否可以选择目标
    /// </summary>
    /// <param name="targetData">目标数据</param>
    /// <param name="selecterData">选择者数据</param>
    /// <returns>是否可以攻击</returns>
    public static bool CouldSelectTarget<T>(T targetData, T selecterData)where T : IAllDataHolder, IBaseMember
    {
        if (selecterData == null || targetData == null)
        {
            return false;
        }
        var result = false;
        // 如果不可攻击建筑并且是建筑 
        // 或不可攻击空中并且是空中 
        // 或不可攻击地面并且是地面 
        // 或处于死亡或假死状态
        // 或目标是障碍物
        // 则不能选择该单位
        if ((selecterData.AllData.SelectWeightData.BuildWeight < 0 &&
             targetData.AllData.MemberData.GeneralType == Utils.GeneralTypeBuilding) ||
            (selecterData.AllData.SelectWeightData.AirWeight < 0 && targetData.AllData.MemberData.GeneralType == Utils.GeneralTypeAir) ||
            (selecterData.AllData.SelectWeightData.SurfaceWeight < 0 &&
             targetData.AllData.MemberData.GeneralType == Utils.GeneralTypeSurface) ||
            targetData.AllData.MemberData.CurrentHP <= 0 ||
            targetData is FixtureData)
        {
            return false;
        }
        // 如果可选类型相等或目标是建筑
        // 如果目标隐形并且选择者反隐, 或者不隐形
        if ((targetData.AllData.MemberData.IsHide && selecterData.AllData.MemberData.IsAntiHide) || !targetData.AllData.MemberData.IsHide)
        {
            // 目标可被选择
            result = true;
        }

        return result;
    }


}
