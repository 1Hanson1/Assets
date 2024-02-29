using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    public Text inf;
    public Text which;
    public Text who;
    public GameObject startButton;
    public GameObject ready;
    
    static UIManager instance;

    private void Start()
    {
        instance = this;
    }

    public static void showUI(string word)
    {
        instance.photonView.RPC("UpdateShowUI", RpcTarget.All, word);
    }

    public static void showUISingle(string word)
    {
        instance.inf.text = word;
    }

    public static void upgradeWhich(string word)
    {
        instance.which.text = word;
    }

    public static void upgradeWho(string word)
    {
        instance.who.text = word;
    }

    public static void deleteStart()
    {
        PhotonNetwork.Destroy(instance.startButton);
    }

    public static void deleteReady()
    {
        PhotonNetwork.Destroy(instance.ready);
    }

    [PunRPC]
    void UpdateShowUI(string word)
    {
        showUISingle(word);
    }
}
