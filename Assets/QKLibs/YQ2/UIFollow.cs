﻿using UnityEngine;
using System.Collections;
public class UIFollow : MonoBehaviour
{
    // 需要跟随的目标对象
    public Transform target;

    // 需要锁定的坐标（可以实时生效）
    public bool freazeX, freazeY, freazeZ;

    // 跟随的平滑时间（类似于滞后时间）
    public float smoothTime = 0.1F;
    private float xVelocity, yVelocity, zVelocity = 0.0F;

    // 跟随的偏移量
    private Vector3 offset = Vector3.zero;

    // 全局缓存的位置变量
    private Vector3 oldPosition;

    // 记录初始位置
    private Vector3 startPosition;
    private MoveRecord mr;

    void Start()
    {
        startPosition = transform.position;

        mr = GetComponent<BoxScrollObject>().m_MoveRecord;
    }

    void LateUpdate()
    {
        if (!target)
        {
            return;
        }
        oldPosition = transform.position;

        if (!freazeX)
        {
            oldPosition.x = Mathf.SmoothDamp(transform.position.x, target.position.x + offset.x, ref xVelocity, smoothTime);
        }

        if (!freazeY)
        {
            oldPosition.y = Mathf.SmoothDamp(transform.position.y, target.position.y + offset.y, ref yVelocity, smoothTime);
        }

        if (!freazeZ)
        {
            oldPosition.z = Mathf.SmoothDamp(transform.position.z, target.position.z + offset.z, ref zVelocity, smoothTime);
        }
        mr.CurrentPos = oldPosition;
        transform.position = oldPosition;
        if (Mathf.Abs(target.position.x - oldPosition.x) < 10)
        {
            target = null;
        }

    }


    /// <summary>
    /// 用于重新开始游戏时直接重置相机位置
    /// </summary>
    public void ResetPosition()
    {
        transform.position = startPosition;
    }
}