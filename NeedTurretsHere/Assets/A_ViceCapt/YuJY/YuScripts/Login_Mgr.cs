using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;

//----------------- �̸��������� �´��� Ȯ���ϴ� ��� ��ũ��Ʈ
using System.Globalization;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Networking;
//----------------- �̸��������� �´��� Ȯ���ϴ� ��� ��ũ��Ʈ
using Altair;

namespace Yuspace
{
    public class Login_Mgr : MonoBehaviour
    {
        [Header("LoginPanel")]              //�̷��� ���� ����â�� �±׵��� ���´�. 
        public GameObject m_LoginPanelObj;
        public Button m_LoginBtn = null;
        public Button m_CreateAccOpenBtn = null;
        public InputField IDInputField;     //Email �� ���� ����
        public InputField PassInputField;

        [Header("CreateAccountPanel")]
        public GameObject m_CreateAccPanelObj;
        public InputField New_IDInputField;  //Email �� ���� ����
        public InputField New_PassInputField;
        public InputField New_NickInputField;
        public Button m_CreateAccountBtn = null;
        public Button m_CancelButton = null;

        [Header("Normal")]
        public Text MessageText;
        float ShowMsTimer = 0.0f;

        bool invalidEmailType = false;       // �̸��� ������ �ùٸ��� üũ
        bool isValidFormat = false;          // �ùٸ� �������� �ƴ��� üũ


        string LoginUrl = "";
        string CreateUrl = "";

        // Start is called before the first frame update
        void Start()
        {


            //------- LoginPanel
            if (m_LoginBtn != null)
                m_LoginBtn.onClick.AddListener(LoginBtn);

            if (m_CreateAccOpenBtn != null)
                m_CreateAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

            //------- CreateAccountPanel
            if (m_CancelButton != null)
                m_CancelButton.onClick.AddListener(CreateCancelBtn);

            if (m_CreateAccountBtn != null)
                m_CreateAccountBtn.onClick.AddListener(CreateAccountBtn);

            LoginUrl = "http://appyzs.dothome.co.kr/ShopLogin.php";
            CreateUrl = "http://appyzs.dothome.co.kr/ShopCreate.php";
        }

        // Update is called once per frame
        void Update()
        {
            if (GlobalData.choi_m_TrList.Count <= 0)           //ó�� �۷ι� ���������͸���Ʈ�� ���ٸ� �ʱ�ȭ����
            {
                GlobalData.choi_InitData();                    //��ü �ͷ��� ������ �ʱ�ȭ
                GlobalValue.InitData();                         //������ ���� �ͷ�Ÿ�� �ʱ�ȭ��
            }
            if (0.0f < ShowMsTimer)
            {
                ShowMsTimer -= Time.deltaTime;
                if (ShowMsTimer <= 0.0f)
                {
                    MessageOnOff("", false); //�޽��� ����
                }
            }

           
        }

        void LoginBtn()
        {
            //SceneManager.LoadScene("LobbyScene");

            //GlobalValue.InitData();
            //GlobalValue.LoadGameData();

            string a_IdStr = IDInputField.text;
            string a_PwStr = PassInputField.text;

            a_IdStr = a_IdStr.Trim();
            a_PwStr = a_PwStr.Trim();

            if (string.IsNullOrEmpty(a_IdStr) == true ||
                 string.IsNullOrEmpty(a_PwStr) == true)
            {
                MessageOnOff("ID, PW ��ĭ ���� �Է��� �ּž� �մϴ�.");
                return;
            }

            if (!(3 <= a_IdStr.Length && a_IdStr.Length < 20))  //3~20
            {
                MessageOnOff("ID�� 3���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
                return;
            }

            if (!(4 <= a_PwStr.Length && a_PwStr.Length < 20))  //6~100
            {
                MessageOnOff("��й�ȣ�� 4���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
                return;
            }
            StartCoroutine(LoginCo(a_IdStr, a_PwStr));
            //if (!CheckEmailAddress(IDInputField.text))
            //{
            //    MessageOnOff("Email ������ ���� �ʽ��ϴ�.");
            //    return;
            //}

            //------- �� �ɼ��� �߰��� ��� �α����ϸ鼭 ������ ���� ������ ������ �� �ִ�.

        }

        IEnumerator LoginCo(string a_IdStr, string a_PwStr)
        {
            WWWForm form = new WWWForm();

            form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);//System.Text.Encoding.UTF8 ���̵� �ѱ۷� �Է������� ������ �ʵ��� ��
            form.AddField("Input_pass", a_PwStr);

            UnityWebRequest a_www = UnityWebRequest.Post(LoginUrl, form);
            yield return a_www.SendWebRequest();
            if (a_www.error == null) //���� �����
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string sz = enc.GetString(a_www.downloadHandler.data);
                if (sz.Contains("Login-Success!!") == false)
                {
                    if (sz.Contains("ID does not exist.") == true)
                        MessageOnOff("���̵� �����ϴ�.");
                    else if (sz.Contains("PassWord does not Match.") == true)
                        MessageOnOff("��й�ȣ�� Ʋ�Ƚ��ϴ�.");
                    else
                        MessageOnOff(sz);

                    yield break;

                }

                if (sz.Contains("{\"") == false)//json�� �׻� �̷��� �����ϱ� ������ ���̽� ������ �´��� Ȯ�� �� �� ����
                {
                    MessageOnOff("������ ������ ���������� �ʽ��ϴ�." + sz);
                    yield break;
                }

                //GlobalValue.g_Unique_ID = a_IdStr;//���߿��� ��ȣȭ �ʿ�
                string a_GetStr = sz.Substring(sz.IndexOf("{\""));
                GlobalData.choi_UniqueID = a_IdStr;//���߿��� ��ȣȭ �ʿ�

                //���̽� �Ľ�
                var N = JSON.Parse(a_GetStr);
                if (N == null)
                    yield break;
                //if (N["nick_name"] != null)
                //    GlobalValue.g_NickName = N["nick_name"];
                //if (N["best_score"] != null)
                //    GlobalValue.g_BestScore = N["best_score"].AsInt;
                if (N["mygold"] != null)
                    GlobalData.choi_userDia = N["mygold"].AsInt;
                if (N["Item_list"] != null)
                {
                    string m_StrJson = (N["Item_list"]);
                    Debug.Log(m_StrJson);
                    if (string.IsNullOrEmpty(m_StrJson) == false && m_StrJson.Contains("UnitList") == true)
                    {
                        //myinfo�� ���̽� ���� �Ľ�
                        var a_N = JSON.Parse(m_StrJson);
                        for (int ii = 0; ii < a_N["UnitList"].Count; ii++)
                        {
                            int a_UniLevel = a_N["UnitList"][ii].AsInt;
                            if (ii < GlobalData.choi_m_TrList.Count)
                            {
                                GlobalData.choi_m_TrList[ii].UpgradeLv = a_UniLevel;
                                Debug.Log(GlobalData.choi_m_TrList[ii].UpgradeLv);
                            }//                        if(ii < GlobalValue.m_CrDataList.Count)

                        }// for (int ii = 0; ii < a_N["CrList"].Count; ii++)

                    }//if(string.IsNullOrEmpty(m_StrJson)  == false&& m_StrJson.Contains("CrList")==true)

                }// if (N["Item_list"] != null)



                SceneManager.LoadScene("ShopSceneTest");
            }
            else
            {
                Debug.Log(a_www.error);
                MessageOnOff(a_www.error);
            }

        }



        public void OpenCreateAccBtn()
        {
            if (m_LoginPanelObj != null)
                m_LoginPanelObj.SetActive(false);

            if (m_CreateAccPanelObj != null)
                m_CreateAccPanelObj.SetActive(true);
        }

        public void CreateCancelBtn()
        {
            if (m_LoginPanelObj != null)
                m_LoginPanelObj.SetActive(true);

            if (m_CreateAccPanelObj != null)
                m_CreateAccPanelObj.SetActive(false);
        }

        public void CreateAccountBtn() //���� ���� ��û �Լ�
        {
            string a_IdStr = New_IDInputField.text;
            string a_PwStr = New_PassInputField.text;
            string a_NickStr = New_NickInputField.text;

            a_IdStr = a_IdStr.Trim();
            a_PwStr = a_PwStr.Trim();
            a_NickStr = a_NickStr.Trim();

            if (string.IsNullOrEmpty(a_IdStr) == true ||
                string.IsNullOrEmpty(a_PwStr) == true ||
                string.IsNullOrEmpty(a_NickStr) == true)
            {
                MessageOnOff("ID, PW, ���� ��ĭ ���� �Է��� �ּž� �մϴ�.");
                return;
            }

            if (!(3 <= a_IdStr.Length && a_IdStr.Length < 20))  //3~20
            {
                MessageOnOff("ID�� 3���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
                return;
            }

            if (!(4 <= a_PwStr.Length && a_PwStr.Length < 20))  //6~100
            {
                MessageOnOff("��й�ȣ�� 4���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
                return;
            }



            //if (!CheckEmailAddress(a_IdStr))
            //{
            //    MessageOnOff("Email ������ ���� �ʽ��ϴ�.");
            //    return;
            //}

            //var request = new RegisterPlayFabUserRequest
            //{
            //    Email = a_IdStr,
            //    Password = a_PwStr,
            //    DisplayName = a_NickStr,
            //    RequireBothUsernameAndEmail = false
            //};
            //PlayFabClientAPI.RegisterPlayFabUser(request,
            //                    RegisterSuccess, RegisterFailure);

            //RequireBothUsernameAndEmail = false
            //RequireBothUsernameAndEmail �⺻���� true�� �����Ǿ� �ִ�.
            //�� �ɼ��� true�� ������̸�(ID)�� �̸����� ��� �ʿ� �ϴ�.
            //�츮�� ID�� ������� �ʰ� �̸����� IDó�� ����ؼ� �α��� �� ���̱� ������
            //false ó���� ��� ������̸�(ID) �Է��� �����ʰ� ���� ������ �� �� �ְ� �ȴ�.
            StartCoroutine(CreateCo(a_IdStr, a_PwStr, a_NickStr));
        } //public void CreateAccountBtn()

        IEnumerator CreateCo(string a_IdStr, string a_PwStr, string a_NickStr)
        {
            WWWForm form = new WWWForm();
            form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);
            form.AddField("Input_pass", a_PwStr);

            UnityWebRequest a_www = UnityWebRequest.Post(CreateUrl, form);
            yield return a_www.SendWebRequest();//������ �ö����� ���

            if (a_www.error == null)
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string sz = enc.GetString(a_www.downloadHandler.data);
                if (sz.Contains("Login-Success") == true)
                    MessageOnOff("���� ����");
                else if (sz.Contains("id dose ex") == true)
                    MessageOnOff("�ߺ��� ���̵� �����մϴ�.");

                else MessageOnOff(sz);

            }
            else
            {
                MessageOnOff("���� ���� : " + a_www.error);
                Debug.Log(a_www.error);
            }
        }

        //private void RegisterSuccess(RegisterPlayFabUserResult result)
        //{
        //    //Debug.Log("���� ����");
        //    MessageOnOff("���� ����");
        //}

        //private void RegisterFailure(PlayFabError error)
        //{
        //    MessageOnOff("���� ���� : " + error.GenerateErrorReport());
        //}


        void MessageOnOff(string Mess = "", bool isOn = true)
        {
            if (isOn == true)
            {
                MessageText.text = Mess;
                MessageText.gameObject.SetActive(true);
                ShowMsTimer = 7.0f;
            }
            else
            {
                MessageText.text = "";
                MessageText.gameObject.SetActive(false);
            }
        }

        //----------------- �̸��������� �´��� Ȯ���ϴ� ��� ��ũ��Ʈ
        //https://blog.naver.com/rlawndks4204/221591566567
        // <summary>
        /// �ùٸ� �̸������� üũ.
        /// </summary>
        private bool CheckEmailAddress(string EmailStr)
        {
            if (string.IsNullOrEmpty(EmailStr)) isValidFormat = false;

            EmailStr = Regex.Replace(EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
            if (invalidEmailType) isValidFormat = false;

            // true �� ��ȯ�� ��, �ùٸ� �̸��� ������.
            isValidFormat = Regex.IsMatch(EmailStr,
                          @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                          RegexOptions.IgnoreCase);
            return isValidFormat;
        }

        /// <summary>
        /// ���������� ��������.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalidEmailType = true;
            }
            return match.Groups[1].Value + domainName;
        }
        //----------------- �̸��������� �´��� Ȯ���ϴ� ��� ��ũ��Ʈ

    }

}


