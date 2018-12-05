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

    public TimeNode(float time, Method method,float continuityTime)
    {
        this.time = time;

        this.method = method;

        this.continuityTime = continuityTime;
    }

    /// <summary>
    /// 到达该节点后所能够触发下一个节点所必须的时间
    /// </summary>
    public float time;

    /// <summary>
    /// 连续触发周期
    /// </summary>
    public float continuityTime;

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
