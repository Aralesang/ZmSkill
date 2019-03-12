using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色属性
/// </summary>
public class Role : MonoBehaviour
{
    /// <summary>
    /// 对象ID(可以随意填写，但必须是唯一ID)
    /// </summary>
    public int id;

    /// <summary>
    /// 对象名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 速度
    /// </summary>
    public float Speed = 1;//速度
    /// <summary>
    /// 生命值
    /// </summary>
    public int hp = 10;
    [SerializeField]
    private int maxHp = 10;
    /// <summary>
    /// 生命形态:0：存活 1：死亡
    /// </summary>
    public int isSurvive = 0;

    public SkillSystem skillSystem;

    /// <summary>
    /// 灵气珠
    /// </summary>
    public int Mp = 10;
    [SerializeField]
    public int MaxMp = 10;


    /// <summary>
    /// 基本攻击力
    /// </summary>
    [SerializeField]
    private int physical_atk = 0;

    /// <summary>
    /// 是否处于移动状态
    /// </summary>
    public bool isMove = false;

    //物防
    [SerializeField]
    private int physique;

    /// <summary>
    /// 体术攻击力
    /// </summary>
    public int Physical_Atk
    {
        get
        {
            return physical_atk;
        }

        set
        {
            physical_atk = value;
        }
    }

    /// <summary>
    /// 最大生命值
    /// </summary>
    public int MaxHp
    {
        get
        {
            return maxHp;
        }

        set
        {
            maxHp = value;
        }
    }

    public CharacterController characterController;
    public Animator animator;


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        skillSystem = GetComponent<SkillSystem>();
    }   

    private void Update()
    {
        if (hp <= 0)
        {
            animator.Play("死亡");
            isSurvive = 1;
        }

    }

    /// <summary>
    /// 生命值变化
    /// </summary>
    public void HpChange(int value)
    {
        hp += value;
        if (hp > MaxHp)
        {
            hp = MaxHp;
        }
        if (hp < 0)
        {
            hp = 0;
        }
    }


    /// <summary>
    /// 灵气变化
    /// </summary>
    /// <param name="value"></param>
    public void MpChange(int value)
    {
        Mp += value;
        if (Mp > MaxMp)
        {
            Mp = MaxMp;
        }
        if (Mp < 0)
        {
            Mp = 0;
        }
    }
}
