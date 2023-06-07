using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    public string playerName;
    public string playerAvatarName;
    private Player _player;

    public TMP_Text playerNameTextfield;
    public TMP_Text playerAvatarNameTextfield;

    public void SetUp(Player player, string avatarName)
    {
        _player = player;
        playerName = _player.NickName;
        playerAvatarName = avatarName;
        playerNameTextfield.text = playerName;
        playerAvatarNameTextfield.text = playerAvatarName;
    }
}
