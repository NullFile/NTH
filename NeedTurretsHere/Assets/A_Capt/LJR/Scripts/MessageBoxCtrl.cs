using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



namespace Enut4LJR
{
    public class MessageBoxCtrl : MonoBehaviour
    {
        public Button m_LeftBtn;
        public Button m_RightBtn;
        public Text m_MessageText;
        public Text m_LeftBtnText;
        public Text m_RightBtnText;
        string m_SceneName = "";

        // Start is called before the first frame update
        void Start()
        {
            if (m_LeftBtn != null)
                m_LeftBtn.onClick.AddListener(LeftBtnFunc);

            if (m_RightBtn != null)
                m_RightBtn.onClick.AddListener(RightBtnFunc);
        }

        void LeftBtnFunc()
		{
            if (m_LeftBtnText.text == "OK")
            {
                SceneManager.LoadScene(m_SceneName);
            }
            Destroy(this.gameObject);
        }

        void RightBtnFunc()
		{
            
            Destroy(this.gameObject);
        }

        public void SetSceneName(string a_SceneName)
		{
            m_SceneName = a_SceneName;
		}

        public void SetMessage(string a_Mess, string a_RBtnMess = "OK", string a_LBtnMess = "Cancel")
		{
            m_MessageText.text = a_Mess;
            m_LeftBtnText.text = a_RBtnMess;
            m_RightBtnText.text = a_LBtnMess;
		}

        //// Update is called once per frame
        //void Update()
        //{
        //
        //}
    }
}
