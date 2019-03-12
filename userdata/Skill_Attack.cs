﻿using System;
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

    int addPwoer = 1; //每一级增加的力量数量

    public Skill_Attack()
    {
        Description = "连按攻击键可以使出连续的左右拳，停止攻击后会使用一记高踢腿进行收尾";
        DoubleActive = true;
        Level = 1;
    }

    public override void Effect_Init()
    {
        role.Power += Level * addPwoer;
    }

    /// <summary>
    /// 技能结束
    /// </summary>
    public override void End()
    {
        //role.GetComponent<EquipmentShow>().Reload();
        //opCode = OpCode.Noth;
        skillSystem.activeId = 0;
        stage = 1;
        isAdd = true;
    }

    public override bool Effect_Limit()
    {
        if (role.WeaponId != (int)EquipmentType.noth)
        {
            return false;
        }
        //达到第三阶段后不能继续追加攻击
        if (stage > 2)
        {
            stage = 1;
        }

        //不允许追加攻击的阶段不接受操作
        if (!isAdd)
        {
            return false;
        }
        return true;
    }

    public override void Effect()
    {

    }


    public override int GetId()
    {
        return (int)SkillId.Attack;
    }

    public override string GetName()
    {
        return "格斗精通";
    }

    public void Trigger()
    {
        Role enemy = null;
        float distance = role.attackDistance;
        foreach (Role go in RoleManager.Instance.RoleMap.Values)
        {
            if (go.Group == role.Group)
            {
                continue;
            }
            if (go.isSurvive == 1)
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
            enemy.soul.Demage(role,1,role.Physical_Atk);
        }
        else
        {
            //Debug.Log("未命中");
        }

    }

    protected override bool Use_Factory(params object[] values)
    {
 
        isAdd = false;

        switch (stage)
        {
            case 1:
                soul.PlayAnimator("右直拳");
                TimeNodeList.Add(new TimeNode(0.2f, Trigger));
                TimeNodeList.Add(new TimeNode(0.2f, AddStage));
                TimeNodeList.Add(new TimeNode(0.2f, Last));
                break;
            case 2:
                soul.PlayAnimator("左直拳");
                TimeNodeList.Add(new TimeNode(0.2f, Trigger));
                TimeNodeList.Add(new TimeNode(0.2f, AddStage));
                TimeNodeList.Add(new TimeNode(0.2f, Last));
                break;
        }

        return true;
    }

    void Last()
    {
        soul.PlayAnimator("高踢(右)");
        TimeNodeList.Add(new TimeNode(0.2f, Trigger));
        TimeNodeList.Add(new TimeNode(1f, End));
    }

    /// <summary>
    /// 进入攻击追加阶段
    /// </summary>
    void AddStage()
    {
        stage++;
        isAdd = true;
    }
}
