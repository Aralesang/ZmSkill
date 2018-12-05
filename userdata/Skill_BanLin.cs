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

    public static int SkillId = 6;

    public Skill_BanLin()
    {
        Description = "身体转换为半人状态,并分离出一半成为幽灵\r\n弹幕攻击由半灵部分接管,按下弹幕攻击键后半灵会立即发射一枚弹幕，按住弹幕键1秒，半灵进入自动射击模式将持续发射弹幕,再次按键则会取消\r\n半灵分享半人部分一半灵力值";
    }

    public override void Forget_Factory()
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
        skillSystem.DeleteNotingSkill(Skill_TanMu.SkillId);
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
        banlin =  AssetsManager.Instance.Get("半灵.prefab") as GameObject;
        banlin.transform.position = role.transform.position;
        //关闭通常弹幕发射
        skillSystem.AddNotningSkill(Skill_TanMu.SkillId);

    }

    public override int GetId()
    {
        return 6;
    }

    public override string GetName()
    {
        return "半灵之躯";
    }

    public override List<OpCode> GetOp(OpCode newOpCode)
    {
        return  new List<OpCode>() { OpCode.TanMu,OpCode.ContinuedTanmu };
    }

    public override SkillTypeEnum GetSkillType()
    {
        return SkillTypeEnum.passive;
    }

    protected override void OpEffect_Factory(OpCode opCode, Role otherRole, params float[] values)
    {
        if (opCode == OpCode.TanMu)
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
        if (opCode == OpCode.ContinuedTanmu)
        {
            about = true;
        }
        
        //AddEvent(0.5f, End);
    }

    /// <summary>
    /// 效果函数限制器
    /// 用于检查技能是否处于可释放状态
    /// 默认情况下
    /// 用于限制同一时间只能释放一个技能
    /// 特殊情况可以重写该函数
    /// </summary>
    public override bool Effect_Limit()
    {
        //玩家死亡状态下不触发技能
        if (role.isSurvive == 1)
        {
            return false;
        }

       

        return true;
    }

    protected override bool Use_Factory()
    {
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
        danmu.atk = role.tanmu_atk * (danmu.multiple / 100);
        GameObject obj = GameObject.Instantiate(danmu.gameObject);
        obj.GetComponent<Tanmu>().userId = role.id;
        obj.transform.position = banlin.transform.position;
        if (attackLock.target != null)
        {
            obj.transform.LookAt(attackLock.target.transform);
        }
    }

    public void Fire()
    {
        Fire(role.baseTanmu);
    }
}
