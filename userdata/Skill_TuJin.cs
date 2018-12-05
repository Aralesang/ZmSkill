using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 突进攻击
/// </summary>
public class Skill_TuJin : SkillBase
{
    GameObject target;
    //命中的敌人队列
    List<Role> enemyQue;
    //攻击距离
    int dican = 10;
    //攻击倍率
    int reat = 5;
    public Skill_TuJin()
    {
        Ep = -10;
        Description = "向前突进瞬间贯穿前方所有的敌人造成"+ reat + "倍伤害";
    }

    public override void Effect()
    {
        
    }

    public override int GetId()
    {
        return 10;
    }

    public override string GetName()
    {
        return "突进";
    }

    public override List<OpCode> GetOp(OpCode newOpCode)
    {
        return null;
    }

    public override SkillTypeEnum GetSkillType()
    {
        return SkillTypeEnum.active;
    }

    protected override void OpEffect_Factory(OpCode opCode, Role otherRole, params float[] values)
    {
        
    }

    protected override bool Use_Factory()
    {
        enemyQue = new List<Role>();
        float distance = role.attackDistance + dican;
        foreach (Role go in RoleManager.Instance.RoleList)
        {
            if (go.Group == role.Group)
            {
                continue;
            }
            if (go.id == role.id)
            {
                continue;
            }
            float temp = Vector3.Distance(go.transform.position, role.transform.position);
            if (temp <= distance)
            {
                enemyQue.Add(go);
                //distance = temp;
            }
        }
        Debug.Log("突进");
        target = attackLock.target;
        attackLock.target = null;
        AddEvent(0, Trigger,0.5f);
        AddEvent(0.5f, End);
        return true;
    }

    public void Trigger()
    {
        role.transform.Translate(Vector3.forward * Time.deltaTime * 30F);
        animator.Play("突进");
    }
    public override void End()
    {
        foreach (Role role in enemyQue)
        {
            SkillSystem skill = role.GetComponent<SkillSystem>();
            skill.AddOp(OpCode.Demage, role, (int)(role.atk * reat), (int)KoType.KNOCK_DOWN);
        }
        equipmentShow.Reload();
        //opCode = OpCode.Noth;
        skillSystem.currentId = 0;
        attackLock.target = target;
    }
}
