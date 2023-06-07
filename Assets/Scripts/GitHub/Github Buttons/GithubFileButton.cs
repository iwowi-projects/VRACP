using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GithubFileButton : MonoBehaviour, IPunInstantiateMagicCallback
{
    public string fileName { get; set; }
    public string fileCode { get; set; }

    public TMP_Text filenameTextfield;
    private TMP_Text codeAreaFilenameTextfield;
    private TMP_Text codeTextfield;

    private void Start()
    {
        codeTextfield = GameObject.Find("Textfield GitHub File Content").GetComponent<TMP_Text>();
        codeAreaFilenameTextfield = GameObject.Find("Textfield GitHub File Name").GetComponent<TMP_Text>();
    }

    public void Setup(string fileName, string fileCode)
    {
        this.fileCode = fileCode;
        this.fileName = fileName;

        filenameTextfield.text = this.fileName;
    }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        this.transform.SetParent(GameObject.Find((string)data[0]).transform);
        this.transform.localScale = new Vector3(1, 1, 1);

        this.fileName = (string)data[1];
        this.fileCode = (string)data[2];

        filenameTextfield.text = this.fileName;
    }

    public void ShowCodeInCodeAreaOnAll()
    {
        Debug.Log("Call ShowCodeInCodeAreaOnAll RPC");
        GetComponent<PhotonView>().RPC("ShowCodeInCodeArea", RpcTarget.AllBuffered, new object[]{ });
    }

    [PunRPC]
    public void ShowCodeInCodeArea()
    {
        codeTextfield.text = fileCode;
        codeAreaFilenameTextfield.text = fileName;
    }

    /**
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {  
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.SynchronizeFileCodeEventCode)
        {
            Debug.Log("SynchronizeFileCodeEvent raised");
            ShowCodeInCodeArea();
        }
    }

    
    public void SynchronizeFileSelectionEvent()
    {
        object[] content = new object[] {  };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeFileCodeEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }**/

}
