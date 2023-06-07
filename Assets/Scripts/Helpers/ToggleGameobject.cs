using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameobject : MonoBehaviour
{
    public GameObject go;
    public bool isActive;

    public void toggleActive()
    {
        isActive = !isActive;
        go.SetActive(isActive);
    }

}
