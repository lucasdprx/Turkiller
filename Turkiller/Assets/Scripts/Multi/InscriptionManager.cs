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
    [SerializeField] private TMP_InputField passwordInputSignUp;
    [SerializeField] private TextMeshProUGUI textErrorSignUp;

    [Header("SignIn")]
    [SerializeField] private GameObject menuSignIn;
    [SerializeField] private TextMeshProUGUI emailInputSignIn;
    [SerializeField] private TMP_InputField passwordInputSignIn;
    [SerializeField] private TextMeshProUGUI textErrorSignIn;


    [Header("Skin")]
    [SerializeField] private List<Sprite> skins;
    private int skinIndex = 0;
    [SerializeField] private GameObject skinPreviewPrefab;
    [SerializeField] private Transform skinSelector;
    [SerializeField] private Image skinSelected;
    [SerializeField] private Transform skinsParent;


    [Header("Voice")]
    [SerializeField] int voiceCount = 2;
    [SerializeField] TextMeshProUGUI voiceText;
    int voiceIndex = 0;


    [Header("Other")]

    [SerializeField] private GameObject objectToDeactivateOnPlay;
    [SerializeField] private NetworkMenu networkMenu;

    private void Start()
    {
        for (int i = 0; i < skins.Count; i++)
        {
            var item = skins[i];
        
            GameObject go = Instantiate(skinPreviewPrefab, skinsParent);
            go.GetComponent<Image>().sprite = item;
            int k = i;
            go.GetComponent<Button>().onClick.AddListener(() => { SetSkinByIndex(k); });
        }
        skinIndex = 0;

        voiceIndex = 0;

        Invoke("SetSkinVisuals", .1f);
    }

    public void StartGame()
    {
        string newName = nameText.text;


        networkMenu.StartClient(objectToDeactivateOnPlay, newName, skinIndex, voiceIndex);
    }

    public void HostGame()
    {
        string newName = nameText.text;
        networkMenu.StartHost(objectToDeactivateOnPlay, newName, skinIndex, voiceIndex);
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

    public void NextVoice()
    {
        voiceIndex = (voiceIndex + 1) % voiceCount;
        SetVoiceText();
    }

    public void PreviousVoice()
    {
        voiceIndex = voiceIndex - 1 < 0 ? voiceCount - 1 : voiceIndex - 1;
        SetVoiceText();
    }

    private void SetVoiceText()
    {
        voiceText.text = "Voice " + (voiceIndex + 1).ToString();
    }

    public void SetSkinByIndex(int index)
    {
        skinIndex = index;
        SetSkinVisuals();
    }

    private void SetSkinVisuals()
    {
        skinSelected.sprite = skins[skinIndex];
        skinSelector.SetParent(skinsParent.GetChild(skinIndex));
        skinSelector.localPosition = new();

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
        if(emailInputSignUp.text.Contains("@") && emailInputSignUp.text.Contains(".") && emailInputSignUp.text.Length > 5)
        {
            if(nameInputSignUp.text.Length > 2)
            {
                if(passwordInputSignUp.text.Length >= 4)
                {
                    StartCoroutine("SignUpCoroutine");
                }
                else
                {
                    textErrorSignUp.SetText("Password need to be at least 4 characters long");
                    print("Password need to be at least 4 characters long");
                }
            }
            else
            {
                textErrorSignUp.SetText("Invalid username");
                print("Invalid username");
            }
        }
        else
        {
            textErrorSignUp.SetText("Wrong email format");
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
                textErrorSignUp.SetText("Server Down");
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
                textErrorSignIn.SetText("The email or password is incorrect");
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
