using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMenu : MonoBehaviour
{
    public List<MenuItem> menuItems;

    public void Start()
    {
        menuItems.ForEach(item =>
        {
            item.button.onClick.AddListener(() =>
            {
                toggleActive(item.targetObject);
            });
        });
    }

    public void toggleActive(GameObject obj)
    {
        MenuItem currentActiveItem = menuItems.Find(item => item.targetObject.activeSelf);
        if (currentActiveItem != null && currentActiveItem.targetObject != obj)
        {
            currentActiveItem.targetObject.SetActive(false);
        }
        obj.SetActive(!obj.activeSelf);
    }

    public void CloseActive()
    {
        MenuItem currentActiveItem = menuItems.Find(item => item.targetObject.activeSelf);
        currentActiveItem.targetObject.SetActive(false);
    }

    [System.Serializable]
    public class MenuItem
    {
        [SerializeField]
        public Button button;
        [SerializeField]
        public GameObject targetObject;
    }
}
