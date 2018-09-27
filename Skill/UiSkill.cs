using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSkill : UiIconBase {
    public SkillBase skill;
    public Text text;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

    public override void Use()
    {
        skill.Use();
    }
}
