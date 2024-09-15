using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //没选择时全部正面，选完一个后别的盖放，在弃牌堆时盖放，盖放时不能放大
    private Character copy;
    private Vector3 bigLocation = new Vector3(-35, 10, 0);
    private Vector3 bigSacle = new Vector3(420, 660, 1);

    public string skill1;
    public string skill2;
    public bool ifshow;
    public int count; //判断技能有没有放
    public Player player;
    public GameObject cranves;

    private void Start()
    {
        ifshow = true;
        player.registerCharacter(this);
    }

    private void OnMouseEnter()
    {
        if (ifshow == false)
        {
            return;
        }
        if (copy == null)
        {
            Transform parent = this.transform.parent;
            copy = Instantiate(this,bigLocation, Quaternion.identity, parent);
            copy.transform.localScale = bigSacle;
        }
    }

    private void OnMouseExit()
    {
        if (copy != null)
        {
            player.characters.Remove(copy);
            Destroy(copy.gameObject);
            
        }
    }

    public void show()
    {
        ifshow = true;
        cranves.SetActive(true);
    }

    public void noShow()
    {
        ifshow = false;
        cranves.SetActive(false);
    }

    public void pushToGM()
    {
        if (GameManager.returnInputMode() != 61 || player.num != GameManager.returnWhoseColor())
        {
            return;
        }
        else
        {
            GameManager.addCharacter(this);
        }
    }

    public void pushToGMS1()
    {
        if(GameManager.returnInputMode() != 51 || count == 1 || player.num != GameManager.returnWhoseColor())
        {
            return;
        }
        else
        {
            count = 1;
            GameManager.addSkill(skill1);
        }
    }

    public void pushToGMS2()
    {
        if (GameManager.returnInputMode() != 51 || count == 2 || player.num != GameManager.returnWhoseColor())
        {
            return;
        }
        else
        {
            count = 2;
            GameManager.addSkill(skill2);
        }
    }
}
