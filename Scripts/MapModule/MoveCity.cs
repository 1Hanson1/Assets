using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveCity : MonoBehaviourPunCallbacks
{
    public int whoseCity; //不能删,用于influence的判断0->4

    public Resource resource;
    public Block block;

    public SpriteRenderer spriteRenderer;

    private Color newGrey = new Color(0.4f, 0.4f, 0.4f);

    //有关第五（六）回合的解禁
    public bool ifallowexplore;

    private void Start()
    {
        ifallowexplore = false;
        Map_Manager.registerMoveCity(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void upgradeColor(int color)
    {
        photonView.RPC("UpdateColor", RpcTarget.All, color);
    }

    public void updateColorSingle(int color)
    {
        whoseCity = color;
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
        }
    }

    [PunRPC]
    void UpdateColor(int x)
    {
        updateColorSingle(x);
    }

    public void pushToGM()
    {
        if (GameManager.returnInputMode() != 30)
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
            GameManager.addMoveCity(this);
        }
    }
}

