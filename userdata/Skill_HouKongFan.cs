using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 后空翻
/// </summary>
public class Skill_HouKongFan : SkillBase {
    public override void Effect()
    {
        
    }

    public override int GetId()
    {
        return (int)SkillId.HouKongFan;
    }

    public override string GetName()
    {
        return "后空翻";
    }

    protected override bool Use_Factory(params object[] values)
    {
        soul.PlayAnimator("后空翻");
        AddEvent(0.1f, Trigger,0.5f);
        AddEvent(0,End);
        return true;
    }

    public void Trigger()
    {
        role.transform.Translate(Vector3.back * Time.deltaTime * 30F);
    }

}
