using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GithubApiCalls : MonoBehaviour
{
    public static GithubApiCalls Instance;

    private string token = "Bearer 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";

    const string baseUrl = "https://api.github.com";




    private void Awake()
    {
        Instance = this;
    }

    public string GetUrl(string endpoint)
    {
        return baseUrl + endpoint;
    }

    public string GetAuthenticationToken()
    {
        return "Bearer " + token;
    }

    public void SetAuthenticationData(string bearerToken)
    {
        token = bearerToken;
    }

    public IEnumerator Authenticate(Action<string> errClb, Action<string> succClb)
    {
        string url = GetUrl(GithubApiEndpoints.AUTHENTICATE);
        yield return GetRequest(url, errClb, succClb);
    }


    public IEnumerator GetProjectList(Action<string> errClb, Action<string> succClb)
    {
        Debug.Log("Requesting projects list");
        string url = GetUrl(GithubApiEndpoints.GET_PROJECT_LIST);
        yield return GetRequest(url, errClb, succClb);
    }


    public IEnumerator GetCommitList(string owner, string projectName, Action<string> errClb, Action<string> succClb)
    {
        string url = GetUrl(String.Format(GithubApiEndpoints.GET_COMMIT_LIST, owner, projectName));
        Debug.Log("Requesting commit list of url: " + url);
        yield return GetRequest(url, errClb, succClb);
    }


    public IEnumerator GetCommit(string owner, string project, string id, Action<string> errClb, Action<string> succClb)
    {
        string url = GetUrl(String.Format(GithubApiEndpoints.GET_COMMIT, owner, project, id));
        Debug.Log("Requesting commit with url: " + url);
        yield return GetRequest(url, errClb, succClb);
    }



    private IEnumerator GetRequest(string url, Action<string> errorCallback, Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            //webRequest.SetRequestHeader("Authorization", GetEncodedAuthentication());
            webRequest.SetRequestHeader("Authorization", GetAuthenticationToken());

            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    errorCallback(webRequest.downloadHandler.text);
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
                    string result = webRequest.downloadHandler.text;
                    callback(result);
                    break;
            }
        }
    }

}
