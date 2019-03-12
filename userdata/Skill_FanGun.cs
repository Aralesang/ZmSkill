using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_FanGun : SkillBase
{
    //移动方向
    Vector3 direction;
    public Skill_FanGun()
    {
        this.CancelAll = true;
        Ep = -10;
    }

    public override void Effect()
    {

    }

    public override int GetId()
    {
        return (int)SkillId.ShunBu;
    }

    public override string GetName()
    {
        return "翻滚";
    }

    protected override bool Use_Factory(params object[] values)
    {
        soul.PlayAnimator("前翻滚");
        AddEvent(0.1f, Trigger, 0.25f);
        AddEvent(0, End);
        return true;
    }

    public void Trigger()
    {
        role.transform.Translate(Vector3.forward * Time.deltaTime * 15F);
    }

    public void Hide()
    {
        role.Mode.SetActive(false);
    }

    public void Show()
    {
        role.Mode.SetActive(true);
    }
    /// <summary>
    /// 突袭至目标背后
    /// </summary>
    public void Sudden()
    {
        //算出目标的后背位置
        Vector3 point = role.Target.transform.position;
        point.x = point.x + 1;
        role.transform.position = point;
    }
}
