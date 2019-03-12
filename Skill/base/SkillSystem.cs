using System.Collections;
using System.Collections.Generic;
using CSScriptLibrary;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 技能系统(每个对象都拥有属于自己的技能系统)
/// </summary>
public class SkillSystem : MonoBehaviour
{
    /// <summary>
    /// 所拥有的技能对象列表
    /// </summary>
    public Dictionary<int, SkillBase> _skillMap;

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
    /// <summary>
    /// 操作码集合
    /// </summary>
    public Queue<OpCode> opCodes = new Queue<OpCode>();

    Animator animator;


    /// <summary>
    /// 被禁止的技能列表
    /// </summary>
    public ArrayList NotningSkillLIst = new ArrayList();
    /// <summary>
    /// buff免疫列表
    /// </summary>
    public List<int> NotningBuffList;
    public Dictionary<int, SkillBase> SkillMap
    {
        get
        {
            if (_skillMap == null)
            {
                _skillMap = new Dictionary<int, SkillBase>();
            }
            return _skillMap;
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
        if (SkillMap == null || SkillMap.Count == 0)
        {
            return;
        }
        foreach (SkillBase skill in SkillMap.Values)
        {
            skill.DestroyEffect();
        }
        ClearEvent();
    }

    /// <summary>
    /// 删除所有技能
    /// </summary>
    public void SkillAllRemove()
    {
        if (SkillMap == null || SkillMap.Count == 0)
        {
            return;
        }
        List<int> skillIdList = new List<int>();
        foreach (int skillid in SkillMap.Keys)
        {
            skillIdList.Add(skillid);
        }
        foreach (int skillid in skillIdList)
        {
            SkillBase skill = GetSkillById(skillid);
            skill.Forget();
        }
        ClearEvent();
    }

    /// <summary>
    /// 销毁所有技能效果
    /// </summary>
    public void SkillAllEffect()
    {
        if (SkillMap == null || SkillMap.Count == 0)
        {
            return;
        }
        foreach (SkillBase skill in SkillMap.Values)
        {
            skill.DestroyEffect();
        }
    }

    //加载技能字典
    public void InitSkillMap()
    {
        SkillAllRemove();
        List<int> SkillIdList = CSscriptManager.Instance.InitSkill(role.ModeId);
        if (SkillIdList == null || SkillIdList.Count == 0)
        {
            Debug.LogError("技能列表加载失败");
            return;
        }
        foreach (int skillId in SkillIdList)
        {
            SkillBase skill = SkillManager.Instance.GetSkillById(skillId);
            if (skill != null)
            {
                skill.Init(role);
                //Debug.Log("技能对象字典初始化:" + skill.GetName());
                SkillMap.Add(skill.GetId(), skill);
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
        
    }

    /// <summary>
    /// 自动自动发动被动技能效果
    /// </summary>
    void FixedUpdate()
    {
        SkillStart();
        //死亡时销毁所有技能效果
        if (role.isSurvive == 1)
        {
            SkillAllDestroy();
        }
    }

    /// <summary>
    /// 技能效果携程
    /// </summary>
    /// <returns></returns>
    public void SkillStart()
    {
        foreach (SkillBase skill in SkillMap.Values)
        {
            if (skill == null)
            {
                Debug.Log("无效技能");
                continue;
            }
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
        if (SkillMap == null || SkillMap.Count == 0)
        {
            Debug.Log("没有可用技能");
            return null;
        }
        if (!SkillMap.ContainsKey(skillId))
        {
            Debug.Log("该技"+ skillId + "尚未学会");
            return null;
        }
        return SkillMap[skillId];
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    /// <param name="skillId">技能Id</param>
    /// <param name="targetRole">目标角色</param>
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
    /// <param name="targetRole">目标角色</param>
    /// <param name="values">参数列表</param>
    /// <returns></returns>
    [PunRPC]
    public bool Use(int skillId,params object[] values)
    {
        SkillBase skill = GetSkillById(skillId);
        if (skill != null)
        {
            //InfoManager.Instance.Add(skill.GetName());
            return skill.Use(values);
        }
        return false;
    }

    /// <summary>
    /// 清除所有技能事件，打断所有技能
    /// </summary>
    public void ClearEvent()
    {
        activeId = 0;
        foreach (SkillBase skill in SkillMap.Values)
        {
            skill.ClearEvent();
        }
    }

    //清除指定的技能
    public void RemoveEvent(int skillId)
    {
        foreach (SkillBase skill in SkillMap.Values)
        {
            if (skillId == skill.GetId())
            {
                skill.ClearEvent();
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
