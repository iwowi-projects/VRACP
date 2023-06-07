using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MenuGithubConfiguration : MonoBehaviour
{
    public TMP_InputField tokenInputfield;
    public TMP_Text feedbackTextfield;
    public TMP_Dropdown projectsDropdown;
    public TMP_Dropdown commitsDropdown;
    public TMP_Dropdown filesDropdown;

    public string owner;
    public string project;

    public List<GithubFile> commitFiles;
    public List<GithubCommitListItem> commits;



    public void OnAuthenticateButtonClick()
    {
        string bearerToken = tokenInputfield.text;

        GithubApiController.Instance.Authenticate(bearerToken, err => SetFeedbackText("Authentication failed"),
            result => {
                string successMessage = "Successfully authenticated as {0}";
                SetFeedbackText(string.Format(successMessage, result.login));
                owner = result.login;
                ManageProjectsDropdown();
            }
        );
    }

    public void SetFeedbackText(string text)
    {
        feedbackTextfield.text = text;
    }

    public void ManageProjectsDropdown()
    {
        GithubApiController.Instance.GetProjects(err => SetFeedbackText("Authentication failed"), result =>
        {
            List<string> repos = new List<string>();
            result.ForEach(project =>
            {
                repos.Add(project.name);
            });
            projectsDropdown.ClearOptions();
            projectsDropdown.AddOptions(repos);
            projectsDropdown.onValueChanged.AddListener(ManageCommitsDropdown);
            ManageCommitsDropdown(0);
        });
    }

    public void ManageCommitsDropdown(int projectsDropdownValue)
    {
        string projectName = projectsDropdown.options[projectsDropdownValue].text;
        this.project = projectName;

        GithubApiController.Instance.GetCommits(owner, projectName, err => ApiFeedback.Instance.DisplayFeedback(err), result =>
        {
            List<string> commitMessages = new List<string>();
            commits = result;
            result.ForEach(commit =>
            {
                commitMessages.Add(commit.commit.message);
            });
            commitsDropdown.ClearOptions();
            commitsDropdown.AddOptions(commitMessages);
            commitsDropdown.onValueChanged.AddListener(ManageFilesDropdown);
            ManageFilesDropdown(0);
        });
    }


    public void ManageFilesDropdown(int commitsDropdownValue)
    {
        string sha = commits[commitsDropdownValue].sha;

        GithubApiController.Instance.GetCommitFiles(owner, project, sha, err => ApiFeedback.Instance.DisplayFeedback(err), result =>
        {
            List<string> filesOptions = new List<string>();
            commitFiles = new List<GithubFile>(result);
            commitFiles.ForEach(file =>
            {
                filesOptions.Add(file.filename);
            });
            filesDropdown.ClearOptions();
            filesDropdown.AddOptions(filesOptions);
        });
    }

    public void OnShowFileButtonClick()
    {
        if (filesDropdown.options.Count == 0)
        {
            ApiFeedback.Instance.DisplayFeedback("You must authenticate before.");
            return;
        }

        // Get file from dropdown
        string fileName = filesDropdown.options[filesDropdown.value].text;
        string filePatch = commitFiles[filesDropdown.value].patch;

        Hashtable content = new Hashtable();

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.RemoveFromRoomCache };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeGitHubFile, content, raiseEventOptions, SendOptions.SendReliable);

        content.Add("FileName", fileName);
        content.Add("FilePatch", filePatch);
        raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCacheGlobal };
        PhotonNetwork.RaiseEvent(EventCodes.SynchronizeGitHubFile, content, raiseEventOptions, SendOptions.SendReliable);

    }


}
