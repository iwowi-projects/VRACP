using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JiraSprint
{
    public int id;
    public int originBoardId;
    public string state;
    public string name;
    public string startDate, endDate;

    public static class States
    {
        public const string ACTIVE = "active";
        public const string FUTURE = "future";
        public const string CLOSED = "closed";
    }
}
