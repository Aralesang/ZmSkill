using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// buff系统
/// </summary>
public class BuffSystem : MonoBehaviour
{
    /// <summary>
    /// buff列表
    /// </summary>
    List<BuffBase> BuffList = new List<BuffBase>();
    Role role;

    public Role Role { get => role; set => role = value; }

    private void Start()
    {
        role = GetComponent<Role>();
    }

    private void FixedUpdate()
    {
        for (int i = BuffList.Count - 1; i >= 0;i --)
        {
            BuffBase buff = BuffList[i];
            if (buff.Activation)
            {
                //非永续buff的情况下检查时间
                if (!buff.IsPermanent)
                {
                    if (buff.Time <= 0)
                    {
                        buff.Activation = false;
                    }
                    buff.Time -= 0.1f;
                }
                buff.Effect_Base();
            }
            else
            {
                BuffList.RemoveAt(i);
            }
        }
    }

    public void addBuff(BuffBase buff)
    {
        buff.Role = Role;
        BuffList.Add(buff);
    }

}