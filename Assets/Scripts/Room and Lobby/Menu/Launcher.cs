using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    public InputField roomNameField;
    public TMP_Text roomName;
    public TMP_Text nickNameField;
    public TMP_InputField nicknameInput;
    public static string nickName;

    // shows all active rooms
    public Transform roomListContent;
    public GameObject roomListItemPrefab;

    // shows all players in a specific room
    public Transform playerListContent;
    public GameObject playerListItemPrefab;

    // show start game only on host
    public GameObject startGameButton;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();


    private void Awake()
    {
        Instance = this;
        
        // If one leaves the room, his nickname is still saved.
        if (PhotonNetwork.NickName.Length == 0)
        {
            nickName = "Player " + Random.Range(0, 1000).ToString("0000");
        } else
        {
            nickName = PhotonNetwork.NickName;
        }
        
    }

    void Start()
    {
        MenuManager.Instance.openMenu("loadingMenu");
        PhotonNetwork.ConnectUsingSettings();
    }


    #region PUN Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");

        PhotonNetwork.NickName = nickName;
        nickNameField.text = nickName;

        PhotonNetwork.JoinLobby();

        // switch scene on all clients if host switches
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.openMenu("startMenu");
        Debug.Log("Joined Lobby");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room: " + PhotonNetwork.CurrentRoom.Name);
        MenuManager.Instance.openMenu("roomMenu");
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        updatePlayerList();

        // deactivate button if you are not the host
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed joining a room");
        MenuManager.Instance.openMenu("loading");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
        MenuManager.Instance.openMenu("startMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // destroy old buttons and insert new ones
        Debug.Log("Updated room list with length: " + roomList.Count);

        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            cachedRoomList[info.Name] = info;
        }

        foreach (Transform button in roomListContent)
        {
            Destroy(button.gameObject);
        }

        foreach(KeyValuePair<string, RoomInfo> rm in cachedRoomList)
        {
            if (!rm.Value.IsOpen || !rm.Value.IsVisible) continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>()
                .SetUp(rm.Value);
        }

        /*foreach (RoomInfo rm in roomList)
        {
            if (rm.RemovedFromList)
            {
                Debug.Log("RemovedFromList: " + rm.Name);
                continue;
            }
            Debug.Log("Instantiating room in roomList: " + rm.Name);
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>()
                .SetUp(rm);

        }*/
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        updatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        updatePlayerList();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        updatePlayerList();
    }

    #endregion


    public void CreateRoom()
    {
        Debug.Log("Creating a room...");
        string roomName = roomNameField.text;
        if (string.IsNullOrEmpty(roomName))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { IsOpen = true, IsVisible = true});
        MenuManager.Instance.openMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        Debug.Log("Joining room " + info.Name);
        PhotonNetwork.JoinRoom(info.Name);
    }

    public void LeaveRoom()
    {
        Debug.Log("Leaving room...");
        PhotonNetwork.LeaveRoom();
    }

    public void updatePlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform button in playerListContent)
        {
            Destroy(button.gameObject);
        }

        foreach (Player player in players)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>()
                .SetUp(player, (string) player.CustomProperties["avatarPrefabName"]);
        }
    }

    public void StartGame()
    {
        // load level battlefield
        PhotonNetwork.LoadLevel(1);
    }

    public void SetNickname()
    {
        string newNickname = nicknameInput.text;
        if (newNickname.Length == 0)
        {
            return;
        }
        nickName = newNickname;
        PhotonNetwork.NickName = nickName;
        ShowNickname();
    }

    private void ShowNickname() 
    {
        nickNameField.text = PhotonNetwork.NickName;
    }

}
