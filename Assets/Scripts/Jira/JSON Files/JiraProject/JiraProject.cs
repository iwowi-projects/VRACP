using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraProject
{
    public string id;
    public string name;
    public List<JiraIssueType> issueTypes;
}
