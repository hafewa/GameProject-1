﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum ArmySquareST
{
    None,//什么也不是
    ReadyIndependentAI,//准备进入独立AI状态
    Standby,//待命 (摄像机环视战场，或播放一些废话)
    Charge,//冲锋
    IndependentAI,//独立AI
}



public class AI_ArmySquare  : AI_FightUnit
{
    public AI_ArmySquare(AI_Battlefield ownerBattlefield)
    {
        this._OwnerBattlefield = ownerBattlefield;
        this._ActionsLimit = new AI_Limit(this);
        this.OwnerArmySquare = this;
    }
    public override short ModelRange { get { return ArmyData.ModelRange; } }
    public void InitAtrr()
    {
        _TiLi.SetV( ArmyData.CalculationTili(BaseAttr.soldiersLevel,  BaseAttr.sxj));
        NTiliBeishu = SData_FightKeyValueMath.Single.Get(BaseAttr.soldiersLevel).NTiliBeishu;
        CTili = SData_FightKeyValue.Single.CTili;

        NJingshenBeishu = SData_FightKeyValueMath.Single.Get(BaseAttr.soldiersLevel).NJingshenBeishu;
        Cjingshen = SData_FightKeyValue.Single.Cjingshen;

        _MaxHP.SetV(ArmyData.CalculationHP(BaseAttr.soldiersLevel, BaseAttr.sxj));
        _Speed.SetV(ArmyData.Speed);

        _WuLi.SetV(ArmyData.CalculationWuli(BaseAttr.soldiersLevel, BaseAttr.sxj));
        _Nu.SetV(ArmyData.CalculationNu(BaseAttr.soldiersLevel, BaseAttr.sxj));
        _Jingshen.SetV(ArmyData.CalculationJingshen(BaseAttr.soldiersLevel, BaseAttr.sxj));
        _Zhili.SetV(ArmyData.CalculationZhili(BaseAttr.soldiersLevel, BaseAttr.sxj));

        _EffectBuffManage = new AI_EffectBuffManage(this);

        UpdateDyAttrs();

    }

    public float FangyuLv     { get        {   return (CTili * FnTili) / (FnTili + NTiliBeishu);  }  }

    public float FashuFangyulv {  get  {    return (Cjingshen * FnJingshen) / (FnJingshen + NJingshenBeishu); } }


    //阵士兵伤血
    public void SquareLostHP(int hp, AI_FightUnit attacker )
    {
        if (hp == 0) return;

        int pjSoldiersCount = 0;//平均分摊伤害的士兵总数统计

        //优先对战斗状态士兵进行伤血
        foreach(AI_Soldiers curr in soldiers)
        {
            if (curr.st != HeroST.Fighting) { pjSoldiersCount++; continue; }//跳过非战斗状态的士兵

            int currHit = Math.Min(curr.CurrHP, hp);
            hp -= currHit;
            curr.LostHP(currHit, attacker, DieEffect.Putong,true);
            if (hp <= 0) return;//已经分摊完所有伤害，结束伤血逻辑
        }
        
        //对剩下的士兵进行平均伤害
        if (pjSoldiersCount > 0)
        {
            int oneHit = hp / pjSoldiersCount;
            if(oneHit>0)
            {
                foreach (AI_Soldiers curr in soldiers)
                    curr.LostHP(oneHit, attacker, DieEffect.Putong,true); 
            }
        }

    }

    float CTili;
    float NTiliBeishu;
    float Cjingshen;
    float NJingshenBeishu;
    

    public readonly AI_Battlefield _OwnerBattlefield;
    public ArmySquareInfo BaseAttr;
    public AI_Hero hero;
    public DPActor_OverheadPanel OverheadPanel = null;//头顶面板
    public DPActor_BelowPanel BelowPanel = null;//角下面板

    //阵属性
    public AAttr _Speed = new AAttr();
    public AAttr _MaxHP = new AAttr();
    public AAttr _WuLi = new AAttr();//武力
    public AAttr _Nu = new AAttr();//怒
    public AAttr _TiLi = new AAttr();//体力

    public AAttr _Jingshen = new AAttr();//精神
    public AAttr _Zhili = new AAttr();//智力

    public AAttrLight _AddFYL = new AAttrLight();//增加防御率
    public AAttrLight _AddBSGL = new AAttrLight();//增加必杀概率
    public TriggerMinOnce triggerMinSoldiersNum = new TriggerMinOnce();//士兵数首次小于统计
    //更新动态属性
    public void UpdateDyAttrs()
    {
        FnWuLi = _WuLi.ToFloat();
        FnZhli = _Zhili.ToFloat();
        FnNu = _Nu.ToFloat();
        FnTili = _TiLi.ToFloat();
        FnAddFYL = _AddFYL.ToFloat();
        FnAddBSGL = _AddBSGL.ToFloat();
        FnMaxHP = _MaxHP.ToFloat();
        FnSpeed = _Speed.ToFloat();
        FnJingshen = _Jingshen.ToFloat();

        float bsgl = SData_FightKeyValue.Single.Cnu / (FnNu + SData_FightKeyValueMath.Single.Get(BaseAttr.soldiersLevel).Nnu);
        FnBishaGL = (int)((bsgl + _AddBSGL.addV) * (1.0f + _AddBSGL.addBFB) * 100.0f);
    }
    public float FnWuLi;
    public float FnZhli;
    public float FnJingshen;
    public float FnNu;
    public float FnTili;
    public float FnAddFYL;
    public float FnAddBSGL;
    public float FnSpeed;
    public float FnMaxHP;
    public int FnBishaGL;

    public AI_Soldiers QizhiOwner = null;//旗帜当前挂在哪个士兵身上
    public DPActor_SquareFlag QizhiActor = null;
    public void FindQizhiOwner(float oldWorldX,float oldWorldZ)
    {
        QizhiOwner = null;
        float distance = 99999999f;
        //在本阵中找一个距离最近的担任抗旗大任
        foreach (var curr in soldiers)
        {
            if (curr.IsDie) continue;

            var d = AI_Math.V2Distance(curr.ownerGrid.WorldX, curr.ownerGrid.WorldZ, oldWorldX, oldWorldZ);
            if (d < distance) { distance = d; QizhiOwner = curr; }
        }
    }
    public HashSet<AI_Soldiers> soldiers = new HashSet<AI_Soldiers>();
    public HashSet<AI_FightUnit> ChargeKeyUnits = new HashSet<AI_FightUnit>();//冲锋关键单位
    public HashSet<AI_FightUnit> FindEnemyKeyUnits = new HashSet<AI_FightUnit>();//寻敌关键单位

    public ArmyFlag flag;
   // public YQ2AABBBox2D AABBBox { get { if (needUpdateAABB) { needUpdateAABB = false; UpdateAABB(); } return _aabb; } }

    public new ArmySquareST st = ArmySquareST.None;

    //阵是否已经死亡
    public override bool IsDie { get { return hero.IsDie && soldiers.Count < 1; } }
      
    public int LastSyncSoldiersCount = -1;//上次同步士兵总数 
    public int SoldiersCount = 0; 
    public AI_ArmySquare targetArmySquare = null;//目标军队
    public ArmyInfo ArmyData;
    public bool IsQixi;
    public AI_EffectBuffManage _EffectBuffManage;

    //抽象AI_FightUnit接口 
    public override void OnAttrChanged() { UpdateDyAttrs(); }

    public override AI_EffectBuffManage EffectBuffManage { get { return _EffectBuffManage; } }

    
    public override void MountActor(DPActor_Base actor) { actor.AddKey_DestroyInstance(CurrTime); }

    protected override void OnDie(AI_FightUnit attacker,float time) { }
    public override void LostHP(int v, AI_FightUnit attacker, DieEffect dieEffect, bool addLianzhan) { }
    public override void AddHP(AI_FightUnit attacker, int v) { } 
    protected override void PlayExplodedDieEffect(AI_FightUnit attacker) { } 
    protected override void PlayEscapeEffect() { }
    public override AI_FightUnit FindEnemy(bool findSoldiers) { return null; } 
    public override AI_Battlefield OwnerBattlefield { get { return _OwnerBattlefield; } }

    public override float FinalSpeed { get { return FnSpeed; } }
    public override int FinalMaxHP { get { return (int)FnMaxHP; } }
    public override int FinalBishaGL { get { return FnBishaGL; } }//必杀释放概率

    public override float FinalNu { get { return FnNu; } }

    /// <summary>
    /// 播放攻击动作
    /// </summary>
    public override void PlayHitAction(string atk, float dirx, float dirz) { throw new Exception(); }
    public override Skill[] BishaSkills { get { throw new Exception(); } }///获取必杀技队列
    public override Skill[] SkillObjs { get { throw new Exception(); } } ///技能列表
    public override string[] SkillAtks { get { throw new Exception(); } }//获取技能动作
    public override short[] SkillLevels { get { throw new Exception(); } }//技能等级
    public override short SuperSkillCountWeight { get { throw new Exception(); } }//必杀技总权重
    public override int Level { get { return 1; } }//等级
    public override AAttr Speed { get { return _Speed; } }//移动速度
    public override AAttr MaxHP { get {return _MaxHP;}}//生命上限
    public override AAttr WuLi { get{return _WuLi;} }//武力
    public override AAttr Zhili { get { return _Zhili; } }

    public override AAttr Nu { get{return _Nu;} }//怒
    public override AAttr TiLi { get{return _TiLi;} }//体力
    public override AAttrLight AddFYL { get{return _AddFYL;} }//增加防御率 
    public override AAttrLight AddBSGL { get{return _AddBSGL;} }//增加必杀概率 

    public override Skill JinshenSkill { get { return null; } }//近身技

    public override void SwapJinYuan(bool isYuan)  {   }

    /// <summary>
    /// 行为限制器
    /// </summary>
    public override AI_Limit ActionsLimit { get { return _ActionsLimit; } }

    AI_Limit _ActionsLimit;


    public override void AddTime(float time)
    {
        base.AddTime(time);
        EffectBuffManage.AddTime(time);

        if (!hero.IsDie)
        {
            //如果不在独立ai状态则增加时间前清除所有自由时间，防止一边移动一边攻击
            if (st != ArmySquareST.IndependentAI) hero.ReduceAllTime();
            hero.AddTime(time);
        }

        

        if(st==ArmySquareST.IndependentAI)//独立AI阶段
        {
            foreach (var sb in soldiers)
            {
                if (!sb.IsDie)
                {
                    sb.AddTime(time);
                }
            }
        }
    }
    public override void DoAI() {
        EffectBuffManage.Update();

        while (HasNotUsedTime)
        {
            switch (st)
            {
                case ArmySquareST.None://什么也不是
                case ArmySquareST.Standby://待命 (摄像机环视战场，或播放一些废话)
                     
                    ReduceAllTime();
                    break;
                case ArmySquareST.ReadyIndependentAI://准备进入独立AI状态
                    {
                        //var ladd = LastAdd;
                        //士兵AI即将启动，同步士兵时间轴和战役一致
                        foreach (var sb in soldiers)
                        {
                            if (!sb.IsDie)
                            {
                                sb.OnJoin(OwnerBattlefield, CurrTime);
                                sb.AddTime(NotUsedTime);//士兵立即拥有行动时间  
                            }
                        }
                         
                        hero.ResetTime(CurrTime, NotUsedTime);//英雄的时间轴和阵完全同步
                        hero.st = HeroST.Moveing;

                        //如果英雄是缩进单位，则立即移动半格，对齐到标准格子
                        if (hero.IndentPos)
                        {
                            var toX = hero.ownerGrid.WorldX;
                            var toZ = hero.ownerGrid.WorldZ;
                            var IndentX = AI_Math.IndentOffset(Flag);
                            var t = hero.Shape.RH.AddKey_MoveTo(hero.CurrTime, toX + IndentX, toZ, toX, toZ, hero.FinalSpeed * DiamondGridMap.WidthSpacingFactor, out hero.dirx, out hero.dirz);
                            hero.ReduceTime(t);
                        }

                        //状态切换为独立AI
                        st = ArmySquareST.IndependentAI;
                    }
                    break; 
                case ArmySquareST.Charge://冲锋 
                    DoCharge2(OwnerBattlefield,this); 
                    break;
                case ArmySquareST.IndependentAI://独立AI
                    {
                        //阵时间损耗
                        ReduceAllTime();
                         
                        
                        //武将AI
                        if (!hero.IsDie) hero.DoAI();

                        //士兵AI
                        foreach (var sb in soldiers)
                        {
                            if (!sb.IsDie) sb.DoAI();
                        }
                         
                    }
                    break;
            }
        }
    }
     


    //切换为独立AI模式
    public void ToIndependentAI()
    {
        if (st != ArmySquareST.Charge) return;
        st = ArmySquareST.ReadyIndependentAI; 
    }

    /// <summary>
    /// 由战役对象调用,执行武将AI(仅仅有技能缓存处理路基)和解体
    /// </summary>
    public void DoCharge1()
    {
        if (st != ArmySquareST.Charge) return;

        //武将AI
        if (!hero.IsDie) hero.DoAI();

        //检查关键单位的射程范围内是否有敌人
        bool hasEnemy = false;
        var eFlag = AI_Math.ReverseFlag(flag); 

        //检查射程范围内是否有敌人
        foreach (var sb in ChargeKeyUnits)
        {
            if (!sb.IsDie&& HasEnemy(sb, eFlag))
            {
                hasEnemy = true;
                break;
            }
        }

        //检查武将射程范围内是否有敌人
        if (!hasEnemy && !hero.IsDie && HasEnemy(hero, eFlag))
            hasEnemy = true;

        if (hasEnemy) //射程内出现了敌人，结束冲锋
        {
            ToIndependentAI();
            return;
        }
    }

    enum ChargeCheckResult
    {
        Charge,//冲锋
        Wait,//原地等待
        ToIndependentAI,//切换为独立AI
        CheckNext,//继续检查下一个阵
    }

    //检查一个方阵是否可以冲锋
    static ChargeCheckResult CanCharge(
        AI_Battlefield ownerBattlefield,
        DiamondGridMap gridmap, 
        short xOffset, 
        AI_ArmySquare square, 
        HashSet<AI_ArmySquare> LiantiArmys,//当前连体阵
        HashSet<AI_ArmySquare> newLiantiArmys//新产生的连体阵
        )
    {
        square.UpdateToIndependentAITime();

       // var BehindX = xOffset > 0 ? ownerBattlefield.m_RightBehindX : ownerBattlefield.m_LeftBehindX;
       
        //检查关键单位是否可行走
        foreach (var sb in square.ChargeKeyUnits)
        {
            var grid = sb.ownerGrid;
            var frontGrid = gridmap.GetGrid(grid.GredX + xOffset, grid.GredZ);

            var r = CheckFrontGrid(
                  square, frontGrid, xOffset,// BehindX,
                  LiantiArmys,//当前连体阵
                  newLiantiArmys//新产生的连体阵
                );
            if (r != ChargeCheckResult.CheckNext) return r;


            frontGrid = gridmap.GetGrid(grid.GredX + xOffset+xOffset, grid.GredZ);
            r = CheckFrontGrid(
                square, frontGrid, xOffset, //BehindX,
                LiantiArmys,//当前连体阵
                newLiantiArmys//新产生的连体阵
              );
            if (r != ChargeCheckResult.CheckNext) return r;
        }
        return ChargeCheckResult.Charge;
    }

    static ChargeCheckResult CheckFrontGrid(
        AI_ArmySquare square, DiamondGrid frontGrid, short xOffset,// byte BehindX,
        HashSet<AI_ArmySquare> LiantiArmys,//当前连体阵
        HashSet<AI_ArmySquare> newLiantiArmys//新产生的连体阵
        )
    {
        if (
            frontGrid == null||//前方不是有效格
            square.OwnerBattlefield.FightLostTime >= square.ToIndependentAITime//冲过了敌军底线
            )
            return ChargeCheckResult.ToIndependentAI;

        /*
        if (xOffset > 0)
        {
            if (frontGrid.GredX > BehindX)
                return ChargeCheckResult.ToIndependentAI;//向右冲过了敌军底线
        }
        else
        {
            if (frontGrid.GredX < BehindX)
                return ChargeCheckResult.ToIndependentAI;//向左冲过了敌军底线
        }*/ 

        if (frontGrid.IsObstacle)//前方是一个障碍物
        {
            if (frontGrid.Obj.Flag != square.flag)//是一个敌军
                return ChargeCheckResult.ToIndependentAI;

            if (frontGrid.Obj.IsDie)//一个已经死亡的友军
                return ChargeCheckResult.ToIndependentAI;

            var frontSquare = frontGrid.Obj.OwnerArmySquare;

            if (LiantiArmys.Contains(frontSquare) || newLiantiArmys.Contains(frontSquare))//是连体移动阵
                return ChargeCheckResult.CheckNext;

            if (
                   frontSquare.st == ArmySquareST.Charge// &&//当前状态是冲锋
                   )
            {
               // if (frontSquare.HasNotUsedTime)//存在未使用的行动时间
                    newLiantiArmys.Add(frontSquare);//加入近新增连体阵队列
                //else
                  //  return ChargeCheckResult.Wait;
            }
            else
                return ChargeCheckResult.ToIndependentAI;//不能连体的友军
        }

        return ChargeCheckResult.CheckNext;
    }

    /// <summary>
    /// 阵移动速度
    /// </summary>
    public float SquareSpeed { get   {  return Math.Min(hero.FinalSpeed, FinalSpeed);    }  }

    //冲锋
    public static void DoCharge2(AI_Battlefield battlefield,AI_ArmySquare selfSquare)
    {

        short xOffset = selfSquare.Flag == ArmyFlag.Attacker ? (short)1 : (short)-1;

        var gridmap = battlefield.GridMap;

        bool canMove = true;
        HashSet<AI_ArmySquare> LiantiArmys = new HashSet<AI_ArmySquare>();//联体的阵

        {
            HashSet<AI_ArmySquare> newLiantiArmys = new HashSet<AI_ArmySquare>();//新产生的连体阵
            newLiantiArmys.Add(selfSquare);

            do
            {
                HashSet<AI_ArmySquare> tmpList = newLiantiArmys;
                newLiantiArmys = new HashSet<AI_ArmySquare>();

                //新增队列加入到连体阵中
                foreach (var curr in tmpList) LiantiArmys.Add(curr);

                //遍历检查新增到连体队列的阵
                foreach (var square in tmpList)
                {
                    ChargeCheckResult re = CanCharge(battlefield, gridmap, xOffset, square, LiantiArmys, newLiantiArmys);
                    switch(re)
                    {
                        case ChargeCheckResult.ToIndependentAI:
                            selfSquare.ToIndependentAI();
                            return;
                        case ChargeCheckResult.Charge:
                            continue; 
                    }
                    canMove = false;
                    break;
                }
            } while (newLiantiArmys.Count > 0);//当新增连体阵为0时，结束
        }

        if (canMove)//移动一格
        {
            float mvspeed = 99999;
            foreach (var curr in LiantiArmys)
            {
                float currSpeed = curr.SquareSpeed  * DiamondGridMap.WidthSpacingFactor;
                if (currSpeed < mvspeed) mvspeed = currSpeed;
            }

            foreach (var curr in LiantiArmys)
            {
                if (!curr.hero.IsDie) curr.hero.SetGridObjFast(null);//ownerGrid.Obj = null;

                foreach (var sb in curr.soldiers)
                {
                    if (!sb.IsDie) sb.SetGridObjFast(null);//ownerGrid.Obj = null; 
                }
            }




            float t = 0;
            var IndentOffsetX = AI_Math.IndentOffset(selfSquare.Flag);//缩进偏移距离
            foreach (var curr in LiantiArmys)
            {

                //武将移动
                if (!curr.hero.IsDie)
                {
                    var indentOffset = curr.hero.IndentPos ? IndentOffsetX : 0;//缩进距离

                    var heroOgrid = curr.hero.ownerGrid;
                    var heroWorldX = heroOgrid.WorldX + indentOffset;
                    var heroWorldZ = heroOgrid.WorldZ;

                    var heroNewGrid = gridmap.GetGrid(heroOgrid.GredX + xOffset, heroOgrid.GredZ);

                   

                    t = curr.hero.Shape.RH.AddKey_MoveTo(
                            curr.CurrTime,
                            heroWorldX, heroWorldZ,
                            heroNewGrid.WorldX + indentOffset, heroWorldZ,
                            mvspeed,
                            out curr.hero.dirx, out curr.hero.dirz
                            );

                    //头顶面板移动
                    curr.OverheadPanel.AddKey_MoveTo(
                        curr.CurrTime,
                        heroWorldX, heroWorldZ,
                        heroNewGrid.WorldX + indentOffset, heroWorldZ,
                        mvspeed,
                        out curr.dirx, out curr.dirz
                        );

                    //武将格子处理 
                    curr.hero.ownerGrid = heroNewGrid;
                    //curr.hero.ownerGrid.Obj = curr.hero;
                    curr.hero.SetGridObjFast(curr.hero);
                }

                float temp;
                //全体士兵移动一格
                foreach (var sb in curr.soldiers)
                {
                    if (sb.IsDie) continue;

                    var ogrid = sb.ownerGrid;
                    var newGrid = gridmap.GetGrid(ogrid.GredX + xOffset, ogrid.GredZ);

                    var sbWorldX = ogrid.WorldX;
                    var sbWorldZ = ogrid.WorldZ;

                    var indentOffset = sb.IndentPos ? IndentOffsetX : 0;//缩进距离

                    t = sb.Shape.RH.AddKey_MoveTo(
                        curr.CurrTime,
                        sbWorldX + indentOffset, sbWorldZ ,
                        newGrid.WorldX + indentOffset, sbWorldZ ,
                        mvspeed,
                        out sb.dirx, out sb.dirz
                    );

                    if (curr.QizhiOwner == sb)
                        curr.QizhiActor.AddKey_MoveTo(
                        curr.CurrTime,
                        sbWorldX + indentOffset, sbWorldZ,
                        newGrid.WorldX + indentOffset, sbWorldZ,
                        mvspeed,
                        out temp, out temp
                    );

                    //士兵格子处理 
                    sb.ownerGrid = newGrid;
                    //sb.ownerGrid.Obj = sb;
                    sb.SetGridObjFast(sb);
                } 

                //阵行动消耗
                curr.ReduceTime(t);

                /*
                if (curr.flag == ArmyFlag.Attacker)
                    battlefield.m_LeftBehindX++;
                else
                    battlefield.m_RightBehindX--;
                */
            }
 
        }
        else
        {
            //全军进入等待状态
            foreach (var curr in LiantiArmys)
            {
                foreach (var sb in curr.soldiers)
                {
                    if (sb.IsDie) continue;
                    sb.Shape.RH.AddKey_Wait(curr.CurrTime, 0f);
                }

                curr.hero.Shape.RH.AddKey_Wait(curr.CurrTime, 0f);


                curr.ReduceAllTime();
            }
        }

    }
 

    //检查某单位射程范围内是否有敌人
    bool HasEnemy(AI_FightUnit unit,ArmyFlag eFlag )
    {
        var grid =  unit.ownerGrid;
        int gx = grid.GredX;
        int gz = grid.GredZ;
        var map = OwnerBattlefield.GridMap;
        int atkRange = unit.AtkRange;

        if (flag == ArmyFlag.Attacker)
        {
            for (int i = 1; i <= atkRange; i++)
            {
                var g1 = map.GetGrid(gx + i, gz);

                if (g1 != null && g1.IsObstacle && g1.Obj.Flag == eFlag && !g1.Obj.IsDie)
                    return true;
            }
        }else
        {
            for (int i = 1; i <= atkRange; i++)
            {
                var g1 = map.GetGrid(gx - i, gz);

                if (g1 != null && g1.IsObstacle && g1.Obj.Flag == eFlag && !g1.Obj.IsDie)
                    return true;
            }
        }
        return false;
    }

    public override int JinshenSkillID{get{return -1;}}

    public override void HitMe(AI_FightUnit enemy)  {  }


    /// <summary>
    /// 常规死亡效果
    /// </summary>
    protected override void PlayCommonDieEffect(AI_FightUnit attacker) { }


    public override void Wait(float t) { }
    public override ArmyFlag Flag { get { return flag; } }
    public override UnitType UnitType { get { return UnitType.Square; } }
     
    public override int AtkRange { get { throw new Exception(); } }

    public override int ID { get { return hero.Fid; } }

    //本阵英雄如果在冲锋阶段释放技能，设置一个英雄重新进入移动状态的时间点
    //英雄释放技能过程中全军停止冲锋
    public void SetHeroWait2MoveTime(float time) {
        if (st != ArmySquareST.Charge) return;
        //HeroWait2MoveTime = time; 

        //全部士兵进入等待状态
        //var heroCurrTime = this.hero.CurrTime;
         /*foreach( AI_Soldiers  sb in soldiers)
         {
             if (sb.IsDie) continue;
             sb.Shape.RH.AddKey_Wait(heroCurrTime, 0f);
         }*/

         if (CurrTime < time)
         {
             foreach (AI_Soldiers sb in soldiers)
             {
                 if (sb.IsDie) continue;
                 sb.Shape.RH.AddKey_Wait(CurrTime, 0f);
             }

             ReduceTime(time - CurrTime);
         }
    }

    /// <summary>
    /// 转换为独立AI的时间
    /// </summary>
    public float ToIndependentAITime = 999999f;

    public void CalculateToIndependentAITime()
    {   
        if (HasTonghangEnemy()) return;//同行有敌人，自动解散时间保持默认值

        int frontX; 
        if(flag== ArmyFlag.Attacker)
        {
            BehindEnemyUnit = OwnerBattlefield.m_BehindInfo.RightBehindX;
            BehindEnemyArmySquare = OwnerBattlefield.m_BehindInfo.RightBehind ;

            frontX = 0;
            foreach(var curr in this.soldiers)
            {
                var ox = curr.ownerGrid.GredX;
                if (ox > frontX) {frontX = ox;FrontUnit=curr;}
            }
        }else
        {
            BehindEnemyUnit = OwnerBattlefield.m_BehindInfo.LeftBehindX;
            BehindEnemyArmySquare = OwnerBattlefield.m_BehindInfo.LeftBehind ;

            frontX = 99999;
            foreach (var curr in this.soldiers)
            {
                var ox = curr.ownerGrid.GredX;
                if (ox < frontX) {frontX = ox;FrontUnit=curr;}
            }
        } 
    }

    void UpdateToIndependentAITime()
    {
        if (FrontUnit==null||BehindEnemyUnit==null|| FrontUnit.IsDie || BehindEnemyUnit.IsDie) return;

        if(flag== ArmyFlag.Attacker)
        {
            //已经冲到底了
            if (FrontUnit.ownerGrid.GredX >= BehindEnemyUnit.ownerGrid.GredX)
            {
                ToIndependentAITime = OwnerBattlefield.FightLostTime + SData_FightKeyValue.Single.JiesanYanchi;
                FrontUnit = BehindEnemyUnit = null;
                return;
            }
        } else
        {
            //已经冲到底了
            if (FrontUnit.ownerGrid.GredX <= BehindEnemyUnit.ownerGrid.GredX)
            {
                ToIndependentAITime = OwnerBattlefield.FightLostTime + SData_FightKeyValue.Single.JiesanYanchi;
                FrontUnit = BehindEnemyUnit = null;
                return;
            }
        }

        ToIndependentAITime = OwnerBattlefield.FightLostTime +
          (float)Math.Abs(BehindEnemyUnit.ownerGrid.GredX - FrontUnit.ownerGrid.GredX) / (BehindEnemyArmySquare.SquareSpeed + SquareSpeed) 
          + SData_FightKeyValue.Single.JiesanYanchi; 
    }

    AI_FightUnit BehindEnemyUnit;
    AI_ArmySquare BehindEnemyArmySquare;
    AI_FightUnit FrontUnit;//最前面的一个单位

    bool HasTonghangEnemy()
    {
        foreach (var curr in this.ChargeKeyUnits)
        {
            var z1 = curr.ownerGrid.GredZ;

            var it = OwnerBattlefield.ArmySquareListEnumerator;
            while (it.MoveNext())
            {
                var square = it.Current;
                if (square.flag == curr.Flag) continue;

                foreach (var curr2 in square.ChargeKeyUnits)
                    if (curr2.ownerGrid.GredZ == z1) return true;
            }
        }

        return false;
    }

    //float HeroWait2MoveTime = 0;//英雄从等待状态切换为移动状态的时间点
   
}