using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraBoardFeatures
{
    public List<JiraBoardFeature> features;

    [System.Serializable]
    public class JiraBoardFeature
    {
        public int boardId;
        public string state;
        public string localisedName;
        public string feature; // backlog, sprints
    }

    public bool IsFeatureEnabled(string feature)
    {
        return features.Find(f => f.feature == feature).state == "ENABLED";
    }

    public bool HasBacklog()
    {
        return IsFeatureEnabled("jsw.agility.backlog");
    }

    public bool HasSprints()
    {
        return IsFeatureEnabled("jsw.agility.sprints");
    }
}
