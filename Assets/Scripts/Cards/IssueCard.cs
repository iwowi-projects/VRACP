using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class IssueCard : BaseCard, IPunInstantiateMagicCallback
{
    public TMP_Text titleTextfield;
    public TMP_Text authorTextfield;
    public TMP_Text typeTextfield;
    public TMP_Text descriptionTextfield;
    public TMP_Text statusTextfield;
    public TMP_Text keyTextfield;
    public string id, key, title, creator, description, typeId, statusId, priorityId, statusName, priorityName, typeName;



    public void Setup(string id, string key, string title, string description, string creator, string typeId, string statusId, string priorityId,
        string typeName, string statusName, string priorityName)
    {
        this.id = id;
        this.key = key;
        this.title = title;
        this.description = description;
        this.creator = creator;

        this.typeId = typeId;
        this.statusId = statusId;
        this.priorityId = priorityId;
        this.typeName = typeName;
        this.statusName = statusName;
        this.priorityName = priorityName;

        titleTextfield.text = this.title;
        authorTextfield.text = this.creator;
        typeTextfield.text = this.typeName;
        descriptionTextfield.text = this.description;
        statusTextfield.text = this.statusName;
        keyTextfield.text = this.key;
    }


    public void Setup(JiraIssue jiraIssue)
    {
        Setup(jiraIssue.id, jiraIssue.key, jiraIssue.fields.summary, jiraIssue.fields.description, jiraIssue.fields.creator.displayName,
                jiraIssue.fields.issuetype.id, jiraIssue.fields.status.id, jiraIssue.fields.priority.id, jiraIssue.fields.issuetype.name, jiraIssue.fields.status.name, jiraIssue.fields.priority.name);
    }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        this.transform.localScale = new Vector3(1, 1, 1);

        string id =           (string)data[0];
        string key =          (string)data[1];
        string title =        (string)data[2];
        string description = (string)data[3];
        string creator =       (string)data[4];
        string typeId =         (string)data[5];
        string statusId =       (string)data[6];
        string priorityId =     (string)data[7];
        string typeName =     (string)data[8];
        string statusName =   (string)data[9];
        string priorityName = (string)data[10];

        Setup(id, key, title, description, creator, typeId, statusId, priorityId, typeName, statusName, priorityName);
    }



    #region Photon

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += BaseCardOnEvent;
        PhotonNetwork.NetworkingClient.EventReceived += IssueCardOnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= BaseCardOnEvent;
        PhotonNetwork.NetworkingClient.EventReceived -= IssueCardOnEvent;
    }

    public void IssueCardOnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.SynchronizeIssueCard)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;

            // other issue card was meant.
            if (GetComponent<PhotonView>().ViewID != (int)data["ViewId"]) return;

            string id = (string)data["Id"];
            string key = (string)data["Key"];
            string title = (string)data["Summary"];
            string description = (string)data["Description"];
            string creator = (string)data["Creator"];
            string typeId = (string)data["TypeId"];
            string statusId = (string)data["StatusId"];
            string priorityId = (string)data["PriorityId"];
            string typeName = (string)data["TypeName"];
            string statusName = (string)data["StatusName"];
            string priorityName = (string)data["PriorityName"];

            Setup(id, key, title, description, creator, typeId, statusId, priorityId, typeName, statusName, priorityName);
        }
    }

    public void SynchronizeIssueCard(JiraIssue jiraIssue)
    {
        int viewId = GetComponent<PhotonView>().ViewID;
        Hashtable content = new Hashtable();

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeIssueCard, content, raiseEventOptions, SendOptions.SendReliable);

        content.Add("ViewId", viewId);
        content.Add("Id", jiraIssue.id);
        content.Add("Key", jiraIssue.key);
        content.Add("Summary", jiraIssue.fields.summary);
        content.Add("Description", jiraIssue.fields.description);
        content.Add("Creator", jiraIssue.fields.creator.displayName);
        content.Add("TypeId", jiraIssue.fields.issuetype.id);
        content.Add("StatusId", jiraIssue.fields.status.id);
        content.Add("PriorityId", jiraIssue.fields.priority.id);
        content.Add("TypeName", jiraIssue.fields.issuetype.name);
        content.Add("StatusName", jiraIssue.fields.status.name);
        content.Add("PriorityName", jiraIssue.fields.priority.name);



        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeIssueCard, content, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion

}
