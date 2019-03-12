using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能窗口
/// </summary>
public class SkillWindowsUI : ContainerWindwosUI
{
    GameObject Item;

    private static SkillWindowsUI _instance;
    public static SkillWindowsUI Instance { get { return _instance; } }

    public SkillSystem skillSystem;

    //信息栏
    public GameObject Info;

    private void Awake()
    {
       
        
    }

    // Use this for initialization
    void Start()
    {
    }



    /// <summary>
    /// 初始化为对象拥有的技能列表
    /// </summary>
    public void InitSkill()
    {
        Clear();
        SkillSystem skillsystem = PlayerManagers.Player.GetComponent<SkillSystem>();

        foreach (SkillBase skill in skillsystem.SkillMap.Values)
        {
            //Debug.Log("创建技能:"+ skill.GetName());
            Pickup(skillsystem.GetSkillById(skill.GetId()));
        }
    }

    public override void Init()
    {
        _instance = this;
        key = KeyCode.K;
        if (uiItem == null)
        {
            uiItem = AssetsManager.Instance.Get<GameObject>("技能ui.prefab");
        }
        Load();
        InitSkill();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 添加一个技能
    /// </summary>
    /// <param name="skill"></param>
    public void Pickup(SkillBase skill)
    {
        //bool end = false;
        Item = Instantiate(uiItem);
        
        UiSkill uiSkill = Item.GetComponent<UiSkill>();
        uiSkill.skill = skill;
        //Debug.Log("创建技能UI:" + skill.Name);
        uiSkill.text = Info.GetComponent<Text>();
        Image imagesingle = Item.transform.GetComponent<Image>();
   

        imagesingle.overrideSprite = skill.GetIcon();

        for (int i = 0; i < UCellList.Count; i++)
        {
            if (UCellList[i].transform.childCount == 0)
            {
                Item.transform.SetParent(UCellList[i].transform);

                Item.transform.localPosition = Vector3.zero;

                //存储格子信息
                //ShowItem.Store(UCell[i].transform.name,baseitem);
                break;
            }

        }
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(0.0f);
        ObjectPool.Set(Item);
    }
}
