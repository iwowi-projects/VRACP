using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraIssueTransition
{
    public string id;
    public string name;
    public JiraStatus to;
}
