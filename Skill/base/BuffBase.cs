using UnityEngine;
using UnityEditor;

/// <summary>
/// buff系统基类
/// </summary>
public abstract class BuffBase
{

    private int id;
    private string name;
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

    public abstract void Effect();
    public abstract void Init();
    public abstract int GetId();
    public abstract string GetName();
}