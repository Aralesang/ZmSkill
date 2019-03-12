using UnityEngine;
using System.Collections;

/// <summary>
/// 锻体
/// </summary>
public class Skill_DuanTi : SkillBase
{
    public Skill_DuanTi()
    {
        Description = "修炼肉身，每一级提升1点力量（现代巫女拥有50级的锻体等级，其体术攻击力与防御力都堪称无双）";
        Level = 10;
    }
    public override void Effect()
    {
        
    }

    public override void Effect_Init()
    {
        role.Power += Level;
    }

    public override int GetId()
    {
        return (int)SkillId.DuanTi;
    }

    public override string GetName()
    {
        return "锻体";
    }

    protected override bool Use_Factory(params object[] values)
    {
        return false;
    }
}
