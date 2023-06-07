using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class JiraController : MonoBehaviour
{
    public static JiraController Instance;

    public List<JiraBoard> boards;
    public int currentBoardId;



    private void Awake()
    {
        Instance = this;
    }



    #region Authentication

    public void Authenticate(string url, string username, string password, Action<string> errorCallback, Action<JiraCreator> callback)
    {
        JiraCalls.Instance.SetAuthenticationData(url, username, password);
        StartCoroutine(JiraCalls.Instance.GetMyself(errorCallback, result =>
        {
            JiraCreator jiraCreator = JsonConvert.DeserializeObject<JiraCreator>(result);
            callback(jiraCreator);
        }));
    }

    #endregion




    #region Boards
    
    public void GetBoards(Action<string> errorCallback, Action<List<JiraBoard>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetBoardList(errorCallback, result =>
        {
            JiraBoardsList jiraBoardsList = JsonConvert.DeserializeObject<JiraBoardsList>(result);
            boards = jiraBoardsList.values;
            callback(jiraBoardsList.values);
        }));
    }

    #endregion




    #region Board

    public void GetBoardIssues(int boardId, string requestParams, Action<string> errorCallback, Action<List<JiraIssue>> callback)
    {
        JiraBoard jiraBoard = boards.Find(board => board.id == boardId);
        switch (jiraBoard.type)
        {
            case "scrum":
                GetScrumBoardIssues(boardId, requestParams, errorCallback, callback);
                break;
            case "kanban":
                GetKanbanBoardIssues(boardId, requestParams, errorCallback, callback);
                break;
            default:
                GetIndividualBoardIssues(boardId, requestParams, errorCallback, callback);
                break;
        }
    }

    private void GetIndividualBoardIssues(int boardId, string requestParams, Action<string> errorCallback, Action<List<JiraIssue>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetBoardFeatures(boardId, errorCallback, result =>
        {
            JiraBoardFeatures jiraBoardFeatures = JsonConvert.DeserializeObject<JiraBoardFeatures>(result);
            if (jiraBoardFeatures.HasSprints())
            {
                GetScrumBoardIssues(boardId, requestParams, errorCallback, callback);
            } else
            {
                GetKanbanBoardIssues(boardId, requestParams, errorCallback, callback);
            }
        }));
    }

    public void GetKanbanBoardIssues(int boardId, string requestParams, Action<string> errorCallback, Action<List<JiraIssue>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetBoardBacklog(boardId, err =>
        {
            StartCoroutine(JiraCalls.Instance.GetBoardIssues(boardId, requestParams, errorCallback, result =>
            {
                JiraBoardIssues jiraBoardIssues = JsonConvert.DeserializeObject<JiraBoardIssues>(result);
                callback(jiraBoardIssues.issues);
            }));
        }, 
        result => {
            List<JiraIssue> jiraBacklogIssues = JsonConvert.DeserializeObject<JiraBoardIssues>(result).issues;
            StartCoroutine(JiraCalls.Instance.GetBoardIssues(boardId, requestParams, errorCallback, result =>
            {
                List<JiraIssue> jiraBoardIssues = JsonConvert.DeserializeObject<JiraBoardIssues>(result).issues;

                List<JiraIssue> jiraBoardIssuesWithoutBacklog = jiraBoardIssues.Except(jiraBacklogIssues).ToList();

                callback(jiraBoardIssuesWithoutBacklog);
            }));
        }));
        
    }

    public void GetScrumBoardIssues(int boardId, string requestParams, Action<string> errorCallback, Action<List<JiraIssue>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetActiveSprintsOfBoard(boardId, errorCallback, res => {
            JiraSprintList jiraSprintList = JsonConvert.DeserializeObject<JiraSprintList>(res);
            if (jiraSprintList.values.Count == 0) return;
            int jiraSprintId = jiraSprintList.values[0].id;
            GetSprintIssues(jiraSprintId, requestParams, errorCallback, callback);
        }));
    }

    public void GetBoardBacklog(int boardId, Action<string> errorCallback, Action<List<JiraIssue>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetBoardBacklog(boardId, errorCallback, result => {
            currentBoardId = boardId;
            JiraBoardIssues jiraBoardIssues = JsonConvert.DeserializeObject<JiraBoardIssues>(result);
            callback(jiraBoardIssues.issues);
        }));
    }

    public void GetBoardConfiguration(int boardId, Action<string> errorCallback, Action<List<JiraBoardColumn>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetBoardConfiguration(boardId, errorCallback, result =>
        {
            JiraBoardConfiguration jiraBoardConfiguration = JsonConvert.DeserializeObject<JiraBoardConfiguration>(result);
            callback(jiraBoardConfiguration.columnConfig.columns);
        }));
    }

    public void GetBoardFeatures(int boardId, Action<string> errorCallback, Action<JiraBoardFeatures> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetBoardFeatures(boardId, errorCallback, result =>
        {
            JiraBoardFeatures jiraBoardFeatures = JsonConvert.DeserializeObject<JiraBoardFeatures>(result);
            callback(jiraBoardFeatures);
        }));
    }

    #endregion




    #region Sprint

    public void GetBoardSprints(int boardId, Action<string> errorCallback, Action<List<JiraSprint>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetBoardSprints(boardId, errorCallback, result =>
        {
            JiraSprintList jiraSprintList = JsonConvert.DeserializeObject<JiraSprintList>(result);
            callback(jiraSprintList.values);
        }));
    }

    public void GetBoardActiveSprint(int boardId, Action<string> errorCallback, Action<JiraSprint> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetActiveSprintsOfBoard(boardId, errorCallback, res => {
            JiraSprintList jiraSprintList = JsonConvert.DeserializeObject<JiraSprintList>(res);
            if (jiraSprintList.values.Count > 0)
            {
                callback(jiraSprintList.values[0]);
            } else
            {
                errorCallback("No active sprint.");
            }
            
        }));
    }

    public void GetSprintIssues(int sprintId, string requestParams, Action<string> errorCallback, Action<List<JiraIssue>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetSprintIssues(sprintId, requestParams ,errorCallback, result =>
        {
            JiraSprintIssues jiraSprintIssues = JsonConvert.DeserializeObject<JiraSprintIssues>(result);
            callback(jiraSprintIssues.issues);
        }));
    }

    public void GetSprint(int sprintId, Action<string> errorCallback, Action<JiraSprint> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetSprint(sprintId, errorCallback, result =>
        {
            JiraSprint jiraSprint = JsonConvert.DeserializeObject<JiraSprint>(result);
            callback(jiraSprint);
        }));
    }

    public void CreateSprint(string name, int originBoardId, string startDate, string endDate, Action<string> errorCallback, Action<PostJiraSprintResponse> callback)
    {
        StartCoroutine(JiraCalls.Instance.PostSprint(name, originBoardId, startDate, endDate, errorCallback, result =>
        {
            PostJiraSprintResponse postJiraSprintResponse = JsonConvert.DeserializeObject<PostJiraSprintResponse>(result);
            callback(postJiraSprintResponse);
        }));
    }

    public void UpdateSprint(int sprintId, string state, Action<string> errorCallback, Action<PostJiraSprintResponse> callback)
    {
        StartCoroutine(JiraCalls.Instance.PostUpdateSprint(sprintId, state, errorCallback, result =>
        {
            PostJiraSprintResponse postJiraSprintResponse = JsonConvert.DeserializeObject<PostJiraSprintResponse>(result);
            callback(postJiraSprintResponse);
        }));
    }

    #endregion




    #region Issue
    public void GetIssue(string issueId, Action<string> errorCallback, Action<JiraIssue> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetIssue(issueId, errorCallback, result =>
        {
            JiraIssue jiraIssue= JsonConvert.DeserializeObject<JiraIssue>(result);
            callback(jiraIssue);
        }));
    }

    public void CreateIssue(string title, string description, string projectId, string typeId, Action<string> errorCallback, Action<PostJiraIssueResponse> callback)
    {
        StartCoroutine(JiraCalls.Instance.PostIssue(title, description, projectId, typeId, errorCallback, result =>
        {
            PostJiraIssueResponse response = JsonConvert.DeserializeObject<PostJiraIssueResponse>(result);
            callback(response);
        }));
    }

    public void GetIssueEditMeta(string issueId, Action<string> errorCallback, Action<JiraIssueEditMeta> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetEditMeta(issueId, errorCallback, result =>
        {
            FieldsWrapper<JiraIssueEditMeta> fieldsWrapper = JsonConvert.DeserializeObject<FieldsWrapper<JiraIssueEditMeta>>(result);
            callback(fieldsWrapper.fields);
        }));
    }

    public void GetIssueTransitions(string issueId, Action<string> errorCallback, Action<List<JiraIssueTransition>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetTransitions(issueId, errorCallback, result =>
        {
            TransitionsWrapper<List<JiraIssueTransition>> transitionsWrapper = JsonConvert.DeserializeObject<TransitionsWrapper<List<JiraIssueTransition>>>(result);
            callback(transitionsWrapper.transitions);
        }));
    }

    public void EditIssue(string issueId, string title, string description, string typeId, string priorityId, string statusId, Action<string> errorCallback, Action<bool> callback)
    {
        StartCoroutine(JiraCalls.Instance.PutIssue(issueId, title, description, typeId, priorityId, errorCallback, result =>
        {
            Debug.Log("Issue successfully edited");
            StartCoroutine(JiraCalls.Instance.PostIssueTransition(issueId, statusId, errorCallback, result =>
            {
                Debug.Log("Issue status successfully posted");
                callback(true);
            }));
        }));

        
    }

    public void DoIssueTransitation(string issueId, string transitionId, Action<string> errorCallback, Action<string> callback)
    {
        StartCoroutine(JiraCalls.Instance.PostIssueTransition(issueId, transitionId, errorCallback, result =>
        {
            callback(result);
        }));
    }

    public void MoveIssuesToBacklog(List<string> issueIds, Action<string> errorCallback, Action<bool> callback)
    {
        StartCoroutine(JiraCalls.Instance.MoveIssuesToBacklog(issueIds, errorCallback, result =>
        {
            callback(true);
        }));
    }

    public void MoveIssuesToSprint(List<string> issueIds, int sprintId, Action<string> errorCallback, Action<bool> callback)
    {
        StartCoroutine(JiraCalls.Instance.MoveIssuesToSprint(issueIds, sprintId, errorCallback, result =>
        {
            callback(true);
        }));
    }

    public void MoveIssuesToBoard(List<string> issueIds, int boardId, Action<string> errorCallback, Action<bool> callback)
    {
        StartCoroutine(JiraCalls.Instance.MoveIssuesToBoard(issueIds, boardId, errorCallback, result =>
        {
            callback(true);
        }));
    }

    #endregion




    #region Projects

    public void GetProjects(Action<string> errorCallback, Action<List<JiraProject>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetProjects(errorCallback, result =>
        {
            List<JiraProject> jiraProjects = JsonConvert.DeserializeObject<List<JiraProject>>(result);
            callback(jiraProjects);
        }));
    }

    public void GetProjectsForBoard(int boardId, Action<string> errorCallback, Action<List<JiraProject>> callback)
    {
        StartCoroutine(JiraCalls.Instance.GetProjectsForBoard(boardId, errorCallback, result =>
        {
            ValuesWrapper<List<JiraProject>> valuesWrapper = JsonConvert.DeserializeObject<ValuesWrapper<List<JiraProject>>>(result);
            callback(valuesWrapper.values);
        }));
    }

    #endregion




    #region Graveyard

    /*
    public void ProcessBoardIssues(string result)
    {
        JiraBoardIssues jiraBoardIssues = JsonConvert.DeserializeObject<JiraBoardIssues>(result);
        cardsGenerator.GenerateIssueCards(jiraBoardIssues.issues);
    }

    private void SetBoardIssues(string result)
    {
        JiraBoardIssues jiraBoardIssues = JsonConvert.DeserializeObject<JiraBoardIssues>(result);
        cardsGenerator.GenerateIssueCards(jiraBoardIssues.issues);
    }

    public void ProcessBoardConfiguration(string result)
    {
        JiraBoardConfiguration jiraBoardConfiguration = JsonConvert.DeserializeObject<JiraBoardConfiguration>(result);
        cardsGenerator.GenerateColumns(jiraBoardConfiguration.columnConfig.columns);
    }

    public void GetBoardConfiguration(int boardId)
    {
        StartCoroutine(JiraCalls.Instance.RequestBoardConfiguration(boardId, ProcessBoardConfiguration));
    }

    public void GetBoardFeatures(int boardId)
    {
        StartCoroutine(JiraCalls.Instance.RequestBoardFeatures(boardId, ProcessBoardFeatures));
    }*/

    /*
public void RefreshBacklog()
{
    GetBoardBacklog(currentBoardId);
}*/

    /*
    public void GetFutureSprints(int boardId)
    {
        StartCoroutine(JiraCalls.Instance.RequestBoardSprints(boardId, ProcessFutureSprints));
    }*/

    /*
public void RefreshFutureSprints()
{
    GetFutureSprints(currentBoardId);
}*/

    /*
    public void GetSprintIssues(int boardId, int sprintId)
    {
        StartCoroutine(JiraCalls.Instance.RequestSprintIssues(sprintId, ProcessSprint));
    }*/

    /*
    public void ProcessFutureSprints(string result)
    {
        JiraSprintList jiraSprintList = JsonConvert.DeserializeObject<JiraSprintList>(result);
        cardsGenerator.GenerateFutureSprintList(jiraSprintList.values);
    }

    public void GetClosedSprints(int boardId)
    {
        StartCoroutine(JiraCalls.Instance.RequestBoardClosedSprints(boardId, ProcessClosedSprints));
    }

    public void ProcessClosedSprints(string result)
    {
        JiraSprintList jiraSprintList = JsonConvert.DeserializeObject<JiraSprintList>(result);
        cardsGenerator.GenerateClosedSprintList(jiraSprintList.values);
    }*/

    /*
    public void ProcessMyself(string result)
    {
        JiraCreator jiraCreator = JsonConvert.DeserializeObject<JiraCreator>(result);
        if (jiraCreator != null)
        {
            isAuthenticated = true;
            GetBoards();
        } else
        {
            isAuthenticated = false;
        }
        cardsGenerator.GenerateAuthenticationMessage(jiraCreator);
    }*/

    /*
    public void GetMyself()
    {
        StartCoroutine(JiraCalls.Instance.RequestMyself(ProcessMyself));
    }*/

    /*
     * Sets currentBoardId and starts coroutine for requestBoard based on its type.
     * Gets called by event on all clients. If client is not yet authenticated, it returns. If he hasn't fetched the boards list yet, it returns.
     */

    /*
    public void GetBoard(int boardId)
    {
        if (!isAuthenticated)
        {
            return;
        }

        JiraBoard jiraBoard = UpdateBoardIdAndName(boardId);

        // return if board list not fetched yet.
        if (jiraBoard == null) return;

        Debug.Log("Setting current board id " + boardId);
        currentBoardId = boardId;
        cardsGenerator.UpdateBoardsDropdown(boardId, jiraBoard.name);

        switch (jiraBoard.type)
        {
            case "scrum":
                //StartCoroutine(RequestScrumBoard(boardId));
                break;
            case "kanban":
                StartCoroutine(RequestKanbanBoard(boardId));
                break;
            default:
                //StartCoroutine(RequestIndividualBoard(boardId));
                break;
        }
    }*/

    /*
    public IEnumerator RequestScrumBoard(int boardId, Action<List<JiraIssue>> callback)
    {
        
        Debug.Log("RequestScrumBoard");
        cardsGenerator.ToggleBacklogArea(true);
        cardsGenerator.ToggleSprintsArea(true);
        cardsGenerator.ToggleSprintReviewArea(true);

        GetBoardBacklog(boardId);
        GetFutureSprints(boardId);
        GetClosedSprints(boardId);
        
        // board issues can only be correctly instantiated when board columns were created -> wait until finished.
        //yield return JiraCalls.Instance.RequestBoardConfiguration(boardId, ProcessBoardConfiguration);
        
        GetScrumBoardIssues(boardId, callback);
    }*/

    /*
    public IEnumerator RequestKanbanBoard(int boardId)
    {
        Debug.Log("RequestKanbanBoard");
        cardsGenerator.ToggleSprintsArea(false);
        cardsGenerator.ToggleBacklogArea(false);
        cardsGenerator.ToggleSprintReviewArea(false);

        yield return JiraCalls.Instance.RequestBoardConfiguration(boardId, ProcessBoardConfiguration);
        GetBoardIssues(boardId);
    }*/

    /*
    public IEnumerator RequestKanbanBoardWithBacklog(int boardId)
    {
        Debug.Log("RequestKanbanBoardWithBacklog");
        cardsGenerator.ToggleSprintsArea(false);
        cardsGenerator.ToggleBacklogArea(true);
        cardsGenerator.ToggleSprintReviewArea(false);

        GetBoardBacklog(boardId);
        yield return JiraCalls.Instance.RequestBoardConfiguration(boardId, ProcessBoardConfiguration);
        GetBoardIssues(boardId);
    }*/

    /*
    public JiraBoard UpdateBoardIdAndName(int boardId)
    {
        JiraBoard jiraBoard = boards.Find(board => board.id == boardId);
        if (jiraBoard == null)
        {
            return null;
        }

        currentBoardId = boardId;
        cardsGenerator.SetBoardName(jiraBoard.name);
        return jiraBoard;
    }*/

    /*
    public void RefreshBoardIssues()
    {
        JiraBoard jiraBoard = boards.Find(board => board.id == currentBoardId);
        switch (jiraBoard.type)
        {
            case "scrum":
                GetScrumBoardIssues(currentBoardId);
                break;
            case "kanban":
                GetBoardIssues(currentBoardId);
                break;
            default:
                StartCoroutine(RequestIndividualBoard(currentBoardId));
                break;
        }
    }
    */

    /*
public void SetAuthenticationData()
{
    baseUrl = baseUrlInputfield.text;
    JiraCalls.Instance.SetAuthenticationData(baseUrlInputfield.text, usernameInputfield.text, passwordInputfield.text);
    GetMyself();
}*/

    /*
    public void GetBoardBacklog(int boardId)
    {
        StartCoroutine(JiraCalls.Instance.RequestBoardBacklog(boardId, ProcessBacklogIssues));
    }

    public void ProcessBacklogIssues(string result)
    {
        Debug.Log("ProcessBoardBacklog");
        JiraBoardIssues jiraBoardIssues = JsonConvert.DeserializeObject<JiraBoardIssues>(result);
        cardsGenerator.GenerateBacklogIssueCards(jiraBoardIssues.issues);
    }*/

    /*
public void ProcessSprint(string result)
{
    JiraBoardIssues jiraBoardIssues = JsonConvert.DeserializeObject<JiraBoardIssues>(result);
    cardsGenerator.GenerateFutureSprintIssues(jiraBoardIssues.issues);
}*/

    /*
    public void GetClosedSprintIssues(int boardId, int sprintId)
    {
        StartCoroutine(JiraCalls.Instance.RequestSprintIssues(sprintId, ProcessClosedSprint));
    }*/

    /*
    public void ProcessClosedSprint(string result)
    {
        JiraBoardIssues jiraBoardIssues = JsonConvert.DeserializeObject<JiraBoardIssues>(result);
        cardsGenerator.GenerateClosedSprintIssues(jiraBoardIssues.issues);
    }*/

    #endregion

}
