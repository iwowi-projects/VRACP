using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class JiraCalls: MonoBehaviour
{
    public static JiraCalls Instance;
    public string baseUrl, email, token, encodedAuthentication;



    private void Awake()
    {
        Instance = this;
    }


    public string GetUrl(string endpoint, string requestParams = "")
    {
        return baseUrl + endpoint + requestParams;
    }



    #region Authentication

    public void SetAuthenticationData(string url, string email, string token)
    {
        this.baseUrl = url;
        this.email = email;
        this.token = token;
        encodedAuthentication = Convert.ToBase64String(Encoding.UTF8.GetBytes(email+":"+token));
    }

    public string GetEncodedAuthentication()
    {
        return $"Basic {encodedAuthentication}";
    }

    public IEnumerator GetMyself(Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(JiraApiEndpoints.GET_MYSELF);
        yield return GetRequest(url, errorCallback, callback);
    }

    #endregion



    #region Fetch boards list

    public IEnumerator GetBoardList(Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(JiraApiEndpoints.GET_ALL_BOARDS);
        yield return GetRequest(url, errorCallback, callback);
    }

    #endregion



    #region Board backlog

    public IEnumerator GetBoardBacklog(int boardId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_BACKLOG_FOR_BOARD, boardId));
        Debug.Log("Requesting board backlog with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    #endregion



    #region Sprints

    public IEnumerator GetSprint(int sprintId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_SPRINT, sprintId));
        Debug.Log("Requesting sprint with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    public IEnumerator GetBoardSprints(int boardId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_SPRINTS_FOR_BOARD, boardId));
        Debug.Log("Requesting board sprints with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    public IEnumerator GetActiveSprintsOfBoard(int boardId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_ACTIVE_SPRINTS_FOR_BOARD, boardId));
        Debug.Log("Requesting active sprints of board with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    public IEnumerator GetSprintIssues(int sprintId, string requestParams, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_SPRINT_ISSUES, sprintId), requestParams);
        Debug.Log("Requesting sprint issues with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    public IEnumerator PostSprint(string name, int originBoardId, string startDate, string endDate, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(JiraApiEndpoints.CREATE_SPRINT);
        Debug.Log("Posting new sprint with url: " + url);

        PostJiraSprint jiraSprint = new PostJiraSprint(name, originBoardId, startDate, endDate);

        string body = JsonConvert.SerializeObject(jiraSprint, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});
        Debug.Log("Posting new sprint with body: " + body);

        yield return PostRequest(url, body, errorCallback, callback);
    }

    public IEnumerator PostUpdateSprint(int sprintId, string state, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.UPDATE_SPRINT, sprintId));
        Debug.Log("Updating sprint with url: " + url);

        PutJiraSprint jiraSprint = new PutJiraSprint(state);

        string body = JsonConvert.SerializeObject(jiraSprint, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        yield return PostRequest(url, body, errorCallback, callback);
    }

    public IEnumerator GetBoardClosedSprints(int boardId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_CLOSED_SPRINTS_FOR_BOARD, boardId));
        Debug.Log("Requesting closed board sprints with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    #endregion


    
    #region Board configuration and board issues

    public IEnumerator GetBoardConfiguration(int boardId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_CONFIGURATION_FOR_BOARD, boardId));
        Debug.Log("Requesting board configuration for url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    public IEnumerator GetBoardFeatures(int boardId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_FEATURES_FOR_BOARD, boardId));
        Debug.Log("Requesting board features with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    public IEnumerator GetBoardIssues(int boardId, string requestParams, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_ISSUES_FOR_BOARD, boardId), requestParams);
        Debug.Log("Requesting board issues for url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    #endregion



    #region Issue

    public IEnumerator GetIssue(string issueId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_ISSUE, issueId));
        Debug.Log("Getting issue with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    public IEnumerator PostIssue(string title, string description, string projectId, string typeId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(JiraApiEndpoints.CREATE_ISSUE);
        Debug.Log("Posting new issue with url: " + url);

        PostJiraIssue jiraIssue = new PostJiraIssue(title, description, projectId, typeId);

        string body = JsonConvert.SerializeObject(jiraIssue);
        Debug.Log("Posting new issue with body: " + body);

        yield return PostRequest(url, body, errorCallback, callback);
    }

    public IEnumerator PutIssue(string issueId, string title, string description, string typeId, string priorityId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.EDIT_ISSUE, issueId));
        Debug.Log("Editing issue with url: " + url);

        PutJiraIssue putJiraIssue = new PutJiraIssue(title, description, typeId, priorityId);

        string body = JsonConvert.SerializeObject(putJiraIssue, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        Debug.Log("Editing issue with body: " + body);

        yield return PutRequest(url, body, errorCallback, callback);
    }

    public IEnumerator GetEditMeta(string issueId, Action<string> errorCallback, Action<string> callback)
    {
        string url = JiraCalls.Instance.GetUrl(String.Format(JiraApiEndpoints.GET_ISSUE_EDITMETA, issueId));
        Debug.Log("Requesting edit meta with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    public IEnumerator GetTransitions(string issueId, Action<string> errorCallback, Action<string> callback)
    {
        string url = JiraCalls.Instance.GetUrl(String.Format(JiraApiEndpoints.GET_ISSUE_TRANSITIONS, issueId));
        Debug.Log("Requesting issue transitions with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }  

    public IEnumerator PostIssueTransition(string issueId, string statusId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.POST_ISSUE_TRANSITION, issueId));
        Debug.Log("Post issue status with url: " + url);

        PostTransition status = new PostTransition(statusId);

        string body = JsonConvert.SerializeObject(status, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        Debug.Log("Post issue status with body: " + body);

        yield return PostRequest(url, body, errorCallback, callback);
    }

    public IEnumerator MoveIssuesToBacklog(List<string> issueIds, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(JiraApiEndpoints.MOVE_ISSUE_TO_BACKLOG);
        Debug.Log("Move issue to backlog with url: " + url);

        PostJiraIssueToBacklog postJiraIssueToBacklog = new PostJiraIssueToBacklog(issueIds);

        string body = JsonConvert.SerializeObject(postJiraIssueToBacklog, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        Debug.Log("Move issue to backlog with body: " + body);

        yield return PostRequest(url, body, errorCallback, callback);
    }

    public IEnumerator MoveIssuesToSprint(List<string> issueIds, int sprintId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.MOVE_ISSUE_TO_SPRINT, sprintId));
        Debug.Log("Move issue to sprint with url: " + url);

        PostJiraIssueMoveToSprint postJiraIssueMoveToSprint = new PostJiraIssueMoveToSprint(issueIds);

        string body = JsonConvert.SerializeObject(postJiraIssueMoveToSprint, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        Debug.Log("Move issue to sprint with body: " + body);

        yield return PostRequest(url, body, errorCallback, callback);
    }

    public IEnumerator MoveIssuesToBoard(List<string> issueIds, int boardId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.MOVE_ISSUE_TO_BACKLOG, boardId));
        Debug.Log("Move issue to board with url: " + url);

        PostJiraIssueToBacklog postJiraIssueToBacklog = new PostJiraIssueToBacklog(issueIds);

        string body = JsonConvert.SerializeObject(postJiraIssueToBacklog, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        Debug.Log("Move issue to board with body: " + body);

        yield return PostRequest(url, body, errorCallback, callback);
    }

    #endregion



    #region Projects

    public IEnumerator GetProjects(Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(JiraApiEndpoints.GET_ALL_PROJECTS);
        Debug.Log("Requesting projects with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }

    public IEnumerator GetProjectsForBoard(int boardId, Action<string> errorCallback, Action<string> callback)
    {
        string url = GetUrl(String.Format(JiraApiEndpoints.GET_PROJECTS_FOR_BOARD, boardId));
        Debug.Log("Requesting projects of board with url: " + url);
        yield return GetRequest(url, errorCallback, callback);
    }
    #endregion



    #region Base Requests

    public IEnumerator GetRequest(string url, Action<string> errorCallback,Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", GetEncodedAuthentication());
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    errorCallback(webRequest.error);
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(webRequest.error);
                    errorCallback(webRequest.downloadHandler.text);
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(webRequest.error);
                    Debug.LogError(webRequest.downloadHandler.text);
                    errorCallback(webRequest.downloadHandler.text);
                    break;

                case UnityWebRequest.Result.Success:
                    callback(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public IEnumerator PostRequest(string url, string body, Action<string> errorCallback, Action<string> callback)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, body))
        {
            webRequest.SetRequestHeader("Authorization", GetEncodedAuthentication());
            webRequest.SetRequestHeader("Content-Type", "application/json");

            byte[] data = Encoding.UTF8.GetBytes(body);
            webRequest.uploadHandler = new UploadHandlerRaw(data);
            
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    errorCallback(webRequest.error);
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(webRequest.error);
                    errorCallback(webRequest.downloadHandler.text);
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(webRequest.error);
                    Debug.LogError(webRequest.downloadHandler.text);
                    errorCallback(webRequest.downloadHandler.text);
                    break;

                case UnityWebRequest.Result.Success:
                    callback(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public IEnumerator PutRequest(string url, string body, Action<string> errorCallback, Action<string> callback)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, body))
        {
            webRequest.SetRequestHeader("Authorization", GetEncodedAuthentication());
            webRequest.SetRequestHeader("Content-Type", "application/json");

            byte[] data = Encoding.UTF8.GetBytes(body);
            webRequest.uploadHandler = new UploadHandlerRaw(data);

            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    errorCallback(webRequest.error);
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    errorCallback(webRequest.error);
                    Debug.LogError(webRequest.downloadHandler.text);
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(webRequest.error);
                    Debug.LogError(webRequest.downloadHandler.text);
                    errorCallback(webRequest.downloadHandler.text);
                    break;

                case UnityWebRequest.Result.Success:
                    callback(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    #endregion



    /*
IEnumerator getRequest(string uri)
{
    UnityWebRequest uwr = UnityWebRequest.Get(uri);
    uwr.SetRequestHeader("Authorization", "Basic aXdpdGFsaW5za2k6JURaNjZhRjN2U3Vm");
    uwr.SetRequestHeader("content-Type", "application/json");
    yield return uwr.SendWebRequest();

    if (uwr.isNetworkError)
    {
        Debug.Log("Error While Sending: " + uwr.error);
    }
    else
    {
        //Debug.Log(uwr.GetResponseHeader("Set-Cookie"));
        Debug.Log("Received: " + uwr.downloadHandler.text);
        splitted = uwr.GetResponseHeader("Set-Cookie").Split(';');


        Debug.Log("Second one");
        UnityWebRequest uwr2 = UnityWebRequest.Get(uri);
        //uwr.SetRequestHeader("Authorization", "Basic aXdpdGFsaW5za2k6JURaNjZhRjN2U3Vm");
        uwr2.SetRequestHeader("content-Type", "application/json");
        foreach (string s in splitted)
        {
            Debug.Log(s);
        }
        Debug.Log(splitted[0].Split('=')[1]);
        //uwr2.SetRequestHeader("cookie", splitted[0].Split('=')[1]);
        yield return uwr2.SendWebRequest();

        if (uwr2.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr2.error);
        }
        else
        {
            Debug.Log("Auth: " + uwr2.GetRequestHeader("cookie"));
            //Debug.Log(uwr.GetResponseHeader("Set-Cookie"));
            Debug.Log("Received: " + uwr2.downloadHandler.text);
        }
    }
}*/
}
