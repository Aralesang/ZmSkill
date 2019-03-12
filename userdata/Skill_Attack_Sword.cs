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
    public int distance = 0;

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
        if (role.WeaponId != (int)EquipmentType.sword)
        {
            return false;
        }
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
        return true;
    }

    public override void Effect()
    {
        
    }

    public Skill_Attack_Sword()
    {
        Description = "可以使用剑术攻击敌人\r\n使用剑系列武器时，攻击范围增加"+ distance;
        DoubleActive = true;
    }


    public override int GetId()
    {
        return (int)SkillId.Attack_Sword;
    }

    public override string GetName()
    {
        return "剑术精通";
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
        Debug.Log("斩！");
        //Role enemy = null;
        float distance = role.attackDistance + this.distance;
        List<Role> targetList = new List<Role>(); 
        foreach (GameObject obj in TargetManager.TargetList)
        {
            Role go = obj.GetComponent<Role>();
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
                targetList.Add(go);
                Vector3 targetPos = go.transform.position;
                targetPos.y = role.transform.position.y;
                role.transform.LookAt(targetPos);
                role.Target = go.gameObject;
                go.soul.Demage(role,1,role.Physical_Atk);
            }
        }
    }

    protected override bool Use_Factory(params object[] values)
    {
        
        isAdd = false;
        switch (stage)
        {
            case 1:
                //skill.ClearEvent();
                soul.PlayAnimator("挥刀1");
                TimeNodeList.Add(new TimeNode(0.0f, Trigger));
                TimeNodeList.Add(new TimeNode(0.1f, AddStage));
                TimeNodeList.Add(new TimeNode(0.5f, End));
                break;
            case 2:
                soul.PlayAnimator("挥刀2");
                TimeNodeList.Add(new TimeNode(0.0f, Trigger));
                TimeNodeList.Add(new TimeNode(0.1f, AddStage));
                TimeNodeList.Add(new TimeNode(0.5f, End));
                break;
            case 3:
                soul.PlayAnimator("挥刀3");
                TimeNodeList.Add(new TimeNode(0.0f, Trigger));
                TimeNodeList.Add(new TimeNode(0.5f, End));
                break;
        }
        return true;
    }

}
