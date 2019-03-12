using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 击倒方向枚举
/// </summary>
public enum KoType
{
    /// <summary>
    /// 无效
    /// </summary>
    NOTH,
    /// <summary>
    /// 僵直
    /// </summary>
    STIFF,
    /// <summary>
    /// 击飞
    /// </summary>
    FLOWN,
    /// <summary>
    /// 击倒
    /// </summary>
    KNOCK_DOWN
}

/// <summary>
/// 被击(玩家基础能力，被攻击时会受伤的技能)
/// </summary>
public class Skill_Demage : SkillBase
{
    int demage;

    Vector3 point;

    GameObject hitObj = null;

    Soul soul = null;

    public Skill_Demage()
    {
        DoubleActive = true;
       
    }

    public override void Effect()
    {

    }

    public override int GetId()
    {
        return (int)SkillId.Demage;
    }

    public override string GetName()
    {
        return "被击";
    }


    public void Trigger()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override bool Use_Factory(params object[] values)
    {
        if (role.isSurvive == 1)
        {
            return false;
        }
        if (role.hp <= 0)
        {
            role.isSurvive = 1;
            soul.PlayAnimator("死亡");
        }
        if (hitObj == null)
        {
            hitObj = AssetsManager.Instance.Get<GameObject>("血液溅射_小.prefab");
        }
        Role otherRole = (Role)values[0];
        demage = (int)values[1];
        //是否击飞
        int isFlown = (int)values[2];

        if (values.Length > 3)
        {
            point = (Vector3)values[3];
        }
        else
        {
            point = role.transform.position;
        }

        Vector3 roPoint = new Vector3();
        
        if (otherRole != null)
        {
            roPoint = otherRole.transform.position;
        }
        else
        {
            roPoint = point;
        }
        roPoint.y = role.transform.position.y;
        role.transform.LookAt(roPoint);
        soul = role.soul;
        //防御状态下体质减伤率为100%
        if (soul.isDef)
        {
            demage = otherRole.Physical_Atk - role.Physique;
        }
        else
        {
            //常态状态下体质减伤率为50%
            demage = otherRole.Physical_Atk - (role.Physique / 2);
        }
        demage = demage < 0 ? 0 : demage;
        role.EpChange(-demage);


        GameObject obj = GameObject.Instantiate(hitObj, point, role.transform.rotation);
        GameObject.Destroy(obj, 1);
        Debug.Log(role.Name + "受到" + demage + "点伤害");
        role.HpChange(-demage);

        //受到伤害，打断技能，清除其他技能的计时器


        if (isFlown == (int)KoType.FLOWN)
        {
            soul.PlayAnimator("击飞");
            AddEvent(0, Flown, 0.2f);
            AddEvent(2f, Flown_Up);
            AddEvent(2, End);
        }
        else if (isFlown == (int)KoType.KNOCK_DOWN)
        {
            soul.PlayAnimator("击倒");
            AddEvent(0.5f, Down, 0.2f);
            AddEvent(2f, Down_Up);
            AddEvent(2, End);
        }
        else if (isFlown == (int)KoType.STIFF)
        {
            soul.PlayAnimator("被攻击", 0, 0f);
            AddEvent(1, End);
        }
        else
        {
            AddEvent(0,End);
        }
        return true;
    }

    /// <summary>
    /// 击飞
    /// </summary>
    public void Flown()
    {
        role.transform.Translate(Vector3.back * Time.deltaTime * 16F);
    }

    /// <summary>
    /// 击倒
    /// </summary>
    public void Down()
    {
        role.transform.Translate(Vector3.back * Time.deltaTime * 16f);
    }

    /// <summary>
    /// 获取物体LookAt后的旋转值
    /// </summary>
    /// <param name="originalObj"></param>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    public static Vector3 GetLookAtEuler(Transform originalObj, Vector3 targetPoint)
    {
        //计算物体在朝向某个向量后的正前方
        Vector3 forwardDir = targetPoint - originalObj.position;


        //计算朝向这个正前方时的物体四元数值
        Quaternion lookAtRot = Quaternion.LookRotation(forwardDir);


        //把四元数值转换成角度
        Vector3 resultEuler = lookAtRot.eulerAngles;

        return resultEuler;
    }

    public void Flown_Up()
    {
        soul.PlayAnimator("仰面起身");
    }
    public void Down_Up()
    {
        soul.PlayAnimator("伏地起身");
    }
}
