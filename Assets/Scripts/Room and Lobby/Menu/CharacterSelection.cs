using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public List<CharacterInfo> characters;
    public TMP_Text avatarNameTextfield;
    private int characterIndex = 0;

    private void Start()
    {
        // If character is already set in the custom properties of the player, change index.
        if (RoomManager.Instance.GetAvatarPrefabName() != null)
        {
            SetCharacter(RoomManager.Instance.GetAvatarPrefabName());
        }
        SetCharacter(characterIndex);
        
    }

    public void NextCharacter()
    {
        int newIndex = (characterIndex + 1) % characters.Count;
        SetCharacter(newIndex);
    }

    public void PreviousCharacter()
    {
        int newIndex = characterIndex - 1;
        if (newIndex < 0)
        {
            newIndex = characters.Count - 1;
        }
        SetCharacter(newIndex);
    }

    public string GetCurrentCharacterName()
    {
        Debug.Log("Current character: " + characters[characterIndex].getCharacterName());
        return characters[characterIndex].getCharacterName();
    }

    public void UpdateCurrentCharacterToRoomManager()
    {
        RoomManager.Instance.SetAvatarPrefab(GetCurrentCharacterName());
    }

    public void SetCharacter(int index)
    {
        characters[characterIndex].getCharacterPrefab().SetActive(false);
        characterIndex = index;
        characters[characterIndex].getCharacterPrefab().SetActive(true);
        avatarNameTextfield.text = characters[characterIndex].characterName;
        UpdateCurrentCharacterToRoomManager();
    }

    public void SetCharacter(string name)
    {
        int index = characters.FindIndex(c => c.characterName == name);
        
        if (index == -1) return;
        SetCharacter(index);
    }

    [System.Serializable]
    public class CharacterInfo
    {
        public GameObject characterPrefab;
        public string characterName;

        public GameObject getCharacterPrefab()
        {
            return characterPrefab;
        }

        public string getCharacterName()
        {
            return characterName;
        }
    }

}
