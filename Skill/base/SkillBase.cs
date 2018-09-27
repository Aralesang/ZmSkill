using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能类型枚举
/// </summary>
public enum SkillTypeEnum
{
    /// <summary>
    /// 主动
    /// </summary>
    active = 0,
    /// <summary>
    /// 被动
    /// </summary>
    passive = 1
}

/// <summary>
/// 技能基类
/// </summary>
public abstract class SkillBase
{
    /// <summary>
    /// 技能名字
    /// </summary>
    public string Name = "技能模板";

    /// <summary>
    /// 技能ID(生成时自动编排)
    /// </summary>
    public int Id = 0;

    /// <summary>
    /// 技能类型:0:主动技能(可以主动按键施展的技能) 1:被动技能(不可以主动施展,但达成条件时会自动施展的技能)
    /// (目前未准备混合型技能，可以通过设置一个主动一个被动来得到效果，混合技能意义不大)
    /// </summary>
    public SkillTypeEnum SkillType = SkillTypeEnum.active;

    /// <summary>
    /// 施展之后，直到第二次施展之间所需要经过的时间
    /// </summary>
    public float Cd = 0;

    /// <summary>
    /// 技能施展时的MP消耗量(技能持续变化MP的情况之后再作考虑)
    /// 主动技能将会在发动的一瞬间进行MP的变化，被动技能默认不参与MP或者HP的变化，如有需要请自行在Effect函数中添加逻辑
    /// </summary>
    public int Mp = 0;

    /// <summary>
    /// 技能施展时的HP消耗量
    /// </summary>
    public int Hp = 0;

    /// <summary>
    /// 施展技能时Ep变化量
    /// </summary>
    public int Ep = 0;

    /// <summary>
    /// 技能发动者对象
    /// </summary>
    public Role role;

    /// <summary>
    /// 技能释放者的动画对象
    /// </summary>
    public Animator animator;

    /// <summary>
    /// 技能系统
    /// </summary>
    public SkillSystem skill;

    /// <summary>
    /// 战斗锁定系统的引用
    /// </summary>
    public AttackLock attackLock;

    /// <summary>
    /// 技能图标路径
    /// </summary>
    public string iconPath = null;

    /// <summary>
    /// 技能描述
    /// </summary>
    public string Description { get; set; }


    /// <summary>
    /// 装备动作外观变化系统
    /// </summary>
    public EquipmentShow equipmentShow;

    /// <summary>
    /// 因为其他对象触发时，获取触发者的对象
    /// </summary>
    public Role otherRole;

    /// <summary>
    /// 时间节点集合
    /// </summary>
    public List<TimeNode> TimeNodeList;

    /// <summary>
    /// 技能主动效果系统函数部分
    /// </summary>
    public bool Use()
    {
        
        if (!Effect_Limit_Factory())
        {
            Debug.Log("发动失败");
            return false;
        }
        
        role.soul.SkillPerception(this);

        role.MpChange(Mp);
        role.HpChange(Hp);
        role.EpChange(Ep);
        this.Start();
        this.Use_Factory();

        return true;
    }

    /// <summary>
    /// 使用技能,主动触发技能时调用
    /// </summary>
    /// <returns></returns>
    protected abstract bool Use_Factory();


    /// <summary>
    /// 技能被动效果系统包装部分
    /// </summary>
    public virtual void Effect_Factory()
    {
        if (TimeNodeList != null && TimeNodeList.Count > 0)
        {
            TimeNode timeNode = TimeNodeList[0];
            timeNode.time = timeNode.time - Time.deltaTime;

            if (timeNode.continuity)
            {
                timeNode.method();
                if (timeNode.time <= 0)
                {
                    TimeNodeList.RemoveAt(0);
                }
            }
            else if (timeNode.time <= 0)
            {
                timeNode.method();
                TimeNodeList.RemoveAt(0);
            }
        }
        //role.soul.SkillPerception(this);
        this.Effect();

    }

    public virtual bool Effect_Limit_Factory()
    {
        if (skill.NotningSkillLIst.Contains(GetId()))
        {
            //Debug.Log(GetName()+"已被禁止");
            return false;
        }
        return Effect_Limit();
    }

    /// <summary>
    /// 效果函数限制器
    /// 用于检查技能是否处于可释放状态
    /// 默认情况下
    /// 用于限制同一时间只能释放一个技能
    /// 特殊情况可以重写该函数
    /// </summary>
    public virtual bool Effect_Limit()
    {
        //玩家死亡状态下不触发技能
        if (role.isSurvive == 1)
        {
            return false;
        }
        //如果当前有技能处于激活状态，则不能激活该技能
        if (skill.currentId > 0 && skill.currentId != GetCd())
        {
            return false;
        }

        //需要消耗的值不足
        if (!CheckConsume())
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 被动技能指令触发部分
    /// </summary>
    public abstract void Effect();

    /// <summary>
    /// 初始化激活效果,该效果会在持有该技能时，玩家第一次创建时进行初始化
    /// 或者学会该技能时进行一次激活
    /// </summary>
    public virtual void Effect_Init()
    {

    }

    /// <summary>
    /// 技能开始
    /// </summary>
    public void Start()
    {
        ClearEvent();
        skill.currentId = this.GetId();
    }

    /// <summary>
    /// 技能结束
    /// </summary>
    public virtual void End()
    {
        equipmentShow.Reload();
        //opCode = OpCode.Noth;
        skill.currentId = 0;
        //Debug.Log("技能结束");
    }

   

    public void Init()
    {
        this.Name = GetName();
        this.Id = GetId();
        this.Cd = GetCd();
        this.SkillType = GetSkillType();
        this.Hp = GetHp();
        this.Mp = GetMp();
    }

    /// <summary>
    /// 初始化技能对象属性
    /// </summary>
    public void Init(Role role)
    {
        this.role = role;
        this.animator = role.GetComponent<Animator>();
        this.skill = role.GetComponent<SkillSystem>();
        this.Name = GetName();
        this.Id = GetId();
        this.Cd = GetCd();
        this.SkillType = GetSkillType();
        this.Hp = GetHp();
        this.Mp = GetMp();
        this.attackLock = role.GetComponent<AttackLock>();
        this.equipmentShow = role.GetComponent<EquipmentShow>();
        this.TimeNodeList = new List<TimeNode>();
        Effect_Init();

    }



    /// <summary>
    /// 技能冷却时间系统(视情况可能不被调用)
    /// 技能可以不需要冷却时间，SpellCard级别以积攒B或卡槽为发动条件
    /// 如果技能没有冷却时间，则灵气的恢复难度需要提高
    /// 非特殊符卡或特殊事件则无法恢复灵气
    /// </summary>
    private void SkillCd()
    {

    }

    /// <summary>
    /// 重写并返回技能名字
    /// </summary>
    public abstract string GetName();

    /// <summary>
    /// 获取技能ID
    /// </summary>
    /// <returns></returns>
    public abstract int GetId();

    /// <summary>
    /// 获取技能类型
    /// </summary>
    /// <returns></returns>
    public abstract SkillTypeEnum GetSkillType();

    /// <summary>
    /// 获取技能冷却
    /// </summary>
    /// <returns></returns>
    public abstract float GetCd();

    public void AddOp(OpCode newOpCod, params float[] values)
    {
        opEffect(newOpCod, null, values);
    }

    /// <summary>
    /// 向技能追加操作指令(每个技能只能接受一种当前指令,且只能接受指定类型的操作指令，如果操作指令无法识别，则什么也不会发生)
    /// </summary>
    public void opEffect(OpCode newOpCode, Role otherrole, params float[] values)
    {
        List<OpCode> opCodelist = GetOp(newOpCode);
        if (opCodelist == null)
        {
            return;
        }
        foreach (OpCode opCode in opCodelist)
        {
            if (opCode == newOpCode)
            {
                if (Effect_Limit_Factory())
                {
                    otherRole = otherrole;
                    this.role.soul.SkillPerception(this);
                    OpEffect_Factory(opCode, otherrole, values);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 获取可用指令
    /// 返回一个指令组，表示可以接受的指令
    /// </summary>
    public abstract List<OpCode> GetOp(OpCode newOpCode);

    public abstract int GetMp();

    public abstract int GetHp();

    /// <summary>
    /// 检查血蓝是否足够消耗
    /// </summary>
    public bool CheckConsume()
    {
        if (Mp < 0 && role.Mp < Math.Abs(Mp))
        {
            Debug.Log("灵力不足");
            return false;
        }

        if (Hp < 0 && role.hp < Math.Abs(Hp))
        {
            Debug.Log("体力不足");
            return false;
        }

        if (Ep < 0 && role.ep < Math.Abs(Ep))
        {
            Debug.Log("耐力不足");
            return false;
        }

        return true;
    }

    Sprite icon;

    /// <summary>
    /// 设置技能图标
    /// </summary>
    public void SetIcon(Sprite icon)
    {
        this.icon = icon;
    }

    /// <summary>
    /// 获取技能图标
    /// </summary>
    /// <returns></returns>
    public Sprite GetIcon()
    {
        return icon;
    }

    public void AddEvent(float time, TimeNode.Method timeEvent, bool continuity)
    {
        TimeNode t = new TimeNode(time, timeEvent, continuity);
        t.Id = TimeNodeList.Count;
        TimeNodeList.Add(t);
    }

    /// <summary>
    /// 添加一个时间事件
    /// </summary>
    /// <param name="time">上一个事件触发之后经过多少秒后触发</param>
    /// <param name="timeEvent">要触发的方法</param>
    public void AddEvent(float time, TimeNode.Method timeEvent)
    {
        AddEvent(time, timeEvent,false);
    }

    /// <summary>
    /// 清除该技能所有技能事件
    /// 一般这种情况为该技能被打断
    /// </summary>
    public void ClearEvent()
    {
        TimeNodeList.Clear();
    }

    protected abstract void OpEffect_Factory(OpCode opCode, Role otherRole, params float[] values);


    public SkillBase Clone()
    {
        //return this as object;      //引用同一个对象
        return this.MemberwiseClone() as SkillBase; //浅复制
        //return new DrawBase() as SkillBase;//深复制
    }

}
