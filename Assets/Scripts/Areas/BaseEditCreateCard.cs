using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class BaseEditCreateCard : MonoBehaviour
{

    protected void Awake()
    {
        Close();
    }

    /// <summary>
    /// Sets the local scale of the card to 0 and clears the fields.
    /// </summary>
    public void Close()
    {
        transform.localScale = new Vector3(0, 0, 0);
        Clear();
    }


    /// <summary>
    /// Sets the local scale of the card to 1.
    /// </summary>
    public void Open()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }



    public abstract void Clear();


    /// <summary>
    /// When pressing 'Save changes' button on the BaseEditCreateCard.
    /// </summary>
    public abstract void OnSaveClick();


    /// <summary>
    /// When pressing 'Cancel' button on the BaseEditCreateCard.
    /// </summary>
    public void OnCancelClick()
    {
        SynchronizeClose();
    }



    #region Photon

    public virtual void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public virtual void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.SynchronizeBaseEditCreateCardOpen)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;
            string name= (string)data["Name"];
            if (name != gameObject.name) return;
            Open();
        }
        else if (eventCode == EventCodes.SynchronizeBaseEditCreateCardClose)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;
            string name = (string)data["Name"];
            if (name != gameObject.name) return;
            Close();
        }
    }

    public void SynchronizeOpen()
    {
        Hashtable content = new Hashtable();
        content.Add("Name", gameObject.name);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeBaseEditCreateCardOpen, content, raiseEventOptions, SendOptions.SendReliable);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeBaseEditCreateCardOpen, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SynchronizeClose()
    {
        Hashtable content = new Hashtable();
        content.Add("Name", gameObject.name);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeBaseEditCreateCardClose, content, raiseEventOptions, SendOptions.SendReliable);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeBaseEditCreateCardClose, content, raiseEventOptions, SendOptions.SendReliable);
    }


    #endregion
}
