using System;
using System.Collections;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

public class InscriptionManager : MonoBehaviour
{
    public TextMeshProUGUI nameInputSignUp;
    public TextMeshProUGUI emailInputSignUp;
    public TextMeshProUGUI passwordInputSignUp;

    public TextMeshProUGUI emailInputSignIn;
    public TextMeshProUGUI passwordInputSignIn;

    public GameObject objectToDesactiveOnPlay;

    public NetworkMenu networkMenu;

    [Header("Legacy")]

    public TextMeshProUGUI nameText;


    public void StartGame()
    {
        string newName = nameText.text;


        networkMenu.StartClient(objectToDesactiveOnPlay, newName);
    }

    public void HostGame()
    {
        string newName = nameText.text;


        networkMenu.StartHost(objectToDesactiveOnPlay, newName);
    }

    public void GoToSignUp()
    {

    }

    public void GoToSignIn()
    {

    }

    public void GoBackToMenu()
    {

    }

    public void SignUp()
    {
        if(emailInputSignUp.text.Contains("@") && emailInputSignUp.text.Contains("."))
        {
            if(nameInputSignUp.text.Length > 1)
            {
                if(passwordInputSignUp.text.Length > 8)
                {
                    StartCoroutine("SignUpCoroutine");
                }
                else
                {
                    print("Password need to be at least 8 characters long");
                }
            }
            else
            {
                print("Invalid username");
            }
        }
        else
        {
            print("Wrong email format");
        }
    }

    public void SignIn()
    {
        StartCoroutine("SignInCoroutine");
    }

    private IEnumerator SignUpCoroutine()
    {
        SignUpData data = new SignUpData()
        {
            username = nameInputSignUp.text,
            email = emailInputSignUp.text,
            password = passwordInputSignUp.text,
        };
        string json = JsonUtility.ToJson(data);
        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.249:3000/register", json, "application/json"))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                Debug.LogError(www.result.ToString());
            }
            else
                Debug.Log("CA A MARCHé 2 ");
        }
    }

    private IEnumerator SignInCoroutine()
    {
        SignInData data = new SignInData()
        {
            email = emailInputSignUp.text,
            password = passwordInputSignUp.text,
        };
        string json = JsonUtility.ToJson(data);
        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.249:3000/login", json, "application/json"))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                Debug.LogError(www.result.ToString());
            }
            else
                Debug.Log("CA A MARCHé");
        }
    }

    [DataContract]
    [Serializable]
    public struct SignUpData
    {
        [DataMember(Name = "username")]
        public string username;
        [DataMember(Name = "email")]
        public string email;
        [DataMember(Name = "password")]
        public string password;
    }

    [DataContract]
    [Serializable]
    public struct SignInData
    {
        [DataMember(Name = "email")]
        public string email;
        [DataMember(Name = "password")]
        public string password;
    }

}
