using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BoardConfiguration : MonoBehaviour
{
    public static BoardConfiguration Instance;

    public TMP_Text boardNameTextfield;

    public BacklogShelf backlogShelf;
    public SprintsShelf sprintsShelf;
    public BoardColumns boardColumns;
    public SprintIssuesShelf sprintIssuesShelf;

    public EditIssueCard editIssueCard;
    public CreateIssueCard createIssueCard;
    public CreateSprintCard createSprintCard;


    private void Awake()
    {
        Instance = this;
    }

    #region Setup Areas

    public void ShowBacklog(int boardId)
    {
        // Call setup on backlog
        backlogShelf.Setup(boardId);
    }


    public void ShowSprints(int boardId)
    {
        // Call setup on sprints
        sprintsShelf.Setup(boardId);
    }


    public void ShowBoardIssuesAndColumns(int boardId)
    {
        // Call setup on board columns
        boardColumns.Setup(boardId);
    }

    public void UpdateBoardIssuesAndColumns()
    {
        boardColumns.Clear();
        RemoteClearBoardColumns();

        int boardId = (int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentBoardId"];
        ShowBoardIssuesAndColumns(boardId);
    }





    public void ShowBoard(int boardId, string boardName)
    {
        // When the player leaves the room, he might have to raise another ClearAll event before leaving.
        Hashtable hashtable = new Hashtable();
        hashtable.Add("BoardIntializer", PhotonNetwork.LocalPlayer);
        hashtable.Add("CurrentBoardId", boardId);
        hashtable.Add("BoardName", boardName);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

        ClearAll();
        RemoteClearAll();

        ShowBacklog(boardId);
        ShowSprints(boardId);
        ShowBoardIssuesAndColumns(boardId);

        SetBoardName(boardName);
    }


    public void SetBoardName(string name)
    {
        boardNameTextfield.SetText(name);
        boardNameTextfield.GetComponent<TextFieldSynchronizer>().SynchronizeTextEvent();
    }

    


    public void ClearAll()
    {
        backlogShelf.Clear();
        sprintsShelf.Clear();
        boardColumns.Clear();
        sprintIssuesShelf.Clear();

        editIssueCard.Close();
        createIssueCard.Close();
        createSprintCard.Close();
    }

    #endregion



    #region Photon

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.ClearAll)
        {
            Debug.Log("Received ClearAll");
            ClearAll();
        }
    }

    public void RemoteClearAll()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.ClearAll, new object[] { }, raiseEventOptions, SendOptions.SendReliable);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.ClearAll, new object[] { }, raiseEventOptions, SendOptions.SendReliable);
    }

    public void RemoteClearBoardColumns()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.ClearBoardColumns, new object[] { }, raiseEventOptions, SendOptions.SendReliable);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.ClearBoardColumns, new object[] { }, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    #endregion

}
