using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraIssueFields
{
    public string summary;
    public string description;
    public JiraIssueType issuetype;
    public JiraStatus status;
    public JiraCreator creator;
    public JiraIssuePriority priority;
}
