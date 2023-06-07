using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager Instance;

    void Awake()
    {
        if (Instance)
        {
            Debug.Log("Destroying RoomManager");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            Vector3 spawnPosition = GameObject.FindGameObjectWithTag("Respawn").transform.position;
            Quaternion spawnRotation = GameObject.FindGameObjectWithTag("Respawn").transform.rotation;

            string prefabName = (string)PhotonNetwork.LocalPlayer.CustomProperties["avatarPrefabName"];
            Debug.Log("Instantiating player with name: " + prefabName);

            GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", prefabName), spawnPosition, spawnRotation);
            //GameObject playerManager = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    public void SetAvatarPrefab(string name)
    {
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        Debug.Log("Setting player avatar with name: " + name);
        hashtable.Add("avatarPrefabName", name);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    public string GetAvatarPrefabName()
    {
        return (string) PhotonNetwork.LocalPlayer.CustomProperties["avatarPrefabName"];
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
        {
            Debug.Log("OnLeftRoom in scene 1: Loading scene 0");
            SceneManager.LoadSceneAsync(0);
        }
        
    }
}
