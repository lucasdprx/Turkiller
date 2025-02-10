using System;
using System.Collections;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class InscriptionManager : MonoBehaviour
{
    [Header("MainPage")]
    public GameObject menuHome;

    [Header("Customization")]
    public GameObject menuCustomization;
    public TextMeshProUGUI nameText;

    [Header("SignUp")]
    public GameObject menuSignUp;
    public TextMeshProUGUI nameInputSignUp;
    public TextMeshProUGUI emailInputSignUp;
    public TextMeshProUGUI passwordInputSignUp;

    [Header("SignIn")]
    public GameObject menuSignIn;
    public TextMeshProUGUI emailInputSignIn;
    public TextMeshProUGUI passwordInputSignIn;

    [Header("Other")]

    public GameObject objectToDesactiveOnPlay;

    public NetworkMenu networkMenu;


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

    public void GoToCustomization()
    {
        menuSignIn.SetActive(false);
        menuSignUp.SetActive(false);
        menuCustomization.SetActive(true);
    }

    public void GoToSignUp()
    {
        menuSignUp.SetActive(true);
        menuHome.SetActive(false);
    }

    public void GoToSignIn()
    {
        menuSignIn.SetActive(true);
        menuHome.SetActive(false);
    }

    public void GoBackToMenu()
    {
        menuHome.SetActive(true);
        menuSignIn.SetActive(false);
        menuSignUp.SetActive(false);
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
                GoToCustomization();
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
                GoToCustomization();
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
