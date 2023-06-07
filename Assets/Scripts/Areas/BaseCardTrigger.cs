using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCardTrigger : MonoBehaviour
{
    public List<string> validTags;
    public BaseShelf shelf;
    public int index;

    protected MeshRenderer attachCubeRenderer;
    protected List<Collider> pendingCards;



    protected void Awake()
    {
        pendingCards = new List<Collider>();
        attachCubeRenderer = GetComponent<MeshRenderer>();
        MakeColorTransparent();
    }



    public virtual void Update()
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



    public virtual void OnTriggerEnter(Collider c)
    {
        if (!shelf.jiraCallEnabled) return;
        if (validTags.Contains(c.gameObject.tag))
        {
            pendingCards.Add(c);
            MakeColorOpaque();
        }

    }



    public virtual void OnTriggerExit(Collider c)
    {
        if (!shelf.jiraCallEnabled) return;
        if (validTags.Contains(c.gameObject.tag))
        {
            MakeColorTransparent();
            pendingCards.Remove(c);
            shelf.RemoveCard(c, index);
        }
    }



    public virtual void ReleaseAndAddCard(Collider c)
    {
        pendingCards.Remove(c);
        shelf.AddCard(c, index);
        MakeColorTransparent();
    }



    protected void MakeColorTransparent()
    {
        var color = attachCubeRenderer.material.color;
        color.a = 0f;
        attachCubeRenderer.material.color = color;
    }



    protected void MakeColorOpaque()
    {
        var color = attachCubeRenderer.material.color;
        color.a = 0.9f;
        attachCubeRenderer.material.color = color;
    }
}
