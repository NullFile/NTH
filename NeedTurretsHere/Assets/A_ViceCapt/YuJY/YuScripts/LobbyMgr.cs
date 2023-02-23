using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Altair;
using Enut4LJR;
using UnityEngine.Networking;

namespace Yuspace
{
    public class LobbyMgr : MonoBehaviour, ISoundPlay
    {
        public Text NickName;
        public Button StartBtn;
        public Button ShopBtn;
        public Button OptionBtn;

        public Button[] Slot = new Button[3];
        public Text[] SlotText = new Text[3];
        public Text[] SlotTextClone = new Text[3];

        //씬 연출
        bool isSceneIn = false;
        bool isSceneOut = false;
        bool droneLeave = false;
        bool isInitScene = false;
        public GameObject StartDrone;
        public GameObject[] CarryDrone;
        public Button[] DroneBtn;
        Vector3 DroneStartPos;
        Vector3 StartBtnPos;
        DroneCtrl a_DroneCtrl;
        Vector3 SDPos;
        float m_DroneSpeed = 20.0f;
        string m_SceneName = null;
        int[] m_StartPackage = new int[6] { 0, 1, 2, 3, 20, 21 };
        public GameObject ArmPanel;
        bool armLeave = false;
        ArmPanelCtrl a_ArmPanelCtrl;
        Vector3 APPos;
        public Image SlotPanelClone;
        Vector3 ArmPanelStartPos;
        Vector3 SlotPanelPos;
        public Image SaveSlotPanel;

        public GameObject configPanel;

        //사운드
        private new AudioSource audio;
        [SerializeField] private AudioClip clip;
        private bool isFirstPlay = true;

        //반장추가
        [SerializeField] private string stage1PickSceneName;
        [SerializeField] private string shoeSceneName;

        //슬롯 데이터 관련 변수
        public GameObject m_SlotDataObj;
        public Text stageText;
        public Text diaText;
        public Button slotdataBackBtn;


        private void Awake()
        {
            Time.timeScale = 1.0f;
            audio = GetComponent<AudioSource>();
            if (clip == null) clip = Resources.Load<AudioClip>("/Sounds/Open_Small.wav");
        }

        private void OnEnable()
        {
            GlobalData.curStage = 0;
            //if (isFirstPlay) isFirstPlay = false; //첫 번째 실행인 경우 소리를 재생하지 않고 boolen만 true로 전환한다.
            //else SoundPlay(ref clip);  //첫 번째 실행이 아닌 경우 정상적으로 소리를 재생한다.
        }

        // Start is called before the first frame update
        void Start()
        {
            GlobalData.masterVolume = PlayerPrefs.GetFloat("masterVolume");

            int volumeisOn = PlayerPrefs.GetInt("volumeisOn");
            if (volumeisOn == 0)
                GlobalData.volumeisOn = false;
            else
                GlobalData.volumeisOn = true;

            if (OptionBtn != null)
            {
                OptionBtn.onClick.AddListener(() =>
                {
                    configPanel.gameObject.SetActive(true);
                });
            }

            if (ShopBtn != null)
                ShopBtn.onClick.AddListener(()=>
                {
                    isSceneOut = true;
                    m_SceneName = shoeSceneName;
                    CarryDrone[2].gameObject.SetActive(false);

                    SaveSlotFunc();
                }
                );

            if (slotdataBackBtn != null)
                slotdataBackBtn.onClick.AddListener(() =>
                {
                    if (isSceneIn == true)
                        return;
                    GlobalData.choi_IsPick = -1;
                    isSceneOut = true;
                });

            
            for (int ii = 0; ii < 3; ii++)
			{
                int aa = ii;
                if (Slot[aa] != null)
                    Slot[aa].onClick.AddListener(() =>
                    {
                        GlobalData.choi_IsPick = aa + 1;
                        Debug.Log(GlobalData.choi_IsPick);
                        SlotSelFunc();
                    });
			}
            

            if (NickName != null)
            {
                NickName.text = Altair.GlobalData.choi_userNick;
            }

            DroneStartPos = StartDrone.transform.position;
            StartBtnPos = StartBtn.transform.position;

            ArmPanelStartPos = ArmPanel.transform.position;
            SlotPanelPos = SaveSlotPanel.transform.position;


            //반장추가
            if (StartBtn != null)
                StartBtn.onClick.AddListener(StartBtnFunc);

            a_DroneCtrl = StartDrone.GetComponent<DroneCtrl>();
            a_ArmPanelCtrl = ArmPanel.GetComponent<ArmPanelCtrl>();

            for (int ss = 1; ss < 4; ss++) 
            {
                if (GlobalData.choi_StageList[ss] != 0)
                {
                    SlotText[ss - 1].text = "Slot " + ss.ToString();
                    SlotTextClone[ss - 1].text = "Slot " + ss.ToString();
                }
				else
                {
                    SlotText[ss - 1].text = "New Slot";
                }
            }

            isInitScene = true;
            GlobalData.choi_IsPick = -1;
            SoundPlay(ref clip);

            
        }

        // Update is called once per frame
        void Update()
        {
            InitSceneUpdate();
            SceneInUpdate();
            SceneOutUpdate();

            GetSlotInfo();
            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log(GlobalData.choi_userDia);
            }
        }

        void StartBtnFunc()
		{
            isSceneOut = true;
            m_SceneName = stage1PickSceneName;
            CarryDrone[0].gameObject.SetActive(false);
            CarryDrone[1].gameObject.SetActive(false);

            if(Slot[GlobalData.choi_IsPick - 1].GetComponentInChildren<Text>().text == "New Slot")  //스타터팩 주기
			{
                for (int ii = 0; ii < m_StartPackage.Length; ii++)
				{
                    GlobalData.choi_m_TrList[m_StartPackage[ii]].UpgradeLv = 1;
                    SwitchManager.SetSwitch(SwitchManager.switchName[m_StartPackage[ii]], true);
                }

            }

            SaveSlotFunc();
        }

        void GetSlotInfo()
		{
            if (GlobalData.choi_IsPick != -1)
			{
                for (int ii = 0; ii < 3; ii++)
				{
                    Slot[ii].gameObject.SetActive(false);
				}

                m_SlotDataObj.SetActive(true);
                int a_Stage = GlobalData.choi_StageList[GlobalData.choi_IsPick];
                if (a_Stage != 0)
                    stageText.text = (a_Stage / 100).ToString() + " - " + (a_Stage % 100).ToString();
                else
                    stageText.text = "New Slot";

                diaText.text = GlobalData.choi_userDia.ToString();

			}
            else
			{
                m_SlotDataObj.SetActive(false);
                for (int ii = 0; ii < 3; ii++)
                {
                    Slot[ii].gameObject.SetActive(true);
                }
            }




        }

        void SaveSlotFunc()
		{
            if (GlobalData.choi_IsPick == 1)
                NetworkMgr.Inst.PushPacket(PacketType.Slot1Update);
            else if (GlobalData.choi_IsPick == 2)
				NetworkMgr.Inst.PushPacket(PacketType.Slot2Update);
			else if (GlobalData.choi_IsPick == 3)
				NetworkMgr.Inst.PushPacket(PacketType.Slot3Update);
        }

        void InitSceneUpdate()
		{
            if (isInitScene == false)
                return;
            
            

            if (!armLeave)
			{
                a_ArmPanelCtrl.MovePanel(ArmPanelStartPos, SlotPanelPos, true);
                if (a_ArmPanelCtrl.value == 1.0f)
                {
                    ArmPanel.transform.position = SlotPanelPos;
                    SaveSlotPanel.gameObject.SetActive(true);
                    SlotPanelClone.gameObject.SetActive(false);
                    armLeave = true;
                }
            }
            else
			{
                APPos = ArmPanel.transform.position;
                APPos.y += Time.deltaTime * 20.0f;
                if (APPos.y >= ArmPanelStartPos.y)
				{
                    APPos.y = ArmPanelStartPos.y;
                    isInitScene = false;
				}
                ArmPanel.transform.position = APPos;
			}
        }

        void SceneInUpdate()
		{
            if (isInitScene == true)
                return;

            if (isSceneIn == false)
                return;

            if (isSceneOut == true)
			{
                a_DroneCtrl.value = 0.0f;
                isSceneOut = false;
			}

            if (!droneLeave)
            {
                a_DroneCtrl.MoveDrone(DroneStartPos, StartBtnPos, true);
                if (a_DroneCtrl.value == 1.0f)
                {
                    StartDrone.transform.position = StartBtnPos;
                    for (int ii = 0; ii < DroneBtn.Length; ii++)
                    {
                        DroneBtn[ii].gameObject.SetActive(false);
                    }

                    StartBtn.gameObject.SetActive(true);
                    ShopBtn.gameObject.SetActive(true);
                    OptionBtn.gameObject.SetActive(true);
                    droneLeave = true;
                }
            }
            else
            {
                SDPos = StartDrone.transform.position;
                SDPos.y += Time.deltaTime * m_DroneSpeed;
                if (SDPos.y >= DroneStartPos.y)
                {
                    SDPos.y = DroneStartPos.y;
                    isSceneIn = false;
                }
                StartDrone.transform.position = SDPos;
            }
        }

        void SlotSelFunc()
        {
            isSceneIn = true;
            if (GlobalData.choi_StageList[GlobalData.choi_IsPick] != 0)
            {
                for (int i = 0; i <GlobalData.choi_m_TrList.Count; i++)
                {
                    GlobalData.choi_m_TrList[i].UpgradeLv = GlobalData.choi_m_SaveTrList[GlobalData.choi_IsPick, i];
                    if (GlobalData.choi_m_TrList[i].UpgradeLv >= 1)
                        SwitchManager.SetSwitch(SwitchManager.switchName[i], true);
                    else
                        SwitchManager.SetSwitch(SwitchManager.switchName[i], false);
                }
                GlobalData.choi_Stage = GlobalData.choi_StageList[GlobalData.choi_IsPick];
                GlobalData.choi_userDia = GlobalData.choi_DiamondList[GlobalData.choi_IsPick];
            }
            else
            {
                for (int i = 0; i < GlobalData.choi_m_TrList.Count; i++)
                {
                    GlobalData.choi_m_TrList[i].UpgradeLv = 0;
                }
                GlobalData.choi_Stage = 101;
                GlobalData.choi_userDia = 0;
            }
        }

        void SceneOutUpdate()
		{
            if (isSceneOut == false)
                return;

            if (isSceneIn == true)
            {
                a_DroneCtrl.value = 0.0f;
                isSceneIn = false;
            }

            if (droneLeave)
			{
                SDPos = StartDrone.transform.position;
                SDPos.y -= Time.deltaTime * m_DroneSpeed;
                if (SDPos.y <= StartBtnPos.y)
                {
                    SDPos.y = StartBtnPos.y;
                    StartDrone.transform.position = StartBtnPos;
                    
                    if(m_SceneName == stage1PickSceneName)
					{
                        ShopBtn.gameObject.SetActive(false);
                        OptionBtn.gameObject.SetActive(false);
                        DroneBtn[1].gameObject.SetActive(true);
                        DroneBtn[2].gameObject.SetActive(true);
                    }
                    else if(m_SceneName == shoeSceneName)
					{
                        StartBtn.gameObject.SetActive(false);
                        OptionBtn.gameObject.SetActive(false);
                        DroneBtn[0].gameObject.SetActive(true);
                        DroneBtn[2].gameObject.SetActive(true);
                    }
                    else
					{
                        StartBtn.gameObject.SetActive(false);
                        ShopBtn.gameObject.SetActive(false);
                        OptionBtn.gameObject.SetActive(false);
                        for (int ii = 0; ii < DroneBtn.Length; ii++)
                        {
                            DroneBtn[ii].gameObject.SetActive(true);
                        }
                    }

                    
                    a_DroneCtrl.value = 0.0f;
                    droneLeave = false;
                }
                StartDrone.transform.position = SDPos;
            }
            else
            {
                a_DroneCtrl.MoveDrone(StartBtnPos, DroneStartPos, false);
                if (StartDrone.transform.position.y >= 7.6f && m_SceneName != null)
				{
                    SceneManager.LoadScene(m_SceneName);
				}
            }
        }

        
        public void SoundPlay(ref AudioClip clip) //인터페이스 명시적 구현
		{
			if (audio == null) return; //AudioSource를 가져오지 않았을 경우를 대비한 예외 처리

			audio.Stop(); //혹시 아직 재생 중인 효과음이 있을 경우 정지

            if(GlobalData.volumeisOn)
                audio.PlayOneShot(clip, GlobalData.masterVolume); //소리를 재생한다.
        }

    }
}
