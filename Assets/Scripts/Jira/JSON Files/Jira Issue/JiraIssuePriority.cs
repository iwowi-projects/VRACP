using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraIssuePriority
{
    public string id;
    public string name;

    public JiraIssuePriority(string? id, string? name)
    {
        this.id = id;
        this.name = name;
    }
}
