using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public Menu[] menues;

    private void Awake()
    {
        Instance = this;
    }

    public void openMenu(string name)
    {
        Debug.Log("Opening menu: " + name);
        foreach (Menu m in menues)
        {
            if (name == m.menuName)
            {
                m.Open();
            } else
            {
                m.Close();
            }
        }
    }
    public void openMenu(Menu menu)
    {
        openMenu(menu.menuName);
    }


    public void closeMenu(Menu menu)
    {
        menu.Close();
    }
}
