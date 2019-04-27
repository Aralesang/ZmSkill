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
public class SkillBase
{
    /// <summary>
    /// 技能名字
    /// </summary>
    private string Name = "技能模板";

    /// <summary>
    /// 技能ID
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

    public string roleId;
    /// <summary>
    /// 是否被禁用
    /// </summary>
    public bool Disable { set; get; }
    public Role role;
    /// <summary>
    /// 技能发动者对象
    /// </summary>
    public Role GetRole()
    {
        return role;
    }


    /// <summary>
    /// 技能图标路径
    /// </summary>
    public string iconPath = null;

    /// <summary>
    /// 技能描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 时间节点集合
    /// </summary>
    public List<TimeNode> TimeNodeList;

    private bool press;
    private bool active;
    private List<int> cancel = new List<int>();
    private bool cancelAll;
    private bool doubleActive = false;
    private bool beAllCancel = false;
    private bool isDeal = false;
    private bool isStiffness = false;
    public int timeNodeIndex = 0;//队列节点激活位置
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
    public List<int> CancelList
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
    public int Id { get => id; set => id = value; }
    /// <summary>
    /// 死亡状态下是否可以发动
    /// </summary>
    public bool IsDeal { get => isDeal; set => isDeal = value; }
    /// <summary>
    /// 是否僵直
    /// </summary>
    public bool IsStiffness { get => isStiffness; set => isStiffness = value; }

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
        //if (!Effect_Limit_Factory())
        //{
        //    //InfoManager.Instance.Add(Name + "发动失败");
        //    return false;
        //}
        this.Start();
        GetRole().MpChange(Mp);
        GetRole().HpChange(Hp);
        oldtime = Time.time;
        this.Use_Factory(values);
        Active = true;
        return true;
    }

    /// <summary>
    /// 使用技能,主动触发技能时调用
    /// </summary>
    /// <returns></returns>
    protected virtual bool Use_Factory(params object[] values)
    {
        return true;
    }


    /// <summary>
    /// 技能被动效果系统包装部分
    /// </summary>
    public virtual void Effect_Base()
    {
        
        //role.soul.SkillPerception(this);
        this.Effect();
    }

    public SkillSystem GetSkillSystem()
    {
        if (GetRole() == null)
        {
            return null;
        }
        return GetRole().GetComponent<SkillSystem>();
    }

    public bool Limit()
    {
        
        return Effect_Limit();
    }

    /// <summary>
    /// 效果函数限制器
    /// 用于自定义函数效果限制
    /// </summary>
    public virtual bool Effect_Limit()
    {
        return true;
    }

    /// <summary>
    /// 被动技能指令触发部分
    /// </summary>
    public virtual void Effect()
    {
        
    }

    /// <summary>
    /// 初始化激活效果,该效果会在持有该技能时，玩家第一次创建时进行初始化
    /// 或者学会该技能时进行一次激活
    /// </summary>
    public virtual void Init()
    {

    }

    /// <summary>
    /// 技能开始
    /// </summary>
    public void Start()
    {
        //每个技能开始之前，清除之前技能遗留的事件节点
        GetSkillSystem().ClearAllEvent();
        GetSkillSystem().activeId = this.GetId();
    }

    /// <summary>
    /// 技能结束
    /// </summary>
    public virtual void End()
    {
        //opCode = OpCode.Noth;
        GetSkillSystem().activeId = 0;
        Active = false;
        ClearEvent();
        End_Custom();
        //Debug.Log("技能结束");
    }

    /// <summary>
    /// 自定义技能结束效果
    /// </summary>
    public virtual void End_Custom()
    {

    }

    /// <summary>
    /// 初始化技能对象属性
    /// </summary>
    public void SkillInit(Role role)
    {
        this.roleId = role.Id;
        this.Name = GetName();
        this.Id = GetId();
        this.SkillType = SkillTypeEnum.active;
        this.TimeNodeList = new List<TimeNode>();
        this.role = role;
        Init();
    }



    /// <summary>
    /// 技能冷却时间系统(视情况可能不被调用)
    /// 技能可以不需要冷却时间，SpellCard级别以积攒B或卡槽为发动条件
    /// 如果技能没有冷却时间，则灵气的恢复难度需要提高
    /// 非特殊符卡或特殊事件则无法恢复灵气
    /// </summary>
    public bool SkillCd()
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
    public virtual string GetName()
    {
        return null;
    }

    /// <summary>
    /// 获取技能ID
    /// </summary>
    /// <returns></returns>
    public virtual int GetId()
    {
        return Id;
    }


    /// <summary>
    /// 检查血蓝是否足够消耗
    /// </summary>
    public bool CheckConsume()
    {
        if (Mp < 0 && GetRole().Mp < Math.Abs(Mp))
        {
            Debug.Log("灵力不足");
            return false;
        }

        if (Hp < 0 && GetRole().Hp < Math.Abs(Hp))
        {
            Debug.Log("体力不足");
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
        foreach (TimeNode timeNode in TimeNodeList)
        {
            timeNode.Activ = false;
        }
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
        GetSkillSystem().SkillObjMap.Remove(Id);
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
        if (CancelList == null)
        {
            return false;
        }
        if (CancelList.Contains(skillId))
        {
            return true;
        }
        return false;
    }
}
