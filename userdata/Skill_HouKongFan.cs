using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 后空翻
/// </summary>
public class Skill_HouKongFan : SkillBase {
    public static int SkillId = 11;
    public override void Effect()
    {
        
    }

    public override int GetId()
    {
        return 11;
    }

    public override string GetName()
    {
        return "后空翻";
    }

    public override List<OpCode> GetOp(OpCode newOpCode)
    {
        return new List<OpCode> { };
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
        animator.Play("后空翻");
        AddEvent(0.1f, Trigger,0.5f);
        AddEvent(0,End);
        return true;
    }

    public void Trigger()
    {
        role.transform.Translate(Vector3.back * Time.deltaTime * 30F);
    }

}
