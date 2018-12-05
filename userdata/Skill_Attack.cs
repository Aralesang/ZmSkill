using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础技能:攻击
/// </summary>
public class Skill_Attack : SkillBase
{
    int stage = 1;//技能攻击阶段

    bool isAdd = true;//是否可以进行攻击追加(多段连锁的间隔

    public override List<OpCode> GetOp(OpCode newOpCode)
    {
        return new List<OpCode> { OpCode.Attack };
    }

    public Skill_Attack()
    {
        Description = "空手时使用拳头进行攻击\r\n使用拳头作战可以更快提升基础体术攻击力";
    }

    /// <summary>
    /// 技能结束
    /// </summary>
    public override void End()
    {
        //role.GetComponent<EquipmentShow>().Reload();
        //opCode = OpCode.Noth;
        skillSystem.currentId = 0;
        stage = 1;
        isAdd = true;
    }

    public override bool Effect_Limit()
    {
        //达到第三阶段后不能继续追加攻击
        if (stage > 3)
        {
            return false;
        }

        //不允许追加攻击的阶段不接受操作
        if (!isAdd)
        {
            return false;
        }

        //玩家死亡状态下不触发技能
        if (role.isSurvive == 1)
        {
            return false;
        }
        //如果当前有技能处于激活状态，且不是该技能，则不能激活该技能
        if (skillSystem.currentId > 0 && skillSystem.currentId != GetId())
        {
            return false;
        }

        //需要消耗的值不足
        if (!CheckConsume())
        {
            return false;
        }

        //if (opCode > OpCode.Noth)
        //{

        return true;
        //}
        //return false;
    }

    public override void Effect()
    {

    }


    public override int GetId()
    {
        return 8;
    }

    public override string GetName()
    {
        return "格斗精通";
    }

    public override SkillTypeEnum GetSkillType()
    {
        return SkillTypeEnum.passive;
    }


    public void Trigger()
    {
        Role enemy = null;
        float distance = role.attackDistance;
        foreach (Role go in RoleManager.Instance.RoleList)
        {
            if (go.Group == role.Group)
            {
                continue;
            }
            float temp = Vector3.Distance(go.transform.position, role.transform.position);
            if (temp <= distance)
            {
                enemy = go;
                distance = temp;
            }
        }
        if (enemy != null)
        {
            Vector3 targetPos = enemy.transform.position;
            targetPos.y = role.transform.position.y;
            role.transform.LookAt(targetPos);
            enemy.GetComponent<SkillSystem>().AddOp(OpCode.Demage, role, (int)role.atk * Level, stage == 3 ? (int)KoType.FLOWN : (int)KoType.STIFF);
        }
        else
        {
            Debug.Log("未命中");
        }

    }

    protected override bool Use_Factory()
    {
        return true;
    }

    /// <summary>
    /// 进入攻击追加阶段
    /// </summary>
    void AddStage()
    {
        stage++;
        isAdd = true;
    }

    protected override void OpEffect_Factory(OpCode opCode,Role otherRole, params float[] values)
    {
        if (role.HandRightEquipentId != (int)EquipmentType.noth)
        {
            return;
        }
        isAdd = false;
        Start();
        if (!CheckConsume())
        {
            Debug.Log("MP不足");
            opCode = OpCode.Noth;
            return;
        }

        switch (stage)
        {
            case 1:
                animator.Play("右直拳");
                TimeNodeList.Add(new TimeNode(0.3f, Trigger));
                TimeNodeList.Add(new TimeNode(0.4f, AddStage));
                TimeNodeList.Add(new TimeNode(0.5f, End));
                break;
            case 2:
                animator.Play("左直拳");
                TimeNodeList.Add(new TimeNode(0.3f, Trigger));
                TimeNodeList.Add(new TimeNode(0.4f, AddStage));
                TimeNodeList.Add(new TimeNode(0.5f, End));
                break;
            case 3:
                animator.Play("高踢(右)");
                TimeNodeList.Add(new TimeNode(0.3f, Trigger));
                TimeNodeList.Add(new TimeNode(1f, End));
                break;
        }
    }
}

