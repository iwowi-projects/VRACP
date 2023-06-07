using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GithubCommitProperties
{
    public string message { get; set; }
    public Author author { get; set; }

    public class Author
    {
        public string name { get; set; }
    }
}
