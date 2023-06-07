using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Vector3 spawnPosition = GameObject.FindGameObjectWithTag("Respawn").transform.position;
        Quaternion spawnRotation = GameObject.FindGameObjectWithTag("Respawn").transform.rotation;

        string prefabName = (string) PhotonNetwork.LocalPlayer.CustomProperties["avatarPrefabName"];
        Debug.Log("Instantiating player with name: " + prefabName);

        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", prefabName), spawnPosition, spawnRotation);
    }
}
