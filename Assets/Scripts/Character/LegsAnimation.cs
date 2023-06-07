using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class LegsAnimation : MonoBehaviour
{
    public InputActionReference moveAction;
    private Animator animator;
    private PhotonView pV;

    private void Awake()
    {
        pV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (!pV.IsMine)
        {
            return;
        }
        animator = GetComponentInChildren<Animator>();
        moveAction.action.started += SetWalkingTrue;
        moveAction.action.canceled += SetWalkingFalse;
    }


    public void OnDisable()
    {
        moveAction.action.started -= SetWalkingTrue;
        moveAction.action.canceled -= SetWalkingFalse;
    }

    public void SetWalkingTrue(CallbackContext context)
    {
        animator.SetBool("isWalking", true);
    }

    public void SetWalkingFalse(CallbackContext context)
    {
        animator.SetBool("isWalking", false);
    }

}
