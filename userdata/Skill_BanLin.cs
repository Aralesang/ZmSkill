using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 半灵
/// </summary>
public class Skill_BanLin : SkillBase
{

    GameObject banlin;
    //自动射击模式
    bool about;
    //弹幕的MP消耗
    public int c_mp = -1;

    public Skill_BanLin()
    {
        Description = "身体转换为半人状态,并分离出一半成为幽灵\r\n弹幕攻击由半灵部分接管,按下弹幕攻击键后半灵会立即发射一枚弹幕，按住弹幕键1秒，半灵进入自动射击模式将持续发射弹幕,再次按键则会取消\r\n半灵分享半人部分一半灵力值";
    }

    public override void DestroyEffect()
    {
        //InfoManager.Instance.Add("遗忘技能:" + Name);
        if (banlin == null)
        {
            return;
        }
        //销毁半灵体
        MonoBehaviour.Destroy(banlin);
        //还原最大灵气值
        role.MaxMp = role.MaxMp * 2;
        //解除弹幕技能接管
        skillSystem.DeleteNotingSkill((int)SkillId.TanMu);
    }

    public override void Effect()
    {
        if (banlin == null)
        {
            return;
        }
        Vector3 traget = new Vector3(role.transform.position.x,role.transform.position.y,role.transform.position.z);

        traget.x = traget.x + 1.5f;
        traget.y = traget.y + 0.5f;


        float distance = Vector3.Distance(role.transform.position, banlin.transform.position);
        if (distance > 2)
        {
            banlin.transform.LookAt(traget);
        }

        if (distance > 1)
        {
            banlin.transform.Translate(Vector3.forward * Time.deltaTime * 2);
        }

        if (about)
        {
            Fire();
        }

    }

    public override void Effect_Init()
    {
        //召唤一个半灵
        role.MaxMp = role.MaxMp / 2;
        if (role.Mp > role.MaxMp)
        {
            role.Mp = role.MaxMp;
        }
        GameObject obj = AssetsManager.Instance.Get<GameObject>("半灵.prefab");
      
        obj.transform.position = role.transform.position;
        banlin = GameObject.Instantiate(obj);

       //关闭通常弹幕发射
       skillSystem.AddNotningSkill((int)SkillId.TanMu);

    }

    public override int GetId()
    {
        return (int)SkillId.BanLin;
    }

    public override string GetName()
    {
        return "半灵之躯";
    }

    protected override bool Use_Factory(params object[] values)
    {
        int opCode = (int)values[0];
        if (opCode == (int)OpCode.TanMu)
        {
            if (!about)
            {
                AddEvent(0.1f, Fire);
            }
            else
            {
                about = false;
            }
        }
        if (opCode == (int)OpCode.ContinuedTanmu)
        {
            about = true;
        }
        return true;
    }

    /// <summary>
    /// 开火(弹幕发射)
    /// </summary>
    public void Fire(Tanmu danmu)
    {
        //danmu.transform.position = gameObject.transform.position;
        //如果基础弹幕为空则无法发射,灵气消耗照常
        if (danmu == null)
        {
            return;
        }
        //需要消耗的值不足
        if (role.Mp < Math.Abs(c_mp))
        {
            about = false;
            return;
        }
        role.MpChange(c_mp);

        //弹幕攻击力算法公式（玩家基础弹幕攻击力 * 弹幕技能攻击比率）
        danmu.atk = role.Tanmu_atk * (danmu.multiple / 100);
        GameObject obj = GameObject.Instantiate(danmu.gameObject);
        obj.GetComponent<Tanmu>().userId = role.id;
        obj.transform.position = banlin.transform.position;
        if (role.Target != null)
        {
            obj.transform.LookAt(role.Target.transform);
        }
    }

    public void Fire()
    {
        Fire(role.baseTanmu);
    }
}
