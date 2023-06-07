using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuJiraConfiguration : MonoBehaviour
{
    public TMP_Dropdown boardsDropdown;

    public TMP_InputField baseUrlInputfield;
    public TMP_InputField usernameInputfield;
    public TMP_InputField passwordInputfield;
    public TMP_Text feedbackTextfield;

    public void GetBoards()
    {
        JiraController.Instance.GetBoards(err => ApiFeedback.Instance.DisplayFeedback(err), GenerateBoardDropdownList);
    }

    private void GenerateBoardDropdownList(List<JiraBoard> jiraBoards)
    {
        boardsDropdown.ClearOptions();
        boardsDropdown.Hide();
        List<string> boardsNames = new List<string>();

        foreach (JiraBoard item in jiraBoards)
        {
            boardsNames.Add(item.id + " " + item.name);
        }

        boardsDropdown.AddOptions(boardsNames);
    }

    public void OnShowBoardButtonClick()
    {
        if (boardsDropdown.options.Count == 0)
        {
            ApiFeedback.Instance.DisplayFeedback("You must authenticate before.");
            return;
        }

        int boardId = GetBoardId();
        string boardName = GetBoardName();

        BoardConfiguration.Instance.ShowBoard(boardId, boardName);
    }

    private int GetBoardId() 
    {
        return Int32.Parse(boardsDropdown.options[boardsDropdown.value].text.Split(" ")[0]);
    }

    private string GetBoardName()
    {
        return boardsDropdown.options[boardsDropdown.value].text;
    }


    public void Authenticate()
    {
        string url = baseUrlInputfield.text;
        string username = usernameInputfield.text;
        string password = passwordInputfield.text;

        JiraController.Instance.Authenticate(url, username, password,
            err => SetFeedbackText("Authentication failed"),
            myself => {
                string successMessage = "Successfully authenticated as {0}";
                SetFeedbackText(string.Format(successMessage, myself.displayName));
            }
        );
    }

    public void SetFeedbackText(string text)
    {
        feedbackTextfield.text = text;
    }

}
