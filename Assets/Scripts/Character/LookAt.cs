using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    public bool shouldLook = false;


    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        this.target = GameObject.FindGameObjectWithTag("LocalPlayer").transform.FindChildRecursive("Main Camera").transform;
        shouldLook = true;
    }

    
    void Update()
    {
        if (!shouldLook) return;
        if (target == null)
        {
            Setup();
            return;
        }
        Vector3 targetPostition = new Vector3(target.position.x,
                                        this.transform.position.y,
                                        target.position.z);
        this.transform.LookAt(targetPostition);
    }
}
