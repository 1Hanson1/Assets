using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject LoginUI;
    public InputField roomName;
    public InputField playerName;
    public InputField Number;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        LoginUI.SetActive(true);
    }

    public void PlayButton()
    {
        if(playerName.text.Length < 1 || roomName.text.Length < 1)
        {
            return;
        }
        if(Number.text != "2" && Number.text != "3" && Number.text != "4")
        {
            return;
        }
        PhotonNetwork.NickName = playerName.text;
        RoomOptions options = new RoomOptions { MaxPlayers = 0 };
        if (Number.text == "2")
        {
            options.MaxPlayers = 2;
        }
        else if (Number.text == "3")
        {
            options.MaxPlayers = 3;
        }
        else if (Number.text == "4")
        {
            options.MaxPlayers = 4;
        }

        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, default);

    }

    public override void OnJoinedRoom()
   {

        if(Number.text == "2")
        {
            PhotonNetwork.LoadLevel(1);
        }
        else if (Number.text == "3")
        {
            PhotonNetwork.LoadLevel(2);
        }
        else if (Number.text == "4")
        {
            PhotonNetwork.LoadLevel(3);
        }
    }
}
