using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraIssueType
{
    public string name;
    public string id;

    public JiraIssueType(string? id, string? name)
    {
        this.id = id;
        this.name = name;
    }
}
