using System.Collections;
using System.Collections.Generic;
using CSScriptLibrary;
using UnityEngine;

/// <summary>
/// 操作码包装类
/// </summary>
public class OpCodeObject
{
    public OpCodeObject(OpCode opCode, params float[] values)
    {
        this.opCode = opCode;
        this.values = values;
    }
    public OpCode opCode;
    public float[] values;
}

/// <summary>
/// 技能系统(每个对象都拥有属于自己的技能系统)
/// </summary>
public class SkillSystem : MonoBehaviour
{

    /// <summary>
    /// 所拥有的技能ID列表
    /// </summary>
    private List<int> skillIdList;

    /// <summary>
    /// 所拥有的技能对象列表
    /// </summary>
    public Dictionary<int, SkillBase> _skillList;

    

    public List<int> shortCutList;

    /// <summary>
    /// 当前激活中的技能
    /// 注意:激活中指释放中
    /// 如果是buff类型技能，buff持续时间不算做激活中
    /// 释放buff技能的过程算作激活
    /// buff的实现另外编写逻辑
    /// </summary>
    public int currentId = -1;
    Role role;
    /// <summary>
    /// 操作码集合
    /// </summary>
    public Queue<OpCode> opCodes = new Queue<OpCode>();

    Animator animator;

    /// <summary>
    /// 被禁止的技能列表
    /// </summary>
    public ArrayList NotningSkillLIst;

    public Dictionary<int, SkillBase> SkillMap
    {
        get
        {
            if (_skillList == null)
            {
                _skillList = new Dictionary<int, SkillBase>();
            }
            return _skillList;
        }
    }

    public List<int> SkillIdList
    {
        get
        {
            if (skillIdList == null)
            {
                skillIdList = CSscriptManager.Instance.InitSkill(role.id);
            }
            return skillIdList;
        }

        set
        {
            skillIdList = value;
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
        if (SkillIdList == null || SkillIdList.Count == 0)
        {
            return;
        }
        if (SkillMap == null || SkillMap.Count == 0)
        {
            return;
        }
        foreach (int id in SkillIdList)
        {
            SkillBase skill = SkillMap[id];
            skill.Forget();
        }
        ClearEvent();
    }

    public void InitSkillMap()
    {
        SkillAllDestroy();
        SkillIdList = CSscriptManager.Instance.InitSkill(role.ModeId);
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
                //InfoManager.Instance.Add("技能对象字典初始化:" + skill.GetId());
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
    }

    /// <summary>
    /// 技能效果携程
    /// </summary>
    /// <returns></returns>
    public void SkillStart()
    {
        foreach (int skillId in SkillIdList)
        {
            SkillBase skill = GetSkillById(skillId);
            if (skill == null)
            {
                Debug.LogError("无效技能");
                continue;
            }
            skill.Effect_Factory();
        }
    }

    /// <summary>
    /// 技能装载
    /// </summary>
    private void SkillLoad()
    {

    }

    public void AddOp(OpCode opCode, params float[] values)
    {
        AddOp(opCode, null, values);
    }

    /// <summary>
    /// 追加操作(已有操作不重复添加)
    /// </summary>
    /// <param name="opCode"></param>
    public void AddOp(OpCode opCode, Role role, params float[] values)
    {
        ////有窗口打开的状态下不接受指令输入
        //if (DevDogOpenManagers.isOpen)
        //{
        //    return;
        //}
        //被攻击时不接受操作
        if (animator)
        {
            
        }
       
        foreach (int skillId in SkillIdList)
        {
            SkillBase skill = GetSkillById(skillId);
            skill.opEffect(opCode, role, values);
        }


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
            InitSkillMap();
        }
        if (!SkillMap.ContainsKey(skillId))
        {
            Debug.LogError(skillId + "错误的技能ID");
            Debug.LogError(SkillMap.Count);
            return null;
        }
        return SkillMap[skillId];
    }

    public bool Use(int skillId)
    {
        SkillBase skill = GetSkillById(skillId);
        if (skill != null)
        {
            //InfoManager.Instance.Add(skill.GetName());
            return skill.Use();
        }
        return false;
    }

    /// <summary>
    /// 清除所有技能事件，打断所有技能
    /// </summary>
    public void ClearEvent()
    {
        currentId = 0;
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


}
