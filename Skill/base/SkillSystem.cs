using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能系统(每个对象都拥有属于自己的技能系统)
/// </summary>
public class SkillSystem : MonoBehaviour
{
    /// <summary>
    /// 技能对象列表
    /// </summary>
    private Dictionary<int, SkillBase> skillObjMap;
    /// <summary>
    /// 技能等级记录
    /// </summary>
    private Dictionary<int, int> skillMap = new Dictionary<int, int>();

    /// <summary>
    /// 状态表
    /// </summary>
    private Dictionary<int, BuffBase> buffMap;

    public Dictionary<int,int> shortCutMap = new Dictionary<int, int>();

    /// <summary>
    /// 当前激活中的技能
    /// 注意:激活中指释放中
    /// 如果是buff类型技能，buff持续时间不算做激活中
    /// 释放buff技能的过程算作激活
    /// buff的实现另外编写逻辑
    /// </summary>
    public int activeId = 0;
    Role role;

    Animator animator;


    /// <summary>
    /// 被禁止的技能列表
    /// </summary>
    public ArrayList NotningSkillLIst = new ArrayList();
    /// <summary>
    /// buff免疫列表
    /// </summary>
    public List<int> NotningBuffList;
    public Dictionary<int, SkillBase> SkillObjMap
    {
        get
        {
            if (skillObjMap == null)
            {
                skillObjMap = new Dictionary<int, SkillBase>();
            }
            return skillObjMap;
        }
        set
        {
            skillObjMap = value;
        }
    }

    public Dictionary<int, BuffBase> BuffMap
    {
        get
        {
            if (buffMap == null)
            {
                buffMap = new Dictionary<int, BuffBase>();
            }
            return buffMap;
        }

        set
        {
            buffMap = value;
        }
    }

    private void Awake()
    {
        role = GetComponent<Role>();
        animator = GetComponent<Animator>();
        NotningSkillLIst = new ArrayList();
        //dynamic skillList = CSScript.Evaluator.LoadFile(path);
        
    }

    /// <summary>
    /// 销毁所有技能
    /// </summary>
    public void SkillAllDestroy()
    {
        if (SkillObjMap == null || SkillObjMap.Count == 0)
        {
            return;
        }
        foreach (SkillBase skill in SkillObjMap.Values)
        {
            skill.DestroyEffect();
        }
        EndAllSkill();
    }

    /// <summary>
    /// 删除所有技能
    /// </summary>
    public void SkillAllRemove()
    {
        if (SkillObjMap == null || SkillObjMap.Count == 0)
        {
            return;
        }
        List<int> skillIdList = new List<int>();
        foreach (int skillid in SkillObjMap.Keys)
        {
            skillIdList.Add(skillid);
        }
        foreach (int skillid in skillIdList)
        {
            SkillBase skill = GetSkillById(skillid);
            skill.Forget();
        }
        EndAllSkill();
    }

    /// <summary>
    /// 销毁所有技能效果
    /// </summary>
    public void SkillAllEffect()
    {
        if (SkillObjMap == null || SkillObjMap.Count == 0)
        {
            return;
        }
        foreach (SkillBase skill in SkillObjMap.Values)
        {
            skill.DestroyEffect();
        }
    }
    /// <summary>
    /// 加载默认技能
    /// </summary>
    public void InitSkillMap()
    {
        if (skillMap.Count == 0)
        {
            foreach (SkillBase skill in SkillManager.Instance.SkillMap.Values)
            {
                skillMap.Add(skill.GetId(),skill.Level);
            }
        }

        InitSkillMap(skillMap);
    }

    public void InitSkillMap(Dictionary<int, int> map)
    {
        SkillAllRemove();
        if (map == null || map.Count == 0)
        {
            Debug.LogError("技能列表加载失败");
            return;
        }
        foreach (int skillId in map.Keys)
        {
            SkillBase skill = SkillManager.Instance.GetSkillById(skillId);
            if (skill != null)
            {
                skill.SkillInit(role);
                //Debug.Log("技能对象字典初始化:" + skill.GetName());
                SkillObjMap.Add(skill.GetId(), skill);
            }
            else
            {
                Debug.LogError("加载技能模板失败");
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        //if (skillObjMap == null || skillObjMap.Count == 0)
        //{
        //    InitSkillMap();
        //}
    }

    /// <summary>
    /// 自动自动发动被动技能效果
    /// </summary>
    void FixedUpdate()
    {
        
        foreach (SkillBase skill in SkillObjMap.Values)
        {
            if (skill.GetRole() == null)
            {
                skill.roleId = this.role.Id;
            }
            if (skill == null)
            {
                Debug.Log("无效技能");
                continue;
            }
            if (skill.Level == 0)
            {
                continue;
            }
            //如果角色已经死亡，且该技能不允许死亡状态下发动
            if (role.IsSurvive == 1 && !skill.IsDeal)
            {
                continue;
            }
            //触发技能队列
            if (skill.TimeNodeList != null && skill.TimeNodeList.Count > 0)
            {
                foreach (TimeNode node in skill.TimeNodeList)
                {
                    if (node.Activ)
                    {
                        if (node.IsOk())
                        {
                            node.method();
                        }
                        break;
                    }
                }
            }
            //清除所有非激活技能
            for (int i = skill.TimeNodeList.Count - 1;i >= 0; i --)
            {
                TimeNode timeNode = skill.TimeNodeList[i];
                if (!timeNode.Activ)
                {
                    skill.TimeNodeList.RemoveAt(i);
                }
            }
            //触发被动效果
            skill?.Effect_Base();
        }
    }
    /// <summary>
    /// buff效果发动
    /// </summary>
    public void BuffEffect()
    {
        foreach (BuffBase buff in BuffMap.Values)
        {
            if (buff == null)
            {
                Debug.Log("无效buff");
                continue;
            }
            buff?.Effect_Base();
        }
    }

    /// <summary>
    /// 技能装载
    /// </summary>
    private void SkillLoad()
    {

    }

    /// <summary>
    /// 通过技能ID获取技能实例
    /// 枚举类型参数重载
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public SkillBase GetSkillById(SkillId skillId)
    {
        return GetSkillById((int)skillId);
    }

    /// <summary>
    /// 通过技能ID获取技能实例
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public SkillBase GetSkillById(int skillId)
    {
        if (SkillObjMap == null || SkillObjMap.Count == 0)
        {
            Debug.Log("没有可用技能:"+role.Id);
            return null;
        }
        if (!SkillObjMap.ContainsKey(skillId))
        {
            Debug.Log("该技"+ skillId + "尚未学会:"+role.Id);
            return null;
        }
        return SkillObjMap[skillId];
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    /// <param name="skillId">技能Id</param>
    /// <param name="tarGetRole()">目标角色</param>
    /// <param name="values">参数列表</param>
    /// <returns></returns>
    public bool Use(SkillId skillId , params object[] values)
    {
        return Use((int)skillId,values);
    }

    public void UseUp(SkillId skillId)
    {
        UseUp((int)skillId);
    }

    public void UseUp(int skillId)
    {
        SkillBase skill = GetSkillById(skillId);
        skill?.UseUp();
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    /// <param name="skillId">技能Id</param>
    /// <param name="tarGetRole()">目标角色</param>
    /// <param name="values">参数列表</param>
    /// <returns></returns>
    public bool Use(int skillId,params object[] values)
    {
        SkillBase skill = GetSkillById(skillId);
        if (skill == null)
        {
            Debug.LogError("技能不存在");
            return false;
        }
        if (skill.Level == 0)
        {
            Debug.LogError("该技能尚未学会");
            return false;
        }
        //如果玩家已经死亡
        if (role.IsSurvive == 1)
        {
            //技能是否允许死亡状态下发动
            if (!skill.IsDeal)
            {
                return false;
            }
        }
        if (!skill.SkillCd())
        {
            Debug.Log(skill.GetName()+":技能正在冷却");
            return false;
        }
        if (IsDisable(skill))
        {
            Debug.Log(skill.GetName()+":已被禁用");
            return false;
        }
        if (!skill.CheckConsume())
        {
            Debug.Log(skill.GetName()+":释放条件不满足");
            return false;
        }
        if (!IsCancel(skill))
        {
            return false;
        }
        //技能基础释放条件
        if (!skill.Limit())
        {
            return false;
        }
        return skill.Use(values);
    }
    /// <summary>
    /// 是否属于禁用技能
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public bool IsDisable(SkillBase skill)
    {
        return skill.Disable;
    }

    /// <summary>
    /// 参数1的技能是否可以被当前打断
    /// </summary>
    /// <returns></returns>
    public bool IsCancel(SkillBase skill)
    {
        //当前没有技能处于激活中
        if (activeId == 0)
        {
            return true;
        }
        //是当前技能
        if (activeId == skill.GetId())
        {
            if (skill.DoubleActive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        SkillBase activeSkill = GetActiveSkill();
        //无法找到当前激活的技能，允许释放其他技能
        if (activeSkill == null)
        {
            return true;
        }
        //目标技能是否可以被打断
        if (!activeSkill.BeAllCancel && !skill.CancelList.Contains(activeId))
        {
            return false;
        }
        //强制结束技能
        activeSkill?.End();
        return true;
    }

    public void ClearAllEvent()
    {
        foreach (SkillBase skill in SkillObjMap.Values)
        {
            skill.ClearEvent();
        }
    }

    /// <summary>
    /// 清除所有技能事件，打断所有技能
    /// </summary>
    public void EndAllSkill()
    {
        activeId = 0;
        foreach (SkillBase skill in SkillObjMap.Values)
        {
            skill.End();
        }
    }

    //清除指定的技能
    public void RemoveEvent(int skillId)
    {
        foreach (SkillBase skill in SkillObjMap.Values)
        {
            if (skillId == skill.GetId())
            {
                skill.End();
                return;
            }
            
        }
    }

    /// <summary>
    /// 添加禁止使用的技能列表
    /// 置于此列表中的技能会立即失效，且无法发动,被动技能也无法幸免
    /// </summary>
    public void AddNotningSkill(int skillId)
    {
        NotningSkillLIst.Add(skillId);
    }

    /// <summary>
    /// 删除指定的技能
    /// </summary>
    /// <param name="skillId"></param>
    public void DeleteNotingSkill(int skillId)
    {
        NotningSkillLIst.Remove(skillId);
    }

    /// <summary>
    /// 获取当前正在激活中的技能
    /// </summary>
    /// <returns></returns>
    public SkillBase GetActiveSkill()
    {
        if (activeId == 0)
        {
            return null;
        }
        SkillBase skill = GetSkillById(activeId);
        return skill;
    }

    /// <summary>
    /// 附加buff
    /// </summary>
    /// <returns></returns>
    public bool AddBuff(BuffBase buff)
    {
        if (NotningBuffList != null && NotningBuffList.Contains(buff.Id))
        {
            Debug.Log(role.Name+"免疫"+buff.Name);
            return false;
        }
        BuffMap.Add(buff.Id, buff);
        return true;
    }

    /// <summary>
    /// 消除buff
    /// </summary>
    /// <returns></returns>
    public bool DelBuff(int buffId)
    {
        if (buffMap == null || !BuffMap.ContainsKey(buffId))
        {
            return false;
        }
        buffMap.Remove(buffId);
        return true;
    }

}
