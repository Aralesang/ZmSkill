using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class RoleAttr
{
    public string id;
    public string modeId;
    public string name;
    /// <summary>
    /// 速度
    /// </summary>
    public float speed = 5;//速度
    public float jumpSpeed = 1;
    /// <summary>
    /// 重力
    /// </summary>
    public float gravity = 20;
    /// <summary>
    /// 生命值
    /// </summary>
    public int hp = 100;
    /// <summary>
    /// 生命上限
    /// </summary>
    public int maxHp = 100;
    /// <summary>
    /// 生命形态:0：存活 1：死亡
    /// </summary>
    public int isSurvive = 0;
    /// <summary>
    /// 灵气珠
    /// </summary>
    public int mp = 100;
    /// <summary>
    /// 灵气上限
    /// </summary>
    public int maxMp = 100;
    /// <summary>
    /// 耐力值
    /// </summary>
    public int ep = 50;
    /// <summary>
    /// 耐力上限
    /// </summary>
    public int maxEp = 50;
    /// <summary>
    /// 基本攻击力
    /// </summary>
    public int physical_atk = 0;
    /// <summary>
    /// 弹幕攻击力
    /// </summary>
    public int tanmu_atk = 1;
    /// <summary>
    /// 是否处于飞行状态
    /// </summary>
    public bool isFly = false;
    /// <summary>
    /// 是否处于移动状态
    /// </summary>
    public bool isMove = false;
    /// <summary>
    /// 基本弹幕
    /// </summary>
    public string baseTanmuName = "弹幕_球_白.prefab";
    /// <summary>
    /// 阵营(用于技能区分攻击目标,可自行决定使用，玩家:0 敌对目标:1或者更高)
    /// 加上符号表示绝对敌对阵营,任意两个不同的阵营可以为敌对，也可以是同盟
    /// 但以下情况则为绝对敌对
    /// 阵营1与-1 阵营 25 与 -25
    /// </summary>
    public int group;
    /// <summary>
    /// 是否处于变身状态
    /// </summary>
    public bool changeBody = false;
    //物防
    public int physique;
    //力量
    public int power;
    //弹幕攻击力
    public int mageic;
    //魔防
    public int magicDefense;
}
