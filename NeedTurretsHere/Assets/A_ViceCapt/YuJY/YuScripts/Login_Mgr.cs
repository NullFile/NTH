using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;

//----------------- 이메일형식이 맞는지 확인하는 방법 스크립트
using System.Globalization;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Networking;
//----------------- 이메일형식이 맞는지 확인하는 방법 스크립트
using Altair;

namespace Yuspace
{
    public class Login_Mgr : MonoBehaviour
    {
        [Header("LoginPanel")]              //이렇게 쓰면 편집창에 태그들이 나온다. 
        public GameObject m_LoginPanelObj;
        public Button m_LoginBtn = null;
        public Button m_CreateAccOpenBtn = null;
        public InputField IDInputField;     //Email 로 받을 것임
        public InputField PassInputField;

        [Header("CreateAccountPanel")]
        public GameObject m_CreateAccPanelObj;
        public InputField New_IDInputField;  //Email 로 받을 것임
        public InputField New_PassInputField;
        public InputField New_NickInputField;
        public Button m_CreateAccountBtn = null;
        public Button m_CancelButton = null;

        [Header("Normal")]
        public Text MessageText;
        float ShowMsTimer = 0.0f;

        bool invalidEmailType = false;       // 이메일 포맷이 올바른지 체크
        bool isValidFormat = false;          // 올바른 형식인지 아닌지 체크


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
            if (GlobalData.choi_m_TrList.Count <= 0)           //처음 글로벌 유저데이터리스트가 없다면 초기화갱신
            {
                GlobalData.choi_InitData();                    //전체 터렛의 데이터 초기화
                GlobalValue.InitData();                         //상점을 위한 터렛타입 초기화ㅣ
            }
            if (0.0f < ShowMsTimer)
            {
                ShowMsTimer -= Time.deltaTime;
                if (ShowMsTimer <= 0.0f)
                {
                    MessageOnOff("", false); //메시지 끄기
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
                MessageOnOff("ID, PW 빈칸 없이 입력해 주셔야 합니다.");
                return;
            }

            if (!(3 <= a_IdStr.Length && a_IdStr.Length < 20))  //3~20
            {
                MessageOnOff("ID는 3글자 이상 20글자 이하로 작성해 주세요.");
                return;
            }

            if (!(4 <= a_PwStr.Length && a_PwStr.Length < 20))  //6~100
            {
                MessageOnOff("비밀번호는 4글자 이상 20글자 이하로 작성해 주세요.");
                return;
            }
            StartCoroutine(LoginCo(a_IdStr, a_PwStr));
            //if (!CheckEmailAddress(IDInputField.text))
            //{
            //    MessageOnOff("Email 형식이 맞지 않습니다.");
            //    return;
            //}

            //------- 이 옵션을 추가해 줘야 로그인하면서 유저의 각종 정보를 가져올 수 있다.

        }

        IEnumerator LoginCo(string a_IdStr, string a_PwStr)
        {
            WWWForm form = new WWWForm();

            form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);//System.Text.Encoding.UTF8 아이디 한글로 입력했을때 깨지지 않도록 함
            form.AddField("Input_pass", a_PwStr);

            UnityWebRequest a_www = UnityWebRequest.Post(LoginUrl, form);
            yield return a_www.SendWebRequest();
            if (a_www.error == null) //에러 어ㅄ으면
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string sz = enc.GetString(a_www.downloadHandler.data);
                if (sz.Contains("Login-Success!!") == false)
                {
                    if (sz.Contains("ID does not exist.") == true)
                        MessageOnOff("아이디가 없습니다.");
                    else if (sz.Contains("PassWord does not Match.") == true)
                        MessageOnOff("비밀번호가 틀렸습니다.");
                    else
                        MessageOnOff(sz);

                    yield break;

                }

                if (sz.Contains("{\"") == false)//json은 항상 이렇게 시작하기 때문에 제이슨 형식이 맞는지 확인 할 수 있음
                {
                    MessageOnOff("서버의 응답이 정상적이지 않습니다." + sz);
                    yield break;
                }

                //GlobalValue.g_Unique_ID = a_IdStr;//나중에는 암호화 필요
                string a_GetStr = sz.Substring(sz.IndexOf("{\""));
                GlobalData.choi_UniqueID = a_IdStr;//나중에는 암호화 필요

                //제이슨 파싱
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
                        //myinfo쪽 제이슨 파일 파싱
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

        public void CreateAccountBtn() //계정 생성 요청 함수
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
                MessageOnOff("ID, PW, 별명 빈칸 없이 입력해 주셔야 합니다.");
                return;
            }

            if (!(3 <= a_IdStr.Length && a_IdStr.Length < 20))  //3~20
            {
                MessageOnOff("ID는 3글자 이상 20글자 이하로 작성해 주세요.");
                return;
            }

            if (!(4 <= a_PwStr.Length && a_PwStr.Length < 20))  //6~100
            {
                MessageOnOff("비밀번호는 4글자 이상 20글자 이하로 작성해 주세요.");
                return;
            }



            //if (!CheckEmailAddress(a_IdStr))
            //{
            //    MessageOnOff("Email 형식이 맞지 않습니다.");
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
            //RequireBothUsernameAndEmail 기본값은 true로 설정되어 있다.
            //이 옵션이 true면 사용자이름(ID)과 이메일이 모두 필요 하다.
            //우리는 ID는 사용하지 않고 이메일을 ID처럼 사용해서 로그인 할 것이기 때문에
            //false 처리해 줘야 사용자이름(ID) 입력을 하지않고도 계정 생성을 할 수 있게 된다.
            StartCoroutine(CreateCo(a_IdStr, a_PwStr, a_NickStr));
        } //public void CreateAccountBtn()

        IEnumerator CreateCo(string a_IdStr, string a_PwStr, string a_NickStr)
        {
            WWWForm form = new WWWForm();
            form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);
            form.AddField("Input_pass", a_PwStr);

            UnityWebRequest a_www = UnityWebRequest.Post(CreateUrl, form);
            yield return a_www.SendWebRequest();//응답이 올때까지 대기

            if (a_www.error == null)
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string sz = enc.GetString(a_www.downloadHandler.data);
                if (sz.Contains("Login-Success") == true)
                    MessageOnOff("가입 성공");
                else if (sz.Contains("id dose ex") == true)
                    MessageOnOff("중복된 아이디가 존재합니다.");

                else MessageOnOff(sz);

            }
            else
            {
                MessageOnOff("가입 실패 : " + a_www.error);
                Debug.Log(a_www.error);
            }
        }

        //private void RegisterSuccess(RegisterPlayFabUserResult result)
        //{
        //    //Debug.Log("가입 성공");
        //    MessageOnOff("가입 성공");
        //}

        //private void RegisterFailure(PlayFabError error)
        //{
        //    MessageOnOff("가입 실패 : " + error.GenerateErrorReport());
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

        //----------------- 이메일형식이 맞는지 확인하는 방법 스크립트
        //https://blog.naver.com/rlawndks4204/221591566567
        // <summary>
        /// 올바른 이메일인지 체크.
        /// </summary>
        private bool CheckEmailAddress(string EmailStr)
        {
            if (string.IsNullOrEmpty(EmailStr)) isValidFormat = false;

            EmailStr = Regex.Replace(EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
            if (invalidEmailType) isValidFormat = false;

            // true 로 반환할 시, 올바른 이메일 포맷임.
            isValidFormat = Regex.IsMatch(EmailStr,
                          @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                          RegexOptions.IgnoreCase);
            return isValidFormat;
        }

        /// <summary>
        /// 도메인으로 변경해줌.
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
        //----------------- 이메일형식이 맞는지 확인하는 방법 스크립트

    }

}


