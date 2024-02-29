using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    //储存单个区块的信息
    //含有的胜利点
    public int winPoint;
    //编号
    public string Name;
    //影响力槽
    public Influence i1;
    public Influence i2;
    //资源
    public Resource rs;
    //移动城市
    public MoveCity mc;
    //有关第五（六）回合的解禁
    public bool ifallowexplore;


    private void Start()
    {
        ifallowexplore = false;
        Map_Manager.registerBlock(this);
    }
}