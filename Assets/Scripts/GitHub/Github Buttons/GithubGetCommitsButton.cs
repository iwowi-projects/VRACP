using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GithubGetCommitsButton : MonoBehaviour
{
    public TMP_Dropdown projectsDropdown;
    public TMP_InputField ownerInput;

    /*
     * Gets executed when clicking on the button. Makes RPC call on master client to generate the list of commits
     */
    public void GenerateCommitButtons()
    {

        this.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        string project = projectsDropdown.options[projectsDropdown.value].text;
        string owner = ownerInput.text;
        object[] github_params= { project, owner };
        this.GetComponent<PhotonView>().RPC("GenerateCommitButtonsOnMaster", RpcTarget.MasterClient, github_params);
    }


    [PunRPC]
    public void GenerateCommitButtonsOnMaster(object[] github_params)
    {
        Debug.Log("Generating commit list on master");
        string project = (string) github_params[0];
        string owner = (string) github_params[1];
        GithubApiController.Instance.GetCommits(owner, project, err => { }, res => { });
    }
}
