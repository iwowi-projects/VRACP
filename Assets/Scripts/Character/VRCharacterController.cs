using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class VRCharacterController : MonoBehaviour
{
    private PhotonView pV;
    private PhotonVoiceView voiceView;
    
    public TMP_Text nicknameTextfield;
    public Canvas nicknameCanvas;
    public Image speakerIcon;

    public GameObject menuGO;
    public GameObject feedbackGO;
    public InputActionReference menuReference;

    [Tooltip("This transform is getting scaled to 0 when entering a room so that the local player does not see his own face")]
    public Transform neckTransform;

    public Camera mainCamera;
    public ActionBasedController rightHandController;
    public ActionBasedController leftHandController;

    public GameObject locomotionSystemGO;


    private void OnDisable()
    {
         menuReference.action.started -= OnMenuButtonPressed;
    }

    public void Awake()
    {
        pV = GetComponent<PhotonView>();
        voiceView = GetComponent<PhotonVoiceView>();
    }

    void Start()
    {
        SetNickname();
        if (pV.IsMine)
        {
            this.tag = "LocalPlayer";
            GetComponent<ApiFeedback>().enabled = true;
            GetComponent<CharacterController>().enabled = true;
            GetComponentInChildren<XROrigin>().enabled = true;
            GetComponent<LegsAnimation>().enabled = true;
            GetComponent<SyncHandAndHead>().enabled = true;
            GetComponent<InputActionManager>().enabled = true;

            mainCamera.enabled = true;
            mainCamera.gameObject.GetComponent<AudioListener>().enabled = true;
            rightHandController.enabled = true;
            leftHandController.enabled = true;

            locomotionSystemGO.GetComponent<LocomotionSystem>().enabled = true;
            locomotionSystemGO.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
            locomotionSystemGO.GetComponent<ActionBasedSnapTurnProvider>().enabled = true;

            locomotionSystemGO.gameObject.SetActive(true);
            menuGO.gameObject.SetActive(true);

            neckTransform.localScale = new Vector3(0, 0, 0);
            menuReference.action.started += OnMenuButtonPressed;

            return;
        } 
        else
        {
            Destroy(GetComponent<ApiFeedback>());
            Destroy(GetComponent<CharacterController>());
            Destroy(GetComponent<XROrigin>());
            Destroy(GetComponent<LegsAnimation>());
            Destroy(GetComponent<SyncHandAndHead>());
            Destroy(GetComponent<InputActionManager>());

            Destroy(mainCamera.gameObject);
            Destroy(rightHandController.gameObject);
            Destroy(leftHandController.gameObject);

            Destroy(locomotionSystemGO);
            Destroy(menuGO);
            Destroy(feedbackGO);
        }
    }

    void SetNickname()
    {
        try
        {
            nicknameTextfield.text = pV.Owner.NickName;
        } catch(Exception e)
        {
            Debug.Log(e);
        }
        
    }

    private void Update()
    {
        speakerIcon.gameObject.SetActive(voiceView.IsSpeaking);
    }

    public void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        menuGO.SetActive(!menuGO.activeSelf);
    }
   

}
