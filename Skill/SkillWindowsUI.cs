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

    //信息栏
    public GameObject Info;

    private void Awake()
    {
        key = KeyCode.K;
    }

    // Use this for initialization
    void Start()
    {
        Load();
        SkillSystem skillsystem = PlayerManagers.Player.GetComponent<SkillSystem>();
        
        foreach (int skillId in skillsystem._skillIdList)
        {
            Pickup(skillsystem.GetSkillById(skillId));
        }
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
        bool end = false;
        Item = Instantiate(UiItem);
        UiSkill uiSkill = Item.GetComponent<UiSkill>();
        uiSkill.skill = skill;
        uiSkill.text = Info.GetComponent<Text>();
        Image imagesingle = Item.transform.GetComponent<Image>();
        

        imagesingle.overrideSprite = skill.GetIcon();
        for (int i = 0; i < UCellList.Count; i++)
        {
            if (UCellList[i].transform.childCount > 0)
            {
                if (skill.Id == UCellList[i].transform.GetChild(0).GetComponent<UiSkill>().skill.Id)
                {
                    end = true;
                    StartCoroutine(ReturnToPool());
                    Item.transform.SetParent(UIManager.Instance.PoolCanvas.transform);
                }
            }
        }
        if (!end)
        {
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
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(0.0f);
        ObjectPool.Set(Item);
    }
}
