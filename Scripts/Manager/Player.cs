using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    public int num; // 编号（同时代表颜色）
    public string Name; //进来的名字
    public GameObject cranves;
    public string end_condition; //收尾状态,因为可能会出现别的屎山，所以先这么命名
    //000是没状态
    public bool ifShowFacility = false;//是否展示自己的设备栏
    public int canUseCharacter;//本回合人物技能使用数
    public Player_UIManager player_UIManager;


    //资源
    [Header("资源")]
    public int money;
    public int stone;
    public int yuanshi;
    public int iron;
    public int bigYuanshi;
    //胜利点
    public int winPoint;
    //移动城市位置
    [Header("移动城市位置")]
    public MoveCity city_block;
    //设施面板
    [Header("设施面板")]
    public List<Facility> facilities;
    [Header("人物")]
    //人物卡
    public List<Character> characters;
    public Character show_character;
    //人物弃牌堆
    public List<Character> characters_lost;

    private void Start()
    {
        stone = 0;
        yuanshi = 0;
        iron = 0;
        bigYuanshi = 0;
        winPoint = 0;
        //设备堆canbuy改为false

        end_condition = "000";

        if (photonView.IsMine)
        {
            Name = PhotonNetwork.NickName;
            cranves.SetActive(true);
        }
        else
        {
            Name = photonView.Owner.NickName;
            cranves.SetActive(false);
        }
        GameManager.registerPlayer(this);
    }

    private void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
    }

    public void chooseStartMoveCity() //选择初始点
    {
        UIManager.showUI(Name + "选择出生点");
        GameManager.addActions(1);
    }

    public void putCharacterCard() //放置人物卡
    {
        UIManager.showUI(Name + "放置人物卡");
        GameManager.addActions(6);
    }

    public void action1()//放置影响力
    {
        if (GameManager.returnWhoseTurn() != num || GameManager.returnIsBusy() == true)
        {
            return;
        }
        UIManager.showUI(Name + "选择放置影响力行动");
        GameManager.addActions(2);
    }
    public void action2()//移动影响力两次
    {
        if (GameManager.returnWhoseTurn() != num || GameManager.returnIsBusy() == true)
        {
            return;
        }
        UIManager.showUI(Name + "选择移动影响力行动");
        GameManager.addActions(4);
        GameManager.addActions(2);
        GameManager.addActions(100);
        GameManager.addActions(4);
        GameManager.addActions(2);
    }
    public void action3()//移动城市
    {
        if (GameManager.returnWhoseTurn() != num || GameManager.returnIsBusy() == true)
        {
            return;
        }
        if (yuanshi < 3)
        {
            UIManager.showUI("连移动城市的三个源石碎片都没有，真是好弱啊～");
            return;
        }
        givePlayerResource(0, 0, -3, 0, 0, 0);
        UIManager.showUI(Name + "选择移动城市行动");
        GameManager.addActions(1);
    }
    public void action4()//建设
    {
        if (GameManager.returnWhoseTurn() != num || GameManager.returnIsBusy() == true)
        {
            return;
        }
        UIManager.showUI(Name + "选择建设行动");
        GameManager.addActions(11);
        if(Map_Manager.returnIfShowFacilityM() == true)
        {
            return;
        }
        else
        {
            changeIfShowFacility();
        }
    }
    public void action5()//探索
    {
        if (GameManager.returnWhoseTurn() != num || GameManager.returnIsBusy() == true)
        {
            return;
        }
        UIManager.showUI(Name + "选择探索行动");
        GameManager.addActions(15);
    }
    public void action6()//特殊行动
    {
        if (GameManager.returnWhoseTurn() != num  || GameManager.returnIsBusy() == true)
        {
            return;
        }
        UIManager.showUI(Name + "选择特殊行动");
    }

    public void quickaction1() //使用人物技能(快速行动）
    {
        if(GameManager.returnIsBusy() == true)
        {
            return;
        }
        if(canUseCharacter == 0)
        {
            return;
        }
        else
        {
            canUseCharacter--;
            GameManager.changeIsQuickAction(true);
            UIManager.showUI(Name + "使用" + show_character.name + "技能");
            GameManager.changeWhoseColor(num);
            GameManager.addActions(8);
        }
    }

    public void quickaction2() //宣言城市（快速行动）
    {
        if (GameManager.returnIsBusy() == true)
        {
            return;
        }
        GameManager.changeIsQuickAction(true);
        UIManager.showUI(Name + "宣言城市样式");
        GameManager.changeWhoseColor(num);
    }

    public void getResource() //采集资源
    {
        UIManager.showUI(Name + "采集资源");
        GameManager.addActions(7);
    }

    public void endOneTurn() //收尾
    {
        //对于不同状态,给管理器不同的命令

        //处理人物卡
        if(show_character.count == 0)
        {
            useUpCharacter();
            backLostCharacter();
        }
        else
        {
            show_character.count = 0;
            useUpCharacter();
            if (characters.Count == 0)
            {
                backLostCharacter();
            }
        }
        GameManager.addActions(100);
    }

    //输入多功能按键
    public void freeButton1()
    {
        if(GameManager.returnInputMode() != 70)
        {
            return;
        }
        GameManager.addFreeButton(1);
    }
    public void freeButton2()
    {
        if (GameManager.returnInputMode() != 70)
        {
            return;
        }
        GameManager.addFreeButton(2);
    }
    public void freeButton3()
    {
        if (GameManager.returnInputMode() != 70)
        {
            return;
        }
        GameManager.addFreeButton(3);
    }
    public void freeButton4()
    {
        if (GameManager.returnInputMode() != 70)
        {
            return;
        }
        GameManager.addFreeButton(4);
    }

    public void changeIfShowCityStyle()
    {
        if (GameManager.returnIfStart() == false)
        {
            return;
        }
        Map_Manager.changeIfShowCityStyleM(num);
    }

    public void changeIfShowFacility()
    {
        if (GameManager.returnIfStart() == false)
        {
            return;
        }
        Map_Manager.changeIfShowFacilityM(num);
    }

    public void changeIfShowSelfFacility() //展示自己的设备栏
    {
        if (GameManager.returnIfStart() == false)
        {
            return;
        }
        ifShowFacility = !ifShowFacility;
        if(ifShowFacility == true)
        {
            if(Map_Manager.returnIfShowFacilityM() == true)
            {
                Map_Manager.changeIfShowFacilityM(num);
            }
            if(Map_Manager.returnIfShowCityStyleM() == true)
            {
                Map_Manager.changeIfShowCityStyleM(num);
            }
        }
        foreach(Facility facility in facilities)
        {
            facility.selfF.SetActive(ifShowFacility);
        }
    }

    public bool returnIfShowSelfFacility()
    {
        return ifShowFacility;
    }

    public void givePlayerResource(int Money, int Stone, int Yuanshi, int Iron, int BigYuanshi, int Winpoint)//获取资源
    {
        money += Money;
        stone += Stone;
        yuanshi += Yuanshi;
        iron += Iron;
        bigYuanshi += BigYuanshi;
        winPoint += Winpoint;
        player_UIManager.upgradeResource(stone, yuanshi, iron, bigYuanshi, money, winPoint);
        GameManager.punGiveResource(num, Money, Stone, Yuanshi, Iron, BigYuanshi, Winpoint);
    }

    public void showCanUseCharacter() //显示所有可选人物卡
    {
        foreach(Character character in characters)
        {
            character.show();
        }
    }

    public void useWhichCharacter(Character character) //选择人物卡
    {
        show_character = character;
        characters.Remove(character);
        foreach(Character character1 in characters)
        {
            character1.noShow();
        }
        
    }

    public void useUpCharacter() //置入弃牌区
    {
        characters_lost.Add(show_character);
        show_character.noShow();
        show_character = null;
    }

    public void backLostCharacter() //回收人物卡
    {
        foreach(Character character in characters_lost)
        {
            characters.Add(character);
        }
        characters_lost.Clear();
    }

    public void skip()
    {
        if (GameManager.returnWhoseColor() != num || GameManager.returnIsBusy() == false || GameManager.returnCanSkip() == false) 
        {
            return;
        }
        GameManager.skip();
    }

    public void yes()
    {
        if (GameManager.returnWhoseColor() != num || GameManager.returnIsBusy() == false)
        {
            return;
        }
        GameManager.changeIfYes(true);
    }

    public void back()
    {
        if (GameManager.returnWhoseColor() != num || GameManager.returnIsBusy() == false)
        {
            return;
        }
        GameManager.changeIfBack(true);
    }

    public void registerCharacter(Character character) // 初始化角色卡
    {
        if (!characters.Contains(character))
        {
            characters.Add(character);
        }
    }
}
