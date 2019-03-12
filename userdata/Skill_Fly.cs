using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飞行
/// </summary>
public class Skill_Fly : SkillBase
{

    //该技能是否激活
    bool active = false;

    bool isMove = false;

    //飞行速度系数
    float flySpeed = 1.5f;

    //是否处于上下移动模式
    bool isUp = false;


    public Skill_Fly()
    {
        Mp = -10;
    }

    public override void Effect()
    {
        
        if (active)
        {
            if (!Effect_Limit())
            {
                return;
            }
            if (!isMove)
            {
                soul.PlayAnimator("漂浮");
            }
            isMove = false;
        }

    }

    public override int GetId()
    {
        return (int)SkillId.Fly;
    }

    public override string GetName()
    {
        return "飞行";
    }

    protected override bool Use_Factory(params object[] values)
    {
        if (values == null)
        {
            if (active)
            {
                Debug.Log("飞行模式关闭......");
                //role.transform.position = new Vector3(role.transform.position.x, role.transform.position.y - 1, role.transform.position.z);

                active = false;
                role.ChangeBody = false;
                role.isFly = false;
                skillSystem.DeleteNotingSkill((int)SkillId.Move);
            }
            else
            {
                //InfoManager.Instance.Add("起飞");
                //role.transform.position = new Vector3(role.transform.position.x, role.transform.position.y + 1, role.transform.position.z);
                soul.PlayAnimator("夜符[夜雀]");
                role.isFly = true;
                active = true;
                role.ChangeBody = true;
                skillSystem.AddNotningSkill((int)SkillId.Move);

            }
            AddEvent(0.5f, End);
        }
        if (values == null)
        {
            return true;
        }
        //移动相关内容
        if (!active)
        {
            return false;
        }
        int opCode = (int)values[0];
        if (opCode == (int)OpCode.Forward || opCode == (int)OpCode.Back || opCode == (int)OpCode.Left || opCode == (int)OpCode.Right || opCode == (int)OpCode.Jump || opCode == (int)OpCode.Down)
        {
            if (opCode == (int)OpCode.Forward || opCode == (int)OpCode.Back || opCode == (int)OpCode.Left || opCode == (int)OpCode.Right)
            {
                isMove = true;
                float y = 0;
                switch (role.WeaponId)
                {
                    case (int)EquipmentType.noth:
                        soul.PlayAnimator("飞行");
                        break;
                    case (int)EquipmentType.sword:
                        soul.PlayAnimator("飞行");
                        break;
                }

                if (role.isLocalPlayer)
                {
                    y = Camera.main.transform.rotation.eulerAngles.y;
                    if (opCode == (int)OpCode.Forward)
                    {
                        role.transform.rotation = Quaternion.Euler(0, y, 0);
                    }
                    if (opCode == (int)OpCode.Back)
                    {

                        role.transform.rotation = Quaternion.Euler(0, y + 180, 0);

                    }

                    if (opCode == (int)OpCode.Left)
                    {
                        role.transform.rotation = Quaternion.Euler(0, y - 90, 0);

                    }
                    if (opCode == (int)OpCode.Right)
                    {
                        role.transform.rotation = Quaternion.Euler(0, y + 90, 0);
                    }
                }
                Vector3 v = role.transform.TransformDirection(Vector3.forward);
                role.characterController.Move(v * Time.deltaTime * role.Speed);
            }
            if (opCode == (int)OpCode.Jump)
            {
                if (!isMove)
                {
                    soul.PlayAnimator("升空");
                }
                isMove = true;
                isUp = true;
                role.characterController.Move(Vector3.up * Time.deltaTime * role.Speed);
            }
            if (opCode == (int)OpCode.Down)
            {
                if (!isMove)
                {
                    soul.PlayAnimator("降落");
                }
                isMove = true;
                isUp = true;
                role.characterController.Move(Vector3.down * Time.deltaTime * role.Speed);
            }
        }
        //移动相关结束

        return true;
    }
    /// <summary>
    /// 技能结束
    /// </summary>
    public override void End()
    {
        //opCode = OpCode.Noth;
        skillSystem.activeId = 0;
        //Debug.Log("技能结束");
    }
}
