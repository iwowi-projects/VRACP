using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateSprintCard: BaseCreateCard
{
    public TMP_InputField nameInputField;
    public GameObject startDateSection;
    public TMP_Dropdown startYearDropdown;
    public TMP_Dropdown startMonthDropdown;
    public TMP_Dropdown startDayDropdown;
    public Toggle startDateToggle;

    public GameObject endDateSection;
    public TMP_Dropdown endYearDropdown;
    public TMP_Dropdown endMonthDropdown;
    public TMP_Dropdown endDayDropdown;
    public Toggle endDateToggle;

    public Toggle activeSprintToggle;



    public void Setup()
    {
        Clear();
    }

    public override void Clear()
    {
        nameInputField.text = "";
        startDateToggle.isOn = false;
        endDateToggle.isOn = false;
    }

    public override void OnSaveClick()
    {
        string name = nameInputField.text;
        if (name == null || name.Length == 0) 
        {
            ApiFeedback.Instance.DisplayFeedback("Sprint title is missing.");
            return;
        }

        int startYear, startMonth, startDay, endYear, endMonth, endDay;
        string startDate = null, endDate = null;

        startYear = GetValueOfDropdown(startYearDropdown);
        startMonth = GetValueOfDropdown(startMonthDropdown);
        startDay = GetValueOfDropdown(startDayDropdown);
        startDate = GetDateString(startYear, startMonth, startDay);
        endYear = GetValueOfDropdown(endYearDropdown);
        endMonth = GetValueOfDropdown(endMonthDropdown);
        endDay = GetValueOfDropdown(endDayDropdown);
        endDate = GetDateString(endYear, endMonth, endDay); ;

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("CurrentBoardId"))
        {
            ApiFeedback.Instance.DisplayFeedback("Select a board first.");
            return;
        }
        int originBoardId = (int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentBoardId"];

        JiraController.Instance.CreateSprint(name, originBoardId, startDate, endDate, err => ApiFeedback.Instance.DisplayFeedback(err), postJiraSprintResponse =>
        {
            if (activeSprintToggle.isOn)
            {
                JiraController.Instance.UpdateSprint(postJiraSprintResponse.id, JiraSprint.States.ACTIVE, err => ApiFeedback.Instance.DisplayFeedback(err), result =>
                {
                    InstantiateSprintCard(postJiraSprintResponse);
                    SynchronizeClose();
                });
            } else
            {
                InstantiateSprintCard(postJiraSprintResponse);
                SynchronizeClose();
            }
        }
        );
    }



    public void InstantiateSprintCard(PostJiraSprintResponse postJiraSprintResponse)
    {
        JiraController.Instance.GetSprint(postJiraSprintResponse.id, err => ApiFeedback.Instance.DisplayFeedback(err), InstantiateSprintCard);
    }

    public void InstantiateSprintCard(JiraSprint jiraSprint)
    {
        object[] customInstantiationData = { jiraSprint.id, jiraSprint.name, jiraSprint.state, jiraSprint.startDate, jiraSprint.endDate };
        string sprintCardPrefabPath = Path.Combine("PhotonPrefabs", "SprintCard");
        GameObject sprintCard = PhotonNetwork.Instantiate(sprintCardPrefabPath, spawnPoint.position, spawnPoint.localRotation, 0, customInstantiationData);
        Clear();
    }

    private int GetValueOfDropdown(TMP_Dropdown dropdown)
    {
        return Int32.Parse(dropdown.options[dropdown.value].text);
    }

    private string GetDateString(int year, int month, int day)
    {
        return $"{year}-{month}-{day}";
    }

}
