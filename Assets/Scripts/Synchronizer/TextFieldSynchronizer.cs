using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TextFieldSynchronizer : MonoBehaviour
{
    private TMP_Text textField;

    private void Start()
    {
        textField = GetComponent<TMP_Text>();
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
        if (eventCode == EventCodes.SynchronizeTextFieldText)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;
            
            if (textField.name != (string)data["Name"]) return;
            
            textField.text = ((string)data["Text"]);
        }
    }

    public void SynchronizeTextEvent()
    {
        SynchronizeTextEvent(textField.text);
    }

    public void SynchronizeTextEvent(string text)
    {
        Hashtable content = new Hashtable();
        content.Add("Name", textField.name);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeTextFieldText, content, raiseEventOptions, SendOptions.SendReliable);

        content.Add("Text", text);
        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeTextFieldText, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
