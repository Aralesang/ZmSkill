using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础技能：移动,使得目标可以进行地面移动
/// </summary>
public class Skill_Move : SkillBase
{
    /// <summary>
    /// 移动速度
    /// </summary>
    public int _speed;

    public bool MoveState = false;

    private void Awake()
    {

    }

    public override void Effect()
    {
        
    }

    public override string GetName()
    {
        return "移动";
    }

    public static int SkillId = 1;

    public override int GetId()
    {
        return 1;
    }

    public override SkillTypeEnum GetSkillType()
    {
        return SkillTypeEnum.passive;
    }

    public override float GetCd()
    {
        return 0;
    }


    public override List<OpCode> GetOp(OpCode newOpCode)
    {
        return new List<OpCode> { OpCode.SoptMove, OpCode.Forward, OpCode.Jump, OpCode.Left, OpCode.Back, OpCode.Right };
    }

    public override int GetMp()
    {
        return 0;
    }

    public override int GetHp()
    {
        return 0;
    }

    protected override bool Use_Factory()
    {
        return true;
    }

    protected override void OpEffect_Factory(OpCode opCode, Role otherRole, params float[] values)
    {
        if (opCode == OpCode.Forward || opCode == OpCode.Back || opCode == OpCode.Left || opCode == OpCode.Right)
        {
            MoveState = true;
            float y = 0;
            switch (role.EquipentType)
            {
                case (int)EquipmentType.noth:
                    animator.Play("奔跑");
                    break;
                case (int)EquipmentType.sword:
                    animator.Play("奔跑_剑");
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
        if (opCode == OpCode.SoptMove)
        {
            Stop();
        }
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    private void Stop()
    {
        End();
        equipmentShow.Reload();
    }

}