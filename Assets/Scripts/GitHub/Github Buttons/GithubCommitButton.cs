using Newtonsoft.Json;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

// Button in the list of commits
// OnClick: Show files in the files list
public class GithubCommitButton : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public TMP_Text textfieldCommitName;
    public TMP_Text textfieldCommitButton;
    GithubCommitProperties properties;
    string commitUrl;
    string commitMessage;

    /*
     * Gets called by Photon when instantiated.
     * PhotonMessageInfo info: contains name of parent object, the url and the commit message
     */
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        this.transform.SetParent(GameObject.Find((string)data[0]).transform);
        this.transform.localScale = new Vector3(1, 1, 1);

        //this.properties = (GithubCommitProperties) data[1];
        this.commitMessage = (string)data[1];
        this.commitUrl = (string)data[2];

        textfieldCommitButton.text = this.commitMessage;
    }


    [PunRPC]
    public void GenerateFileButtons()
    {
        //GithubApiController.Instance.GetCommitFiles(commitUrl, err => { }, result => { });
    }


    public void GenerateFileButtonsOnMaster()
    {
        Debug.Log("Called GenerateFileButtonsOnMaster");
        GetComponent<PhotonView>().RPC("GenerateFileButtons", RpcTarget.MasterClient, new object[0]);
    }

    
    

}
