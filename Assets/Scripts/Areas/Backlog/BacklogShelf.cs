using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BacklogShelf : BaseShelf
{
    public override byte SetupEventCode => EventCodes.SetupBacklogShelf;
    public override byte SynchronizePageEventCode => EventCodes.SynchronizeBacklogPage;


    public override void Setup(int boardId)
    {
        JiraController.Instance.GetBoardBacklog(boardId, err => ApiFeedback.Instance.DisplayFeedback(err), InstantiateIssueCards);
    }



    /// <summary>
    /// Instantiate issue cards and raise event on all clients to setup their backlog shelf.
    /// </summary>
    /// <param name="issues">List of jira issues</param>
    public void InstantiateIssueCards(List<JiraIssue> issues)
    {
        List<int> issueCardViewIds = new List<int>();

        foreach (JiraIssue jiraIssue in issues)
        {
            // Set custom instantiation data and instantiate the issue card with photon.
            object[] customInstantiationData = GetObjectArrayOfJiraIssue(jiraIssue);
            GameObject issueCard = PhotonNetwork.Instantiate(PrefabPaths.ISSUE_CARD_PREFAB_PATH, this.transform.position, this.transform.rotation, 0, customInstantiationData);

            issueCard.SetActive(false);

            // Get all view ids of issue cards.
            issueCardViewIds.Add(issueCard.GetPhotonView().ViewID);
        }

        // Remove from cache
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(SetupEventCode, new Hashtable(), raiseEventOptions, SendOptions.SendReliable);

        // Raise event to other clients and tell them to setup their shelves.
        Hashtable content = new Hashtable();
        content.Add("ViewIdsArray", issueCardViewIds.ToArray());
        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(SetupEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }



    public override void AddCard(Collider c, int triggerIndex)
    {
        if (IsCardInTheCardArray(c)) return;

        InsertCardIntoArray(c, triggerIndex);

        if (jiraCallEnabled == false) return;

        IssueCard issueCard = c.GetComponent<IssueCard>();

        // Only make the owner of the card send request to jira.
        if (issueCard.GetComponent<PhotonView>().IsMine)
        {
            string issueKey = issueCard.key;
            JiraController.Instance.MoveIssuesToBacklog(new List<string> { issueKey }, err => ApiFeedback.Instance.DisplayFeedback(err), result => {
                Debug.Log("Issue moved to backlog");
            });
        }
    }



}
