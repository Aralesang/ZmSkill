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
        return (int)SkillId.TuJin;
    }

    public override string GetName()
    {
        return "突进";
    }


    protected override bool Use_Factory(params object[] values)
    {
        enemyQue = new List<Role>();
        float distance = role.attackDistance + dican;
        foreach (Role go in RoleManager.Instance.RoleMap.Values)
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
        if (role.Target != null)
        {
            role.transform.LookAt(role.Target.transform);
        }
        Debug.Log("突进");
        AddEvent(0, Trigger,0.5f);
        AddEvent(0.5f, End);
        return true;
    }

    public void Trigger()
    {
        role.transform.Translate(Vector3.forward * Time.deltaTime * 30F);
        soul.PlayAnimator("突进");
    }
    public override void End()
    {
        foreach (Role role in enemyQue)
        {
            SkillSystem skill = role.GetComponent<SkillSystem>();
            skill.Use((int)SkillId.Demage, role, (int)(role.Physical_Atk * reat), (int)KoType.KNOCK_DOWN);
        }
        equipmentShow.Reload();
        //opCode = OpCode.Noth;
        skillSystem.activeId = 0;
    }
}
