using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 时间节点对象
/// </summary>
public class TimeNode {

    public TimeNode(float time,Method method)
    {
        this.time = time;

        this.method = method;
    }

    public TimeNode(float time, Method method,bool continuity)
    {
        this.time = time;

        this.method = method;

        this.continuity = continuity;
    }

    /// <summary>
    /// 到达该节点后所能够触发下一个节点所必须的时间
    /// </summary>
    public float time;

    /// <summary>
    /// 是否连续触发
    /// </summary>
    public bool continuity;

    public int Id;

    /// <summary>
    /// 记录方法的委托
    /// </summary>
    public delegate void Method();

    /// <summary>
    /// 记录方法的委托对象
    /// </summary>
    public Method method;

}
