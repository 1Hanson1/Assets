using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLibrary : MonoBehaviour
{
    static SkillLibrary instance;
    private Dictionary<string, System.Action> characterSkills = new Dictionary<string, System.Action>();
    private Dictionary<string, System.Action> eventSkills = new Dictionary<string, System.Action>();
    private Dictionary<string, System.Action> facilitySkills = new Dictionary<string, System.Action>();
    private Dictionary<string, System.Action> cityStyleSkills = new Dictionary<string, System.Action>();

    //如果选择并确定了使用技能但是不能支付代价，那么该技能会被直接跳过，望周知
    void Start()
    {
        instance = this;
        if (true)
        {
            characterSkills.Add("DXSS1", DKSS1);
            characterSkills.Add("DXSS2", DKSS2);
            characterSkills.Add("XR1", XR1);
            characterSkills.Add("XR2", XR2);
            characterSkills.Add("JJ1", JJ1);
            characterSkills.Add("JJ2", JJ2);
            characterSkills.Add("LS1", LS1);
            characterSkills.Add("LS2", LS2);
            characterSkills.Add("KNT1", KNT1);
            characterSkills.Add("KNT2", KNT2);
        } //添加人物技能
        if (true)
        {
            eventSkills.Add("T1", T1);
            eventSkills.Add("T2", T2);
            eventSkills.Add("T3", T3);
        }//测试
    }

    // 通过技能名称调用对应的技能方法
    public static void UseCharacterSkill(string skillName)
    {
        System.Action skill;
        if (instance.characterSkills.TryGetValue(skillName, out skill))
        {
            skill.Invoke(); // 调用技能方法
        }
        else
        {
            Debug.LogError("Unknown skill: " + skillName);
        }
    }

    public static void UseEventSkill(string skillName)
    {
        System.Action skill;
        if (instance.eventSkills.TryGetValue(skillName, out skill))
        {
            skill.Invoke(); // 调用技能方法
        }
        else
        {
            Debug.LogError("Unknown skill: " + skillName);
        }
    }

    // 各种技能的实现方法
    private void DKSS1()
    {
        Debug.Log("DKSS1");
    }
    private void DKSS2()
    {
        Debug.Log("DKSS2");
    }
    private void XR1()
    {
        Debug.Log("XR1");
    }
    private void XR2()
    {
        Debug.Log("XR2");
    }
    private void LS1()
    {
        Debug.Log("LS1");
    }
    private void LS2()
    {
        Debug.Log("LS2");
    }
    private void JJ1()
    {
        Debug.Log("JJ1");
    }
    private void JJ2()
    {
        Debug.Log("JJ2");
    }
    private void KNT1()
    {
        Debug.Log("KNT1");
    }
    private void KNT2()
    {
        Debug.Log("KNT2");
    }
    private void T1()
    {
        
        GameManager.giveReasource(10, 10, 10, 10, 10, 10);
        Debug.Log("T1");
    }
    private void T2()
    {
        GameManager.giveReasource(10, 10, 10, 10, 10, 10);
        Debug.Log("T2");
    }
    private void T3()
    {
        GameManager.giveReasource(10, 10, 10, 10, 10, 10);
        Debug.Log("T3");
    }
}
