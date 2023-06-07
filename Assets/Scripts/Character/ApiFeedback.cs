using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApiFeedback : MonoBehaviour
{
    public GameObject feedbackGO;
    public TMP_Text feedbackText;
    public static ApiFeedback Instance;

    private void Start()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            Instance = this;
        }
        feedbackGO.SetActive(false);
    }

    public void DisplayFeedback(string text)
    {
        Debug.Log("DisplayFeedback: " + text);
        feedbackGO.SetActive(true);
        feedbackText.text = text;
    }

    public void Close()
    {
        feedbackGO.SetActive(false);
    }

}
