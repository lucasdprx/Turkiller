using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    [SerializeField] private TextMeshProUGUI passwordInputSignUp;

    [Header("SignIn")]
    [SerializeField] private GameObject menuSignIn;
    [SerializeField] private TextMeshProUGUI emailInputSignIn;
    [SerializeField] private TextMeshProUGUI passwordInputSignIn;

    [Header("Skin")]
    [SerializeField] private List<Sprite> skins;
    private int skinIndex = 0;
    [SerializeField] private GameObject skinPreviewPrefab;
    [SerializeField] private Transform skinSelector;
    [SerializeField] private Image skinSelected;
    [SerializeField] private Transform skinsParent;

    [Header("Other")]

    [SerializeField] private GameObject objectToDeactivateOnPlay;
    [SerializeField] private NetworkMenu networkMenu;

    private void Start()
    {
        foreach (var item in skins)
        {
            Instantiate(skinPreviewPrefab, skinsParent).GetComponent<Image>().sprite = item;
        }
        skinIndex = 0;

        Invoke("SetSkinVisuals", .1f);
    }

    public void StartGame()
    {
        string newName = nameText.text;


        networkMenu.StartClient(objectToDeactivateOnPlay, newName, skinIndex);
    }

    public void HostGame()
    {
        string newName = nameText.text;

        print(newName);
        networkMenu.StartHost(objectToDeactivateOnPlay, newName, skinIndex);
    }

    public void NextSkin()
    {
        skinIndex = (skinIndex + 1) % skins.Count;
        SetSkinVisuals();
    }

    public void PreviousSkin()
    {
        skinIndex = skinIndex - 1 < 0 ? skins.Count - 1 : skinIndex - 1;
        SetSkinVisuals();
    }

    private void SetSkinVisuals()
    {
        skinSelected.sprite = skins[skinIndex];
        skinSelector.position = skinsParent.GetChild(skinIndex).position;
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
