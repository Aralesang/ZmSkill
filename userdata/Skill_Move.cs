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

    public Skill_Move()
    {
        DoubleActive = true;
        BeAllCancel = true;
    }

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


    public override int GetId()
    {
        return (int)SkillId.Move;
    }

    protected override bool Use_Factory(params object[] values)
    {
        
        int opCode = (int)values[0];
        if (opCode == (int)OpCode.Forward || opCode == (int)OpCode.Back || opCode == (int)OpCode.Left || opCode == (int)OpCode.Right)
        {
            MoveState = true;
            float y = 0;
            switch (role.WeaponId)
            {
                case (int)EquipmentType.noth:
                    soul.PlayAnimator("奔跑");
                    break;
                case (int)EquipmentType.sword:
                    soul.PlayAnimator("奔跑_剑");
                    break;
            }
            if (role.photonView != null && role.photonView.IsMine)
            {
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
            
        }
        if (opCode == (int)OpCode.SoptMove)
        {
            Stop();
        }
        return true;
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