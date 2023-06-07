using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSynchronizer : MonoBehaviour
{
    public ScrollRect scrollRect;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    /*
     * Gets called when a scroll rect position was changed.
     * PROBLEM: OnValueChanged always called as there is no method without notifying.
     */
    private void OnEvent(EventData photonEvent)
    {
        Debug.Log("SynchronizeScrollRectEvent raised");
        byte eventCode = photonEvent.Code;
        /*if (eventCode == EventCodes.SynchronizeScrollRectEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector2 pos = (Vector2)data[0];
            scrollRect.normalizedPosition = pos;
        }*/
    }

    public void SynchronizeValueEvent(Vector2 pos)
    {
        object[] content = new object[] { pos };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        //PhotonNetwork.RaiseEvent(EventCodes.SynchronizeScrollRectEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
