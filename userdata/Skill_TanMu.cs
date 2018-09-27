using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础弹幕攻击
/// </summary>
public class Skill_TanMu : SkillBase
{
    public override List<OpCode> GetOp(OpCode newOpCode)
    {
        return new List<OpCode> { OpCode.TanMu };
    }

    public static int SkillId = 2;

    protected override void OpEffect_Factory(OpCode opCode,Role otherRole, params float[] values)
    {
        //技能前摇开始计时
        //StartTriggerTime();
        Start();
        TimeNodeList.Add(new TimeNode(0.5f, Trigger));
        TimeNodeList.Add(new TimeNode(0.1f, End));
        role.MpChange(Mp);
        //animator.SetTrigger("攻击阶段1");
        animator.Play("空中_左手出掌");
        //opCode = OpCode.Noth;

    }

    public override void Effect()
    {
        
        
    }

    public override float GetCd()
    {
        return 0;
    }

    public override int GetHp()
    {
        return 0;
    }

    public override int GetId()
    {
        return 2;
    }

    public override int GetMp()
    {
        return -1;
    }

    public override string GetName()
    {
        return "弹幕";
    }

    public override SkillTypeEnum GetSkillType()
    {
        return SkillTypeEnum.passive;
    }


    public void Trigger()
    {
        Fire();
    }

    /// <summary>
    /// 开火(弹幕发射)
    /// </summary>
    public void Fire(Tanmu danmu)
    {
        //danmu.transform.position = gameObject.transform.position;
        //如果基础弹幕为空则无法发射,灵气消耗照常
        if (danmu == null)
        {
            return;
        }
        //弹幕攻击力算法公式（玩家基础弹幕攻击力 * 弹幕技能攻击比率）
        danmu.atk = role.tanmu_atk * (danmu.multiple / 100);
        GameObject obj = GameObject.Instantiate(danmu.gameObject);
        obj.GetComponent<Tanmu>().userId = role.id;
        obj.transform.position = role.leftHand.transform.position;
        if (attackLock.target != null)
        {
            obj.transform.LookAt(attackLock.target.transform);
        }
    }

    public void Fire()
    {
        Fire(role.baseTanmu);
    }

    protected override bool Use_Factory()
    {
        return true;
    }
}
