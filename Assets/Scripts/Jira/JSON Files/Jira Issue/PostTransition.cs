using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostTransition
{
    public PostTransitionInner transition;

    [System.Serializable]
    public class PostTransitionInner
    {
        public string id;

        public PostTransitionInner(string id)
        {
            this.id = id;
        }
    }

    public PostTransition(string id)
    {
        transition = new PostTransitionInner(id);
    }
}
