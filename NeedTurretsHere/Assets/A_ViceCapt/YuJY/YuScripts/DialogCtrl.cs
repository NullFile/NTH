using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Yuspace
{


    public class DialogCtrl : MonoBehaviour
    { 
        [HideInInspector] public UnitType m_UniType;
        public delegate void DLT_Res();     //델리게이트
        DLT_Res DltMet;

        [Header("----NoticeDialog----")]
        public Text NoticeDialogTxt;
        public Button NoticeDialogBtn;
        [Header("----BuyDialog----")]
        public Button OkBtn;
        public Button CancelBtn;
        public GameObject BuySelect;
        public GameObject NoticeSelect;

        // Start is called before the first frame update
        void Start()
        {
            if(NoticeDialogBtn != null)
            {
                NoticeDialogBtn.onClick.AddListener(() =>
                {
                    this.gameObject.SetActive(false);
                });
            }

            if (OkBtn != null)
                OkBtn.onClick.AddListener(() =>
                {
                    if (DltMet != null)
                        DltMet();
                    this.gameObject.SetActive(false);

                });

            if (CancelBtn != null)
                CancelBtn.onClick.AddListener(() =>
                {
                    this.gameObject.SetActive(false);

                });
            
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return))        //엔터시 델리게이트함수 호출
            {
                if (DltMet != null)
                    DltMet();
                this.gameObject.SetActive(false);
            }

        }


        public void MessageSet(string a_str = "", int a_fontsize = 45, DLT_Res a_DltMtd = null)
        {

            NoticeDialogTxt.text = a_str;
            NoticeDialogTxt.fontSize = a_fontsize;
            DltMet = a_DltMtd;

        }



    }
}
