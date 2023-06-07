using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditIssueCard : BaseEditCreateCard
{
    public TMP_Text keyTextfield;
    public TMP_Text creatorTextfield;
    public TMP_InputField titleInputfield;
    public TMP_InputField descriptionInputfield;
    public TMP_Dropdown transitionDropdown;
    public TMP_Dropdown typeDropdown;
    public TMP_Dropdown priorityDropdown;
    public GameObject prioritySection;

    public IssueCard issueCard;

    private string id, key, title, initialTypeId, creator, description, initialStatusId, initialPriorityId;
    private string initialStatusName, initialPriorityName, initialTypeName;

    private List<JiraIssueType> typesList;
    private List<JiraIssuePriority> prioritiesList;
    private List<JiraIssueTransition> transitionsList;
    private bool isPriorityProvided = false;



    public void Setup(string id, string key, string title, string description, string creator,
        string initialStatusId, string initialTypeId, string initialPriorityId, string initialStatusName, string initialTypeName, string initialPriorityName)
    {
        this.id = id;
        this.key = key;
        this.title = title;
        this.description = description;
        this.creator = creator;

        this.initialTypeId = initialTypeId;
        this.initialStatusId = initialStatusId;
        this.initialPriorityId = initialPriorityId;
        this.initialTypeName = initialTypeName;
        this.initialStatusName = initialStatusName;
        this.initialPriorityName = initialPriorityName;

        keyTextfield.text = this.key;
        creatorTextfield.text = this.creator;
        titleInputfield.text = this.title;
        descriptionInputfield.text = this.description;

        if (!string.IsNullOrEmpty(id) && issueCard.GetComponent<PhotonView>().IsMine)
        {
            InitDropdowns();
        }
    }

    public void Setup(IssueCard issueCard)
    {
        this.issueCard = issueCard;
        Setup(issueCard.id, issueCard.key, issueCard.title, issueCard.description, issueCard.creator,
                issueCard.statusId, issueCard.typeId, issueCard.priorityId, issueCard.statusName, issueCard.typeName, issueCard.priorityName);
    }



    /// <summary>
    /// Resets all variables and fields. 
    /// </summary>
    public override void Clear()
    {
        this.id = "";
        this.key = "";
        this.title = "";
        this.description = "";
        this.creator = "";

        this.initialTypeId = "";
        this.initialStatusId = "";
        this.initialPriorityId = "";
        this.initialTypeName = "";
        this.initialStatusName = "";
        this.initialPriorityName = "";

        isPriorityProvided = false;

        keyTextfield.text = this.key;
        titleInputfield.text = this.title;
        descriptionInputfield.text = this.description;
        creatorTextfield.text = this.creator;

        typeDropdown.ClearOptions();
        priorityDropdown.ClearOptions();
        transitionDropdown.ClearOptions();
    }



    /// <summary>
    /// Initialize the type, transition and priority dropdown fields. Two separate jira api calls must be executed beforehand. Is executed on the owner of the issue card.
    /// </summary>
    private void InitDropdowns()
    {
        GetEditMeta(id);
        GetTransitions(id);
    }



    /// <summary>
    /// Jira API call for requesting valid type and priority values. Calls ManageTypeAndPriorityDropdown when api call was successful.
    /// </summary>
    /// <param name="issueId">ID of the issue that is placed on the EditIssueAreaTrigger</param>
    private void GetEditMeta(string issueId)
    {
        JiraController.Instance.GetIssueEditMeta(issueId, err => ApiFeedback.Instance.DisplayFeedback(err), ManageTypeAndPriorityDropdown);
    }



    /// <summary>
    /// Fill values of type and priority dropdowns.
    /// </summary>
    /// <param name="jiraIssueEditMeta">Holds valid values when editing an issue</param>
    private void ManageTypeAndPriorityDropdown(JiraIssueEditMeta jiraIssueEditMeta)
    {
        priorityDropdown.ClearOptions();
        typeDropdown.ClearOptions();
        prioritySection.SetActive(false);

        typesList = jiraIssueEditMeta.issuetype.allowedValues;
        int typeIndex = 0;
        int priorityIndex = 0;
         
        if (jiraIssueEditMeta.priority != null)
        {
            isPriorityProvided = true;
            prioritiesList = jiraIssueEditMeta.priority.allowedValues;
            SynchronizePriorityActive(true);
            priorityDropdown.AddOptions(jiraIssueEditMeta.GetPrioritiesAsStringList());
            priorityIndex = priorityDropdown.options.FindIndex(option => option.text.Contains(initialPriorityName));
            priorityDropdown.GetComponent<DropdownSynchronizer>().SynchronizeDropdownOptionsEvent();
        }

        if (jiraIssueEditMeta.issuetype != null)
        {
            typesList = jiraIssueEditMeta.issuetype.allowedValues;
            typeDropdown.AddOptions(jiraIssueEditMeta.GetIssueTypesAsStringList());
            typeIndex = typeDropdown.options.FindIndex(option => option.text.Contains(initialTypeName));
            typeDropdown.GetComponent<DropdownSynchronizer>().SynchronizeDropdownOptionsEvent();
        }

        typeDropdown.value = typeIndex;
        priorityDropdown.value = priorityIndex;
    }




    /// <summary>
    /// Jira API call for requesting valid transition values. Calls ManageTransitionDropdown when api call was successful.
    /// </summary>
    /// <param name="issueId">ID of the issue that is placed on the EditIssueAreaTrigger</param>
    private void GetTransitions(string issueId)
    {
        JiraController.Instance.GetIssueTransitions(issueId, err => ApiFeedback.Instance.DisplayFeedback(err), ManageTransitionDropdown);
    }



    /// <summary>
    /// Fill values of transition dropdown.
    /// </summary>
    /// <param name="transitions">List of valid transitions</param>
    private void ManageTransitionDropdown(List<JiraIssueTransition> transitions)
    {
        transitionDropdown.ClearOptions();

        transitionsList = transitions;
        string startTransition = null;

        if (transitions != null)
        {
            List<string> statusStringList = new List<string>();
            transitions.ForEach(transition =>
            {
                //statusStringList.Add(s.to.name);
                statusStringList.Add(transition.id + " " + transition.name);
                if (transition.to.name == initialStatusName)
                {
                    startTransition = transition.name;
                }
            });
            transitionDropdown.AddOptions(statusStringList);
        }

        transitionDropdown.GetComponent<DropdownSynchronizer>().SynchronizeDropdownOptionsEvent();
        transitionDropdown.value = transitionDropdown.options.FindIndex(transition => transition.text == startTransition);
    }




    public override void OnSaveClick()
    {
        string issueId = id;
        string title = titleInputfield.text;
        string description = descriptionInputfield.text;

        string typeId = typeDropdown.options[typeDropdown.value].text.Split(" ")[0];
        string transitionId = transitionDropdown.options[transitionDropdown.value].text.Split(" ")[0];

        string priorityId = null;
        if (isPriorityProvided)
        {
            priorityId = priorityDropdown.options[priorityDropdown.value].text.Split(" ")[0];
        }

        JiraController.Instance.EditIssue(issueId, title, description, typeId, priorityId, transitionId, err => ApiFeedback.Instance.DisplayFeedback(err),
            res =>
            {
                JiraController.Instance.GetIssue(issueId, err => ApiFeedback.Instance.DisplayFeedback(err), issueCard.SynchronizeIssueCard);
            }
        );

        SynchronizeClose();
    }


    #region Photon

    public void EditIssueCardOnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.SynchronizeEditIssueCardPriority)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;
            bool active = (bool)data["Active"];
            prioritySection.SetActive(active);
        }
    }


    public void SynchronizePriorityActive(bool active)
    {
        Hashtable content = new Hashtable();
        content.Add("Active", active);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeEditIssueCardPriority, content, raiseEventOptions, SendOptions.SendReliable);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal};
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeEditIssueCardPriority, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        PhotonNetwork.NetworkingClient.EventReceived += EditIssueCardOnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        PhotonNetwork.NetworkingClient.EventReceived -= EditIssueCardOnEvent;
    }

    #endregion
}
