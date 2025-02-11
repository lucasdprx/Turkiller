using System;
using System.Collections;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class InscriptionManager : MonoBehaviour
{
    [Header("MainPage")]
    [SerializeField] private GameObject menuHome;

    [Header("Customization")]
    [SerializeField] private GameObject menuCustomization;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("SignUp")]
    [SerializeField] private GameObject menuSignUp;
    [SerializeField] private TextMeshProUGUI nameInputSignUp;
    [SerializeField] private TextMeshProUGUI emailInputSignUp;
    [SerializeField] private TMP_InputField passwordInputSignUp;

    [Header("SignIn")]
    [SerializeField] private GameObject menuSignIn;
    [SerializeField] private TextMeshProUGUI emailInputSignIn;
    [SerializeField] private TMP_InputField passwordInputSignIn;

    [Header("Other")]

    [SerializeField] private GameObject objectToDeactivateOnPlay;
    [SerializeField] private NetworkMenu networkMenu;


    public void StartGame()
    {
        string newName = nameText.text;


        networkMenu.StartClient(objectToDeactivateOnPlay, newName);
    }

    public void HostGame()
    {
        string newName = nameText.text;

        networkMenu.StartHost(objectToDeactivateOnPlay, newName);
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
                if(passwordInputSignUp.text.Length >= 8)
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
            email = emailInputSignIn.text,
            password = passwordInputSignIn.text,
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
