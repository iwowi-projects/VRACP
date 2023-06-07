using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class CreateIssueCard : BaseCreateCard
{
    public TMP_InputField titleInputfield;
    public TMP_InputField descriptionInputfield;
    public TMP_Dropdown projectsDropdown;
    public TMP_Dropdown typesDropdown;

    public List<JiraProject> projects;


    private void Start()
    {
        projectsDropdown.onValueChanged.AddListener(OnProjectDropdownValueChange);
    }


    public void Setup()
    {
        JiraController.Instance.GetProjects(err => ApiFeedback.Instance.DisplayFeedback(err), SetProjects);
    }

    public override void Clear()
    {
        projectsDropdown.ClearOptions();
        typesDropdown.ClearOptions();
        titleInputfield.text = "";
        descriptionInputfield.text = "";
    }



    /// <summary>
    /// Sets projects into the projects dropdown and synchronizes the options.
    /// </summary>
    /// <param name="projects"></param>
    private void SetProjects(List<JiraProject> projects)
    {
        projectsDropdown.ClearOptions();
        this.projects = projects;
        List<string> projectsAsStringList = new List<string>();

        projects.ForEach(project =>
        {
            projectsAsStringList.Add(project.id + " " + project.name);
        });

        projectsDropdown.AddOptions(projectsAsStringList);
        projectsDropdown.GetComponent<DropdownSynchronizer>().SynchronizeDropdownOptionsEvent();

        int boardId = (int) PhotonNetwork.CurrentRoom.CustomProperties["CurrentBoardId"];
        JiraController.Instance.GetProjectsForBoard(boardId, err => ApiFeedback.Instance.DisplayFeedback(err), jiraProjectList =>
        {
            int index = projects.FindIndex(project => project.id == jiraProjectList[0].id);
            projectsDropdown.value = index;
        });

        OnProjectDropdownValueChange(projectsDropdown.value);
    }



    /// <summary>
    /// When project from the projects dropdown changes, update the types dropdown as issue types are project-dependant.
    /// </summary>
    /// <param name="projectIndex">Index of project in projects dropdown</param>
    private void OnProjectDropdownValueChange(int projectIndex)
    {
        typesDropdown.ClearOptions();
        List<string> typesAsStringList = new List<string>();
        JiraProject project = projects.Find(p => p.id == projectsDropdown.options[projectIndex].text.Split(" ")[0]);
        project.issueTypes.ForEach(type =>
        {
            typesAsStringList.Add(type.id + " " + type.name);
        });
        typesDropdown.AddOptions(typesAsStringList);
        typesDropdown.GetComponent<DropdownSynchronizer>().SynchronizeDropdownOptionsEvent();
    }



    public override void OnSaveClick()
    {
        string title = titleInputfield.text;
        string description = descriptionInputfield.text;
        string projectId = projectsDropdown.options[projectsDropdown.value].text.Split(" ")[0];
        string typeId = typesDropdown.options[typesDropdown.value].text.Split(" ")[0];
        //string typeId = projects.Find(p => p.id == projectId).issueTypes.Find(t => t.name == typesDropdown.options[typesDropdown.value].text).id;

        if (title == null || title.Length == 0)
        {
            ApiFeedback.Instance.DisplayFeedback("Issue title is missing.");
            return;
        }

        // Create issue and instantiate new issue card when creation was successfull.
        JiraController.Instance.CreateIssue(title, description, projectId, typeId, err => ApiFeedback.Instance.DisplayFeedback(err), response => {
            JiraController.Instance.GetIssue(response.id, err => ApiFeedback.Instance.DisplayFeedback(err), InstantiateIssueCard);
        });

        SynchronizeClose();
    }



    /// <summary>
    /// Instantiate issue card via Photon at spawnPoint.
    /// </summary>
    /// <param name="jiraIssue">Newly created jira issue</param>
    private void InstantiateIssueCard(JiraIssue jiraIssue)
    {
        object[] customInstantiationData = { jiraIssue.id, jiraIssue.key, jiraIssue.fields.summary, jiraIssue.fields.description, jiraIssue.fields.creator.displayName,
                jiraIssue.fields.issuetype.id, jiraIssue.fields.status.id, jiraIssue.fields.priority.id, jiraIssue.fields.issuetype.name, jiraIssue.fields.status.name, jiraIssue.fields.priority.name };
        string issueCardPrefabPath = Path.Combine("PhotonPrefabs", "IssueCard");
        GameObject issueCard = PhotonNetwork.Instantiate(issueCardPrefabPath, spawnPoint.position, spawnPoint.localRotation, 0, customInstantiationData);
    }


}
