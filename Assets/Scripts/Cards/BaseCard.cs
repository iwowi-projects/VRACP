using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// The base card for sprint and issue cards. Relevant for synchronizing when the card is being hold by a user.
/// </summary>
public abstract class BaseCard : MonoBehaviour
{
    public bool beingHold;

    public void SynchronizeBeingHold(bool beingHold)
    {
        // Remove from cache
        Hashtable content = new Hashtable();
        content.Add("ViewId", gameObject.GetComponent<PhotonView>().ViewID);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SyncIssueCardHolding, content, raiseEventOptions, SendOptions.SendReliable);

        content = new Hashtable();
        content.Add("ViewId", gameObject.GetComponent<PhotonView>().ViewID);
        content.Add("BeingHold", beingHold);
        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SyncIssueCardHolding, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void BaseCardOnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.SyncIssueCardHolding)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;

            // other card was meant.
            if (gameObject.GetComponent<PhotonView>().ViewID != (int)data["ViewId"]) return;

            beingHold = (bool)data["BeingHold"];
        }
    }

    public virtual void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += BaseCardOnEvent;
    }

    public virtual void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= BaseCardOnEvent;
    }
}
