using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EditIssueAreaTrigger : MonoBehaviour
{
    public EditIssueCard editIssueCard;
    public List<string> validTags;
    public Transform attachPoint;
    public GameObject attachCube;

    private List<Collider> pendingCards;
    private MeshRenderer attachCubeRenderer;

    private GameObject currentCard;


    private void Awake()
    {
        pendingCards = new List<Collider>();
    }

    void Start()
    {
        attachCubeRenderer = attachCube.GetComponent<MeshRenderer>();
        MakeColorTransparent();
    }

    void Update()
    {
        if (pendingCards.Count == 0) return;

        List<Collider> pendingCardsCopy = new List<Collider>(pendingCards);
        pendingCardsCopy.ForEach(card =>
        {
            if (!card.gameObject.GetComponent<BaseCard>().beingHold)
            {
                ReleaseAndAddCard(card);
            }
        });
    }

    public void OnTriggerEnter(Collider c)
    {
        if (currentCard != null) return;
        if (!validTags.Contains(c.gameObject.tag)) return;
        
        pendingCards.Add(c);
        MakeColorOpaque();
    }


    public void OnTriggerExit(Collider c)
    {
        if (c.gameObject != currentCard) return;
        if (validTags.Contains(c.gameObject.tag))
        {
            pendingCards.Remove(c);
            try
            {
                MakeColorTransparent();
                editIssueCard.Close();
                currentCard = null;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    public void ReleaseAndAddCard(Collider c)
    {
        pendingCards.Remove(c);
        currentCard = c.gameObject;

        editIssueCard.Setup(c.GetComponent<IssueCard>());
        editIssueCard.Open();

        if (c.GetComponent<PhotonView>().IsMine)
        {
            SnapColliderToAttachPoint(c);
        }
              
    }

    public void SnapColliderToAttachPoint(Collider c)
    {
        c.transform.position = attachPoint.transform.position;
        c.transform.rotation = attachPoint.transform.rotation;
    }

    private void MakeColorTransparent()
    {
        var color = attachCubeRenderer.material.color;
        color.a = 0f;
        attachCubeRenderer.material.color = color;
    }

    private void MakeColorOpaque()
    {
        var color = attachCubeRenderer.material.color;
        color.a = 0.9f;
        attachCubeRenderer.material.color = color;
    }

}
