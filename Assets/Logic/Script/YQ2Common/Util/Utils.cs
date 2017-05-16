﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = System.Random;


/// <summary>
/// 工具类
/// </summary>
public class Utils
{
    /// <summary>
    /// 单例
    /// </summary>
    public static Utils Single
    {
        get {
            if (single == null)
            {
                single = new Utils();
            } 
            return single;
        }
    }

    /// <summary>
    /// 单例对象
    /// </summary>
    private static Utils single;

    /// <summary>
    /// 队员
    /// </summary>
    public const int Member = 2;

    /// <summary>
    /// 障碍物
    /// </summary>
    public const int Obstacle = 1;

    /// <summary>
    /// 无障碍
    /// </summary>
    public const int Accessibility = 0;

    /// <summary>
    /// 已关闭
    /// </summary>
    public const int Closed = -1;

    /// <summary>
    /// 已打开
    /// </summary>
    public const int Opened = -1;


    // --------------------------单位类型----------------------------
    /// <summary>
    /// 单位类型-士兵
    /// </summary>
    public const short MemberItemTypeSoldier = 1;

    /// <summary>
    /// 单位类型-重型载具(坦克)
    /// </summary>
    public const short MemberItemTypeTank = 2;

    /// <summary>
    /// 单位类型-轻型载具
    /// </summary>
    public const short MemberItemTypeLV = 3;

    /// <summary>
    /// 单位类型-火炮
    /// </summary>
    public const short MemberItemTypeCannon = 4;

    /// <summary>
    /// 单位类型-飞行器
    /// </summary>
    public const short MemberItemTypeAircraft = 5;

    // ----------------------------单位类型--------------------------

    // ----------------------------单位大类型-----------------------------

    /// <summary>
    /// 单位大类型-地面单位
    /// </summary>
    public const short GeneralTypeSurface = 1;

    /// <summary>
    /// 单位大类型-空中单位
    /// </summary>
    public const short GeneralTypeAir = 2;

    /// <summary>
    /// 单位大类型-建筑
    /// </summary>
    public const short GeneralTypeBuilding = 3;

    // ----------------------------单位大类型-----------------------------


    /// <summary>
    /// 弧度转角度常数
    /// </summary>
    public static readonly float RadianToAngle = 180/(float)Math.PI;

    /// <summary>
    /// 角度转弧度常数
    /// </summary>
    public static readonly float AngleToRadian = (float)Math.PI / 180;

    /// <summary>
    /// 弧度转角度常数
    /// </summary>
    public static readonly float HalfPI = (float)Math.PI * 0.5f;

    /// <summary>
    /// 未计算标记
    /// </summary>
    public static readonly int NotCompleted = -998;

    /// <summary>
    /// 无解标记
    /// </summary>
    public static readonly int NoValue = -98;

    /// <summary>
    /// Float类型符号位
    /// </summary>
    public static readonly long FloatSign = 0x80000000;

    /// <summary>
    /// Double类型符号位
    /// </summary>
    public static readonly ulong DoubleSign = 0x8000000000000000;

    /// <summary>
    /// 趋近0值
    /// </summary>
    public static readonly float ApproachZero = 0.00001f;

    /// <summary>
    /// 负趋近0值
    /// </summary>
    public static readonly float ApproachKZero = -0.00001f;


    /// <summary>
    /// 计时器tick
    /// </summary>
    private MonoEX.Timer _timer;

    /// <summary>
    /// 计时器单次tick时间
    /// </summary>
    private const float TickTime = 0.01f;


    //-------------------------静态方法--------------------------

    /// <summary>
    /// 位置转行列
    /// </summary>
    /// <param name="planePosOffset">地图底板位置偏移</param>
    /// <param name="position">当前在plane上的位置(区间, 比如0-1 为同一个位置)</param>
    /// <param name="unitWidth">单位宽度</param>
    /// <param name="mapWidth">地图宽度</param>
    /// <param name="mapHight">地图高度</param>
    /// <returns>map中的行列位置 0位x 1位y</returns>
    public static int[] PositionToNum(Vector3 planePosOffset, Vector3 position, float unitWidth, float mapWidth, float mapHight)
    {
        var x = (int)((position.x - planePosOffset.x + mapWidth * unitWidth * 0.5f) / unitWidth);
        var y = (int)((position.z - planePosOffset.z + mapHight * unitWidth * 0.5f) / unitWidth);
        if (x < 0)
        {
            x = 0;
        }
        if (x >= mapWidth)
        {
            x = (int)mapWidth - 1;
        }
        if (y < 0)
        {
            y = 0;
        }
        if (y >= mapHight)
        {
            y = (int)mapHight - 1;
        }
        return new[] { x, y };
    }


    /// <summary>
    /// 行列转位置
    /// </summary>
    /// <param name="planePosOffset">地图底板位置偏移</param>
    /// <param name="num">map中的行列位置</param>
    /// <param name="unitWidth">单位宽度</param>
    /// <param name="mapWidth">地图宽度</param>
    /// <param name="mapHight">地图高度</param>
    /// <returns>当前plane对应位置, 固定位置的中心点</returns>
    public static Vector3 NumToPosition(Vector3 planePosOffset, Vector2 num, float unitWidth, float mapWidth, float mapHight)
    {
        var result = new Vector3(
            planePosOffset.x - mapWidth * unitWidth * 0.5f
            //+ unitWidth * 0.5f 
            + num.x * unitWidth,
            planePosOffset.y + unitWidth,
            planePosOffset.z - mapHight * unitWidth * 0.5f
            //+ unitWidth * 0.5f
            + num.y * unitWidth);

        return result;
    }


    /// <summary>
    /// 判断点是否在区域内
    /// </summary>
    /// <param name="rectX1">区域点1x</param>
    /// <param name="rectY1">区域点1y</param>
    /// <param name="rectX2">区域点2x</param>
    /// <param name="rectY2">区域点2y</param>
    /// <param name="pointX">位置点x</param>
    /// <param name="pointY">位置点y</param>
    /// <returns></returns>
    public static bool IsCoverage(float rectX1, float rectY1, float rectX2, float rectY2, float pointX, float pointY)
    {
        // 在区域内 返回true
        if (pointX > rectX1 && pointX < rectX2 && pointY > rectY1 && pointY < rectY2)
        {
            return true;
        }

        return false;
    }



    /// <summary>
    /// 泛型排序
    /// 要求泛型继承对比接口
    /// </summary>
    /// <typeparam name="T">传入泛型</typeparam>
    /// <param name="list">排序列表</param>
    /// <param name="isDesc">是否反向排序</param>
    /// <param name="compareTo">对比方法, 参数一大于参数二返回1相等返回0否则返回-1</param>
    /// <returns></returns>
    public static T GetNodeFromListWithCompare<T>(IEnumerable<T> list, bool isDesc, Func<T, T, int> compareTo)
    {
        if (list == null || compareTo == null)
        {
            return default(T);
        }

        var head = list.First();
        var compareValue = 0;
        foreach (var item in list)
        {
            compareValue = compareTo(item, head);
            if (compareValue > 0 && !isDesc || compareValue < 0 && isDesc)
            {
                head = item;
            }
        }
        return head;
    }

    /// <summary>
    /// 绘制图形
    /// </summary>
    /// <param name="graphics">图形对象</param>
    public static void DrawGraphics(ICollisionGraphics graphics, Color color)
    {
        if (graphics == null)
        {
            return;
        }
        switch (graphics.GraphicType)
        {
            case GraphicType.Circle:
                var circle = graphics as CircleGraphics;
                if (circle != null)
                {
                    DrawCircle(new Vector3(circle.Postion.x, 0, circle.Postion.y), circle.Radius, color);
                }
                break;
            case GraphicType.Rect:
                var rect = graphics as RectGraphics;
                if (rect != null)
                {
                    DrawRect(new Vector3(rect.Postion.x, 0, rect.Postion.y), rect.Width, rect.Height, rect.Rotation, color);
                }
                break;
            case GraphicType.Sector:
                var sector = graphics as SectorGraphics;
                if (sector != null)
                {
                    DrawSector(new Vector3(sector.Postion.x, 0, sector.Postion.y), sector.Radius, sector.Rotation, sector.OpenAngle, color);
                }
                break;
        }
    }

    ///// <summary>
    ///// 回执矩形
    ///// </summary>
    ///// <param name="rectangle"></param>
    ///// <param name="color"></param>
    //public static void DrawRect(Rectangle rectangle, Color color)
    //{
    //    Debug.DrawLine(new Vector3(rectangle.X, 0, rectangle.Y), new Vector3(rectangle.X, 0, rectangle.Y + rectangle.Height), color);
    //    Debug.DrawLine(new Vector3(rectangle.X, 0, rectangle.Y), new Vector3(rectangle.X + rectangle.Width, 0, rectangle.Y), color);
    //    Debug.DrawLine(new Vector3(rectangle.X + rectangle.Width, 0, rectangle.Y + rectangle.Height), new Vector3(rectangle.X, 0, rectangle.Y + rectangle.Height), color);
    //    Debug.DrawLine(new Vector3(rectangle.X + rectangle.Width, 0, rectangle.Y + rectangle.Height), new Vector3(rectangle.X + rectangle.Width, 0, rectangle.Y), color);
    //}

    /// <summary>
    /// 回执矩形
    /// </summary>
    /// <param name="position">举行中心位置</param>
    /// <param name="width">举行宽度</param>
    /// <param name="height">举行高度</param>
    /// <param name="rotation">基于矩形中心旋转角度</param>
    /// <param name="color">绘制颜色</param>
    public static void DrawRect(Vector3 position, float width, float height, float rotation, Color color)
    {
        var angle = rotation*Math.PI/180;
        var halfWidth = width*0.5f;
        var halfHeight = height*0.5f;

        var sin = (float) (Math.Sin(angle));
        var cos = (float) (Math.Cos(angle));
        var left = (-halfWidth);
        var right = (halfWidth);
        var top = (halfHeight);
        var bottom = (-halfHeight);
        var point1 = new Vector3(left * cos - bottom * sin, 0, left * sin + bottom * cos) + position;
        var point2 = new Vector3(left * cos - top * sin, 0, left * sin + top * cos) + position;
        var point3 = new Vector3(right * cos - top * sin, 0, right * sin + top * cos) + position;
        var point4 = new Vector3(right * cos - bottom * sin, 0, right * sin + bottom * cos) + position;
        Debug.DrawLine(point1, point2, color);
        Debug.DrawLine(point2, point3, color);
        Debug.DrawLine(point3, point4, color);
        Debug.DrawLine(point4, point1, color);
    }

    /// <summary>
    /// 绘制扇形
    /// </summary>
    /// <param name="position">圆心位置</param>
    /// <param name="radius">圆半径</param>
    /// <param name="rotation">扇形旋转角度</param>
    /// <param name="openAngle">扇形开口角度</param>
    /// <param name="color">绘制颜色</param>
    public static void DrawSector(Vector3 position, float radius, float rotation, float openAngle, Color color)
    {

    }

    /// <summary>
    ///  绘制三角形
    /// </summary>
    /// <param name="point1">三角形点1</param>
    /// <param name="point2">三角形点2</param>
    /// <param name="point3">三角形点3</param>
    /// <param name="color">绘制颜色</param>
    public static void DrawTriangle(Vector3 point1, Vector3 point2, Vector3 point3, Color color)
    {
        Debug.DrawLine(point1, point2, color);
        Debug.DrawLine(point2, point3, color);
        Debug.DrawLine(point3, point1, color);
    }


    /// <summary>
    /// 绘制圆
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius"></param>
    /// <param name="color">颜色</param>
    public static void DrawCircle(Vector3 position, float radius, Color color)
    {
        // 绘制圆环
        Vector3 beginPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.1f)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 endPoint = position + new Vector3(x, 0, z);
            if (Math.Abs(theta) < Utils.ApproachZero)
            {
                firstPoint = endPoint;
            }
            else
            {
                Debug.DrawLine(beginPoint, endPoint, color);
            }
            beginPoint = endPoint;
        }

        // 绘制最后一条线段
        Debug.DrawLine(firstPoint, beginPoint, color);

        //Debug.DrawLine(firstPoint, firstPoint + new Vector3(radius, 0, 0), color);
    }

    /// <summary>
    /// 获取node的key值
    /// </summary>
    /// <param name="x">位置x</param>
    /// <param name="y">位置y</param>
    /// <returns>key值</returns>
    public static long GetKey(long x, long y)
    {
        var result = (x << 32) + y;
        return result;
    }

    // ---------------------------图形-------------------------------

    /// <summary>
    /// 获取矩形水平检测线
    /// </summary>
    /// <param name="rotation">旋转角度-360-360°</param>
    /// <returns>水平检测线标量</returns>
    public static Vector2 GetHorizonalTestLine(float rotation)
    {
        var angle = rotation * Math.PI / 360;
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    }

    /// <summary>
    /// 获取矩形垂直检测线
    /// </summary>
    /// <param name="rotation">旋转角度-360-360°</param>
    /// <param name="radius">检测线长度</param>
    /// <returns>垂直检测线标量</returns>
    public static Vector2 GetVerticalTestLine(float rotation, float radius = 1f)
    {
        var angle = rotation * Math.PI / 360;
        return new Vector2(-(float)Math.Sin(angle) * radius, (float)Math.Cos(angle) * radius);
    }

    /// <summary>
    /// 计算点到线的距离
    /// </summary>
    /// <param name="lineP1">线点1</param>
    /// <param name="lineP2">线点2</param>
    /// <param name="point">求距离点</param>
    /// <returns>点到线最短距离</returns>
    public static float GetDistancePointToLine(Vector2 lineP1, Vector2 lineP2, Vector2 point)
    {
        float a = lineP2.y - lineP1.y;
        float b = lineP1.x - lineP2.x;
        float c = lineP2.x * lineP1.y - lineP1.x * lineP2.y;

        return (float)(Math.Abs(a * point.x + b * point.y + c) / Math.Sqrt(a * a + b * b));

    }

    /// <summary>
    /// 计算两点之间距离
    /// </summary>
    /// <param name="point1">点1</param>
    /// <param name="point2">点2</param>
    /// <returns>距离</returns>
    public static float GetDistancePointToPoint(Vector2 point1, Vector2 point2)
    {
        var xOff = (point2.x - point1.x);
        var yOff = (point2.y - point2.y);
        return (float)Math.Sqrt(xOff * xOff + yOff * yOff);
    }

    // ---------------------------文件操作----------------------------

    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="fileName">文件名</param>
    /// <param name="info">文件内容</param>
    public static void CreateOrOpenFile(string path, string fileName, string info)
    {
        StreamWriter sw;
        FileInfo fi = new FileInfo(path + Path.AltDirectorySeparatorChar + fileName);
        sw = fi.CreateText();
        sw.WriteLine(info);
        sw.Close();
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns>文件内容, 如果文件不存在则返回null</returns>
    public static string LoadFileInfo(string path)
    {
        string result = null;

        StreamReader sr;
        var fi = new FileInfo(path);
        if (fi.Exists)
        {
            sr = new StreamReader(fi.OpenRead());
            result = sr.ReadToEnd();
        }

        return result;
    }

    /// <summary>
    /// 求射出角度
    /// 角度 = arctan((speed^2 +- √(speed^4 - g(g*x^2 + 2yspeed^2)))/(gx))
    /// </summary>
    /// <param name="targetPos">目标位置</param>
    /// <param name="startPos">其实位置</param>
    /// <param name="speed">速度</param>
    /// <param name="gravity">重力加速度</param>
    /// <returns>射出角度, 如果是小于-998则无解, 否则返回射出的弧度值</returns>
    public static float GetTheta(Vector3 targetPos, Vector3 startPos, float speed, float gravity)
    {
        var speedSquare = speed*speed;
        var offsetTarget = targetPos - startPos;

        // 计算角度全部取正值
        if (offsetTarget.x < 0)
        {
            offsetTarget.x = -offsetTarget.x;
        }

        // 求根号中的中间量, 求是否有解
        var delta = speedSquare * speedSquare - gravity * (gravity * offsetTarget.x * offsetTarget.x + 2 * offsetTarget.y * speedSquare);
        if (delta < 0)
        {
            // 无解
            return NoValue - 1;
        }

        var theta1 = (float)Math.Atan((speedSquare + Math.Sqrt(delta)) / (gravity * offsetTarget.x));
        var theta2 = (float)Math.Atan((speedSquare - Math.Sqrt(delta)) / (gravity * offsetTarget.x));
        if (theta1 < 0) {
            theta1 = theta2;
        }else if (theta2 < theta1) {
            // 取小角度解
            theta1 = theta2;
        }

        return theta1;
    }

    /// <summary>
    /// 求两点间距离2d
    /// </summary>
    /// <param name="x1">点1x</param>
    /// <param name="y1">点1y</param>
    /// <param name="x2">点2x</param>
    /// <param name="y2">点2y</param>
    /// <returns>两点距离</returns>
    public static float GetTwoPointDistance2D(float x1, float y1, float x2, float y2)
    {
        var xOffset = x1 - x2;
        var yOffset = y1 - y2;
        return (float)Math.Sqrt(xOffset*xOffset + yOffset*yOffset);
    }

    /// <summary>
    /// 拷贝int二维数组
    /// </summary>
    /// <param name="from">数据源</param>
    /// <param name="to">目标数组</param>
    /// <param name="rowCount">行数</param>
    /// <param name="colCount">列数</param>
    public static void CopyArray(int[][] from,out int[][] to, int rowCount, int colCount)
    {
        if (from == null || rowCount <= 0 || colCount <= 0)
        {
            throw new Exception("拷贝数据错误.");
        }
        to = new int[rowCount][];
        for (var i = 0; i < rowCount; i++)
        {
            to[i] = new int[colCount];
            for (var j = 0; j < colCount; j++)
            {
                to[i][j] = from[i][j];
            }
        }
    }

    /// <summary>
    /// 拷贝int一维数组
    /// </summary>
    /// <param name="from">数据源</param>
    /// <param name="to">目标数组</param>
    /// <param name="length">拷贝长度</param>
    public static void CopyArray<T>(T[] from, T[] to, int length)
    {
        if (from == null || to == null || length <= 0 || from.Length < length || to.Length < length)
        {
            return;
        }
        for (var i = 0; i < length; i++)
        {
            to[i] = from[i];
        }
    }

    


    // ---------------------静态方法-------------------------


    // ---------------------连贯视野--------------------------


    /// <summary>
    /// 对物体进行连续移动
    /// </summary>
    /// <param name="paramList">位移列表</param>
    /// <param name="obj">被位移对象</param>
    /// <param name="frameRate">游戏帧数</param>
    /// <param name="completeCallback">回调方法</param>
    public void MoveAndRotateObj(IList<MoveTarget> paramList, GameObject obj, int frameRate, Action completeCallback)
    {

        // 验证参数有效
        if (obj == null || paramList == null)
        {
            return;
        }
        // 中断
        StopMove();

        // 本地化数据
        var tmpParamEnumerator = paramList.GetEnumerator();
        var tmpObj = obj;
        var tmpCompleteCallback = completeCallback;


        MonoEX.TimerCallback tick = null;

        tmpParamEnumerator.MoveNext();

        // tick定义
        tick = () =>
        {
            // 获取迭代器当前对象
            var param = tmpParamEnumerator.Current;
            // 计算移动次数
            if (param != null)
            {
                if (param.MoveObj == null)
                {
                    param.MoveObj = tmpObj;
                }
                if (param.FrameRate == 0)
                {
                    param.FrameRate = frameRate;
                }

                // 是否移动结束
                if (!param.MoveNext())
                {
                    if (tmpParamEnumerator.MoveNext())
                    {
                        // tick
                        _timer = new MonoEX.Timer(TickTime).Play().OnComplete(tick);
                    }
                    else
                    {
                        // 结束回调
                        if (tmpCompleteCallback != null)
                        {
                            tmpCompleteCallback();
                        }
                    }
                }
                else
                {
                    _timer = new MonoEX.Timer(TickTime).Play().OnComplete(tick);
                }
            }
        };

        // 开始tick
        _timer = new MonoEX.Timer(TickTime).Play().OnComplete(tick);
    }

    /// <summary>
    /// 停止当前连续移动
    /// </summary>
    public void StopMove()
    {
        if (_timer != null)
        {
            _timer.Kill();
            Debug.Log("tick is Killed!");
        }
    }

    // ---------------------连贯视野--------------------------
}


/// <summary>
/// 移动目标虚类
/// </summary>
public abstract class MoveTarget
{
    /// <summary>
    /// 被位移对象
    /// </summary>
    public GameObject MoveObj { get; set; }

    /// <summary>
    /// 游戏帧数
    /// 用于计算单位位移
    /// </summary>
    public int FrameRate { get; set; }

    /// <summary>
    /// 下一次移动
    /// 如果移动结束返回false
    /// 否则返回true
    /// </summary>
    /// <returns></returns>
    public abstract bool MoveNext();
}

/// <summary>
/// 相机目标
/// 该类储存相机的位移, 转向等操作数据
/// </summary>
public class MoveAndRotateTarget : MoveTarget
{
    // --------------------------公共变量---------------------------
    /// <summary>
    /// 相机位移目方向
    /// </summary>
    public Vector3 MoveDirection { set; get; }

    /// <summary>
    /// 相机位移速度
    /// 每秒走过的距离 单位 米
    /// 该值可以为负数
    /// </summary>
    public float MoveSpeed { set; get; }

    /// <summary>
    /// 相机位移时间
    /// 在该时间内完成移动 单位秒
    /// 该值必须为正值或0, 如果赋值负数则置为0
    /// </summary>
    public float MoveTime
    {
        set { _moveTime = value < 0 ? 0 : value; }
        get { return _moveTime; }
    }

    /// <summary>
    /// Vector3的三个值对应每个轴旋转角度
    /// </summary>
    public Vector3 RotateTarget { set; get; }

    /// <summary>
    /// 相机旋转时间
    /// 在该时间内完成旋转 单位秒
    /// </summary>
    public float RotateTime
    {
        set { _rotateTime = value < 0 ? 0 : value; }
        get { return _rotateTime; }
    }

    // --------------------------公共变量---------------------------

    // --------------------------私有变量---------------------------
    /// <summary>
    /// 移动时间
    /// </summary>
    private float _moveTime;

    /// <summary>
    /// 旋转时间
    /// </summary>
    private float _rotateTime;


    /// <summary>
    /// 单位位移计数器
    /// </summary>
    private int _counter;

    /// <summary>
    /// 位移总次数
    /// </summary>
    private int _moveTimes;

    /// <summary>
    /// 单位位移
    /// </summary>
    private Vector3 _unitMove;

    /// <summary>
    /// 旋转总次数
    /// </summary>
    private int _rotateTimes;

    /// <summary>
    /// 单位旋转
    /// </summary>
    private Quaternion _unitQuaternion;


    private bool _isInit = false;

    // --------------------------私有变量---------------------------

    /// <summary>
    /// 下一次位移
    /// </summary>
    /// <returns>是否移动结束</returns>
    public override bool MoveNext()
    {
        // 验证数据
        if (MoveObj == null || FrameRate == 0)
        {
            Debug.LogError("数据未初始化.");
            return false;
        }
        // 初始化数据, 初始化后不再改变, 除非游戏帧数有变化, 否则该实现没问题
        if (!_isInit)
        {
            // 位移次数
            _moveTimes = (int)(FrameRate * MoveTime);
            // 单位移动位置
            _unitMove = MoveDirection * MoveSpeed;

            // 计算旋转次数
            _rotateTimes = (int)(FrameRate * RotateTime);
            // 单位转动角度
            _unitQuaternion = Quaternion.Euler(
                RotateTarget.x / _rotateTimes,
                RotateTarget.y / _rotateTimes,
                RotateTarget.z / _rotateTimes);
            _isInit = true;
        }

        // 是否结束标志
        var notOver = false;
        // 判断位移是否结束
        if (_counter < _moveTimes)
        {
            MoveObj.transform.localPosition += _unitMove;
            notOver = true;
        }
        // 判断转角是否结束
        if (_counter < _rotateTimes)
        {
            MoveObj.transform.localRotation = MoveObj.transform.localRotation * _unitQuaternion;
            notOver = true;
        }

        _counter++;
        return notOver;
    }
}

/// <summary>
/// 跟随目标
/// </summary>
public class FollowTarget : MoveTarget
{
    /// <summary>
    /// 跟随目标对象
    /// </summary>
    public GameObject TargetObj { get; set; }

    /// <summary>
    /// 相对于目标体的位置与距离
    /// </summary>
    public Vector3 RelativePos { get; set; }

    /// <summary>
    /// 跟随时的相对z轴朝向
    /// </summary>
    public Vector3 Direction { get; set; }

    /// <summary>
    /// 跟随时间
    /// 时间结束退出跟随
    /// </summary>
    public float FollowTime { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    private float _startTime = -1;

    /// <summary>
    /// 下一次位移
    /// </summary>
    /// <returns>是否移动结束</returns>
    public override bool MoveNext()
    {
        // 验证数据
        if (TargetObj == null || MoveObj == null || Direction == Vector3.zero)
        {
            Debug.LogError("FollowTarget: 跟随失败, 目标对象或方向值非法.");
            return false;
        }

        // 初始化
        if (_startTime < 0)
        {
            _startTime = Time.time;
        }

        // 判断时间是否结束
        if (Time.time - _startTime > FollowTime)
        {
            Debug.Log("FollowTarget End");
            return false;
        }

        // 获取目标位置
        var targetPos = TargetObj.transform.localPosition;
        // 目标位置+relativePos
        var relativePos = targetPos + RelativePos;
        // 自己赋值
        MoveObj.transform.localPosition = relativePos;
        // 转向
        MoveObj.transform.localRotation = Quaternion.Euler(Direction);
        return true;
    }
}

/// <summary>
/// 跟随旋转
/// </summary>
public class FollowAndAroundTarget : MoveTarget
{

    /// <summary>
    /// 跟随目标对象
    /// </summary>
    public GameObject TargetObj { get; set; }

    /// <summary>
    /// 旋转半径
    /// </summary>
    public float Radius { get; set; }

    /// <summary>
    /// Y轴偏移
    /// </summary>
    public float YOffset { get; set; }

    /// <summary>
    /// 旋转速度
    /// </summary>
    public float RotateSpeed { get; set; }

    /// <summary>
    /// 旋转开始角度
    /// 如果角度大于360或小于-360则旋转超过1圈
    /// </summary>
    public float StartAngle { get; set; }

    /// <summary>
    /// 旋转结束角度
    /// 如果角度大于360或小于-360则旋转超过1圈
    /// </summary>
    public float EndAngle { get; set; }

    /// <summary>
    /// 是否为正向旋转
    /// </summary>
    public bool IsPositive { get; set; }


    /// <summary>
    /// 移动计数器
    /// </summary>
    private int _counter = 0;

    /// <summary>
    /// 是否已初始化
    /// </summary>
    private bool _notInit = true;


    private float _angle = 0;


    /// <summary>
    /// 下一次位移
    /// </summary>
    /// <returns>是否移动结束</returns>
    public override bool MoveNext()
    {
        if (TargetObj == null || MoveObj == null)
        {
            Debug.LogError("FollowAndAroundTarget: 跟随失败, 目标对象或方向值非法.");
            return false;
        }
        // 如果未初始化 初始化起始角度与旋转正向负向
        if (_notInit)
        {
            // 旋转角度
            _angle = EndAngle - StartAngle;
            // 无差角, 不旋转
            if (_angle < 0.00001f)
            {
                return false;
            }
            // 旋转方向
            IsPositive = _angle > 0 || ((_angle < 0.00001f) && IsPositive);

            // 规格化
            if (_angle > 0 && StartAngle > 360)
            {
                StartAngle -= StartAngle / 360 * 360;
                EndAngle = EndAngle - EndAngle / 360 * 360 + _angle;
            }
            if (_angle < 0 && StartAngle < -360)
            {
                StartAngle -= StartAngle / 360 * 360;
                EndAngle = EndAngle - EndAngle / 360 * 360 + _angle;
            }
            // 初始化完毕
            _notInit = false;
            _counter = Math.Abs((int)(StartAngle / RotateSpeed));
        }

        var targetPos = TargetObj.transform.localPosition;
        // --------------------------计算位置----------------------------
        // 每帧旋转角度
        // 以中心点为基准使用
        // x = x_center + Math.Sin(2 * Math.PI / 360) * r, 
        // y = y_center + Math.Cos(2 * Math.PI / 360) * r
        // 的公式加上速度计算当前位置
        var radian = (2 * Math.PI / 360) * RotateSpeed * _counter;
        var pointX = targetPos.x + Math.Sin(radian) * Radius * (IsPositive ? 1 : -1);
        var pointY = targetPos.z + Math.Cos(radian) * Radius * (IsPositive ? 1 : -1);

        // 设置位置
        MoveObj.transform.localPosition = new Vector3((float)pointX, targetPos.y + YOffset, (float)pointY);
        // --------------------------计算位置----------------------------

        // --------------------------计算旋转----------------------------

        // 获取相机与目标位置差值
        var posDiff = targetPos - MoveObj.transform.localPosition;

        // 计算三个平面上的角度旋转
        posDiff.Normalize();
        // Y轴旋转
        var yRotate = Quaternion.AngleAxis(RotateSpeed * _counter + 180, Vector3.up);
        // 求与当前运行轨迹圆的切线向量
        var tangent = Vector3.Cross(
            new Vector3(
                (float)Math.Sin((RotateSpeed * _counter + 180) * Math.PI / 180),
                0,
                (float)Math.Cos((RotateSpeed * _counter + 180) * Math.PI / 180)),
            Vector3.down);
        // 相对X轴旋转
        var xRotate = Quaternion.AngleAxis(angle: (float)(90 - Math.Atan(d: Radius / YOffset) / Math.PI * 180), axis: tangent);
        MoveObj.transform.localRotation = xRotate * yRotate;
        // --------------------------计算旋转---------------------------- 
        _counter++;
        // 计算结束点
        var radianNow = (_counter - 1) * RotateSpeed;
        if (IsPositive && (EndAngle < radianNow) || !IsPositive && (radianNow < EndAngle))
        {
            Debug.Log("FollowAndAroundTarget End");
            return false;
        }

        _counter++;
        return true;
    }
}