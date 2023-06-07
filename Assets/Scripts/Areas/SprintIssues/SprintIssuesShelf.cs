using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SprintIssuesShelf : BaseShelf
{
    public int currentSprintId;
    public override byte SetupEventCode => EventCodes.SetupSprintIssuesShelf;
    public override byte SynchronizePageEventCode => EventCodes.SynchronizeSprintIssuesPage;




    public override void Setup(int sprintId)
    {
        SetSprintId(sprintId);
        LoadSprintIssues(sprintId);
    }

    public void LoadSprintIssues(int sprintId)
    {
        JiraController.Instance.GetSprintIssues(sprintId, "", err => ApiFeedback.Instance.DisplayFeedback(err), CreateIssueCards);
    }

    public void SetSprintId(int id)
    {
        currentSprintId = id;
    }



    public void CreateIssueCards(List<JiraIssue> issues)
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

        // Raise event to other clients and tell them to setup their shelves.
        Hashtable content = new Hashtable();

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(SetupEventCode, content, raiseEventOptions, SendOptions.SendReliable);

        content.Add("ViewIdsArray", issueCardViewIds.ToArray());
        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(SetupEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }


    public override void AddCard(Collider c, int triggerIndex)
    {
        if (IsCardInTheCardArray(c)) return;

        InsertCardIntoArray(c, triggerIndex);

        if (!jiraCallEnabled) return;
        
        IssueCard issueCard = c.GetComponent<IssueCard>();

        // Only make the owner of the card send request to jira.
        if (issueCard.GetComponent<PhotonView>().IsMine)
        {
            string issueKey = issueCard.key;
            JiraController.Instance.MoveIssuesToSprint(new List<string> { issueKey }, currentSprintId, err => ApiFeedback.Instance.DisplayFeedback(err), result => {
                Debug.Log("Issue moved to sprint");
            });
        }
    }
}
