﻿using UnityEngine;
using System.Collections;

public class PatrolManager {
    static PatrolManager _Single = null;

    public static PatrolManager Single
    {
        get
        {
            if (_Single == null) _Single = new PatrolManager();
            return _Single;
        }
    }
    /// <summary>
    /// 根据当前玩家阵容创建主场景巡逻（演员）
    /// </summary>
    public void CreatePatrolActors()
    {
        //var actor = DP_FightPrefabManage.InstantiateAvatar(new CreateActorParam(
        //    0,
        //     // act.Res.MaskColor,
        //     false,
        //    1,
        //    "ganning1",
        //    "ganning1",
        //    false)
        //    );
        //Debug.Log("加载角色进场景-------" + actor);
    }


}