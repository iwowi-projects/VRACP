using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class DropdownSynchronizer : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(SynchronizeDropdownValueEvent);
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
            case EventCodes.SynchronizeDropdownValue:
                data = (Hashtable)photonEvent.CustomData;
                // return if other dropdown was meant.
                if ((string)data["Name"] != dropdown.name) return;

                int value = (int)data["Value"];
                SynchronizeDropdownValueLocal(value);
                break;
            

            case EventCodes.SynchronizeDropdownOptions:
                data = (Hashtable)photonEvent.CustomData;
                // return if other dropdown was meant.
                if ((string)data["Name"] != dropdown.name) return;

                string[] options = (string[])data["Options"];
                SynchronizeDropdownOptionsLocal(options);
                break;
        }
    }

    public void SynchronizeDropdownValueEvent(int value)
    {
        Hashtable hashtable = new Hashtable();
        hashtable.Add("Name", dropdown.name);


        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeDropdownValue, hashtable, raiseEventOptions, SendOptions.SendReliable);

        hashtable.Add("Value", value);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeDropdownValue, hashtable, raiseEventOptions, SendOptions.SendReliable);
    }

    private void SynchronizeDropdownValueLocal(int value)
    {
        dropdown.SetValueWithoutNotify(value);
    }

    public void SynchronizeDropdownOptionsEvent()
    {
        List<string> options = new List<string>(); 
        dropdown.options.ForEach(action =>
        {
            options.Add(action.text);
        });

        Hashtable hashtable = new Hashtable();
        hashtable.Add("Name", dropdown.name);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeDropdownOptions, hashtable, raiseEventOptions, SendOptions.SendReliable);

        hashtable.Add("Options", options.ToArray());

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeDropdownOptions, hashtable, raiseEventOptions, SendOptions.SendReliable);
    }

    private void SynchronizeDropdownOptionsLocal(string[] options)
    {
        List<string> optionsList = new List<string>(options);
        dropdown.ClearOptions();
        dropdown.AddOptions(optionsList);
    }


}
