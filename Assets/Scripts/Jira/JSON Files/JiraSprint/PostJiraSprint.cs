using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostJiraSprint
{
    public string name;
    public int originBoardId;
    public string startDate;
    public string endDate;

    public PostJiraSprint(string name, int originBoardId, string startDate, string endDate)
    {
        this.name = name;
        this.originBoardId = originBoardId;
        this.startDate = startDate;
        this.endDate = endDate;
    }
}
