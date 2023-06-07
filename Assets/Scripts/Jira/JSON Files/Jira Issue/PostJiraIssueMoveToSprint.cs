using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostJiraIssueMoveToSprint
{
    public List<string> issues;

    public PostJiraIssueMoveToSprint(List<JiraIssue> issues)
    {
        issues.ForEach(issue =>
        {
            this.issues.Add(issue.key);
        });
    }

    public PostJiraIssueMoveToSprint(List<string> issueIds)
    {
        issues = issueIds;
    }

}
