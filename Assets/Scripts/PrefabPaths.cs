using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PrefabPaths
{
    public static string ISSUE_CARD_PREFAB_PATH = Path.Combine("PhotonPrefabs", "IssueCard");
    public static string SPRINT_CARD_PREFAB_PATH = Path.Combine("PhotonPrefabs", "SprintCard");
    public static string BOARD_COLUMN_PREFAB_PATH = Path.Combine("PhotonPrefabs", "BoardColumn");
    public static string BOARD_ISSUE_CARD_PREFAB_PATH = Path.Combine("PhotonPrefabs", "BoardIssueCard");

    // Github prefabs
    public static string COMMIT_BUTTON = Path.Combine("PhotonPrefabs", "Button Github Commit");
    public static string FILE_BUTTON = Path.Combine("PhotonPrefabs", "Button Github File");

}
