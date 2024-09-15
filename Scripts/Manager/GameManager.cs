using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    static GameManager instance;
    public int FrameRate;
    public List<Player> players; //{1,2,3,4}

    public int sceNum;

    public List<int> actions; //行动指令

    [Header("输入内容")]
    public int inputMode = 0;
    //输入模式，10影响力，10/11航道影响力，20资源点，30移动城市, 40城市样式, 41购买方式
    //51人物技能，52事件技能，53设施技能,61人物，62设备,70多功能按键
    public List<Resource> resources;
    public List<Influence> influences;
    public List<Influence_Channel> influence_Channels;
    public List<MoveCity> moveCities;
    public List<CityStyle> cityStyles;
    public List<Character> characters;
    public List<string> skills;
    public List<int> freeButtons;
    public List<Facility> facilities;

    [Header("工作列表")]
    public List<int> getMoney;//采集资源时获得的钱在一轮结算完后再给钱,探索时当场结算0银行1234
    public List<MoveCity> canAddMoveCities;//移动城市时可被选择的移动城市
    public List<Influence> canAddInfluences;//可以被选择的航道影响力
    public List<Influence_Channel> canAddInfluence_Channels;//可以选择的航道影响力
    public List<Resource> canAddResources;//可以被选择的资源点
    public List<Block> GetBlocks;//采集的地块

    [Header("屎山参数")]
    public int whoseTurn = 0; // 1，2（，3）（，4）
    public int whoseSmallTurn = 0; // 1,2,3,4 //谁的快速行动
    public bool isBusy; //是否有正在进行的动作（用于给快速行动的）
    public bool isStart; //是否正在进行游戏
    public int FarExplore; //解封的回合
    public int whoseColor; //目前用于移除别人影响力,以及快速行动时的临时变量
    public bool isQuickAction;//是否为快速行动，快速行动不需要进位，默认不是
    public bool ifYes; //用于确认
    public bool ifBack; //用于撤回
    public bool ifMove; //用于区分刷资源的时候是不是移动城市时刷的
    public string whichSkill; //character, event, facility
    public bool canSkip; //能不能skip
    public int workCount; //判断有没有新输入
    public Event workEvent;
    public int extraTurn = 0;

    [Header("游戏内轮换参数")]
    //主计数器 0初始 1-8回合 9结算分数0->10, 10时游戏结算
    public int main_count;

    //回合计数器，一回合包含0一轮放置角色卡12两轮回合3一轮采集资源4一轮收尾0->5,5时主计数器进位，收尾要恢复城市样式，人物卡的几种情况
    public int turn_count;

    //轮计数器0->players.count， players.count时回合计数器进位
    public int lun_count;

    //先手指示器,0->players.count，到players.count进位
    public int whofirst; //强制获取时数值应该是需要的数值-1，因为回合交替时需要+1

    private void Awake()
    {
        instance = this;
        isBusy = false;
        isStart = false;
        FrameRate = 60;
        Application.targetFrameRate = FrameRate;

        main_count = -1;
        turn_count = 0;
        lun_count = 0;
        
        inputMode = 0;
        whoseTurn = 0;
    }

    private void Update()
    {
        if(isStart == true)
        {
            UIManager.upgradeWhich("阶段编号" + main_count + "." + turn_count + "." + lun_count);
            UIManager.upgradeWho("现在是" + players[whoseColor - 1].Name + "的回合");
        }//显示ui
        else
        {
            return;
        }
        if(players[whoseColor - 1].Name != PhotonNetwork.NickName)
        {
            return;
        }
        if (actions.Count == 0)
        {
            if (main_count == 0)
            {
                players[whoseTurn - 1].chooseStartMoveCity();
            }//开局选择移动城市位置

            if (main_count == FarExplore && turn_count == 0)
            {
                Map_Manager.canExplore();
                //到时候换成红色区域变换
                UIManager.showUI("红色封闭区域可以探索");
            }//解封不可访问地

            if (main_count == 9)
            {
                int count = 0;
                int winCount = 0;
                foreach(int x in Map_Manager.calWinPoint())
                {
                    players[count].givePlayerResource(0, 0, 0, 0, 0, x);
                    count++;
                    if(count == players.Count)
                    {
                        break;
                    }
                }
                count = 0;
                foreach(Player player in players)
                {
                    winCount += player.stone / 8;
                    winCount += player.yuanshi / 7;
                    winCount += player.iron / 6;
                    winCount += player.bigYuanshi;
                    players[count].givePlayerResource(0, 0, 0, 0, 0, winCount);
                    count++;
                    winCount = 0;
                    if (count == players.Count)
                    {
                        break;
                    }
                }
                UIManager.showUI("游戏结束");
                photonView.RPC("End", RpcTarget.All);
                return;
            }//游戏结束

            if (turn_count == 0 && main_count != 0)
            {
                players[whoseTurn - 1].putCharacterCard();
            }//一轮放人物卡
            if (turn_count == 3 && main_count != 0)
            {
                players[whoseTurn - 1].getResource();
            }//一轮采集资源
            if (turn_count == 4 && main_count != 0)
            {
                players[whoseTurn - 1].endOneTurn();
            }//一轮收尾
            return;
        }//整个if-else执行行动列表中的第一个行动
        else if (actions[0] == 1)
        {
            changeInputMode(30);
            if (moveCities.Count == 0)
            {
                if (ifYes == true)
                {
                    ifYes = false;
                }
                return;
            }
            else
            {
                if (ifBack == true)
                {
                    ifBack = false;
                    moveCities.RemoveAt(0);
                    UIManager.showUISingle("选择移动城市行动");
                    return;
                }
                if (ifYes == false)
                {
                    UIManager.showUISingle(moveCities[0].block.Name + "被选择");
                    return;
                }
                if (main_count == 0) { }
                else
                {
                    //添加与city_block相邻的格子或者是符合移动条件的格子,移动目标处有别人也不能移动(记得处理jj的条件，建议往map函数里加参数，然后这个管理器记录状态）
                    //用地图管理器寻找
                    //如果输入不在符合条件的格子里，退回
                    foreach (Influence_Channel influence_Channel in Map_Manager.blockToInfluenceChannels(players[whoseColor - 1].city_block.block))
                    {
                        foreach (Block block in Map_Manager.influenceChannelsToBlock(influence_Channel))
                        {
                            if (block != players[whoseColor - 1].city_block.block)
                            {
                                if (!canAddMoveCities.Contains(block.mc))
                                {
                                    canAddMoveCities.Add(block.mc);
                                }
                            }
                        }
                    }
                    if (!canAddMoveCities.Contains(moveCities[0]))
                    {
                        UIManager.showUISingle(moveCities[0].block.Name + "不可被选择,请重新选择");
                        moveCities.RemoveAt(0);
                        ifYes = false;
                        return;
                    }
                    //移除当前角色的移动城市,记得放一个影响力在出发点
                    if (players[whoseTurn - 1].city_block.block.i1.isEmpty == true)
                    {
                        players[whoseTurn - 1].city_block.block.i1.upgradeColor(whoseTurn);
                    }
                    else
                    {
                        if (players[whoseTurn - 1].city_block.block.i2.isEmpty == true)
                        {
                            players[whoseTurn - 1].city_block.block.i2.upgradeColor(whoseTurn);
                        }
                    }
                    players[whoseTurn - 1].city_block.upgradeColor(0);
                    players[whoseTurn - 1].city_block.whoseCity = 0;

                }
                inputMode = 0;
                canAddMoveCities.Clear();
                moveCities[0].upgradeColor(whoseTurn);
                moveCities[0].whoseCity = whoseTurn;
                //创飞目的地影响力
                if (moveCities[0].block.i1.whosecolor == whoseTurn) { }
                else
                {
                    moveCities[0].block.i1.upgradeColor(0);
                }
                if (moveCities[0].block.i2.whosecolor == whoseTurn) { }
                else
                {
                    moveCities[0].block.i2.upgradeColor(0);
                }

                instance.players[whoseTurn - 1].city_block = moveCities[0];
                if (moveCities[0].block.Name == "B-1" && main_count == 0)
                {
                    players[whoseTurn - 1].givePlayerResource(0, 2, 2, 2, 0, 0);
                }//初始到B-1的判断

                //添加探索行动
                actions.Add(3);
                resources.Add(moveCities[0].resource);
                ifMove = true;

                moveCities.RemoveAt(0);
                next_action();
            }
        }//放置移动城市;
        else if (actions[0] == 2)
        {
            changeInputMode(10);
            if (influences.Count == 0 && influence_Channels.Count == 0)
            {
                return;
            }
            if (influences.Count == 0)
            {
                if (influence_Channels[0].canBeAdd() == false)
                {
                    influence_Channels.RemoveAt(0);
                    return;
                }
                inputMode = 0;
                influence_Channels[0].upgradeColor(whoseColor);
                influence_Channels.RemoveAt(0);
                next_action();
            }
            else if (influence_Channels.Count == 0)
            {

                if (influences[0].canBeAdd(whoseColor) == false)
                {
                    influences.RemoveAt(0);
                    return;
                }
                inputMode = 0;
                influences[0].upgradeColor(whoseColor);
                influences.RemoveAt(0);
                next_action();
            }
            UIManager.showUI("");
        }// 放置影响力
        else if (actions[0] == 3)
        {
            changeInputMode(20);
            if (resources.Count == 0)
            {
                if (ifYes == true)
                {
                    ifYes = false;
                }
                return;
            }
            else
            {
                if (ifBack == true)
                {
                    ifBack = false;
                    resources.RemoveAt(0);
                    UIManager.showUISingle("选择资源点，亏钱别来找我");
                    return;
                }
                if (ifYes == false)
                {
                    UIManager.showUISingle(resources[0].block.Name + "被选择");
                    return;
                }
                if (resources[0].whichColor != 0 && ifMove == false)
                {
                    UIManager.showUISingle(resources[0].block.Name + "这个地块已经被探索");
                    resources.RemoveAt(0);
                    return;
                }
                if (!canAddResources.Contains(resources[0]) && ifMove == false)
                {
                    resources.Clear();
                    UIManager.showUISingle("一言顶针");
                    return;
                }
                if(resources[0].whichColor == 0)
                {
                    workEvent = Map_Manager.giveEvents(resources[0].whichDiff);
                    resources[0].upGradeResource(workEvent.whichColor, workEvent.count);
                    UIManager.showUISingle("请选择发动的事件技能");
                    addActions(100);
                    addActions(10);
                    resources[0].i1.changeIfExplore(true);
                    resources[0].i2.changeIfExplore(true);
                }
                inputMode = 0;
                if (ifMove == true)
                {
                    ifMove = false;
                    resources.RemoveAt(0);
                    next_action();
                }
                else
                {
                    foreach (Influence_Channel influence_Channel in influence_Channels)
                    {
                        getMoney[whoseColor] -= 2;
                        getMoney[influence_Channel.whosecolor] += 2;
                        influence_Channel.ToggleSelection();
                    }
                    for (int i = 1; i <= players.Count; i++)
                    {
                        players[i - 1].givePlayerResource(getMoney[i], 0, 0, 0, 0, 0);
                        getMoney[i] = 0;
                    }
                    if (resources[0].i1.isEmpty == true)
                    {
                        resources[0].i1.upgradeColor(whoseColor);
                    }
                    else
                    {
                        if (resources[0].i2.isEmpty == true)
                        {
                            resources[0].i2.upgradeColor(whoseColor);
                        }
                    }//在探索的地方加一个自己的影响力
                    resources.RemoveAt(0);
                    next_action();
                }
            }
        }//刷资源点
        else if (actions[0] == 4)
        {
            changeInputMode(10);
            if (influences.Count == 0 && influence_Channels.Count == 0)
            {
                return;
            }
            else if (influences.Count == 0)
            {
                if (influence_Channels[0].whosecolor != whoseTurn)
                {
                    UIManager.showUISingle("这个影响力不是" + players[whoseTurn - 1] + "的");
                    influence_Channels.RemoveAt(0);
                    return;
                }
                inputMode = 0;
                influence_Channels[0].upgradeColor(10);
                influence_Channels.RemoveAt(0);
                next_action();
            }
            else if (influence_Channels.Count == 0)
            {
                if (influences[0].whosecolor != whoseTurn)
                {
                    UIManager.showUISingle("这个影响力不是" + players[whoseTurn - 1] + "的");
                    influences.RemoveAt(0);
                    return;
                }
                inputMode = 0;
                influences[0].upgradeColor(10);
                influences.RemoveAt(0);
                next_action();
            }
            canSkip = false;
        } //移除自己影响力
        else if (actions[0] == 5)
        {
            changeInputMode(10);
            if (influences.Count == 0 && influence_Channels.Count == 0)
            {
                return;
            }
            else if (influences.Count == 0)
            {
                inputMode = 0;
                whoseColor = influence_Channels[0].whosecolor;
                influence_Channels[0].upgradeColor(10);
                influence_Channels.RemoveAt(0);
                next_action();
            }
            else if (influence_Channels.Count == 0)
            {
                inputMode = 0;
                whoseColor = influences[0].whosecolor;
                influences[0].upgradeColor(10);
                influences.RemoveAt(0);
                next_action();
            }
            canSkip = false;
        }//移除别人影响力
        else if (actions[0] == 6)
        {
            changeInputMode(61);
            players[whoseTurn - 1].showCanUseCharacter();
            if (characters.Count == 0)
            {
                return;
            }
            else
            {
                inputMode = 0;
                players[whoseTurn - 1].useWhichCharacter(characters[0]);
            }
            if (whoseTurn - 1 == whofirst)
            {
                players[whoseTurn - 1].canUseCharacter = 2;
            }
            else
            {
                players[whoseTurn - 1].canUseCharacter = 1;
            }
            characters.RemoveAt(0);
            UIManager.showUI("");
            next_action();
        } //放置角色卡,等待角色输入
        else if (actions[0] == 7)
        {
            canSkip = false;
            inputMode = 11;
            if (influence_Channels.Count == 0)
            {
                if (ifYes == true)
                {
                    ifYes = false;
                }
                else
                {
                    workCount = 0;
                    if (!GetBlocks.Contains(players[whoseColor - 1].city_block.block))
                    {
                        GetBlocks.Add(players[whoseColor - 1].city_block.block);
                        foreach (Influence_Channel influence_Channel in Map_Manager.blockToInfluenceChannels(GetBlocks[0]))
                        {
                            canAddInfluence_Channels.Add(influence_Channel);
                        }
                    }
                    return;
                }
            }
            else
            {
                if (ifBack == true)
                {
                    ifBack = false;
                    foreach (Influence_Channel influence_Channel in influence_Channels)
                    {
                        influence_Channel.ToggleSelection();
                    }
                    clearList();
                    return;
                }
                if (ifYes == false)
                {
                    if (workCount == influence_Channels.Count)
                    {
                        return;
                    }
                    else
                    {
                        workCount = influence_Channels.Count;
                        if (canAddInfluence_Channels.Contains(influence_Channels[workCount - 1]))
                        {
                            if (influence_Channels[workCount - 1].channel_2 != null)
                            {
                                if (influence_Channels[workCount - 1].channel_2.i1 == influence_Channels[workCount - 1])
                                {
                                    if (influence_Channels[workCount - 1].channel_2.i2.whosecolor != 0 && influence_Channels[workCount - 1].channel_2.i1.whosecolor == 0)
                                    {
                                        UIManager.showUISingle("你需要优先选择有人的影响力");
                                        influence_Channels.RemoveAt(workCount - 1);
                                        workCount = influence_Channels.Count;
                                        return;
                                    }
                                }
                                if (influence_Channels[workCount - 1].channel_2.i2 == influence_Channels[workCount - 1])
                                {
                                    if (influence_Channels[workCount - 1].channel_2.i1.whosecolor != 0 && influence_Channels[workCount - 1].channel_2.i1.whosecolor == 0)
                                    {
                                        UIManager.showUISingle("你需要优先选择有人的影响力");
                                        influence_Channels.RemoveAt(workCount - 1);
                                        workCount = influence_Channels.Count;
                                        return;
                                    }
                                }
                            }
                            influence_Channels[workCount - 1].ToggleSelection();
                            canAddInfluence_Channels.Remove(influence_Channels[workCount - 1]);
                            foreach (Block block in Map_Manager.influenceChannelsToBlock(influence_Channels[workCount - 1]))
                            {
                                if (!GetBlocks.Contains(block))
                                {
                                    if (block.i1.whosecolor == whoseColor || block.i2.whosecolor == whoseColor || block.mc.whoseCity == whoseColor)
                                    {
                                        GetBlocks.Add(block);
                                    }
                                    foreach (Influence_Channel influence_Channel in Map_Manager.blockToInfluenceChannels(block))
                                    {
                                        if (!canAddInfluence_Channels.Contains(influence_Channel) && !influence_Channels.Contains(influence_Channel))
                                        {
                                            canAddInfluence_Channels.Add(influence_Channel);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            influence_Channels.RemoveAt(workCount - 1);
                            workCount = influence_Channels.Count;
                        }
                        return;
                    }
                }
            }
            if (players[whoseColor - 1].money < 2 * influence_Channels.Count)
            {
                UIManager.showUISingle("钱不够哦～");
                ifYes = false;
                return;
            }
            foreach (Influence_Channel influence_Channel in influence_Channels)
            {
                getMoney[whoseColor] -= 2;
                getMoney[influence_Channel.whosecolor] += 2;
                influence_Channel.ToggleSelection();
                foreach (Block block in Map_Manager.influenceChannelsToBlock(influence_Channel))
                {
                    if (!GetBlocks.Contains(block))
                    {
                        if (block.i1.whosecolor == whoseColor || block.i2.whosecolor == whoseColor || block.mc.whoseCity == whoseColor)
                        {
                            GetBlocks.Add(block);
                        }
                    }
                }
            }
            foreach (Block block1 in GetBlocks)
            {
                if (block1.rs.whichColor == 1)
                {
                    players[whoseColor - 1].givePlayerResource(0, block1.rs.re_count, 0, 0, 0, 0);
                }
                else if (block1.rs.whichColor == 2)
                {
                    players[whoseColor - 1].givePlayerResource(0, 0, block1.rs.re_count, 0, 0, 0);
                }
                else if (block1.rs.whichColor == 3)
                {
                    players[whoseColor - 1].givePlayerResource(0, 0, 0, block1.rs.re_count, 0, 0);
                }
            }
            inputMode = 0;
            next_action();
        }//采集资源多选航道
        else if (actions[0] == 15)
        {
            inputMode = 11;
            if (influence_Channels.Count == 0)
            {
                if (ifYes == true)
                {
                    ifYes = false;
                }
                workCount = 0;
                if (!GetBlocks.Contains(players[whoseColor - 1].city_block.block))
                {
                    GetBlocks.Add(players[whoseColor - 1].city_block.block);
                    foreach (Influence_Channel influence_Channel in Map_Manager.blockToInfluenceChannels(GetBlocks[0]))
                    {
                        canAddInfluence_Channels.Add(influence_Channel);
                    }
                }
                return;
            }
            else
            {
                if (ifBack == true)
                {
                    ifBack = false;
                    foreach (Influence_Channel influence_Channel in influence_Channels)
                    {
                        influence_Channel.ToggleSelection();
                    }
                    clearList();
                    return;
                }
                if (ifYes == false)
                {
                    if (workCount == influence_Channels.Count)
                    {
                        return;
                    }
                    else
                    {
                        workCount = influence_Channels.Count;
                        if (canAddInfluence_Channels.Contains(influence_Channels[workCount - 1]))
                        {
                            if (influence_Channels[workCount - 1].channel_2 != null)
                            {
                                if (influence_Channels[workCount - 1].channel_2.i1 == influence_Channels[workCount - 1])
                                {
                                    if (influence_Channels[workCount - 1].channel_2.i2.whosecolor != 0)
                                    {
                                        UIManager.showUISingle("你需要优先选择有人的影响力");
                                        influence_Channels.RemoveAt(workCount - 1);
                                        workCount = influence_Channels.Count;
                                        return;
                                    }
                                }
                                if (influence_Channels[workCount - 1].channel_2.i2 == influence_Channels[workCount - 1])
                                {
                                    if (influence_Channels[workCount - 1].channel_2.i1.whosecolor != 0)
                                    {
                                        UIManager.showUISingle("你需要优先选择有人的影响力");
                                        influence_Channels.RemoveAt(workCount - 1);
                                        workCount = influence_Channels.Count;
                                        return;
                                    }
                                }
                            }
                            influence_Channels[workCount - 1].ToggleSelection();
                            canAddInfluence_Channels.Remove(influence_Channels[workCount - 1]);
                            foreach (Block block in Map_Manager.influenceChannelsToBlock(influence_Channels[workCount - 1]))
                            {
                                if (!GetBlocks.Contains(block))
                                {
                                    GetBlocks.Add(block);
                                    foreach (Influence_Channel influence_Channel in Map_Manager.blockToInfluenceChannels(block))
                                    {
                                        if (!canAddInfluence_Channels.Contains(influence_Channel) && !influence_Channels.Contains(influence_Channel))
                                        {
                                            canAddInfluence_Channels.Add(influence_Channel);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            influence_Channels.RemoveAt(workCount - 1);
                            workCount = influence_Channels.Count;
                        }
                    }
                }
                else
                {
                    if (players[whoseColor - 1].money < 2 * influence_Channels.Count)
                    {
                        UIManager.showUISingle("钱不够哦～");
                        ifYes = false;
                        return;
                    }
                    inputMode = 0;
                    addActions(3);
                    UIManager.showUISingle("选择资源点，亏钱别来找我");
                    foreach (Influence_Channel influence_Channel in influence_Channels)
                    {
                        foreach (Block block in Map_Manager.influenceChannelsToBlock(influence_Channel))
                        {
                            if (GetBlocks.Contains(block))
                            {
                                canAddResources.Add(block.rs);
                            }
                        }
                    }
                    next_action();
                }

            }
        } //探索多选航道
        else if (actions[0] == 8)
        {
            //用whosecolor来辨别是谁的角色卡，在player处更改
            //然后等待角色技能的输入
            changeInputMode(51);
            if (skills.Count == 0)
            {
                return;
            }
            else
            {
                if (ifBack == true)
                {
                    ifBack = false;
                    skills.RemoveAt(0);
                    UIManager.showUISingle("请选择发动的人物技能");
                    return;
                }
                if (ifYes == false)
                {
                    UIManager.showUISingle(skills[0] + "被选择");
                    return;
                }
                inputMode = 0;
                SkillLibrary.UseCharacterSkill(skills[0]);
                skills.RemoveAt(0);
                UIManager.showUI("");
                next_action();
            }
        }//使用角色技能(快速行动）
        else if (actions[0] == 9)
        {
            inputMode = 40;
            inputMode = 0;
            next_action();
        }//宣告城市样式(快速行动）选完城市后选择匹配设备xxx
        else if (actions[0] == 10)
        {
            inputMode = 52;
            if (skills.Count == 0)
            {
                return;
            }
            else
            {
                if (ifBack == true)
                {
                    ifBack = false;
                    skills.RemoveAt(0);
                    UIManager.showUISingle("请选择发动的事件技能");
                    return;
                }
                if (ifYes == false)
                {
                    UIManager.showUISingle(skills[0] + "被选择");
                    return;
                }
                UIManager.showUI("");
                Destroy(workEvent.self);
                workEvent = null;
                inputMode = 0;
                SkillLibrary.UseEventSkill(skills[0]);
                skills.RemoveAt(0);
                next_action();
            }
            //等待事件技能输入
        }//发动事件技能
        else if (actions[0] == 11)
        {
            inputMode = 41;
            if (facilities.Count == 0)
            {
                return;
            }
            else
            {
                if(ifBack == true)
                {
                    ifBack = false;
                    facilities.RemoveAt(0);
                    UIManager.showUISingle("请选择要购买的设备");
                    return;
                }
                if(ifYes == false)
                {
                    UIManager.showUISingle(facilities[0].id + "被选择");
                    return;
                }
                addActions(13);
                if(players[whoseColor - 1].returnIfShowSelfFacility() == true)
                {
                    return;
                }
                else
                {
                    players[whoseColor - 1].changeIfShowSelfFacility();
                }
                UIManager.showUISingle("请选择放置的位置");
                inputMode = 0;
                next_action();
            }
        } //输入购买方法(直接放进facilities)，并移除该设备并翻牌（MapManager)，后接13
        else if (actions[0] == 12)
        {
            inputMode = 41;
            inputMode = 0;
            next_action();
        } //发动城市样式技能xxx
        else if (actions[0] == 13)
        {
            inputMode = 62;
            if(facilities.Count == 1)
            {
                return;
            }
            else
            {
                if(ifBack == true)
                {
                    ifBack = false;
                    facilities.RemoveAt(1);
                    UIManager.showUISingle("请选择放置的位置");
                    return;
                }
                if(ifYes == false)
                {
                    UIManager.showUISingle(facilities[1].id + "被选择");
                    return;
                }
                facilities[1].becomeOtherFacility(facilities[0]);
                if(facilities[1].skill1 == "" && facilities[1].skill2 == "")
                {
                    UIManager.showUI("");
                }
                else if (facilities[1].skill1 != "" && facilities[1].skill2 ==""){
                    SkillLibrary.UseFacilitySkill(facilities[1].skill1);
                    UIManager.showUI("");
                }
                else
                {
                    addActions(16);
                    UIManager.showUISingle("请选择设备技能");
                }
                inputMode = 0;
                next_action();
            }
        } //选择设备(放的地方)行动，放下后执行16
        else if (actions[0] == 14)
        {
            inputMode = 63;
            inputMode = 0;
            next_action();
        } //选择匹配设备起始点xxx
        else if (actions[0] == 16)
        {
            inputMode = 53;
            if(skills.Count == 0)
            {
                return;
            }
            else
            {
                if(ifBack == true)
                {
                    ifBack = false;
                    skills.RemoveAt(0);
                    UIManager.showUISingle("请输入发动的设备技能");
                    return;
                }
                if(ifYes == false)
                {
                    UIManager.showUISingle(skills[0] + "被选择");
                    return;
                }
                UIManager.showUI("");
                SkillLibrary.UseFacilitySkill(skills[0]);
                inputMode = 0;
                next_action();
            }
        } //发动设施技能
        else
        {
            canSkip = true;
            ifYes = false;
            ifBack = false;
            whoseColor = whoseSmallTurn;
            inputMode = 0;
            clearList();
            next_action();
        }//用于间隔行动（移除别人影响力，放置自己影响力）
        //所有快速行动都要换isQucikAction，不在这个地方换，在添加第一个行动时就换
        if (actions.Count == 0)
        {
            UIManager.showUI("");
            changeBusy(false);
            photonView.RPC("res", RpcTarget.All);
            if(isQuickAction == true)
            {
                photonView.RPC("UpdateQuickTurn", RpcTarget.All);
                return;
            }
            if(extraTurn != 0)
            {
                extraTurn--;
                photonView.RPC("UpdateQuickTurn", RpcTarget.All);
                return;
            }
            photonView.RPC("UpdateTurn", RpcTarget.All);
        } //如果运行完一个行动后没有下一个行动
    }

    public void startGame()
    {
        if (sceNum != players.Count)
        {
            return;
        }
        if(PhotonNetwork.MasterClient.NickName != PhotonNetwork.NickName)
        {
            return;
        }
        whofirst = Random.Range(0, players.Count);
        UIManager.deleteStart();
        UIManager.deleteReady();
        //在这里添加给予先手资源的代码
        photonView.RPC("UpdateStart", RpcTarget.All, whofirst);
        
    }

    public static void registerPlayer(Player player)
    {
        if (instance == null)
        {
            return;
        }

        if (!instance.players.Contains(player))
        {
            instance.players.Add(player);
        }
    } //初始化玩家

    public static int returnWhoseTurn()
    {
        return instance.whoseTurn;
    } //返回是谁的回合

    public static int returnWhoseColor()
    {
        return instance.whoseColor;
    } //返回快速行动参数

    public static void changeBusy(bool ifb)
    {
        instance.photonView.RPC("UpdateBusy", RpcTarget.All, ifb);
    }//改变忙不忙

    public static bool returnIsBusy()
    {
        return instance.isBusy;
    } //返回是否现在有行动在进行

    public static bool returnIfStart()
    {
        return instance.isStart;
    }

    public static int returnWhoFirst()
    {
        return instance.whofirst;
    }//返回这回合谁先手

    public static void next_player()
    {
        instance.whoseTurn++;
        if(instance.whoseTurn == instance.players.Count + 1)
        {
            instance.whoseTurn = 1;
        }
    } //进入下一个玩家的行动

    public static void next_action()
    {
        instance.actions.RemoveAt(0);
    } //进行下一个行动

    public static void changeInputMode(int mode)
    {
        instance.inputMode = mode;
    } //改变输入模式

    public static int returnInputMode()
    {
        return instance.inputMode;
    } //返回输入模式

    public static bool returnCanSkip()
    {
        return instance.canSkip;
    } //返回可不可以跳过

    public static Player returnPlayer(int index)
    {
        return instance.players[index];
    }

    public static void addActions(int action)
    {
        changeBusy(true);
        changeIfYes(false);
        changeIfBack(false);
        instance.actions.Add(action);
    } //添加行动列表

    public static void addResource(Resource resource)
    {
        instance.resources.Add(resource);
    } //获得资源点输入

    public static void addInfluence(Influence influence)
    {
        instance.influences.Add(influence);
    } //获得影响力输入

    public static void addInfluence_Channel(Influence_Channel influence_Channel)
    {
        instance.influence_Channels.Add(influence_Channel);
    } //获得航道影响力输入

    public static void addMoveCity(MoveCity moveCity)
    {
        instance.moveCities.Add(moveCity);
    } //获得移动城市输入

    public static void addCharacter(Character character)
    {
        instance.characters.Add(character);
    }//获得角色卡输入

    public static void addCityStyle(CityStyle cityStyle)
    {
        instance.cityStyles.Add(cityStyle);
    }//获得城市样式输入

    public static void addFacility(Facility facility)
    {
        instance.facilities.Add(facility);
    }

    public static void addSkill(string skill)
    {
        instance.skills.Add(skill);
    }//获得技能输入

    public static void addCanAddInfluenceChannel(Influence_Channel influence_Channel)
    {
        instance.canAddInfluence_Channels.Add(influence_Channel);
    }

    public static void addCanAddInfluence(Influence influence)
    {
        instance.canAddInfluences.Add(influence);
    }

    public static void addFreeButton(int x)
    {
        instance.freeButtons.Add(x);
    }

    public static void sortPlayers()
    {
        List<Player> work = new List<Player>();
        for(int i = 1; i <= instance.players.Count; i++)
        {
            foreach(Player player in instance.players)
            {
                if(player.num == i)
                {
                    work.Add(player);
                }
            }
        }
        instance.players = work;
    } //按颜色排序进来的玩家

    public static void skip()
    {
        if(instance.turn_count == 0 || instance.turn_count == 3 || instance.turn_count == 4 || instance.main_count == 0)
        {
            return;
        }
        else
        {
            instance.inputMode = 0;
            clearList();
            while (true)
            {
                if(instance.actions.Count == 0)
                {
                    changeBusy(false);
                    if (instance.isQuickAction == true)
                    {
                        instance.photonView.RPC("UpdateQuickTurnS", RpcTarget.All);
                        break;
                    }
                    instance.photonView.RPC("UpdateTurnS", RpcTarget.All);
                    break;
                }
                if(instance.actions[0] == 100)
                {
                    break;
                }
                else
                {
                    instance.actions.RemoveAt(0);
                }
            }
            UIManager.showUI("");
        }
    }//跳过行动

    public static void changeIsQuickAction(bool x)
    {
        instance.isQuickAction = x;
    } //改变是否在一次行动结束后进位

    public static void changeWhoseColor(int x)
    {
        instance.whoseColor = x;
        instance.whoseSmallTurn = x;
    }//改变快速行动的参数

    public static void changeIfYes(bool ify)
    {
        instance.ifYes = ify;
    }

    public static void changeIfBack(bool ifB)
    {
        instance.ifBack = ifB;
    }

    public static void changeCanSkip(bool canSkip)
    {
        instance.canSkip = canSkip;
    }

    public static void clearList()
    {
        instance.resources.Clear();
        instance.influences.Clear();
        instance.influence_Channels.Clear();
        instance.moveCities.Clear();
        instance.cityStyles.Clear();
        instance.characters.Clear();
        instance.skills.Clear();
        instance.freeButtons.Clear();
        instance.canAddMoveCities.Clear();
        instance.canAddInfluences.Clear();
        instance.canAddInfluence_Channels.Clear();
        instance.canAddResources.Clear();
        instance.GetBlocks.Clear();
    }//清空输入列表

    public static void giveReasource(int Money, int Stone, int Yuanshi, int Iron, int BigYuanshi, int Winpoint)
    {
        instance.players[instance.whoseColor - 1].givePlayerResource(Money, Stone, Yuanshi, Iron, BigYuanshi, Winpoint);
    }//中转用

    public static void punGiveResource(int num, int Money, int Stone, int Yuanshi, int Iron, int BigYuanshi, int Winpoint)
    {
        instance.photonView.RPC("UpdateResource", RpcTarget.Others, num, Money, Stone, Yuanshi, Iron, BigYuanshi, Winpoint);
    }

    public static bool canBuyFacility(int Money, int Stone, int Yuanshi, int Iron, int BigYuanshi, int Winpoint)
    {
        if(Money > instance.players[instance.whoseColor - 1].money
        && Stone > instance.players[instance.whoseColor - 1].stone
        && Yuanshi > instance.players[instance.whoseColor - 1].yuanshi
        && Iron > instance.players[instance.whoseColor - 1].iron
        && BigYuanshi > instance.players[instance.whoseColor - 1].bigYuanshi
        && Winpoint > instance.players[instance.whoseColor - 1].winPoint
        )
        {
            return false;
        }

        return true;
    } //判断能不能买得起设备

    public static Player returnAPlayer(int num)
    {
        return instance.players[num - 1];
    }

    public static Player returnWhichPlayerNow()
    {
        return instance.players[instance.whoseColor - 1];
    }

    public static void ready1()
    {
        foreach(Player player in instance.players)
        {
            if(PhotonNetwork.NickName == player.Name)
            {
                return;
            }
            if (player.num == 1)
            {
                return;
            }
        }
        PhotonNetwork.Instantiate("Player 1", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }
    public static void ready2()
    {
        foreach (Player player in instance.players)
        {
            if (PhotonNetwork.NickName == player.Name)
            {
                return;
            }
            if (player.num == 2)
            {
                return;
            }
        }
        PhotonNetwork.Instantiate("Player 2", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }
    public static void ready3()
    {
        foreach (Player player in instance.players)
        {
            if (PhotonNetwork.NickName == player.Name)
            {
                return;
            }
            if (player.num == 3)
            {
                return;
            }
        }
        PhotonNetwork.Instantiate("Player 3", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }
    public static void ready4()
    {
        foreach (Player player in instance.players)
        {
            if (PhotonNetwork.NickName == player.Name)
            {
                return;
            }
            if (player.num == 4)
            {
                return;
            }
        }
        PhotonNetwork.Instantiate("Player 4", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }


    [PunRPC]
    void UpdateStart(int x)
    {
        sortPlayers();
        isStart = true;
        ifYes = false;
        ifBack = false;
        whofirst = x;
        whoseTurn = whofirst + 1;
        whoseColor = whoseTurn;
        whoseSmallTurn = whoseTurn;
        Map_Manager.startBlock();
        main_count = 0;
        for (int i = 0; i <= players.Count; i++)
        {
            getMoney.Add(0);
        }
        if (sceNum == 2)
        {
            FarExplore = 7;
        }
        else if (sceNum == 3)
        {
            FarExplore = 6;
        }
        else if (sceNum == 4)
        {
            FarExplore = 6;
        }
        else
        {
            FarExplore = 5;
        }
    }
    [PunRPC]
    void UpdateBusy(bool isb)
    {
        instance.isBusy = isb;
    }
    [PunRPC]
    void res()
    {
        canSkip = true;
        ifYes = false;
        ifBack = false;
        inputMode = 0;
        clearList();
    }
    [PunRPC]
    void UpdateQuickTurn()
    {
        whoseColor = whoseTurn;
        whoseSmallTurn = whoseTurn;
        isQuickAction = false;
    }
    [PunRPC]
    void UpdateQuickTurnS()
    {
        instance.whoseColor = instance.whoseTurn;
        instance.isQuickAction = false;
    }
    [PunRPC]
    void UpdateTurnS()
    {
        next_player();
        instance.whoseColor = instance.whoseTurn;
        instance.whoseSmallTurn = instance.whoseTurn;
        instance.lun_count++;
        if (instance.lun_count == instance.players.Count)
        {
            instance.turn_count++;
            instance.lun_count = 0;
        }//进位turn
    }
    [PunRPC]
    void UpdateTurn()
    {
        next_player();
        whoseColor = whoseTurn;
        whoseSmallTurn = whoseTurn;
        lun_count++;
        if (lun_count == players.Count && main_count == 0)
        {
            main_count++;
            lun_count = 0;
            Map_Manager.midMap();
            return;
        }//游戏开始
        if (lun_count == players.Count)
        {
            if (turn_count == 3)
            {
                for (int i = 1; i <= players.Count; i++)
                {
                    players[i - 1].givePlayerResource(getMoney[i], 0, 0, 0, 0, 0);
                    getMoney[i] = 0;
                }
                //结算getmoney
            }
            turn_count++;
            lun_count = 0;
        }//进位turn
        if (turn_count == 5)
        {
            main_count++;
            turn_count = 0;
            whofirst++;
            if (whofirst == players.Count)
            {
                whofirst = 0;
            }
            whoseTurn = whofirst + 1;
            whoseColor = whoseTurn;
            whoseSmallTurn = whoseTurn;
        }//进位main
    }
    [PunRPC]
    void UpdateResource(int num, int Money, int Stone, int Yuanshi, int Iron, int BigYuanshi, int Winpoint)
    {
        players[num - 1].money += Money;
        players[num - 1].stone += Stone;
        players[num - 1].yuanshi += Yuanshi;
        players[num - 1].iron += Iron;
        players[num - 1].bigYuanshi += BigYuanshi;
        players[num - 1].winPoint += Winpoint;
    }

    [PunRPC]
    void End()
    {
        foreach(Player player in players)
        {
            player.player_UIManager.upgradeResource(player.stone, player.yuanshi, player.iron, player.bigYuanshi, player.money, player.winPoint);
        }
        isStart = false;
    }
}

