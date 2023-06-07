using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SprintCard : BaseCard, IPunInstantiateMagicCallback
{
    public TMP_Text titleTextfield;
    public TMP_Text statusTextfield;
    public TMP_Text startDateTextfield;
    public TMP_Text endDateTextfield;

    public int id;
    public string title, status, startDate, endDate;



    public void Setup(int id, string title, string status, string startDate, string endDate)
    {
        this.id = id;
        this.title = title;
        this.status = status;
        this.startDate = startDate;
        this.endDate = endDate;

        titleTextfield.text = this.title;
        statusTextfield.text = this.status;
        if (startDate != null)
        {
            startDateTextfield.text = ConvertDate(this.startDate);
        }
        if (endDate != null)
        {
            endDateTextfield.text = ConvertDate(this.endDate);
        }
    }

    public void Setup(JiraSprint jiraSprint)
    {
        Setup(jiraSprint.id, jiraSprint.name, jiraSprint.state, jiraSprint.startDate, jiraSprint.endDate);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        this.transform.localScale = new Vector3(1, 1, 1);

        int id = (int) data[0];
        string title = (string)data[1];
        string status = (string)data[2];
        string startDate = (string)data[3];
        string endDate = (string)data[4];

        Setup(id, title, status, startDate, endDate);
    }

    public string ConvertDate(string date)
    {
        // Date is of format 2022-08-01T11:35:28.700Z -> split on 'T' and return first section
        return date.Split('T')[0];
    }


}
