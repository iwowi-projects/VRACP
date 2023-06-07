using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetCorrectDays : MonoBehaviour
{
    public TMP_Dropdown yearDropdown;
    public TMP_Dropdown monthDropdown;
    public TMP_Dropdown dayDropdown;

    private void Awake()
    {
        InitYears();
        InitMonths();
        UpdateDays(0);
        yearDropdown.onValueChanged.AddListener(UpdateDays);
        monthDropdown.onValueChanged.AddListener(UpdateDays);
    }

    void InitYears()
    {
        // Set years in dropdown
        int currentYear = DateTime.Now.Year;
        List<string> years = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            years.Add("" + (currentYear + i));
        }
        yearDropdown.AddOptions(years);
    }

    void InitMonths()
    {
        // Set months in dropdown
        List<string> months = new List<string>();
        for (int i = 0; i < 12; i++)
        {
            months.Add("" + (i + 1));
        }
        monthDropdown.AddOptions(months);
    }

    void UpdateDays(int value)
    {
        int year = Int32.Parse(yearDropdown.options[yearDropdown.value].text);
        int month = Int32.Parse(monthDropdown.options[monthDropdown.value].text);
        int daysInMonth = DateTime.DaysInMonth(year, month);
        List<string> days = new List<string>();
        dayDropdown.ClearOptions();
        for (int i = 0; i < daysInMonth; i++)
        {
            days.Add("" + (i + 1));
        }

        dayDropdown.AddOptions(days);
    }

}
