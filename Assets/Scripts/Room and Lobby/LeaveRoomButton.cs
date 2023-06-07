using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveRoomButton : MonoBehaviourPunCallbacks
{
    public void LeaveRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties["BoardIntializer"] == PhotonNetwork.LocalPlayer)
        {
            BoardConfiguration.Instance.RemoteClearAll();
        }

        PhotonNetwork.LeaveRoom();
    }
}
