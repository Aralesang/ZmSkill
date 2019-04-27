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

    /// <summary>
    /// 是否处于激活状态，非激活状态下不会被调用
    /// </summary>
    public bool Activ = true;

    /// <summary>
    /// 委托被创建的时间
    /// </summary>
    public float createTime;

    /// <summary>
    /// 委托是否已经可以激活
    /// </summary>
    /// <returns></returns>
    public bool IsOk()
    {
        float tempTime = Time.time;
        if (createTime == 0)
        {
            //Debug.Log(method.Method+"准备触发"+tempTime);
            createTime = tempTime;
        }
        if (!Activ)
        {
            return false;
        }
        //技能前摇是否已经结束
        if (tempTime < createTime + time)
        {
            return false;
        }

        //Debug.Log("节点触发"+method.Method);
        //技能触发周期是否已经结束
        if (tempTime >= createTime + time + continuityTime)
        {
            //Debug.Log("节点关闭"+ method.Method);
            Activ = false;
        }
        return true;
    }
}
