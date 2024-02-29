using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Resource : MonoBehaviourPunCallbacks
{
    public Influence i1;
    public Influence i2;

    public int re_count;
    public Text Count;

    //有关第五（六）回合的解禁
    public bool ifallowexplore;

    public Block block;

    public int whichDiff; //1绿2橙3红
    public int whichColor;//1固源岩，2源石，3异铁
    public SpriteRenderer spriteRenderer;
    public Sprite stone;
    public Sprite Yuanshi;
    public Sprite Iron;

    private void Start()
    {
        ifallowexplore = false;
        Map_Manager.registerResource(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void upGradeResource(int which, int count)
    {
        photonView.RPC("updateResource", RpcTarget.All, which, count);
    }

    public void updateResourceSingle(int which, int count)
    {
        if (which == 1)
        {
            spriteRenderer.sprite = stone;
        }
        else if (which == 2)
        {
            spriteRenderer.sprite = Yuanshi;
        }
        else if (which == 3)
        {
            spriteRenderer.sprite = Iron;
        }
        whichColor = which;
        re_count = count;
        Count.text = count.ToString();
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    [PunRPC]
    void updateResource(int which, int count)
    {
        updateResourceSingle(which, count);
    }

    public void pushToGM()
    {
        if(GameManager.returnInputMode() != 20)
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
            GameManager.addResource(this);
        }
    }
}
