using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveAuthenticationDataButton : MonoBehaviour
{
    public TMP_InputField baseUrlInputfield;
    public TMP_InputField usernameInputfield;
    public TMP_InputField passwordInputfield;
    public TMP_Text feedbackTextfield;

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
