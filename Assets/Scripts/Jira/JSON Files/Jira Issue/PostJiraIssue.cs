using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/*
 * Class for creating jira issue.
 */
public class PostJiraIssue
{
    public PostJiraIssueInner fields;

    public PostJiraIssue(string summary, string description, string projectId, string issueTypeId)
    {
        fields = new PostJiraIssueInner(summary, description, projectId, issueTypeId);
    }

    public class PostJiraIssueInner
    {
        public PostJiraIssueProject project;
        public PostJiraIssueType issuetype;
        public string summary;
        public string description;

        [System.Serializable]
        public class PostJiraIssueProject
        {
            public string id;
            public PostJiraIssueProject(string id)
            {
                this.id = id;
            }
        }

        [System.Serializable]
        public class PostJiraIssueType
        {
            public string id;
            public PostJiraIssueType(string id)
            {
                this.id = id;
            }
        }

        public PostJiraIssueInner(string summary, string description, string projectId, string issueTypeId)
        {
            this.summary = summary;
            this.description = description;
            project = new PostJiraIssueProject(projectId);
            issuetype = new PostJiraIssueType(issueTypeId);
        }

    }
}

