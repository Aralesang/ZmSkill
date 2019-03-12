using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生死流转斩,剑系列技能可以用的主动技能
/// 大范围挥动剑旋转一圈
/// 该技能为测试用主动技能
/// </summary>
public class Skill_HuiXuanZhan : SkillBase
{
    //第一段攻击伤害倍率
    float atk1 = 1.5f;
    //第二段攻击伤害倍率
    float atk2 = 10f;

    //攻击范围
    float dican = 2;

    public Skill_HuiXuanZhan()
    {
        Ep = -5;
        Description = "必须装备剑系列装备才可以施展\r\n首先使用刺击对正前方的敌人造成"+atk1+"倍伤害，之后使出大范围的旋转斩击对攻击范围内所有敌人造成"+atk2+"倍伤害";
    }

    public override void Effect()
    {
        
        
    }

    public override int GetId()
    {
        return (int)SkillId.HuiXuanZhan;
    }

    public override string GetName()
    {
        return "生死流转斩";
    }

    /// <summary>
    /// 第一击
    /// </summary>
    public void One()
    {
        Role enemy = null;
        float distance = role.attackDistance;
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
                enemy = go;
                distance = temp;
            }
        }
        if (enemy != null)
        {
            Vector3 targetPos = enemy.transform.position;
            targetPos.y = role.transform.position.y;
            role.transform.LookAt(targetPos);
            enemy.GetComponent<SkillSystem>().Use((int)SkillId.Demage, role, (int)role.Physical_Atk, (int)KoType.STIFF);
        }
    }

    /// <summary>
    /// 第二击
    /// </summary>
    public void Two()
    {
        float distance = role.attackDistance + dican;

        List<Role> enemys = new List<Role>();
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
                enemys.Add(go);
                //distance = temp;
            }
        }
        foreach(Role enemy in enemys)
        {
            Vector3 targetPos = enemy.transform.position;
            targetPos.y = role.transform.position.y;
            role.transform.LookAt(targetPos);
            SkillSystem skill = enemy.GetComponent<SkillSystem>();
            skill.Use((int)SkillId.Demage,role,(int)(role.Physical_Atk * atk2), (int)KoType.KNOCK_DOWN);
        }
    }

    public new bool Effect_Limit()
    {
        //玩家死亡状态下不触发技能
        if (role.isSurvive == 1)
        {
            return false;
        }
        //如果当前有技能处于激活状态，则不能激活该技能
        if (skillSystem.activeId > 0)
        {
            Debug.Log("有其他技能激活");
            return false;
        }

        if (role.WeaponId != (int)EquipmentType.sword)
        {
            Debug.Log("必须装备剑才可以施展");
            return false;
        }

        //需要消耗的值不足
        if (!CheckConsume())
        {
            return false;
        }

        return true;
    }

    protected override bool Use_Factory(params object[] values)
    {
        //Debug.Log("回旋斩！");
        soul.PlayAnimator("生死流转斩");
        AddEvent(0.5f,One);
        AddEvent(1f, Two);
        AddEvent(0.3f, End);
        return true;
    }

}
