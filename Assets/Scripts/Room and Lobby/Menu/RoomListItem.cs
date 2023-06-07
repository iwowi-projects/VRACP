using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public string roomName;
    private RoomInfo _info;

    public void SetUp(RoomInfo info)
    {
        _info = info;
        roomName = _info.Name;
        GetComponentInChildren<Text>().text = roomName;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(_info);
    }

}
