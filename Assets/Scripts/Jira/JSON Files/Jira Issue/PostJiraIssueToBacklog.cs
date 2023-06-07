using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostJiraIssueToBacklog
{
    public List<string> issues;

    public PostJiraIssueToBacklog(List<JiraIssue> issues)
    {
        issues.ForEach(issue =>
        {
            this.issues.Add(issue.key);
        });
    }

    public PostJiraIssueToBacklog(List<string> issueIds)
    {
        issues = issueIds;
    }
}
