using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Channel_1 : MonoBehaviour
{
    //存储单个航道的信息（一个影响力槽）
    //含有的胜利点
    public int winPoint;
    //编号
    public string Name;
    //影响力槽
    public Influence_Channel i1;
    //连接的区块
    public List<Block> Connect_Blocks;
    //有关第五（六）回合的解禁
    public bool ifallowexplore;

    private void Start()
    {
        ifallowexplore = false;
        Map_Manager.registerChannel1(this);
    }
}
