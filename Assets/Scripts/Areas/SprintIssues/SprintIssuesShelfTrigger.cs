using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintIssuesShelfTrigger : BaseCardTrigger
{

    public override void Update()
    {
        List<Collider> pendingCardsCopy = new List<Collider>(pendingCards);
        pendingCardsCopy.ForEach(card =>
        {
            if (!card.gameObject.GetComponent<BaseCard>().beingHold)
            {
                ReleaseAndAddCard(card);
            }
        });
    }

    public override void OnTriggerEnter(Collider c)
    {
        if (validTags.Contains(c.gameObject.tag))
        {
            pendingCards.Add(c);
            MakeColorOpaque();
        }

    }


    public override void OnTriggerExit(Collider c)
    {
        if (validTags.Contains(c.gameObject.tag))
        {
            pendingCards.Remove(c);
            try
            {
                MakeColorTransparent();
                shelf.Clear();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    public override void ReleaseAndAddCard(Collider c)
    {
        pendingCards.Remove(c);

        int sprintId = c.GetComponent<SprintCard>().id;
        shelf.GetComponent<SprintIssuesShelf>().SetSprintId(sprintId);

        if (c.GetComponent<PhotonView>().IsMine)
        {
            shelf.GetComponent<SprintIssuesShelf>().Setup(sprintId);
            SnapColliderToAttachPoint(c);
        }
    }

    public void SnapColliderToAttachPoint(Collider c)
    {
        c.transform.position = transform.position;
        c.transform.rotation = transform.rotation;
    }
}
