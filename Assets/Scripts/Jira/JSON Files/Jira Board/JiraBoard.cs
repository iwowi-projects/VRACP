using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraBoard
{
    public int id;
    public string name;
    public string type; // scrum or kanban, but for team-managed projects it is necessary to find it out manually in /features endpoint.
    public JiraBoardFeatures jiraBoardFeatures;

    public bool HasBacklog()
    {
        return jiraBoardFeatures.IsFeatureEnabled("jsw.agility.backlog");
    }

    public bool HasSprints()
    {
        return jiraBoardFeatures.IsFeatureEnabled("jsw.agility.sprints");
    }

    public new string GetType()
    {
        return type;
    }

}
