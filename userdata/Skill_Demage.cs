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

    public static int SkillId = 3;

    GameObject hitObj = null;
    public override List<OpCode> GetOp(OpCode newOpCode)
    {
        return new List<OpCode> { OpCode.Demage };
    }

    public override bool Effect_Limit()
    {
        //玩家死亡状态下不触发技能
        if (role.isSurvive == 1)
        {
            return false;
        }

        //需要消耗的值不足
        if (!CheckConsume())
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
        return 3;
    }

    public override string GetName()
    {
        return "被击";
    }

    public override SkillTypeEnum GetSkillType()
    {
        return SkillTypeEnum.passive;
    }

    public void Trigger()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override bool Use_Factory()
    {
        return true;
    }

    protected override void OpEffect_Factory(OpCode opCode,Role otherRole, params float[] values)
    {
        if (role.isSurvive == 1)
        {
            return;
        }
        if (role.hp <= 0)
        {
            role.isSurvive = 1;
            animator.Play("死亡");
        }
        if (hitObj == null)
        {
            hitObj = AssetsManager.Instance.Get("hitmonster.prefab") as GameObject;
        }
        demage = (int)values[0];
        //是否击飞
        int isFlown = (int)values[1];

        if (values.Length > 2)
        {
            point.x = values[2];
            point.y = values[3];
            point.z = values[4];
        }
        else
        {
            point = role.transform.position;
        }

        Vector3 roPoint = new Vector3();
        //Quaternion quaten = new Quaternion();
        if (otherRole != null)
        {
            roPoint = otherRole.transform.position;
        }
        else
        {
            roPoint = point;
        }

        //roPoint.x = role.transform.position.x;
        //roPoint.z = role.transform.position.z;
        roPoint.y = role.transform.position.y;
        role.transform.LookAt(roPoint);
        //Vector3 rota = GetLookAtEuler(role.transform, point);
        //role.transform.rotation = Quaternion.Euler(rota.x,rota.y,rota.z);
        //Debug.Log("转向："+ rota);
        GameObject obj = GameObject.Instantiate(hitObj, point, role.transform.rotation);
        GameObject.Destroy(obj, 1);
        //InfoManager.Instance.Add(role.Name + "受到" + demage + "点伤害");
        Debug.Log(role.Name + "受到" + demage + "点伤害");
        role.HpChange(-demage);
        
        //受到伤害，打断技能，清除其他技能的计时器
        

        if (isFlown == (int)KoType.FLOWN)
        {
            Start();
            this.ClearEvent();
            animator.Play("击飞");
            AddEvent(0, Flown, 0.5f);
            AddEvent(2f,Flown_Up);
            AddEvent(2, End);
        }
        else if (isFlown == (int)KoType.KNOCK_DOWN)
        {
            Start();
            this.ClearEvent();
            animator.Play("击倒");
            AddEvent(0.5f, Down, 0.5f);
            AddEvent(2f, Down_Up);
            AddEvent(2, End);
        }
        else if (isFlown == (int)KoType.STIFF)
        {
            Start();
            this.ClearEvent();
            animator.Play("被攻击", 0, 0f);
            AddEvent(1, End);
        }
    }

    /// <summary>
    /// 击飞
    /// </summary>
    public void Flown()
    {
        role.transform.Translate(Vector3.back * Time.deltaTime * 4F);
    }

    /// <summary>
    /// 击倒
    /// </summary>
    public void Down()
    {
        role.transform.Translate(Vector3.back * Time.deltaTime * 4f);
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
        animator.Play("仰面起身");
    }
    public void Down_Up()
    {
        animator.Play("伏地起身");
    }
}
