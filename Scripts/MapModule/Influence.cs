using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Influence : MonoBehaviourPunCallbacks
{
    //1234红黄蓝绿
    private SpriteRenderer spriteRenderer;
    public int whosecolor;
    
    //需要调整的颜色
    private Color newGrey = new Color(0.4f, 0.4f, 0.4f);

    //能不能被放置的三个条件
    public bool ifExplore = false; //是否被开采，在探索区块的时候进行修改
    public MoveCity moveCity;
    public bool isEmpty = true; //是否为空;

    //有关第五（六）回合的解禁
    public bool ifallowexplore;

    private void Start()
    {
        ifallowexplore = false;
        ifExplore = false;
        isEmpty = true;
        Map_Manager.registerInfluence(this);
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
            spriteRenderer.color = newGrey;
            photonView.RPC("updateIsEmpty", RpcTarget.All, true);
            return;
        }
        photonView.RPC("updateIsEmpty", RpcTarget.All, false);
    }

    public void pushToGM()
    {
        if (GameManager.returnInputMode() != 10)
        {
            return;
        }
        if(ifallowexplore == false)
        {
            UIManager.showUISingle("这个地块还未被允许探索");
            return;
        }
        else
        {
            GameManager.addInfluence(this);
        }
    }

    public bool canBeAdd(int whoseTurn)
    {
        if (ifExplore == true && isEmpty == true)
        {
            if (moveCity.whoseCity == 0 || moveCity.whoseCity == whoseTurn)
            {
                return true;
            }    
        }
        UIManager.showUISingle("这个影响力槽不允许放置");
        return false;
    }

    public void changeIfExplore(bool ife)
    {
        photonView.RPC("updateIfExplore", RpcTarget.All, ife);
    }

    [PunRPC]
    void updateIsEmpty(bool ise)
    {
        isEmpty = ise;
    }
    [PunRPC]
    void UpdateColor(int x)
    {
        updateColorSingle(x);
    }
    [PunRPC]
    void updateIfExplore(bool ife)
    {
        ifExplore = ife;
    }
}
