using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class Map_Manager : MonoBehaviourPunCallbacks
{
    //地图管理
    [Header("地图")]
    static Map_Manager instance;
    public List<Block> blocks;
    public List<Channel_1> channels_1;
    public List<Channel_2> channels_2;
    public List<MoveCity> moveCities;
    public List<Resource> resources;
    public List<Influence> influences;
    public List<Influence_Channel> influence_Channels;

    public bool ifShowFacility;
    public bool ifShowCityStyle;

    public List<Facility> facilities;
    public List<Facility> show_facilities;
    public List<Facility> special_facilities;
    public Facility workFacility;

    public List<CityStyle> cityStyles;

    public List<Event> green_events;
    public List<Event> orange_events;
    public List<Event> red_events;
    public Event workEvent;
    //初始可选择的点
    [Header("初始点")]
    public List<Block> startblocks;
    //五六回合解封的点
    [Header("封闭区")]
    public List<Block> farBlocks;
    public List<Channel_1> farChannels_1;
    public List<Channel_2> farChannels_2;
    //计算分数
    [Header("计算器")]
    public List<int> recode;
    public List<int> cal;
    public List<int> maxindex;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        for(int i = 0; i < 4; i++)
        {
            cal.Add(0);
            recode.Add(0);
        }
    }

    public static void registerBlock(Block block) // 初始化地图区块
    {
        if (instance == null)
        {
            return;
        }

        if (!instance.blocks.Contains(block))
        {
            instance.blocks.Add(block);
        }
    }

    public static void registerResource(Resource resource) // 初始化地图资源点
    {
        if (instance == null)
        {
            return;
        }

        if (!instance.resources.Contains(resource))
        {
            instance.resources.Add(resource);
        }
    }

    public static void registerMoveCity(MoveCity moveCity) // 初始化地图移动城市点
    {
        if (instance == null)
        {
            return;
        }

        if (!instance.moveCities.Contains(moveCity))
        {
            instance.moveCities.Add(moveCity);
        }
    }

    public static void registerInfluence(Influence influence) // 初始化影响力
    {
        if (instance == null)
        {
            return;
        }

        if (!instance.influences.Contains(influence))
        {
            instance.influences.Add(influence);
        }
    }

    public static void registerInfluence_Channel(Influence_Channel influence_Channel) // 初始化航道影响力
    {
        if (instance == null)
        {
            return;
        }

        if (!instance.influence_Channels.Contains(influence_Channel))
        {
            instance.influence_Channels.Add(influence_Channel);
        }
    }

    public static void registerChannel1(Channel_1 channel_1) // 初始化单个影响力的航道
    {
        if (instance == null)
        {
            return;
        }

        if (!instance.channels_1.Contains(channel_1))
        {
            instance.channels_1.Add(channel_1);
        }

    }

    public static void registerChannel2(Channel_2 channel_2) // 初始化两个个影响力的航道
    {
        if (instance == null)
        {
            return;
        }

        if (!instance.channels_2.Contains(channel_2))
        {
            instance.channels_2.Add(channel_2);
        }

    }

    public static void registerFacility(Facility facility) // 初始化设备
    {
        if (instance == null)
        {
            return;
        }

        instance.facilities.Add(facility);
    }

    public static void registerEvent(Event events) // 初始化事件
    {
        if (instance == null)
        {
            return;
        }
        if(events.whichDiff == 1)
        {
            instance.green_events.Add(events);

        }
        else if (events.whichDiff == 2)
        {
            instance.orange_events.Add(events);
        }
        else if (events.whichDiff == 3)
        {
            instance.red_events.Add(events);
        }


    }

    public static void registerCityStyle(CityStyle cityStyle)
    {
        if (instance == null)
        {
            return;
        }

        instance.cityStyles.Add(cityStyle);
    }

    public static void startBlock() //开始选择初始点
    {
        foreach(Block block in instance.startblocks)
        {
            block.mc.ifallowexplore = true;
        }
    }

    public static void midMap()//游戏开始
    {
        foreach(Block block in instance.blocks)
        {
            if (instance.farBlocks.Contains(block))
            {
                
            }
            else
            {
                block.mc.ifallowexplore = true;
                block.i1.ifallowexplore = true;
                block.i2.ifallowexplore = true;
                block.rs.ifallowexplore = true;
            }
        }
        foreach(Channel_1 channel_1 in instance.channels_1)
        {
            if (instance.farChannels_1.Contains(channel_1))
            {
                
            }
            else
            {
                channel_1.i1.ifallowexplore = true;
                channel_1.ifallowexplore = true;
            }
        }
        foreach (Channel_2 channel_2 in instance.channels_2)
        {
            if (instance.farChannels_2.Contains(channel_2))
            {

            }
            else
            {
                channel_2.i1.ifallowexplore = true;
                channel_2.i2.ifallowexplore = true;
                channel_2.ifallowexplore = true;
            }
        }
    }

    public static void canExplore()//后期地块解禁
    {
        foreach (Block block in instance.farBlocks)
        {
            block.mc.ifallowexplore = true;
            block.i1.ifallowexplore = true;
            block.i2.ifallowexplore = true;
            block.rs.ifallowexplore = true;
        }
        foreach (Channel_1 channel_1 in instance.farChannels_1)
        {
            channel_1.i1.ifallowexplore = true;
            channel_1.ifallowexplore = true;
        }
        foreach (Channel_2 channel_2 in instance.farChannels_2)
        {
            channel_2.i1.ifallowexplore = true;
            channel_2.i2.ifallowexplore = true;
            channel_2.ifallowexplore = true;
        }
    }

    public static Event giveEvents(int x)
    {
        int i;
        
        if(x == 1)
        {
            i = Random.Range(0, instance.green_events.Count - 1);
            instance.workEvent = instance.green_events[i];
            instance.photonView.RPC("deleteWorkEvent", RpcTarget.All, instance.workEvent.id, instance.workEvent.whichDiff);
        }
        else if(x == 2)
        {
            i = Random.Range(0, instance.orange_events.Count - 1);
            instance.workEvent = instance.orange_events[i];
            instance.photonView.RPC("deleteWorkEvent", RpcTarget.All, instance.workEvent.id, instance.workEvent.whichDiff);
        }
        else if(x == 3)
        {
            i = Random.Range(0, instance.red_events.Count - 1);
            instance.workEvent = instance.red_events[i];
            instance.photonView.RPC("deleteWorkEvent", RpcTarget.All, instance.workEvent.id, instance.workEvent.whichDiff);
        }
        else
        {
            i = 0;
            UIManager.showUI("出错了，估计是跑不下去了");
            instance.workEvent = null;
        }
        Player player = GameManager.returnPlayer(GameManager.returnWhoseColor() - 1);
        instance.workEvent.self.SetActive(true);
        return instance.workEvent;
    } //x为颜色

    [PunRPC]
    void deleteWorkEvent(string id, int diff)
    {
        if(diff == 1)
        {
            for(int i = 0; i < green_events.Count; i++)
            {
                if(green_events[i].id == id)
                {
                    green_events.RemoveAt(i);
                    break;
                }
            }
        }
        else if (diff == 2)
        {
            for (int i = 0; i < orange_events.Count; i++)
            {
                if (orange_events[i].id == id)
                {
                    orange_events.RemoveAt(i);
                    break;
                }
            }
        }
        else if (diff == 3)
        {
            for (int i = 0; i < red_events.Count; i++)
            {
                if (red_events[i].id == id)
                {
                    red_events.RemoveAt(i);
                    break;
                }
            }
        }
        return;
    }

    public static void startFacility()
    {
        foreach(Facility facility in instance.show_facilities)
        {
            giveFacility(facility);
        }
    }

    public static void giveFacility(Facility facility)
    {
        int i;
        i = Random.Range(0, instance.facilities.Count - 1);
        instance.workFacility = instance.facilities[i];
        Debug.Log("随机到"+instance.facilities[i].id);
        Debug.Log("选择格子" + facility.id);
        instance.photonView.RPC("updateShowFacility", RpcTarget.All, facility.id, instance.workFacility.id);
        instance.photonView.RPC("deleteWorkFacility", RpcTarget.All, instance.workFacility.id);
    }//从设备堆抽取一个设备

    [PunRPC]
    void deleteWorkFacility(string id)
    {
        for(int i = 0; i < facilities.Count; i++)
        {
            if(facilities[i].id == id)
            {
                facilities.RemoveAt(i);
                //workFacility = null;
                return;
            }
        }
        return;
    }

    [PunRPC]
    void updateShowFacility(string showFacilityId , string becomeFacilityID)
    {
        for(int i = 0; i < show_facilities.Count; i++)
        {
            if(showFacilityId == show_facilities[i].id)
            {
                Debug.Log("匹配格子到" + show_facilities[i].id);
                for(int j = 0; j < facilities.Count; j++)
                {
                    if(facilities[j].id == becomeFacilityID)
                    {
                        Debug.Log("匹配列表" + facilities[j].id);
                        workFacility = facilities[j];
                        show_facilities[i].becomeOtherFacility(facilities[j]);
                        return;
                    }
                }
            }
        }
        return;
    }//从设备堆到展示堆

    public static List<Influence_Channel> blockToInfluenceChannels(Block block)
    {
        List<Influence_Channel> worklst = new List<Influence_Channel>();
        foreach(Channel_1 channel_1 in instance.channels_1)
        {
            foreach(Block block1 in channel_1.Connect_Blocks)
            {
                if(block1 == block)
                {
                    if (!worklst.Contains(channel_1.i1))
                    {
                        worklst.Add(channel_1.i1);
                    }
                }
            }
        }
        foreach (Channel_2 channel_2 in instance.channels_2)
        {
            foreach (Block block1 in channel_2.Connect_Blocks)
            {
                if (block1 == block)
                {
                    if (!worklst.Contains(channel_2.i1))
                    {
                        worklst.Add(channel_2.i1);
                    }
                    if (!worklst.Contains(channel_2.i2))
                    {
                        worklst.Add(channel_2.i2);
                    }
                }
            }
        }
        return worklst;
    }

    public static List<Block> influenceChannelsToBlock(Influence_Channel influence_Channel)
    {
        List<Block> worklst = new List<Block>();
        if(influence_Channel.channel_1 == null)
        {
            foreach(Block block in influence_Channel.channel_2.Connect_Blocks)
            {
                if (!worklst.Contains(block))
                {
                    worklst.Add(block);
                }
            }
            
        }
        else
        {
            foreach (Block block in influence_Channel.channel_1.Connect_Blocks)
            {
                if (!worklst.Contains(block))
                {
                    worklst.Add(block);
                }
            }
        }
        return worklst;
    }

    public static List<int> calWinPoint()
    {
        int maxsc;
        foreach(Block block in instance.blocks)
        {
            if(block.i1.whosecolor != 0)
            {
                instance.cal[block.i1.whosecolor - 1]++;
            }
            if (block.i2.whosecolor != 0)
            {
                instance.cal[block.i2.whosecolor - 1]++;
            }
            if(block.mc.whoseCity != 0)
            {
                instance.cal[block.mc.whoseCity - 1]+=2;
            }
            maxsc = instance.cal.Max();
            if (maxsc == 0) { }
            else
            {
                for (int i = 0; i < instance.cal.Count; i++)
                {
                    if (instance.cal[i] == maxsc)
                    {
                        instance.maxindex.Add(i);
                    }
                    instance.cal[i] = 0;
                }
                foreach (int x in instance.maxindex)
                {
                    instance.recode[x] += block.winPoint;
                }
                instance.maxindex.Clear();
            }
            
        }
        foreach(Channel_1 channel_1 in instance.channels_1)
        {
            if(channel_1.i1.whosecolor != 0)
            {
                instance.cal[channel_1.i1.whosecolor - 1]++;
            }
            maxsc = instance.cal.Max();
            if (maxsc == 0) { }
            else
            {
                for (int i = 0; i < instance.cal.Count; i++)
                {
                    if (instance.cal[i] == maxsc)
                    {
                        instance.maxindex.Add(i);
                    }
                    instance.cal[i] = 0;
                }
                foreach (int x in instance.maxindex)
                {
                    instance.recode[x] += channel_1.winPoint;
                }
                instance.maxindex.Clear();
            }
        }
        foreach (Channel_2 channel_2 in instance.channels_2)
        {
            if (channel_2.i1.whosecolor != 0)
            {
                instance.cal[channel_2.i1.whosecolor - 1]++;
            }
            if (channel_2.i2.whosecolor != 0)
            {
                instance.cal[channel_2.i2.whosecolor - 1]++;
            }
            maxsc = instance.cal.Max();
            if(maxsc == 0) { }
            else
            {
                for (int i = 0; i < instance.cal.Count; i++)
                {
                    if (instance.cal[i] == maxsc)
                    {
                        instance.maxindex.Add(i);
                    }
                    instance.cal[i] = 0;
                }
                foreach (int x in instance.maxindex)
                {
                    instance.recode[x] += channel_2.winPoint;
                }
                instance.maxindex.Clear();
            }
        }
        return instance.recode;
    }

    public static void changeIfShowFacilityM(int num)
    {
        instance.ifShowFacility = !instance.ifShowFacility;
        if(instance.ifShowFacility == true)
        {
            if(GameManager.returnAPlayer(num).returnIfShowSelfFacility() == true)
            {
                GameManager.returnAPlayer(num).changeIfShowSelfFacility();
            }
            if(instance.ifShowCityStyle == true)
            {
                changeIfShowCityStyleM(num);
            }
        }
        foreach(Facility facility in instance.show_facilities)
        {
            facility.selfF.SetActive(instance.ifShowFacility);
        }
    } // 切换购买设备面板

    public static bool returnIfShowFacilityM()
    {
        return instance.ifShowFacility;
    }

    public static void changeIfShowCityStyleM(int num)
    {
        instance.ifShowCityStyle = !instance.ifShowCityStyle;
        if (instance.ifShowCityStyle == true)
        {
            if (GameManager.returnAPlayer(num).returnIfShowSelfFacility() == true)
            {
                GameManager.returnAPlayer(num).changeIfShowSelfFacility();
            }
            if (instance.ifShowFacility == true)
            {
                changeIfShowFacilityM(num);
            }
        }
        foreach (CityStyle cityStyle in instance.cityStyles)
        {
            cityStyle.selfF.SetActive(instance.ifShowCityStyle);
        }
    } //切换城市样式面板

    public static bool returnIfShowCityStyleM()
    {
        return instance.ifShowCityStyle;
    }
}
