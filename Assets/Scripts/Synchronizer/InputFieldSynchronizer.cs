using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InputFieldSynchronizer : MonoBehaviour
{
    private TMP_InputField inputField;


    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(SynchronizeInputTextEvent);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    /*
     * Gets called when a textfield was changed.
     */
    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.SynchronizeInputFieldText)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;

            // only synchronize if it is meant to be
            if (inputField.name != (string)data["Name"]) return;

            Debug.Log("SynchronizeInputFieldEvent raised");
            inputField.SetTextWithoutNotify((string)data["Text"]);
            
        }
    }

    public void SynchronizeInputTextEvent(string newText)
    {
        Hashtable hashtable = new Hashtable();
        hashtable.Add("Name", inputField.name);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeInputFieldText, hashtable, raiseEventOptions, SendOptions.SendReliable);

        hashtable.Add("Text", newText);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeInputFieldText, hashtable, raiseEventOptions, SendOptions.SendReliable);
    }

}
