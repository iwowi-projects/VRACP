using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SprintsShelf : BaseShelf
{
    public override byte SetupEventCode => EventCodes.SetupSprintShelf;
    public override byte SynchronizePageEventCode => EventCodes.SynchronizeSprintPage;



    public override void Setup(int boardId)
    {
        JiraController.Instance.GetBoardSprints(boardId, err => ApiFeedback.Instance.DisplayFeedback(err), CreateSprintCards);
    }


    /// <summary>
    /// Instantiate sprint cards and raise event to all clients to setup the sprint shelf.
    /// </summary>
    /// <param name="sprints">List of jira sprints</param>
    public void CreateSprintCards(List<JiraSprint> sprints)
    {
        List<int> sprintCardViewIds = new List<int>();


        foreach (JiraSprint jiraSprint in sprints)
        {
            // Set custom instantiation data and instantiate the sprint card with photon.
            object[] customInstantiationData = { jiraSprint.id, jiraSprint.name, jiraSprint.state, jiraSprint.startDate, jiraSprint.endDate };
            GameObject sprintCard = PhotonNetwork.Instantiate(PrefabPaths.SPRINT_CARD_PREFAB_PATH, this.transform.position, this.transform.rotation, 0, customInstantiationData);

            sprintCard.SetActive(false);

            // Get all view ids of sprint cards.
            sprintCardViewIds.Add(sprintCard.GetPhotonView().ViewID);
        }

        // Raise event to other clients and tell them to setup their shelves.
        Hashtable content = new Hashtable();

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(SetupEventCode, content, raiseEventOptions, SendOptions.SendReliable);

        content.Add("ViewIdsArray", sprintCardViewIds.ToArray());
        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(SetupEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }


    public override void AddCard(Collider c, int triggerIndex)
    {
        InsertCardIntoArray(c, triggerIndex);
    }

}
