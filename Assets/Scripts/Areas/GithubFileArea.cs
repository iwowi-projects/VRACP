using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GithubFileArea : MonoBehaviour
{
    public TMP_Text fileNameTextfield;
    public TMP_Text fileCodeTextfield;

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.SynchronizeGitHubFile)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;

            fileNameTextfield.text = (string) data["FileName"];
            string patchText = (string)data["FilePatch"];

            // Insert line numbers
            int currentLineNumber = 0;
            for (int i = 0; i<patchText.Length; i++)
            {
                if (patchText[i] == '\n')
                {
                    patchText = patchText.Insert(i + 1, currentLineNumber + "  ");
                    currentLineNumber++;
                } 
            }
            fileCodeTextfield.text = patchText;
        }
    }


    public virtual void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public virtual void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
}
