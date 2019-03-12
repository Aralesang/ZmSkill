using UnityEngine;
using System.Collections;

public class Skill_Jump : SkillBase
{
    public override void Effect()
    {

    }

    public override int GetId()
    {
        return (int)SkillId.Jump;
    }

    public override string GetName()
    {
        return "跳跃";
    }

    protected override bool Use_Factory(params object[] values)
    {
        soul.PlayAnimator("跳跃");
        AddEvent(1f, End);
        return true;
    }
}
