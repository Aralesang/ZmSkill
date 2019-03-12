using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// 技能管理器，所有对象从该管理器实例化技能
/// </summary>
public class SkillManager : MonoBehaviour {
    private static SkillManager _instance;
    public static SkillManager Instance { get { return _instance; } }

    private Dictionary<int, SkillBase> SkillMap = new Dictionary<int, SkillBase>();
    //public 

    private void Awake()
    {
        Init();
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void AddSkill(SkillBase skill) {
        SkillMap.Add(skill.GetId(),skill);
    }

    /// <summary>
    /// 根据技能ID获取技能新实例
    /// </summary>
    /// <param name="SkillId"></param>
    /// <returns></returns>
    public SkillBase GetSkillById(int SkillId) {
        return SkillMap[SkillId].Clone();
    }

    /// <summary>
    /// 创建技能图标
    /// </summary>
    public void CreateSkillIcon()
    {

    }

    public void Init()
    {
        _instance = this;
        //bundle = 
        var types = Assembly.GetAssembly(typeof(SkillBase)).GetTypes();
        var cType = typeof(SkillBase);
        foreach (var type in types)
        {
            var baseType = type.BaseType;  //获取基类
            while (baseType != null)  //获取所有基类
            {
                if (baseType.Name == cType.Name)
                {
                    object obj = Activator.CreateInstance(type);
                    if (obj != null)
                    {
                        var skill = obj as SkillBase;
                        skill.Init();
                        AddSkill(skill);
                    }
                    break;
                }
                else
                {
                    baseType = baseType.BaseType;
                }
            }

        }
    }
}
