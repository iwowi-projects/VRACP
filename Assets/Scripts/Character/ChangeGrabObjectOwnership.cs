using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeGrabObjectOwnership : MonoBehaviour
{
    List<string> valid = new List<string> { "IssueCard", "SprintCard", "BoardIssueCard" };


    public void ChangeOwnership(SelectEnterEventArgs selectEnterEventArgs)
    {
        GameObject interactable = selectEnterEventArgs.interactableObject.transform.gameObject;

        if (valid.Exists(s => interactable.CompareTag(s)))
        {
            // relevant when placing issue card in backlog so that only the owner makes the api call.
            interactable.GetComponent<PhotonView>().RequestOwnership();
            interactable.GetComponent<BaseCard>().SynchronizeBeingHold(true);
        }
    }

    public void SelectExit(SelectExitEventArgs selectExitEventArgs)
    {
        GameObject interactable = selectExitEventArgs.interactableObject.transform.gameObject;
        if (valid.Exists(s => interactable.CompareTag(s)))
        {
            interactable.GetComponent<BaseCard>().SynchronizeBeingHold(false);
        }
    }

}
