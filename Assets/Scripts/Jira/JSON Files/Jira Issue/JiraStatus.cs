using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraStatus
{
    public string id;
    public string name;

    public JiraStatus(string? id, string? name)
    {
        this.id = id;
        this.name = name;
    }
}
