using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Facility : MonoBehaviour
{
    public GameObject selfF;
    public string id; //编号，首字母缩写
    public Text NameT;
    public Text winpointT;
    public Text s1T;
    public Text s2T;
    public Text p1T;
    public Text p2T;

    public int winPoint;
    public int color; // 0无, 1->3红黄蓝, 4三色 设备颜色
    public int index = -1; //索引，没被买-1
    public bool ifNeedOnlyOne;

    public SpriteRenderer spriteRenderer;
    public string skill1;
    public string skill2;
    //钱，固源岩，源石碎片，异铁, 源石，胜利点（只是为了统一）
    public List<int> payment1;
    public List<int> payment2;

    public Facility top;
    public Facility right;
    public Facility bottom;
    public Facility left;

    private Color red = new Color(0.8f, 0.5f, 0.5f);
    private Color yellow = new Color(0.85f, 0.85f, 0.5f);
    private Color blue = new Color(0.4f, 0.5f, 0.7f);
    private Color all = new Color(0.88f, 0.61f, 1f);

    private void Start()
    {
        updateColorSingle(color);
        selfF.SetActive(false);
        if(winPoint != 0)
        {
            winpointT.text = winPoint.ToString();
        }
        
        if (color != 0 && index == -1)
        {
            Map_Manager.registerFacility(this);
        }
        
    }

    public void pushToGM()
    {
        if(color == 0 && index == -1)
        {
            Map_Manager.giveFacility(this);
        }
        if(GameManager.returnInputMode() != 62 || index == -1)
        {
            return;
        }
        else
        {
            GameManager.addFacility(this);
        }
    }

    public void pushToGMSkill1()
    {
        if (GameManager.returnInputMode() != 53 || skill1 == "" || index == -1)
        {
            return;
        }
        else
        {
            GameManager.addSkill(skill1);
        }
    }

    public void pushToGMSkill2()
    {
        if (GameManager.returnInputMode() != 53 || skill2 == "" || index == -1)
        {
            return;
        }
        else
        {
            GameManager.addSkill(skill2);
        }
    }

    public void pushToGMPayment1()
    {
        if (GameManager.returnInputMode() != 41 || index != -1)
        {
            return;
        }
        if(ifNeedOnlyOne == true)
        {

        }
        if(NameT.text == "xxx")
        {
            //关于部分设施的特殊情况就写这吧，实力有限
        }
        else
        {
            if (GameManager.canBuyFacility(payment1[0], payment1[1], payment1[2], payment1[3], payment1[4], payment1[5]) == false)
            {
                return;
            }
        }
        GameManager.addFacility(this);
    }

    public void pushToGMPayment2()
    {
        if (GameManager.returnInputMode() != 41 || index != -1)
        {
            return;
        }
        if(ifNeedOnlyOne == true)
        {

        }
        if(NameT.text == "xxx")
        {

        }
        else
        {
            if (GameManager.canBuyFacility(payment2[0], payment2[1], payment2[2], payment2[3], payment2[4], payment2[5]) == false)
            {
                return;
            }
        }
        GameManager.addFacility(this);
    }

    public void updateColorSingle(int color)
    {
        if (color == 1)
        {
            spriteRenderer.color = red;
        }
        else if (color == 2)
        {
            spriteRenderer.color = yellow;
        }
        else if (color == 3)
        {
            spriteRenderer.color = blue;
        }
        else if (color == 4)
        {
            spriteRenderer.color = all;
        }
        else
        {
            spriteRenderer.color = Color.white;
            return;
        }
    }


    public void becomeOtherFacility(Facility facility)
    {
        //这个函数作用是换文本和属性，注册进player使用player里的函数，传入索引
        //用于两个地方，设备堆进展示设备堆，展示设备堆进玩家设备面板
        id = facility.id;
        NameT.text = facility.NameT.text;
        winpointT.text = facility.winpointT.text;
        s1T.text = facility.s1T.text;
        s2T.text = facility.s2T.text;
        p1T.text = facility.p1T.text;
        p2T.text = facility.p2T.text;
        winPoint = facility.winPoint;
        color = facility.color;
        updateColorSingle(color);
        ifNeedOnlyOne = facility.ifNeedOnlyOne;
        //不需要索引
        skill1 = facility.skill1;
        skill2 = facility.skill2;
        for(int i = 0; i < 6; i++)
        {
            payment1[i] = facility.payment1[i];
            payment2[i] = facility.payment2[i];
        }

    }
}
