using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Influence_Channel : MonoBehaviourPunCallbacks
{
    //1234红黄蓝绿
    private SpriteRenderer spriteRenderer;
    public int whosecolor;

    public Channel_1 channel_1;
    public Channel_2 channel_2;

    //需要调整的颜色
    private Color newGrey = new Color(0.4f, 0.4f, 0.4f);

    //能不能被放置的一个条件
    public bool isEmpty = true; //是否为空;

    //有关第五（六）回合的解禁
    public bool ifallowexplore;

    //关于选中
    public bool isSelected;

    private void Start()
    {
        isEmpty = true;
        ifallowexplore = false;
        isSelected = false;
        whosecolor = 0;
        Map_Manager.registerInfluence_Channel(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void upgradeColor(int color)
    {
        photonView.RPC("UpdateColor", RpcTarget.All, color);
    }

    public void updateColorSingle(int color)
    {
        whosecolor = color;
        if (color == 1)
        {
            spriteRenderer.color = Color.red;
        }
        else if (color == 2)
        {
            spriteRenderer.color = Color.yellow;
        }
        else if (color == 3)
        {
            spriteRenderer.color = Color.blue;
        }
        else if (color == 4)
        {
            spriteRenderer.color = Color.green;
        }
        else
        {
            photonView.RPC("updateIsEmpty", RpcTarget.All, true);
            spriteRenderer.color = newGrey;
            return;
        }
        photonView.RPC("updateIsEmpty", RpcTarget.All, false);
    }

    [PunRPC]
    void UpdateColor(int x)
    {
        updateColorSingle(x);
    }

    public void pushToGM()
    {
        if (GameManager.returnInputMode() != 10 && GameManager.returnInputMode() != 11)
        {
            return;
        }
        if (ifallowexplore == false)
        {
            UIManager.showUI("这个地块还未被允许探索");
            return;
        }
        else
        {
            GameManager.addInfluence_Channel(this);
        }
    }

    public bool canBeAdd()
    {
        if(isEmpty == true)
        {
            return true;
        }
        else
        {
            UIManager.showUI("这个影响力槽不允许放置");
            return false;
        }
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;

        if (isSelected)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            upgradeColor(whosecolor);
        }
    }

    [PunRPC]
    void updateIsEmpty(bool ise)
    {
        isEmpty = ise;
    }
}
