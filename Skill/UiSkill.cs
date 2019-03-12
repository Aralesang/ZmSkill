using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSkill : UiIconBase {
    public SkillBase skill;
    public Text text;
    //技能等级
    public Text level;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        level.text = skill.Level.ToString();
	}



    /// <summary>
    /// 鼠标悬停
    /// </summary>
    public void OnVideoPointerEnter()
    {
        if (text != null)
        {
            string info = skill.Name;
            info += "\r\n\r\n" + skill.Description;
            text.text = info;

        }
    }

    public override void Use(Role otherRole, params float[] values)
    {
        skill.Use(otherRole,values);
    }
}
