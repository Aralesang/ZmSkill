using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// buff系统基类
/// </summary>
public class BuffBase
{

    private int id;
    private string name;
    //持续时间
    private float time;
    //是否存活，该字段为false的情况下，buff则会从列表中删除
    private bool activation = true;
    //是否为永续buff
    private bool isPermanent;
    private Role role;

    public BuffBase()
    {
        Init_Base();
    }

    /// <summary>
    /// buff序列
    /// </summary>
    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }
    /// <summary>
    /// buff名称
    /// </summary>
    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }
    /// <summary>
    /// 持续时间
    /// </summary>
    public float Time { get => time; set => time = value; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool Activation { get => activation; set => activation = value; }
    public Role Role { get => role; set => role = value; }
    public bool IsPermanent { get => isPermanent; set => isPermanent = value; }

    /// <summary>
    /// buff效果持续发生部分
    /// </summary>
    public void Effect_Base()
    {
        Effect();
    }

    /// <summary>
    ///  buff附加瞬间效果
    /// </summary>
    public void Init_Base()
    {
        Id = GetId();
        Name = GetName();
        Init();
    }

    public virtual void Effect() { }
    public virtual void Init() { }
    public virtual int GetId() { return 0; }
    public virtual string GetName() { return null; }
}