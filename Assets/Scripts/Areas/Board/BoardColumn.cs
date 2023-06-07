using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BoardColumn : BaseShelf, IPunInstantiateMagicCallback
{
    public string columnName;
    public TMP_Text columnNameTextfield;

    public List<string> statusIds;
    public int boardId;

    public override byte SetupEventCode => throw new NotImplementedException();
    public override byte SynchronizePageEventCode => throw new NotImplementedException();



    public override void Awake()
    {
        base.Awake();
        visible = new Vector3(1.5f, 1.5f, 1.5f);
    }

    public override void Setup(int id)
    {

    }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        this.transform.localScale = new Vector3(1, 1, 1);

        string name = (string) data[0];
        string[] statuses = (string[]) data[1];
        int boardId = (int)data[2];

        SetupAfterInstantiation(name, new List<string>(statuses), boardId);
    }



    /// <summary>
    /// Initial setup after column was instantiated via Photon. Sets the name and status ids.
    /// </summary>
    /// <param name="columnName">Name of the column to be displayed on the columnNameTextfield</param>
    /// <param name="statuses">List of status ids of the column</param>
    public void SetupAfterInstantiation(string columnName, List<string> statuses, int boardId)
    {
        this.columnName = columnName;
        columnNameTextfield.text = this.columnName;
        this.statusIds = statuses;
        this.boardId = boardId;
        jiraCallEnabled = false;

        // Only make the owner of the column call the api
        if (!GetComponent<PhotonView>().IsMine) return;

        string statusIdsAsList = string.Join(",", statusIds.Select(t => t));
        string requestParams = string.Format("?jql=status[id] in ({0})", statusIdsAsList);

        JiraController.Instance.GetBoardIssues(boardId, requestParams, err => ApiFeedback.Instance.DisplayFeedback(err), InstantiateIssueCards);
    }




    /// <summary>
    /// Instantiate board issue cards based on the issues list and call InstantiateColumns after fetching the board configuration.
    /// </summary>
    /// <param name="issues">List of issue cards</param>
    public void InstantiateIssueCards(List<JiraIssue> issues)
    {
        List<int> issueCardViewIds = new List<int>();

        foreach (JiraIssue jiraIssue in issues)
        {
            // Set custom instantiation data and instantiate the issue card with photon.
            object[] customInstantiationData = { jiraIssue.id, jiraIssue.key, jiraIssue.fields.summary, jiraIssue.fields.description, jiraIssue.fields.creator.displayName,
                jiraIssue.fields.issuetype.id, jiraIssue.fields.status.id, jiraIssue.fields.priority.id, jiraIssue.fields.issuetype.name, jiraIssue.fields.status.name, jiraIssue.fields.priority.name };
            GameObject issueCard = PhotonNetwork.Instantiate(PrefabPaths.BOARD_ISSUE_CARD_PREFAB_PATH, this.transform.position, this.transform.rotation, 0, customInstantiationData);

            issueCard.SetActive(false);

            // Get all view ids of issue cards.
            issueCardViewIds.Add(issueCard.GetPhotonView().ViewID);
        }

        Hashtable content = new Hashtable();
        content.Add("ViewId", GetComponent<PhotonView>().ViewID);
        content.Add("IssueCardViewIds", issueCardViewIds.ToArray());

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SetupBoardColumn, new Hashtable(), raiseEventOptions, SendOptions.SendReliable);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SetupBoardColumn, content, raiseEventOptions, SendOptions.SendReliable);
    }




    public override void AddCard(Collider issueCardCollider, int triggerIndex)
    {
        if (IsCardInTheCardArray(issueCardCollider)) return;

        InsertCardIntoArray(issueCardCollider, triggerIndex);

        if (jiraCallEnabled == false) return;

        IssueCard issueCard = issueCardCollider.GetComponent<IssueCard>();

        // Only make the owner of the card send request to jira.
        if (issueCard.GetComponent<PhotonView>().IsMine)
        {
            string issueId = issueCard.id;

            // check if issue got to be moved into board or into sprint
            //bool hasSprints = (bool) PhotonNetwork.CurrentRoom.CustomProperties["CurrentBoardHasSprints"]; 
            //bool hasBacklog = (bool)PhotonNetwork.CurrentRoom.CustomProperties["CurrentBoardHasBacklog"];
            JiraBoardFeatures jiraBoardFeatures = JsonConvert.DeserializeObject<JiraBoardFeatures>((string)PhotonNetwork.CurrentRoom.CustomProperties["BoardFeatures"]);

            if (jiraBoardFeatures.HasSprints())
            {
                if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("CurrentSprintId"))
                {
                    ApiFeedback.Instance.DisplayFeedback("There is no active sprint yet.");
                    return;
                } else
                {
                    int currentSprintId = (int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentSprintId"];
                    JiraController.Instance.MoveIssuesToSprint(new List<string> { issueId }, currentSprintId, err => ApiFeedback.Instance.DisplayFeedback(err), result =>
                    {
                        MakeIssueTransition(issueId, issueCard);
                    });
                }

                return;
            }

            if (jiraBoardFeatures.HasBacklog())
            {
                int currentBoardId = (int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentBoardId"];
                JiraController.Instance.MoveIssuesToBoard(new List<string> { issueId }, boardId, err => ApiFeedback.Instance.DisplayFeedback(err), result =>
                {
                    MakeIssueTransition(issueId, issueCard);
                });

                return;
            }

            MakeIssueTransition(issueId, issueCard);

        }
    }

    public void MakeIssueTransition(string issueId, IssueCard issueCard)
    {
        JiraController.Instance.GetIssueTransitions(issueId, err => ApiFeedback.Instance.DisplayFeedback(err), transitions => {

            // Iterate through each transition, take 'to' field and compare it with statuses in status list.
            string transitionId = "";
            foreach (JiraIssueTransition transition in transitions)
            {
                if (statusIds.Exists(s => s == transition.to.id))
                {
                    transitionId = transition.id;
                    break;
                }
            }

            // Make transition and synchronize card data.
            if (transitionId != "")
            {
                JiraController.Instance.DoIssueTransitation(issueId, transitionId, err => ApiFeedback.Instance.DisplayFeedback(err), result =>
                {
                    Debug.Log("Issue transition successful");
                    JiraController.Instance.GetIssue(issueId, err => ApiFeedback.Instance.DisplayFeedback(err), issueCard.SynchronizeIssueCard);
                });
            }

        });
    }

    public override void RemoveCard(Collider c, int triggerIndex)
    {
        // when card is already placed and another card enters and exits, it should not remove it.
        try
        {
            if (cardsArray[currentPage][triggerIndex].gameObject != c.gameObject) return;
        } 
        catch(Exception e) { }
        

        cardsArray[currentPage][triggerIndex].transform.localScale = new Vector3(1, 1, 1);
        cardsArray[currentPage][triggerIndex] = null;
    }



    #region Photon

    public override void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.SynchronizeBoardColumnPage)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;

            // other column was meant.
            if (GetComponent<PhotonView>().ViewID != (int)data["ViewId"]) return;

            int page = (int)data["Page"];
            currentPage = page;
        }
        else if (eventCode == EventCodes.SetupBoardColumn)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;

            // other column was meant.
            if (GetComponent<PhotonView>().ViewID != (int)data["ViewId"]) return;

            int[] issueViewIds = (int[])data["IssueCardViewIds"];
            SetupCardList(issueViewIds);
        }
    }


    /// <summary>
    /// Synchronize page based on ViewId of column, therefore had to be overriden.
    /// </summary>
    /// <param name="page"></param>
    public override void SynchronizePage(int page)
    {
        int viewId = GetComponent<PhotonView>().ViewID;
        Hashtable content = new Hashtable();
        content.Add("ViewId", viewId);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeBoardColumnPage, content, raiseEventOptions, SendOptions.SendReliable);

        content = new Hashtable();
        content.Add("ViewId", viewId);
        content.Add("Page", page);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeBoardColumnPage, content, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion


    
}
