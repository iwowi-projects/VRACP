using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardColumns : MonoBehaviour
{
    private int boardId;
    public List<GameObject> boardColumns;
    public TMP_Text boardNameTextfield;



    public void Setup(int boardId)
    {
        this.boardId = boardId;
        JiraController.Instance.GetBoardConfiguration(boardId, err => ApiFeedback.Instance.DisplayFeedback(err), InstantiateColumns);
        ManageCurrentBoardName(boardId);
    }


    /// <summary>
    /// Destroys all the columns and clears the columns and cards list.
    /// </summary>
    public void Clear()
    {
        boardColumns.ForEach(col =>
        {
            try
            {
                col.GetComponent<BoardColumn>().Clear();
                if (col.GetComponent<PhotonView>().IsMine)
                {
                    PhotonNetwork.Destroy(col);
                }
            } catch(Exception e)
            {
                Debug.Log(e);
            }
            
        });
        boardColumns.Clear();
    }


    public void ManageCurrentBoardName(int boardId)
    {
        JiraController.Instance.GetBoardFeatures(boardId, err => ApiFeedback.Instance.DisplayFeedback(err), features =>
        {
            if (features.HasSprints())
            {
                // When the player leaves the room, he might have to raise another ClearAll event before leaving.
                Hashtable hashtable = new Hashtable();
                hashtable.Add("BoardFeatures", JsonConvert.SerializeObject(features));
                //hashtable.Add("CurrentBoardHasSprints", features.HasSprints());
                //hashtable.Add("CurrentBoardHasBacklog", features.HasBacklog());
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

                JiraController.Instance.GetBoardActiveSprint(boardId, err => { }, sprint =>
                {
                    if (sprint != null)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("CurrentSprintId", sprint.id);
                        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

                        SetCurrentBoardName(sprint.name);
                    }
                });
            }
            else
            {
                SetCurrentBoardName((string)PhotonNetwork.CurrentRoom.CustomProperties["BoardName"]);
            }
        });
    }

    public void SetCurrentBoardName(string name)
    {
        boardNameTextfield.SetText(name);
        boardNameTextfield.GetComponent<TextFieldSynchronizer>().SynchronizeTextEvent();
    }


    /// <summary>
    /// Instantiate columns based on the columns list and raise event to all clients to setup the board columns.
    /// </summary>
    /// <param name="columns">List of board columns</param>
    public void InstantiateColumns(List<JiraBoardColumn> jiraColumns)
    {
        List<int> columnViewIds = new List<int>();
        List<GameObject> columns = new List<GameObject>();

        Vector3 pos = transform.position;
        float step = 1.5f;

        foreach (JiraBoardColumn jiraColumn in jiraColumns)
        {
            // Set custom instantiation data and instantiate the column with photon.
            List<string> statusIds = new List<string>();
            jiraColumn.statuses.ForEach(status => statusIds.Add(status.id));

            object[] customInstantiationData = { jiraColumn.name, statusIds.ToArray(), this.boardId };
            GameObject column = PhotonNetwork.Instantiate(PrefabPaths.BOARD_COLUMN_PREFAB_PATH, pos, this.transform.rotation, 0, customInstantiationData);
            pos = new Vector3(pos.x, pos.y, pos.z - step);
            columnViewIds.Add(column.GetComponent<PhotonView>().ViewID);
        }

        Hashtable content = new Hashtable();

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SetupBoardColumns, content, raiseEventOptions, SendOptions.SendReliable);

        content.Add("ColumnViewIds", columnViewIds.ToArray());

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SetupBoardColumns, content, raiseEventOptions, SendOptions.SendReliable);

    }

    public void SetupColumnList(int[] columnViewIds)
    {
        List<int> viewIds = new List<int>(columnViewIds);
        Clear();
        viewIds.ForEach(id =>
        {
            boardColumns.Add(PhotonView.Find(id).gameObject);
        });
    }


    #region Photon

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.SetupBoardColumns)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;

            int[] issueViewIds = (int[])data["ColumnViewIds"];
            SetupColumnList(issueViewIds);
        } else if (eventCode == EventCodes.ClearBoardColumns)
        {
            Clear();
        }
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
