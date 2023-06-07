using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GithubApiController : MonoBehaviour
{
    public static GithubApiController Instance;


    private void Awake()
    {
        Instance = this;
    }

    public void Authenticate(string bearerToken, Action<string> errorCallback, Action<GithubAuthenticationResponse> callback)
    {
        GithubApiCalls.Instance.SetAuthenticationData(bearerToken);
        StartCoroutine(GithubApiCalls.Instance.Authenticate(errorCallback, result =>
        {
            GithubAuthenticationResponse githubAuthenticationResponse = JsonConvert.DeserializeObject<GithubAuthenticationResponse>(result);
            callback(githubAuthenticationResponse);
        }));
    }


    public void GetProjects(Action<string> errClb, Action<List<GithubProjectListItem>> succClb)
    {
        StartCoroutine(GithubApiCalls.Instance.GetProjectList(errClb, result => {
            List<GithubProjectListItem> githubProjectListItems = JsonConvert.DeserializeObject<List<GithubProjectListItem>>(result);
            succClb(githubProjectListItems);
        }));
    }

    public void GetCommits(string owner, string projectName, Action<string> errClb, Action<List<GithubCommitListItem>> succClb)
    {
        StartCoroutine(GithubApiCalls.Instance.GetCommitList(owner, projectName, errClb , result =>
        {
            //textfield.text = webRequest.downloadHandler.text.Replace("\\n", "\n");
            List<GithubCommitListItem> githubCommitListItems = JsonConvert.DeserializeObject<List<GithubCommitListItem>>(result);
            //GenerateCommitList(githubCommitListItems);
            succClb(githubCommitListItems);
        }));
    }

    public void GetCommitFiles(string owner, string project, string sha, Action<string> errClb, Action<GithubFile[]> succClb)
    {
        StartCoroutine(GithubApiCalls.Instance.GetCommit(owner, project, sha, errClb, result =>
        {
            //textfield.text = webRequest.downloadHandler.text.Replace("\\n", "\n");
            GithubCommit githubCommit = JsonConvert.DeserializeObject<GithubCommit>(result);
            succClb(githubCommit.files);
        }));

    }
}
