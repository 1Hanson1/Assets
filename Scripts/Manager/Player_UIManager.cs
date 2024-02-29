using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_UIManager : MonoBehaviour
{
    public Text Stone;
    public Text Yuanshi;
    public Text Iron;
    public Text BigYuanshi;
    public Text Money;
    public Text WinPoint;

    public void upgradeResource(int stone, int yuanshi, int iron, int bigyuanshi, int money, int winpoint)
    {
        Stone.text = stone.ToString();
        Yuanshi.text = yuanshi.ToString();
        Iron.text = iron.ToString();
        BigYuanshi.text = bigyuanshi.ToString();
        Money.text = money.ToString();
        WinPoint.text = winpoint.ToString();
    }
}
