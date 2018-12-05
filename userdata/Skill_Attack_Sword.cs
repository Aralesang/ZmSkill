using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通攻击:剑
/// </summary>
public class Skill_Attack_Sword : SkillBase {

    int stage = 1;//技能攻击阶段
    bool isAdd = true;//是否可以进行攻击追加(多段连锁的间隔

    /// <summary>
    /// 攻击范围追加
    /// </summary>
    public int distance = 2;

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
        return true;
    }

    public override void Effect()
    {
        
    }

    public Skill_Attack_Sword()
    {
        Description = "可以使用剑术攻击敌人\r\n使用剑系列武器时，攻击范围增加"+ distance;
    }


    public override int GetId()
    {
        return 5;
    }

    public override string GetName()
    {
        return "剑术精通";
    }

    public override List<OpCode> GetOp(OpCode newOpCode)
    {
        return new List<OpCode>() {OpCode.Attack};
    }

    public override SkillTypeEnum GetSkillType()
    {
        return SkillTypeEnum.passive;
    }


    protected override void OpEffect_Factory(OpCode opCode,Role otherRole, params float[] values)
    {
        if (role.HandRightEquipentId != (int)EquipmentType.sword)
        {
            return;
        }
        Start();
        isAdd = false;
        switch (stage)
        {
            case 1:
                //skill.ClearEvent();
                animator.Play("挥刀1");
                TimeNodeList.Add(new TimeNode(0.2f, Trigger));
                TimeNodeList.Add(new TimeNode(0.3f, AddStage));
                TimeNodeList.Add(new TimeNode(0.5f, End));
                break;
            case 2:
                animator.Play("挥刀2");
                TimeNodeList.Add(new TimeNode(0.2f, Trigger));
                TimeNodeList.Add(new TimeNode(0.3f, AddStage));
                TimeNodeList.Add(new TimeNode(0.5f, End));
                break;
            case 3:
                animator.Play("挥刀3");
                TimeNodeList.Add(new TimeNode(0.3f, Trigger));
                TimeNodeList.Add(new TimeNode(1f, End));
                break;
        }
    }

    /// <summary>
    /// 进入攻击追加阶段
    /// </summary>
    void AddStage()
    {
        stage++;
        isAdd = true;
    }

    public void Trigger()
    {
        //Role enemy = null;
        float distance = role.attackDistance + this.distance;
        foreach (Role go in RoleManager.Instance.RoleMap.Values)
        {
            if (go.Group == role.Group)
            {
                continue;
            }
            float temp = Vector3.Distance(go.transform.position, role.transform.position);
            if (temp <= distance)
            {
                Vector3 targetPos = go.transform.position;
                targetPos.y = role.transform.position.y;
                role.transform.LookAt(targetPos);
                go.GetComponent<SkillSystem>().AddOp(OpCode.Demage, role, (int)role.atk, stage == 3 ? (int)KoType.KNOCK_DOWN : (int)KoType.STIFF);
            }
        }
        //if (enemy != null)
        //{
            
        //}
    }

    protected override bool Use_Factory()
    {
        return true;
    }

}
