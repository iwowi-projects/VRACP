using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public class PutJiraIssue
{
    public PutJiraIssueInner fields;

    [System.Serializable]
    public class PutJiraIssueInner
    {
        public string summary;
        public string description;
        public JiraIssueType issuetype;
        public JiraIssuePriority priority;

        public PutJiraIssueInner(string summary, string description, string issueTypeId, string priorityId)
        {
            this.summary = summary;
            this.description = description;
            this.issuetype = new JiraIssueType(issueTypeId, null);
            if (priorityId != null)
            {
                this.priority = new JiraIssuePriority(priorityId, null);
            } 
            
        }
    }

    public PutJiraIssue(string summary, string description, string issueTypeId, string priorityId)
    {
        fields = new PutJiraIssueInner(summary, description, issueTypeId, priorityId);
    }
}
