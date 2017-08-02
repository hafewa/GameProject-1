﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// buff编辑界面
/// </summary>
public class BuffScriptEditor : BaseScriptEditor
{
    [MenuItem("ScriptEditor/Open Buff Script Editor")]
    static void OpenBuffScriptEditor()
    {
        BuffScriptEditor window = GetWindow<BuffScriptEditor>(true, "BuffScriptEditor");
        window.Init();
    }

    /// <summary>
    /// 文件名称
    /// </summary>
    public override string FileName
    {
        get { return "BuffScript"; } 
    }

    /// <summary>
    /// 脚本起始内容
    /// </summary>
    public override string NumStart
    {
        get { return "Buff"; } 
    }

    /// <summary>
    /// GUI刷新
    /// </summary>
    void OnGUI()
    {
        Refresh();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("box", GUILayout.Width(400), GUILayout.Height(400));
        #region ScrollView - Script Content

        EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(400), GUILayout.Height(400));
        //GUIStyle style = new GUIStyle(GUI.skin.button);
        for (int i = 0; i < WorkingScriptContent.Count; i++)
        {
            var index = i;
            if (selectScriptContentIndex == index)
                GUI.color = Color.green;

            if (GUILayout.Button(WorkingScriptContent[index]))
            {
                selectScriptContentIndex = index;
                selectDataContentIndex = -1;
            }
            GUI.color = Color.white;
        }

        EditorGUILayout.EndScrollView();
        #endregion

        #region ScrollView - Data Content

        EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(400), GUILayout.Height(400));
        //GUIStyle style = new GUIStyle(GUI.skin.button);
        for (int i = 0; i < dataContent.Count; i++)
        {
            int index = i;
            if (selectDataContentIndex == index)
                GUI.color = Color.cyan;

            if (GUILayout.Button(dataContent[index]))
            {
                selectDataContentIndex = index;
                selectScriptContentIndex = -1;
            }
            GUI.color = Color.white;
        }

        EditorGUILayout.EndScrollView();
        #endregion
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box", GUILayout.Width(200), GUILayout.Height(440));
        #region UI
        Num = EditorGUILayout.IntField("script ID:", Num);
        if (GUILayout.Button("输入ID加载脚本"))
            LoadScript();
        if (GUILayout.Button("保存脚本"))
            SaveScript();
        if (GUILayout.Button("创建新脚本"))
            ClearContent();
        if (GUILayout.Button("删除行"))
            DeleteContent();
        if (GUILayout.Button("上移"))
            MoveUpContent();
        if (GUILayout.Button("下移"))
            MoveDownContent();
        #region Add Trigger
        EditorGUILayout.BeginVertical("box");
        GUI.color = Color.yellow;

        // 获取的实例
        BuffTriggerScriptEditor.GetIns();

        if (GUILayout.Button("子级左括号"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.LeftBracket, this.position);
        }
        if (GUILayout.Button("子级右括号"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.RightBracket, this.position);
        }
        if (GUILayout.Button("点对点特效"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.PointToPoint, this.position);
        }
        if (GUILayout.Button("点对对象特效(跟踪)"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.PointToObj, this.position);
        }
        if (GUILayout.Button("点特效"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.Point, this.position);
        }
        if (GUILayout.Button("范围碰撞检测"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.CollisionDetection, this.position);
        }
        if (GUILayout.Button("滑动碰撞检测"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.SlideCollisionDetection, this.position);
        }
        if (GUILayout.Button("音效"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.Audio, this.position);
        }
        if (GUILayout.Button("Buff"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.Buff, this.position);
        }
        if (GUILayout.Button("Skill"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.Skill, this.position);
        }
        if (GUILayout.Button("If"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.If, this.position);
        }
        if (GUILayout.Button("伤害/治疗结算"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.HealthChange, this.position);
        }
        if (GUILayout.Button("伤害吸收"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.ResistDemage, this.position);
        }
        if (GUILayout.Button("移动"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.Move, this.position);
        }
        if (GUILayout.Button("范围持续技"))
        {
            BuffTriggerScriptEditor.ShowTriggerScriptWindow(this, TriggerType.Remain, this.position);
        }
        
        GUI.color = Color.white;
        EditorGUILayout.EndVertical();
        #endregion

        #region Data
        // Buff
        EditorGUILayout.BeginVertical("box");
        GUI.color = Color.green;

        if (GUILayout.Button("Buff等级数据"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.LevelData, this.position);
        }
        if (GUILayout.Button("BuffCD值"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.CDTime, this.position);
        }
        if (GUILayout.Button("BuffCD组"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.CDGroup, this.position);
        }
        if (GUILayout.Button("Buff可释放次数"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.ReleaseTime, this.position);
        }
        if (GUILayout.Button("Buff描述"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.Description, this.position);
        }
        if (GUILayout.Button("数据修正"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.ChangeData, this.position);
        }
        if (GUILayout.Button("BuffIcon路径"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.Icon, this.position);
        }
        if (GUILayout.Button("Buff持续时间"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.BuffTime, this.position);
        }
        if (GUILayout.Button("Buff TickTime"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.TickTime, this.position);
        }
        if (GUILayout.Button("Buff类型"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.BuffType, this.position);
        }
        if (GUILayout.Button("Buff触发事件1"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.TriggerLevel1, this.position);
        }
        if (GUILayout.Button("Buff触发事件2"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.TriggerLevel2, this.position);
        }
        if (GUILayout.Button("Buff Detach触发事件1"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.DetachTriggerLevel1, this.position);
        }
        if (GUILayout.Button("Buff Detach触发事件2"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.DetachTriggerLevel2, this.position);
        }
        if (GUILayout.Button("Buff优先级"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.BuffLevel, this.position);
        }
        if (GUILayout.Button("Buff所在互斥组"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.BuffGroup, this.position);
        }
        if (GUILayout.Button("Buff是否有益"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.IsBeneficial, this.position);
        }
        if (GUILayout.Button("Buff Detach 条件"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.DetachQualified, this.position);
        }
        if (GUILayout.Button("Buff是否死亡消失"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.IsDeadDisappear, this.position);
        }
        if (GUILayout.Button("Buff是否不致死"))
        {
            BuffTriggerScriptEditor.ShowDataScriptWindow(this, DataType.IsNotLethal, this.position);
        }

        EditorGUILayout.EndVertical();
        #endregion
        #endregion

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }



    /// <summary>
    /// 转换脚本内容
    /// </summary>
    /// <param name="filename">被加载文件路径</param>
    /// <returns>是否加载成功</returns>
    protected override bool ParseScript(string filename)
    {
        bool ret = false;
        try
        {
            var fileData = Utils.LoadFileInfo(filename);
            // 解析内容
            var splitData = fileData.Split('\n');

            bool scriptBracket = false;
            bool dataBracket = false;
            for (var i = 0; i < splitData.Length; i++)
            {
                var line = splitData[i];
                // 消除空格
                line = line.Trim();
                // 跳过空行
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                // 跳过注释行
                if (line.StartsWith("//"))
                {
                    continue;
                }

                // 如果是Buff描述开始
                if (line.StartsWith("BuffNum"))
                {
                    // 读取Buff号
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

                    // 读取BuffID
                    var strSkillId = line.Substring(start + 1, length);
                    Num = Convert.ToInt32(strSkillId);
                    // 创建新Buff
                }
                else if (line.StartsWith("{"))
                {
                    // 开始括号内容
                    scriptBracket = true;
                }
                else if (line.StartsWith("}"))
                {
                    // 关闭括号内容
                    scriptBracket = false;
                }
                else if (line.StartsWith("["))
                {
                    // 开始括号内容
                    dataBracket = true;
                }
                else if (line.StartsWith("]"))
                {
                    // 关闭括号内容
                    dataBracket = false;
                }
                else
                {
                    // 解析内容
                    if (scriptBracket)
                    {
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

                        //// 行为类型
                        //var type = line.Substring(0, start);
                        //// 行为参数
                        //var args = line.Substring(start + 1, length);
                        //// 消除参数空格
                        //args = args.Replace(" ", "");

                        ActionScriptContent.Add(line.Replace("\t", ""));
                    }
                    if (dataBracket)
                    {
                        // 解析数据内容
                        dataContent.Add(line.Replace("\t", ""));
                    }
                }
            }
            ret = true;
        }
        catch (Exception e)
        {
            string err = "Exception:" + e.Message + "\n" + e.StackTrace + "\n";
            //LogSystem.ErrorLog(err);
        }
        return ret;
    }
}