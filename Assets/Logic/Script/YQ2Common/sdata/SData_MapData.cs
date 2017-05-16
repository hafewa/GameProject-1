﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SData_MapData : MonoEX.Singleton<SData_MapData>
{
    public SData_MapData()
    {
        MonoEX.Debug.Logout(MonoEX.LOG_TYPE.LT_INFO, "开始装载 MapData");

        using (ITabReader reader = TabReaderManage.Single.CreateInstance())
        {
            reader.Load("bsv", "MapData");

            short IMapMaxRow = reader.ColumnName2Index("MapMaxRow");
            short IMapMaxColumn = reader.ColumnName2Index("MapMaxColumn");
            short Iterrain_cell_bianchang = reader.ColumnName2Index("terrain_cell_bianchang");
            short ICamera_ZhuanshenTime = reader.ColumnName2Index("Camera_ZhuanshenTime");
            short ICamera_ZhuanshenCD = reader.ColumnName2Index("Camera_ZhuanshenCD");
            short ICamera_XOffsetMin = reader.ColumnName2Index("Camera_XOffsetMin");
            short ICamera_XOffsetMax = reader.ColumnName2Index("Camera_XOffsetMax");
            short ICamera_YOffsetMin = reader.ColumnName2Index("Camera_YOffsetMin");
            short ICamera_YOffsetMax = reader.ColumnName2Index("Camera_YOffsetMax");
            short ICamera_ZOffsetMin = reader.ColumnName2Index("Camera_ZOffsetMin");
            short ICamera_ZOffsetMax = reader.ColumnName2Index("Camera_ZOffsetMax");
            short ICamera_RotationMin = reader.ColumnName2Index("Camera_RotationMin");
            short ICamera_RotationMax = reader.ColumnName2Index("Camera_RotationMax");
            short IFreeCamera_Scale = reader.ColumnName2Index("FreeCamera_Scale");
            short IFreeCamera_Scale_End = reader.ColumnName2Index("FreeCamera_Scale_End");
            short ICameraY_Xuanzhuan_Start = reader.ColumnName2Index("CameraY_Xuanzhuan_Start");
            short ICameraY_Xuanzhuan_End = reader.ColumnName2Index("CameraY_Xuanzhuan_End");
            short IFreeCamera_Scale_StopRange_Min = reader.ColumnName2Index("FreeCamera_Scale_StopRange_Min");
            short IFreeCamera_Scale_StopRange_Max = reader.ColumnName2Index("FreeCamera_Scale_StopRange_Max");
            short ICameraY_Xuanzhuan_StopRange_Min = reader.ColumnName2Index("CameraY_Xuanzhuan_StopRange_Min");
            short ICameraY_Xuanzhuan_StopRange_Max = reader.ColumnName2Index("CameraY_Xuanzhuan_StopRange_Max");

            short ICamera_Rotate_Radius = reader.ColumnName2Index("Camera_Rotate_Radius");
            short ICamera_Rotate_YOffset = reader.ColumnName2Index("Camera_Rotate_Yoffset");
            short ICamera_Rotate_Speed = reader.ColumnName2Index("Camera_Rotate_Speed");

            short ICamera_Overall_Yoffset = reader.ColumnName2Index("Camera_Overall_Yoffset");
            short ICamera_Overall_Zoffset = reader.ColumnName2Index("Camera_Overall_Zoffset");
            short ICamera_Overall_Yrotate = reader.ColumnName2Index("Camera_Overall_Yrotate");
            short ICamera_Overall_Xrotate = reader.ColumnName2Index("Camera_Overall_Xrotate");

            short IFreeCamera_Scale_Speed = reader.ColumnName2Index("FreeCamera_Scale_Speed");
            short IFreeCameraXmin = reader.ColumnName2Index("FreeCameraXmin");
            short IFreeCameraXmax = reader.ColumnName2Index("FreeCameraXmax");
            short IFreeCameraZmin = reader.ColumnName2Index("FreeCameraZmin");
            short IFreeCameraZmax = reader.ColumnName2Index("FreeCameraZmax");
            short ICameraY_XuanzhuanMin = reader.ColumnName2Index("CameraY_XuanzhuanMin");
            short ICameraY_XuanzhuanMax = reader.ColumnName2Index("CameraY_XuanzhuanMax");
            short ICameraY_XuanzhuanSpeed = reader.ColumnName2Index("CameraY_XuanzhuanSpeed");


            int rowCount = reader.GetRowCount();
            for (int row = 0; row < rowCount; row++)
            {
                MapMaxRow = reader.GetI16(IMapMaxRow, row);
                MapMaxColumn = reader.GetI16(IMapMaxColumn, row);
                HarfMapMaxColumn = (short)(MapMaxColumn / 2);
                TerrainCellBianchang = (float)reader.GetI16(Iterrain_cell_bianchang, row) / 10f;
                Camera_ZhuanshenTime = (float)reader.GetI16(ICamera_ZhuanshenTime, row) / 1000f;
                Camera_ZhuanshenCD = (float)reader.GetI16(ICamera_ZhuanshenCD, row) / 1000f;

                FreeCamera_Scale = reader.GetF(IFreeCamera_Scale, row);

                FreeCamera_Scale_End = reader.GetF(IFreeCamera_Scale_End, row);
                CameraY_Xuanzhuan_Start = reader.GetF(ICameraY_Xuanzhuan_Start, row);
                CameraY_Xuanzhuan_End = reader.GetF(ICameraY_Xuanzhuan_End, row);


                FreeCamera_Scale_StopRange_Min = reader.GetF(IFreeCamera_Scale_StopRange_Min, row);
                FreeCamera_Scale_StopRange_Max = reader.GetF(IFreeCamera_Scale_StopRange_Max, row);
                CameraY_Xuanzhuan_StopRange_Min = reader.GetF(ICameraY_Xuanzhuan_StopRange_Min, row);
                CameraY_Xuanzhuan_StopRange_Max = reader.GetF(ICameraY_Xuanzhuan_StopRange_Max, row);


                Camera_Overall_Zoffset = reader.GetF(ICamera_Overall_Zoffset, row);
                Camera_Overall_Yoffset = reader.GetF(ICamera_Overall_Yoffset, row);
                Camera_Overall_Yrotate = reader.GetF(ICamera_Overall_Yrotate, row);
                Camera_Overall_Xrotate = reader.GetF(ICamera_Overall_Xrotate, row);


                Camera_Rotate_Radius = reader.GetF(ICamera_Rotate_Radius, row);
                Camera_Rotate_YOffset = reader.GetF(ICamera_Rotate_YOffset, row);
                Camera_Rotate_Speed = reader.GetF(ICamera_Rotate_Speed, row);


                Camera_RotationMin = reader.GetF(ICamera_RotationMin, row);
                Camera_RotationMax = reader.GetF(ICamera_RotationMax, row);

                Camera_XOffsetMin = reader.GetF(ICamera_XOffsetMin, row) / 10f;
                Camera_XOffsetMax = reader.GetF(ICamera_XOffsetMax, row) / 10f;
                Camera_YOffsetMin = reader.GetF(ICamera_YOffsetMin, row) / 10f;
                Camera_YOffsetMax = reader.GetF(ICamera_YOffsetMax, row) / 10f;
                Camera_ZOffsetMin = reader.GetF(ICamera_ZOffsetMin, row) / 10f;
                Camera_ZOffsetMax = reader.GetF(ICamera_ZOffsetMax, row) / 10f;


                FreeCamera_Scale_Speed = reader.GetF(IFreeCamera_Scale_Speed, row);
                FreeCameraXmin = reader.GetF(IFreeCameraXmin, row) / 10f;
                FreeCameraXmax = reader.GetF(IFreeCameraXmax, row) / 10f;
                FreeCameraZmin = reader.GetF(IFreeCameraZmin, row) / 10f;
                FreeCameraZmax = reader.GetF(IFreeCameraZmax, row) / 10f;

                CameraY_XuanzhuanMin = reader.GetF(ICameraY_XuanzhuanMin, row);
                CameraY_XuanzhuanMax = reader.GetF(ICameraY_XuanzhuanMax, row);
                CameraY_XuanzhuanSpeed = reader.GetF(ICameraY_XuanzhuanSpeed, row);
            }
        }
    }

    public readonly short MapMaxRow;
    public readonly short MapMaxColumn;
    public readonly short HarfMapMaxColumn;
    public readonly float TerrainCellBianchang;//菱形格子边长 米
    public readonly float Camera_ZhuanshenTime;
    public readonly float Camera_ZhuanshenCD;

    public readonly float FreeCamera_Scale;
    public readonly float FreeCamera_Scale_End;
    public readonly float CameraY_Xuanzhuan_Start;
    public readonly float CameraY_Xuanzhuan_End;
    public readonly float Camera_XOffsetMin;
    public readonly float Camera_XOffsetMax;
    public readonly float Camera_YOffsetMin;
    public readonly float Camera_YOffsetMax;
    public readonly float Camera_ZOffsetMin;
    public readonly float Camera_ZOffsetMax;
    public readonly float Camera_RotationMin;
    public readonly float Camera_RotationMax;


    public readonly float FreeCamera_Scale_StopRange_Min;
    public readonly float FreeCamera_Scale_StopRange_Max;
    public readonly float CameraY_Xuanzhuan_StopRange_Min;
    public readonly float CameraY_Xuanzhuan_StopRange_Max;


    public readonly float Camera_Rotate_Radius;     // 相机旋转半径
    public readonly float Camera_Rotate_YOffset;    // 相机旋转时Y轴偏移
    public readonly float Camera_Rotate_Speed;      // 相机旋转速度

    public readonly float Camera_Overall_Zoffset;    // 相机Z轴偏移
    public readonly float Camera_Overall_Yoffset;    // 相机Y轴偏移
    public readonly float Camera_Overall_Yrotate;    // 相机旋转时Y轴偏移
    public readonly float Camera_Overall_Xrotate;    // 相机旋转速度


    public readonly float FreeCamera_Scale_Speed;
    public readonly float FreeCameraXmin;
    public readonly float FreeCameraXmax;
    public readonly float FreeCameraZmin;
    public readonly float FreeCameraZmax;
    public readonly float CameraY_XuanzhuanMin;
    public readonly float CameraY_XuanzhuanMax;
    public readonly float CameraY_XuanzhuanSpeed;
}