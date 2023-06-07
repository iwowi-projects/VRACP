using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraIssueEditMeta
{
    public JiraIssueEditMetaIssueType issuetype;
    public JiraIssueEditMetaPriority priority;

    [System.Serializable]
    public class JiraIssueEditMetaIssueType
    {
        public List<JiraIssueType> allowedValues;
    }

    [System.Serializable]
    public class JiraIssueEditMetaPriority
    {
        public List<JiraIssuePriority> allowedValues;
    }

    public List<string> GetPrioritiesAsStringList()
    {
        List<string> priorities = new List<string>();
        priority.allowedValues.ForEach(p =>
        {
            priorities.Add(p.id + " " + p.name);
        });
        return priorities;
    }

    public List<string> GetIssueTypesAsStringList()
    {
        List<string> types = new List<string>();
        issuetype.allowedValues.ForEach(t =>
        {
            types.Add(t.id + " " + t.name);
        });
        return types;
    }

}
