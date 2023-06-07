using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraBoardColumn
{
    public string name;
    public List<JiraStatus> statuses;

    public string[] GetStatusesAsStringList()
    {
        string[] arr = new string[statuses.Count];
        int i = 0;

        statuses.ForEach(status =>
        {
            arr[i] = status.id;
            i++;
        });

        return arr;
    }
}
