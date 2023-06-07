using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ToggleSynchronizer : MonoBehaviour
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(SynchronizeToggleState);
    }

    public void SynchronizeToggleState(bool val)
    {
        Hashtable hashtable = new Hashtable();
        hashtable.Add("Name", toggle.name);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeToggleState, hashtable, raiseEventOptions, SendOptions.SendReliable);

        hashtable.Add("Value", val);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeToggleState, hashtable, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        Hashtable data;

        switch (eventCode)
        {
            case EventCodes.SynchronizeToggleState:
                data = (Hashtable)photonEvent.CustomData;

                // return if other toggle was meant.
                if ((string)data["Name"] != toggle.name) return;

                Debug.Log("SynchronizeToggleState event received");
                bool value = (bool)data["Value"];

                toggle.SetIsOnWithoutNotify(value);
                                
                break;
        }
    }
}
