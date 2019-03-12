using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础弹幕攻击
/// </summary>
public class Skill_TanMu : SkillBase
{
    private int dir = 0;//弹幕发射位置

    public Skill_TanMu()
    {
        DoubleActive = true;
        Cd = 0.25f;
        Mp = -1;
    }
    public override void Effect()
    {
        
        
    }

    public override int GetId()
    {
        return (int)SkillId.TanMu;
    }

    public override string GetName()
    {
        return "弹幕";
    }


    /// <summary>
    /// 开火(弹幕发射)
    /// </summary>
    public void Fire(Tanmu danmu,int dir)
    {
        //danmu.transform.position = gameObject.transform.position;
        //如果基础弹幕为空则无法发射,灵气消耗照常
        if (danmu == null)
        {
            //InfoManager.Instance.Add("没有基础弹幕");
            return;
        }
        if (role.LeftHand == null)
        {
            return;
        }
        //弹幕攻击力算法公式（玩家基础弹幕攻击力 * 弹幕技能攻击比率）
        danmu.atk = role.Tanmu_atk * (danmu.multiple / 100);
        GameObject obj = GameObject.Instantiate(danmu.gameObject);
        obj.GetComponent<Tanmu>().userId = role.id;
        obj.transform.rotation = role.transform.rotation;
        if (dir == 0)
        {
            obj.transform.position = role.RightHand.transform.position;
            dir = 1;
        }
        if (dir == 1)
        {
            obj.transform.position = role.LeftHand.transform.position;
            dir = 0;
        }
        if (role.Target != null)
        {
            obj.transform.LookAt(role.Target.transform);
        }

    }

    public void Fire()
    {
        Fire(role.baseTanmu,dir);
    }
    
    protected override bool Use_Factory(params object[] values)
    {
        //技能前摇开始计时
        //StartTriggerTime();
        TimeNodeList.Add(new TimeNode(0.1f, Fire));
        TimeNodeList.Add(new TimeNode(0.1f, Fire));
        TimeNodeList.Add(new TimeNode(0.1f, End));
        role.MpChange(Mp);

        //animator.SetTrigger("攻击阶段1");
        soul.PlayAnimator("双手了连环气弹");
        role.soul.LookTarget();
        //opCode = OpCode.Noth;
        return true;
    }
}
