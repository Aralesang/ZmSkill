using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飞行
/// </summary>
public class Skill_Fly : SkillBase {

    //该技能是否激活
    bool active = false;

    bool isMove = false;

    //飞行速度系数
    float flySpeed = 1.5f;

    //是否处于上下移动模式
    bool isUp = false;

    public static int SkillId = 7;

    public Skill_Fly()
    {
        Mp = -10;
    }

    public override void Effect()
    {
        if (!Effect_Limit())
        {
            return;
        }
        if (active)
        {
            if (!isMove)
            {
                animator.Play("漂浮");
            }
            isMove = false;
        }

    }

    public override int GetId()
    {
        return 9;
    }

    public override string GetName()
    {
        return "飞行";
    }

    public override List<OpCode> GetOp(OpCode newOpCode)
    {
        return new List<OpCode> { OpCode.SoptMove, OpCode.Forward, OpCode.Jump, OpCode.Left, OpCode.Back, OpCode.Right, OpCode.Down };
    }

    public override SkillTypeEnum GetSkillType()
    {
        return SkillTypeEnum.active;
    }

    protected override void OpEffect_Factory(OpCode opCode, Role otherRole, params float[] values)
    {
        if (!active)
        {
            return;
        }

        if (opCode == OpCode.Forward || opCode == OpCode.Back || opCode == OpCode.Left || opCode == OpCode.Right || opCode == OpCode.Jump || opCode == OpCode.Down)
        {
            if (opCode == OpCode.Forward || opCode == OpCode.Back || opCode == OpCode.Left || opCode == OpCode.Right)
            {
                isMove = true;
                float y = 0;
                switch (role.HandRightEquipentId)
                {
                    case (int)EquipmentType.noth:
                        animator.Play("飞行");
                        break;
                    case (int)EquipmentType.sword:
                        animator.Play("飞行");
                        break;
                }

                if (role.isLocalPlayer)
                {
                    y = Camera.main.transform.rotation.eulerAngles.y;
                    if (opCode == OpCode.Forward)
                    {
                        role.transform.rotation = Quaternion.Euler(0, y, 0);
                    }
                    if (opCode == OpCode.Back)
                    {

                        role.transform.rotation = Quaternion.Euler(0, y + 180, 0);

                    }

                    if (opCode == OpCode.Left)
                    {
                        role.transform.rotation = Quaternion.Euler(0, y - 90, 0);

                    }
                    if (opCode == OpCode.Right)
                    {
                        role.transform.rotation = Quaternion.Euler(0, y + 90, 0);
                    }
                }
                Vector3 v = role.transform.TransformDirection(Vector3.forward);
                role.characterController.Move(v * Time.deltaTime * role.Speed);
            }
            if (opCode == OpCode.Jump)
            {
                if (!isMove)
                {
                    animator.Play("升空");
                }
                isMove = true;
                isUp = true;
                role.characterController.Move(Vector3.up * Time.deltaTime * role.Speed);
            }
            if (opCode == OpCode.Down)
            {
                if (!isMove)
                {
                    animator.Play("降落");
                }
                isMove = true;
                isUp = true;
                role.characterController.Move(Vector3.down * Time.deltaTime * role.Speed);
            }
        }


    }

    protected override bool Use_Factory()
    {
        if (active)
        {
            Debug.Log("飞行模式关闭......");
            //role.transform.position = new Vector3(role.transform.position.x, role.transform.position.y - 1, role.transform.position.z);

            active = false;
            role.ChangeBody = false;
            role.isFly = false;
            skillSystem.DeleteNotingSkill(Skill_Move.SkillId);
        }
        else
        {
            //InfoManager.Instance.Add("起飞");
            //role.transform.position = new Vector3(role.transform.position.x, role.transform.position.y + 1, role.transform.position.z);
            animator.Play("夜符[夜雀]");
            role.isFly = true;
            active = true;
            role.ChangeBody = true;
            skillSystem.AddNotningSkill(Skill_Move.SkillId);

        }
        AddEvent(0.5f, End);
        return true;
    }
    /// <summary>
    /// 技能结束
    /// </summary>
    public override void End()
    {
        //opCode = OpCode.Noth;
        skillSystem.currentId = 0;
        //Debug.Log("技能结束");
    }
}
