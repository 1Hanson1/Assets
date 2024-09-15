using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
    public GameObject self;
    public string id; //编号,首字母缩写
    public int whichDiff; //123
    public int whichColor; //是什么资源
    public int count; //资源数量
    public Text countText;
    public string skill1;
    public string skill2;
    public string skill3;

    public SpriteRenderer spriteRenderer;
    public Sprite stone;
    public Sprite Yuanshi;
    public Sprite Iron;

    private void Start()
    {
        self.SetActive(false);
        countText.text = count.ToString();
        Map_Manager.registerEvent(this);
        if (whichColor == 1)
        {
            spriteRenderer.sprite = stone;
        }
        else if (whichColor == 2)
        {
            spriteRenderer.sprite = Yuanshi;
        }
        else if (whichColor == 3)
        {
            spriteRenderer.sprite = Iron;
        }
    }

    public void pushToGMS1()
    {
        if (GameManager.returnInputMode() != 52 || skill1 == "")
        {
            return;
        }
        else
        {
            GameManager.addSkill(skill1);
        }
    }

    public void pushToGMS2()
    {
        if (GameManager.returnInputMode() != 52 || skill2 == "")
        {
            return;
        }
        else
        {
            GameManager.addSkill(skill2);
        }
    }

    public void pushToGMS3()
    {
        if (GameManager.returnInputMode() != 52 || skill3 == "")
        {
            return;
        }
        else
        {
            GameManager.addSkill(skill3);
        }
    }
}
