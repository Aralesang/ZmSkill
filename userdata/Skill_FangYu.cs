using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 防御
/// </summary>
public class Skill_FangYu : SkillBase
{
    public Skill_FangYu()
    {
        
    }

    public override void Effect()
    {
        if (!Press && skillSystem.activeId == GetId())
        {
            AddEvent(0,End);
        }
    }
    public override void End_Custom()
    {
        role.soul.isDef = false;
    }
    public override int GetId()
    {
        return (int)SkillId.FangYu;
    }

    public override string GetName()
    {
        return "防御";
    }

    protected override bool Use_Factory(params object[] values)
    {
        soul.PlayAnimator("防御");
        role.soul.isDef = true;
        return true;
    }
}