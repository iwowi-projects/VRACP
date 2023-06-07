using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class BaseShelf : MonoBehaviour
{
    public GameObject[][] cardsArray;

    [Tooltip("List of card triggers. When awake, index will be automatically set")]
    public List<BaseCardTrigger> triggers;
    public int currentPage, pages;
    public bool jiraCallEnabled;

    protected Vector3 visible = new Vector3(1, 1, 1);
    protected Vector3 invisible = new Vector3(0, 0, 0);

    public abstract byte SetupEventCode { get; }
    public abstract byte SynchronizePageEventCode { get; }


    public virtual void Awake()
    {
        for (int i = 0; i<triggers.Count; i++)
        {
            triggers[i].index = i;
        }
        jiraCallEnabled = false;
    }

    public abstract void Setup(int id);



    #region Photon Event

    /// <summary>
    /// Manages the card list setup und page synchronization
    /// </summary>
    /// <param name="photonEvent"></param>
    public virtual void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == SetupEventCode)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;
            int[] viewIdsArray = (int[])data["ViewIdsArray"];
            SetupCardList(viewIdsArray);
        }
        else if (eventCode == SynchronizePageEventCode)
        {
            Hashtable data = (Hashtable)photonEvent.CustomData;
            int page = (int)data["Page"];
            currentPage = page;
        }
    }



    /// <summary>
    /// Synchronizes the current page when clicked on up/down button based on the SynchronizePageEventCode.
    /// </summary>
    /// <param name="page"></param>
    public virtual void SynchronizePage(int page)
    {
        Hashtable content = new Hashtable();
        content.Add("Page", page);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(SynchronizePageEventCode, content, raiseEventOptions, SendOptions.SendReliable);

        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(SynchronizePageEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }



    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    #endregion



    #region Setup

    /// <summary>
    /// Destroys all cards in the card array and sets value to null.
    /// </summary>
    public virtual void Clear()
    {
        jiraCallEnabled = false;

        // return when board is fetched for first time.
        if (cardsArray == null) return;

        try
        {
            for (int i = 0; i < cardsArray.Length; i++)
            {
                for (int j = 0; j < cardsArray[i].Length; j++)
                {
                    if (cardsArray[i][j] == null) continue;
                    try
                    {
                        if (cardsArray[i][j].GetComponent<PhotonView>().IsMine)
                        {
                            PhotonNetwork.Destroy(cardsArray[i][j]);
                            cardsArray[i][j] = null;
                        }
                    }
                    catch (Exception e) { Debug.Log(e); }
                }
            }
        } catch (Exception e) { Debug.Log(e); }
    }


    /// <summary>
    /// Calculate the pages based on the viewIds and attachPoints. Initialize and fill the cardsArray with cards.  
    /// </summary>
    /// <param name="viewIdsArray">Photon ViewIds of the cards that has to be inserted into the shelf.</param>
    public void SetupCardList(int[] viewIdsArray)
    {
        List<int> viewIds = new List<int>(viewIdsArray);

        // calculate how many pages are needed
        int attachPointsCount = triggers.Count;
        pages = (int) Math.Ceiling((double)viewIdsArray.Length / (double)attachPointsCount);


        // extend array by one page if pages are full
        if (viewIds.Count % attachPointsCount == 0)
        {
            pages += 1;
        }


        // Initialize cardsArray based on pages needed.
        cardsArray = new GameObject[pages][];
        for (int k = 0; k<pages; k++)
        {
            cardsArray[k] = new GameObject[attachPointsCount];
        }


        // Fill cardsArray with cards.
        int i = 0;
        int j = 0;

        viewIds.ForEach(id =>
        {
            PhotonView cardPV = PhotonView.Find(id);
            if (cardPV != null)
            {
                cardsArray[i][j] = cardPV.gameObject;
                j++;
                if (j == attachPointsCount)
                {
                    i++;
                    j = 0;
                }
            }
        });


        SetupPlaces();
        ShowCardsByPage(0);

        jiraCallEnabled = true;
    }



    /// <summary>
    /// Place all cards of the cards array to their correct position.
    /// </summary>
    public void SetupPlaces()
    {
        for (int i = 0; i<cardsArray.Length; i++)
        {
            for (int j = 0; j < cardsArray[i].Length; j++)
            {
                if (cardsArray[i][j] == null) continue;
                try
                {
                    cardsArray[i][j].transform.position = triggers[j].transform.position;
                    cardsArray[i][j].transform.rotation = triggers[j].transform.rotation;
                    cardsArray[i][j].transform.localScale = invisible;
                    cardsArray[i][j].gameObject.SetActive(true);
                } catch (Exception e) { Debug.Log(e); }
            }
        }
    }



    /// <summary>
    /// Snap card to the attach point.
    /// </summary>
    /// <param name="cardCollider">Card to be snapped</param>
    /// <param name="attachPoint">Snap target transform</param>
    public void SnapCardToAttachPoint(Collider cardCollider, Transform attachPoint)
    {
        cardCollider.transform.position = attachPoint.transform.position;
        cardCollider.transform.rotation = attachPoint.transform.rotation;
    }


    #endregion



    #region Base methods

    /// <summary>
    /// Makes the cards of the current page invisible and the ones of the new page visible.
    /// </summary>
    /// <param name="page"></param>
    public void ShowCardsByPage(int page)
    {
        // make all visible issue cards invisible
        for (int i = 0; i < cardsArray[currentPage].Length; i++)
        {
            if (cardsArray[currentPage][i] == null) continue;
            try
            {
                cardsArray[currentPage][i].transform.localScale = invisible;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        }

        currentPage = page;

        // make the issue cards visible.
        for (int i = 0; i < cardsArray[currentPage].Length; i++)
        {
            if (cardsArray[currentPage][i] == null) continue;
            try
            {
                cardsArray[currentPage][i].transform.localScale = visible;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

    }



    /// <summary>
    /// Checks the parameter if valid and calls the method for synchronizing the current page on other clients.
    /// </summary>
    /// <param name="page"></param>
    public void ShowPage(int page)
    {
        if (page < 0) return;
        if (page > pages - 1) return;

        ShowCardsByPage(page);
        SynchronizePage(page);
    }



    /// <summary>
    /// Function when clicking on the up button. Has to be set in the onClick field.
    /// </summary>
    public void OnLeftButtonClick()
    {
        ShowPage(currentPage - 1);
    }



    /// <summary>
    /// Function when clicking on the down button. Has to be set in the onClick field.
    /// </summary>
    public void OnRightButtonClick()
    {
        ShowPage(currentPage + 1);
    }

    #endregion




    #region Add and Remove Card

    /// <summary>
    /// Gets called by BaseCardTrigger and adds a new card to the shelf.
    /// </summary>
    /// <param name="c">Card collider</param>
    /// <param name="triggerIndex">Index of the trigger to determine the index of the card in the cardsArray</param>
    public abstract void AddCard(Collider c, int triggerIndex);



    /// <summary>
    /// Gets called by BaseCardTrigger and removes a card from the shelf.
    /// </summary>
    /// <param name="c">Card collider</param>
    /// <param name="triggerIndex">Index of the trigger to determine the index of the card in the cardsArray</param>
    public virtual void RemoveCard(Collider c, int triggerIndex)
    {
        // when card is already placed and another card enters and exits, it should not remove it.
        if (cardsArray[currentPage][triggerIndex].gameObject != c.gameObject) return;

        cardsArray[currentPage][triggerIndex] = null;
    }



    /// <summary>
    /// Insert card into cardsArray and place it to triggerIndex. Part of AddCard method.
    /// </summary>
    /// <param name="cardCollider"></param>
    /// <param name="triggerIndex"></param>
    protected void InsertCardIntoArray(Collider cardCollider, int triggerIndex)
    {
        // return if card is already in the cardsArray
        if (IsCardInTheCardArray(cardCollider)) return;

        // return if place is already occupied
        if (cardsArray[currentPage][triggerIndex] != null) return;


        // add the issue card to the current page
        cardsArray[currentPage][triggerIndex] = cardCollider.gameObject;


        // move card to the correct attach point
        cardCollider.gameObject.transform.localScale = visible;
        SnapCardToAttachPoint(cardCollider, triggers[triggerIndex].transform);


        // check if page is full, then create another one
        ExtendCardsArrayIfNeeded();
    }



    /// <summary>
    /// Check if card is already somewhere in the cardsArray
    /// </summary>
    /// <param name="cardCollider">Collider of the card</param>
    /// <returns>True if card is in the cardsArray</returns>
    protected bool IsCardInTheCardArray(Collider cardCollider)
    {
        for (int i = 0; i < cardsArray.Length; i++)
        {
            for (int j = 0; j < cardsArray[currentPage].Length; j++)
            {
                if (cardsArray[i][j] == cardCollider.gameObject)
                {
                    return true;
                }
            }
        }

        return false;
    }



    /// <summary>
    /// If the last page is full, extend the cardsArray by one additional page.
    /// </summary>
    public void ExtendCardsArrayIfNeeded()
    {
        bool lastPageFull = true;

        // Return if its not the last page
        if (currentPage != cardsArray.Length - 1) return;

        // Go through each cardsArray place and check if it is empty
        for (int i = 0; i < cardsArray[currentPage].Length; i++)
        {
            if (cardsArray[currentPage][i] == null)
            {
                lastPageFull = false;
                break;
            }
        }

        if (lastPageFull)
        {
            pages += 1;
            GameObject[][] newCardsArray = new GameObject[pages][];
            for (int i = 0; i < cardsArray.Length; i++)
            {
                newCardsArray[i] = cardsArray[i];
            }
            newCardsArray[pages - 1] = new GameObject[triggers.Count];
            cardsArray = newCardsArray;
        }
    }

    #endregion




    #region IssueCard

    public object[] GetObjectArrayOfJiraIssue(JiraIssue jiraIssue)
    {
        return new object[] { jiraIssue.id, jiraIssue.key, jiraIssue.fields.summary, jiraIssue.fields.description, jiraIssue.fields.creator.displayName,
                jiraIssue.fields.issuetype.id, jiraIssue.fields.status.id, jiraIssue.fields.priority.id, jiraIssue.fields.issuetype.name, jiraIssue.fields.status.name, jiraIssue.fields.priority.name };
    }

    #endregion


}
