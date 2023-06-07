using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraIssue
{
    public string id;
    public string key;
    public JiraIssueFields fields;

    public override bool Equals(object obj)
    {
        return id == (obj as JiraIssue).id;
    }

    public override int GetHashCode()
    {
        return (id).GetHashCode();
    }
}
