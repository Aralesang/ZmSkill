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
    private int id = 0;

    /// <summary>
    /// 技能等级，技能属性会根据成长控制函数决定变化
    /// 测试阶段默认每一个等级增加一倍
    /// </summary>
    public int Level = 1;

    /// <summary>
    /// 技能类型:0:主动技能(可以主动按键施展的技能) 1:被动技能(不可以主动施展,但达成条件时会自动施展的技能)
    /// (目前未准备混合型技能，可以通过设置一个主动一个被动来得到效果，混合技能意义不大)
    /// </summary>
    public SkillTypeEnum SkillType = SkillTypeEnum.active;

    /// <summary>
    /// 技能最近一次施展的时间
    /// </summary>
    private float oldtime = 0;

    /// <summary>
    /// 施展之后，直到第二次施展之间所需要经过的时间
    /// </summary>
    private float cd = 0;

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
    /// 发动者灵魂
    /// </summary>
    public Soul soul;

    /// <summary>
    /// 技能系统
    /// </summary>
    public SkillSystem skillSystem;

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
    /// 时间节点集合
    /// </summary>
    public List<TimeNode> TimeNodeList;

    private bool press;
    private bool active;
    private List<int> cancel;
    private bool cancelAll;
    private bool doubleActive = false;
    private bool beAllCancel = false;
    /// <summary>
    /// 技能是否已经处于激活状态（该字段技能第一次激活时为false，当激活中再释放则为true
    /// </summary>
    public bool Active
    {
        get
        {
            return active;
        }

        set
        {
            active = value;
        }
    }
    /// <summary>
    /// 可以打断的技能列表
    /// </summary>
    public List<int> Cancel
    {
        get
        {
            return cancel;
        }

        set
        {
            cancel = value;
        }
    }
    /// <summary>
    /// 是否按住
    /// </summary>
    public bool Press
    {
        get
        {
            return press;
        }

        set
        {
            press = value;
        }
    }
    /// <summary>
    /// 连续激活(是否可以打断自己)
    /// 技能是否可以在已激活状态下再次激活
    /// 适用于连续按下可以进行的连续技类型的技能
    /// </summary>
    public bool DoubleActive
    {
        get
        {
            return doubleActive;
        }

        set
        {
            doubleActive = value;
        }
    }
    /// <summary>
    /// 技能冷却时间
    /// </summary>
    public float Cd
    {
        get
        {
            return cd;
        }

        set
        {
            cd = value;
        }
    }

    /// <summary>
    /// 可以打断所有的技能
    /// 该属性屏蔽打断技能列表，该属性为true时无视打断列表
    /// </summary>
    public bool CancelAll
    {
        get
        {
            return cancelAll;
        }

        set
        {
            cancelAll = value;
        }
    }
    /// <summary>
    /// 是否可以被任何技能打断
    /// </summary>
    public bool BeAllCancel { get => beAllCancel; set => beAllCancel = value; }

    /// <summary>
    /// 弹起，按下技能键后停止按下时调用
    /// </summary>
    public void UseUp()
    {
        Press = false;
    }

    /// <summary>
    /// 技能主动效果系统函数部分
    /// </summary>
    public bool Use(params object[] values)
    {
        Press = true;
        if (!Effect_Limit_Factory())
        {
            //InfoManager.Instance.Add(Name + "发动失败");
            return false;
        }
        this.Start();
        role.MpChange(Mp);
        role.HpChange(Hp);
        role.EpChange(Ep);
        role.soul.SkillPerception(this,values);
        oldtime = Time.time;
        this.Use_Factory(values);
        Active = true;
        return true;
    }

    /// <summary>
    /// 使用技能,主动触发技能时调用
    /// </summary>
    /// <returns></returns>
    protected abstract bool Use_Factory(params object[] values);


    /// <summary>
    /// 技能被动效果系统包装部分
    /// </summary>
    public virtual void Effect_Base()
    {
        if (TimeNodeList != null && TimeNodeList.Count > 0)
        {
            TimeNode timeNode = TimeNodeList[0];
            timeNode.time = timeNode.time - Time.deltaTime;
            //技能前摇倒计时为0，技能开始触发
            if (timeNode.time <= 0)
            {
                timeNode.method();
                timeNode.continuityTime = timeNode.continuityTime - Time.deltaTime;
                //持续触发周期，如果该
                if (timeNode.continuityTime < 0)
                {
                    TimeNodeList.RemoveAt(0);
                }

            }
        }
        //role.soul.SkillPerception(this);
        this.Effect();
    }

    public virtual bool Effect_Limit_Factory()
    {
        //是否属于被禁止使用的技能
        if (skillSystem.NotningBuffList != null && skillSystem.NotningSkillLIst.Contains(GetId()))
        {
            //Debug.Log(GetName()+"已被禁止");
            return false;
        }

        if (!SkillCd())
        {
            return false;
        }

        //检查技能是否符合基本打断规则
        if (!IsCancel())
        {
            return false;
        }
        //玩家死亡状态下不触发技能
        if (role.isSurvive == 1)
        {
            return false;
        }

        //需要消耗的值不足
        if (!CheckConsume())
        {
            return false;
        }
        return Effect_Limit();
    }

    /// <summary>
    /// 该技能是否可以取消当前技能
    /// </summary>
    /// <returns></returns>
    public bool IsCancel()
    {
        int activeId = skillSystem.activeId;
        //当前没有技能处于激活中
        if (activeId == 0)
        {
            return true;
        }
        //是当前技能
        if (activeId == id)
        {
            if (DoubleActive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       
        SkillBase activeSkill = skillSystem.GetActiveSkill();
        //当前技能拥有被任何技能打断的特性，直接返回true
        if (activeSkill.BeAllCancel)
        {
            return true;
        }
        //当前技能不具备打断其他技能的特性
        if (!IsCancel(activeId))
        {
            return false;
        }
        //无法找到当前激活的技能，允许释放其他技能
        if (activeSkill == null)
        {
            return true;
        }
        //目标技能是否可以被打断
        bool isok = IsCancel(activeSkill.GetId());
        if (isok)
        {
            //如果目标技能被打断，则清除目标技能所有事件
            activeSkill?.ClearEvent();
            //并强制结束技能
            activeSkill?.End();
        }
        return isok;
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
        skillSystem.ClearEvent();
        skillSystem.activeId = this.GetId();
    }

    /// <summary>
    /// 技能结束
    /// </summary>
    public virtual void End()
    {
        equipmentShow.Reload();
        //opCode = OpCode.Noth;
        skillSystem.activeId = 0;
        Active = false;
        End_Custom();
        //Debug.Log("技能结束");
    }

    /// <summary>
    /// 自定义技能结束效果
    /// </summary>
    public virtual void End_Custom()
    {

    }

    public void Init()
    {
        this.Name = GetName();
        this.id = GetId();
    }

    /// <summary>
    /// 初始化技能对象属性
    /// </summary>
    public void Init(Role role)
    {
        this.role = role;
        this.soul = role.GetComponent<Soul>();
        this.skillSystem = role.GetComponent<SkillSystem>();
        this.Name = GetName();
        this.id = GetId();
        this.SkillType = SkillTypeEnum.active;
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
    private bool SkillCd()
    {
        //是否处于冷却时间
        if ((oldtime > 0 && Cd > 0) && (Time.time - oldtime < Cd))
        {
            //Debug.Log("技能尚未冷却完毕");
            return false;
        }
        else
        {
            return true;
        }
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

    public void AddEvent(float time, TimeNode.Method timeEvent, float continuityTime)
    {
        TimeNode t = new TimeNode(time, timeEvent, continuityTime);
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
        AddEvent(time, timeEvent, 0);
    }

    /// <summary>
    /// 清除该技能所有技能事件
    /// 一般这种情况为该技能被打断
    /// </summary>
    public void ClearEvent()
    {
        TimeNodeList.Clear();
    }

    /// <summary>
    /// 技能对象复制
    /// </summary>
    /// <returns></returns>
    public SkillBase Clone()
    {
        //return this as object;      //引用同一个对象
        return this.MemberwiseClone() as SkillBase; //浅复制
        //return new DrawBase() as SkillBase;//深复制
    }

    /// <summary>
    /// 技能遗忘:将技能从技能系统中清除，并销毁所有技能所产生的特效和对象
    /// </summary>
    public void Forget()
    {
        DestroyEffect();
        //销毁技能队列

        //删除技能
        skillSystem.SkillMap.Remove(id);
    }

    /// <summary>
    /// 技能效果销毁
    /// </summary>
    public virtual void DestroyEffect()
    {
    }

    /// <summary>
    /// 是否可以打断该技能
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public bool IsCancel(int skillId)
    {
        if (CancelAll)
        {
            return true;
        }
        if (Cancel == null)
        {
            return false;
        }
        if (Cancel.Contains(skillId))
        {
            return true;
        }
        return false;
    }

}
