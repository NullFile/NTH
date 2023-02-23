using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleJSON;
using UnityEngine.Networking;
using Altair;
using Enut4LJR;


namespace Yuspace
{
    public enum UnitType            //터렛들의 타입(여기서 주석을 없애면 상점에 해당 터렛이 생김)
    {
        Rocket = 0,
        Air_Strike = 1,
        Claymore = 2,
        Electric = 3,
        DoubleRocket = 4,
        Militia = 5,
        Flame = 6,
        Sniper = 7,
        EMP = 8,
        LittleBoy = 9,
        KamiKaze = 10,
        ContraTurret,
        IED,
        CarpetBombing,
        BoobyTrap,
        RecycleTurret,
        RichTurret,
        BalisticTurret,
        TowedTurret,
        GalaxyTurret,
        ATM,
        OutPost,
        Moneylender,
        Negotiator,
        Engineer,
        Constractor,
        Accelerator,
        Fortress,
        Barricade,
        AlienCollector,
        Terraforming,
        Protector,
        AATurret,
        BlackMarket,
        MultipleTurret,
        Bank,
        Pyromaniac,
        HomingTurret,
        ArcriteTurret,
        DebtCollecter,
        MetalTrap,
        ICBM,
        UnCount
    }


    public class StoreMgr : MonoBehaviour
    {
        public static StoreMgr Inst = null;
        public Button UnitBtn;
        public Text UnitBtnTxt;
        public Button UpgradeBtn;
        public Text UpgradeBtnTxt;
        public GameObject m_Unit_ScrollContent;
        public GameObject m_UnitNodeObj;
        Color ChangeColor;

        UnitNodeCtrl[] m_UnitNodeList;
        public GameObject UnitInfoobj;
        public GameObject UnitShop;
        public GameObject UpgradeShop;
        Image UnitBtnImg;
        Image UpgradeBtnImg;

        public GameObject m_Upgrade_ScrollContent;
        public GameObject m_UpgradeNodeObj;
        [HideInInspector] public UpgradeNodeCtrl[] m_UpgradeNodeList;
        //public Button TestBtn;

        public GameObject NoticeDialogObj;              //알림용 다이얼로그 프리팹
        public GameObject Canvas;
        public Text GameMoney;
        UnitType m_UniType;
        string m_SvStrJson1 = "";
        string m_SvStrJson2 = "";
        string m_SvStrJson3 = "";

        int m_SvMyGold1 = 0;
        int m_SvMyGold2 = 0;
        int m_SvMyGold3 = 0;

        List<int> a_SetLevel = new List<int>();
        //JSON 만들기
        JSONObject a_MKJSON;
        JSONArray JArray;     //JSON 배열

        string buyRequestUrl = "";
        string goldUpdateUrl = "";
        string SaveLevelUrl = "";
        bool isNetworkLock = false;

        //--------------------------------테스트용 버튼(다이아)
        public Button TestMoneyBtn;
        //--------------------------------테스트용 버튼(다이아)


        //반장 추가
        [Header("Back to Lobby")]
        [SerializeField] private Button backToLobby;
        [SerializeField] private string lobbySceneName;

        private void Awake()
        {
            if (Inst == null)
                Inst = this;
            else if(Inst != this)
                Destroy(this.gameObject);
        }
        // Start is called before the first frame update
        void Start()
        {
            if (UnitBtn != null)
                UnitBtn.onClick.AddListener(UnitBtnFunc);
            if (UpgradeBtn != null)
                UpgradeBtn.onClick.AddListener(UpgradeBtnFunc);
            //if (TestBtn != null)
            //    TestBtn.onClick.AddListener(() =>
            //    {
            //        SceneManager.LoadScene("TestLobby");
            //    });

            if (backToLobby != null)
                backToLobby.onClick.AddListener(()=> SceneManager.LoadScene(lobbySceneName));

            if (TestMoneyBtn != null)
            {
                TestMoneyBtn.onClick.AddListener(() =>
                {
                    if (GlobalData.choi_IsPick == -1)
                        return;

                    GlobalData.choi_userDia += 1000;
                    if (GlobalData.choi_IsPick == 1)
                        NetworkMgr.Inst.PushPacket(PacketType.Dia1Update);
                    else if(GlobalData.choi_IsPick == 2)
                        NetworkMgr.Inst.PushPacket(PacketType.Dia2Update);
                    else if(GlobalData.choi_IsPick == 3)
                        NetworkMgr.Inst.PushPacket(PacketType.Dia3Update);

                    GameMoney.text = GlobalData.choi_userDia.ToString();
                    //GlobalData.choi_Stage += 1;
                    //StartCoroutine(GoldFunc());
                });
            }


            ChangeColor = new Color(1, 1, 1, 1);    //색상값을 변경해주기 위한 컬러값
            buyRequestUrl = "http://myturrets.dothome.co.kr//Buy_Request.php";
            //아이템 구매시 레벨저장
            goldUpdateUrl = "http://myturrets.dothome.co.kr/UpdateUserDia.php";

            //SaveLevelUrl = "http://appyzs.dothome.co.kr/ShopLevelLoad.php";
            //상점에 들어올시 현재 레벨 저장

            UnitBtnImg = UnitBtn.GetComponent<Image>();
            UpgradeBtnImg = UpgradeBtn.GetComponent<Image>();



            StartInitList(GlobalData.choi_IsPick);

            //-------아이템 목록 추가----------

            for(int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)
            {
                Debug.Log("레벨 : " + a_SetLevel[ii]);
            }


            //-------돈 확인
            GameMoney.text  = GlobalData.choi_userDia.ToString();
            //-------돈 확인

           
        }

        IEnumerator GoldFunc()
        {
            WWWForm form = new WWWForm();

            Debug.Log(GlobalData.choi_UniqueID);

            form.AddField("Account", GlobalData.choi_UniqueID,
                                    System.Text.Encoding.UTF8);
            form.AddField("Diamonds", GlobalData.choi_userDia);
            form.AddField("Stage", GlobalData.choi_Stage.ToString());

            UnityWebRequest a_www = UnityWebRequest.Post(goldUpdateUrl, form);
            yield return a_www.SendWebRequest();    //응답이 올때까지 대기하기...


            if (a_www.error == null) //에러가 나지 않았을 때 동작
            {

                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string a_ReStr = enc.GetString(a_www.downloadHandler.data);
                //응답완료가 되면 전체 갱신(전체 값을 받아서 갱신하는 방법이 있고,
                //m_SvMyPoint, m_BuyCrType 를 가지고 갱신하는 방법이 있다.)
                Debug.Log(a_ReStr);
                if (a_ReStr.Contains("UpDateSuccess~") == true)
                {
                    GameMoney.text = GlobalData.choi_userDia.ToString();
                    StageParse();
                }
                else
                    Debug.Log(a_ReStr);


                
            }
            else
            {
                Debug.Log(a_www.error);
            }
        }

        //-----------------------------테스트용 함수--------------------------------
        void StageParse()
        {
            //int Chapter = (int)GlobalData.choi_Stage;
            //
            //int a_Stage = (int)((GlobalData.choi_Stage - Chapter) * 100);

            int Chapter = GlobalData.choi_Stage / 100;

            int a_Stage = GlobalData.choi_Stage % 100;
            //= GlobalData.choi_Stage - Chapter;

            Debug.Log("지금은 " + Chapter + "-" + a_Stage + "입니다..");
        }
        //-----------------------------테스트용 함수--------------------------------


        void StartInitList(int isPick)
        {
            //-------아이템 목록 추가----------
            if (GlobalData.choi_m_TrList.Count > 0)        //이미 글로벌데이터에 리스트가 갱신이 되어있다면 스타트함수에서 처리
            {
                //GlobalValue.InitData();
                GameObject a_UnitObj = null;
                UnitNodeCtrl a_UnitNode = null;
                GameObject a_UpgradeObj = null;
                UpgradeNodeCtrl a_UpNode = null;
                for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)  //샵리스트는 터렛타입의 갯수만큼 들어있다.
                {  //
                    a_UnitObj = Instantiate(m_UnitNodeObj) as GameObject;
                    a_UnitNode = a_UnitObj.GetComponent<UnitNodeCtrl>();

                    a_UnitNode.InitData(GlobalValue.m_ShopDataList[ii].m_UniType);

                    a_UnitObj.transform.SetParent(m_Unit_ScrollContent.transform, false);
                }
                RefreshUnitList();                           //상점리스트 갱신
                if (UpgradeShop.activeSelf == false)         //업그레이드 상점은 꺼져있기때문에 잠깐 킨다음 초기화갱신해주고 다시 끔
                {
                    UpgradeShop.SetActive(true);
                }

                for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ++ii)
                {
                    a_UpgradeObj = Instantiate(m_UpgradeNodeObj) as GameObject;
                    a_UpNode = a_UpgradeObj.GetComponent<UpgradeNodeCtrl>();
                    a_UpNode.UpdateType(GlobalValue.m_ShopDataList[ii].m_UniType);
                    a_UpgradeObj.transform.SetParent(m_Upgrade_ScrollContent.transform, false);
                    if (GlobalData.choi_m_TrList[ii].UpgradeLv >= GlobalValue.m_ShopDataList[ii].m_MaxLevel)      //로그인할때 불러온 레벨의 값이 유닛의 최대레벨을 넘어가면
                        GlobalData.choi_m_TrList[ii].UpgradeLv = GlobalValue.m_ShopDataList[ii].m_MaxLevel;       //유닛의 값들을 최대레벨의 값으로 변경
                    a_SetLevel.Add(GlobalData.choi_m_SaveTrList[isPick,ii]);      //유닛들의 레벨값들을 레벨리스트에 넣어준다.
                    Debug.Log("startInit" + a_SetLevel[ii]);
                }

                GlobalData.choi_userDia = GlobalData.choi_DiamondList[GlobalData.choi_IsPick];


                NullJson();     //JSON이 null이면 초기화

                if (UpgradeShop.activeSelf == true)
                    UpgradeShop.SetActive(false);


                StartCoroutine(SaveLevelCo());
            }
        }

        public void RefreshUnitList()
        {
            if (m_Unit_ScrollContent != null)
            {
                if (m_UnitNodeList == null || m_UnitNodeList.Length <= 0)
                    m_UnitNodeList = m_Unit_ScrollContent.GetComponentsInChildren<UnitNodeCtrl>();
            }

            int a_FindAv = -1;
            for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)
            {

                if (m_UnitNodeList[ii].m_UniType != GlobalValue.m_ShopDataList[ii].m_UniType)
                    continue;

                if (0 < GlobalData.choi_m_TrList[ii].UpgradeLv)
                {
                    m_UnitNodeList[ii].SetState(UnitShopState.Active, GlobalData.choi_m_TrList[ii].m_buycost);
                    continue;
                }

                if (a_FindAv < 0)
                {
                    m_UnitNodeList[ii].SetState(UnitShopState.BuyBefore, GlobalData.choi_m_TrList[ii].m_buycost);

                    a_FindAv = ii;
                }
                else
                {
                    m_UnitNodeList[ii].SetState(UnitShopState.Lock, GlobalData.choi_m_TrList[ii].m_buycost);
                }
                //Debug.Log(m_UnitNodeList[ii].m_UniState);

                if (GameMoney != null)
                    GameMoney.text = m_SvMyGold1.ToString();
            }

            GameMoney.text = GlobalData.choi_userDia.ToString();

        }

        // Update is called once per frame
        void Update()
        {
 
            if (UnitShop.activeSelf == true)        //처음 리스트갱신을 해주려면 Update에서 처음 해주어야할듯 Json설정이 다 되고나서 그후 리스트 갱신
            {
                if (GlobalData.choi_m_TrList.Count <= 0)           //처음 글로벌 유저데이터리스트가 없다면 초기화갱신
                {
                    
                    GlobalData.choi_InitData();         //전체 터렛의 정보를 초기화
                    GlobalValue.InitData();             //상점을 위한 터렛 타입을 초기화 
                    GameObject a_UnitObj = null;
                    UnitNodeCtrl a_UnitNode = null;
                    GameObject a_UpgradeObj = null;
                    UpgradeNodeCtrl a_UpNode = null;
                    for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)       //터렛 타입의 갯수만큼만 생성
                    {
                        Debug.Log("zz");
                        a_UnitObj = Instantiate(m_UnitNodeObj) as GameObject;
                        a_UnitNode = a_UnitObj.GetComponent<UnitNodeCtrl>();

                        a_UnitNode.InitData(GlobalValue.m_ShopDataList[ii].m_UniType);

                        a_UnitObj.transform.SetParent(m_Unit_ScrollContent.transform, false);
                    }
                    RefreshUnitList();
                    if(UpgradeShop.activeSelf == false)         //업그레이드 상점은 꺼져있기때문에 잠깐 킨다음 초기화갱신해주고 다시 끔
                    {
                        UpgradeShop.SetActive(true);
                    }
 
                    for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ++ii)
                    {
                        a_UpgradeObj = Instantiate(m_UpgradeNodeObj) as GameObject;
                        a_UpNode = a_UpgradeObj.GetComponent<UpgradeNodeCtrl>();
                        a_UpNode.UpdateType(GlobalValue.m_ShopDataList[ii].m_UniType);
                        a_UpgradeObj.transform.SetParent(m_Upgrade_ScrollContent.transform, false);
                    }

                    //if(a_MKJSON == null)
                    //{
                    //    for(int ii = 0; ii < a_SetLevel.Count; ii++)
                    //    {
                    //        a_MKJSON = new JSONObject();
                    //        JArray = new JSONArray();
                    //        for (int i = 0; i < a_SetLevel.Count; i++)
                    //        {
                    //            JArray.Add(a_SetLevel[i]);
                    //        }
                    //        a_MKJSON.Add("TrList", JArray);
                    //    }
                    //}

                    if (UpgradeShop.activeSelf == true)
                        UpgradeShop.SetActive(false);

                    NullJson();     //JSon값이 null이라면 리스트의 레벨값을 받아서 Json값을 불러옴

                }


            }



            //Debug.Log(GlobalValue.g_UserGold);

        }



        void UpgradeNodeRefresh()
        {
            if(m_Upgrade_ScrollContent != null)
            {
                if(m_UpgradeNodeList == null || m_UpgradeNodeList.Length <= 0)
                {
                    m_UpgradeNodeList = m_Upgrade_ScrollContent.GetComponentsInChildren<UpgradeNodeCtrl>();
                }
            }
            for(int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)                                 //글로벌변수인 리스트에 저장된 값들을 업그레이드노드리스트에 연결
            {
                m_UpgradeNodeList[ii].UpdateType(GlobalValue.m_ShopDataList[ii].m_UniType);
                m_UpgradeNodeList[ii].SetState(m_UnitNodeList[ii].m_UniState);
            }

            for(int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)
            {
                if (m_UpgradeNodeList[ii].m_UniState != UnitShopState.Active)
                {
                    if(m_UpgradeNodeList[ii].gameObject.activeSelf == true)
                        m_UpgradeNodeList[ii].gameObject.SetActive(false);

                }
                else
                {
                    if (m_UpgradeNodeList[ii].gameObject.activeSelf == false)
                        m_UpgradeNodeList[ii].gameObject.SetActive(true);
                }
            }

            GameMoney.text = GlobalData.choi_userDia.ToString();

        }

        void UnitBtnFunc()
        {
            UnitBtn.image.color = new Color(1, 1, 1, 1);

            if (UnitShop.activeSelf == false)
                UnitShop.SetActive(true);

            if (UpgradeShop.activeSelf == true)
                UpgradeShop.SetActive(false);

            if (UnitInfoobj.activeSelf == true)
                UnitInfoobj.SetActive(false);


            //if (UnitBtnImg.sprite == SelectBtnOnOff[1]) //유닛버튼의 스프라이트가 오프상태라면
            //    UnitBtnImg.sprite = SelectBtnOnOff[0];  //온으로 바꿔줌

            //if (UpgradeBtnImg.sprite == SelectBtnOnOff[0])       //업그레이드 버튼이 온상태라면
            //    UpgradeBtnImg.sprite = SelectBtnOnOff[1];       //오프로 바꿔줌

            if(UnitBtnImg.color.a <= 0.5f)
            {
                ChangeColor.a = UnitBtnImg.color.a;
                ChangeColor.a = 1;
                UnitBtnImg.color = ChangeColor;
            }

            if(UnitBtnTxt.color.a  <= 0.5f)
            {
                ChangeColor.a = UnitBtnTxt.color.a;
                ChangeColor.a = 1;
                UnitBtnTxt.color = ChangeColor;
            }

            if (UpgradeBtnImg.color.a > 0.5f)
            {
                ChangeColor.a = UpgradeBtnImg.color.a;
                ChangeColor.a = 0.5f;
                UpgradeBtnImg.color = ChangeColor;
            }

            if (UpgradeBtnTxt.color.a > 0.5f)
            {
                ChangeColor.a = UpgradeBtnTxt.color.a;
                ChangeColor.a = 0.5f;
                UpgradeBtnTxt.color = ChangeColor;
            }

        }


        void UpgradeBtnFunc()
        {
            UpgradeBtn.image.color = new Color(1, 1, 1, 1);


            if (UnitShop.activeSelf == true)
                UnitShop.SetActive(false);

            if (UpgradeShop.activeSelf == false)
                UpgradeShop.SetActive(true);

            if (UnitInfoobj.activeSelf == true)
                UnitInfoobj.SetActive(false);


            //if (UnitBtnImg.sprite == SelectBtnOnOff[0]) //유닛버튼의 스프라이트가 온상태라면
            //    UnitBtnImg.sprite = SelectBtnOnOff[1];  //오프으로 바꿔줌

            //if (UpgradeBtnImg.sprite == SelectBtnOnOff[1])       //업그레이드 버튼이 오프상태라면
            //    UpgradeBtnImg.sprite = SelectBtnOnOff[0];       //온로 바꿔줌

            if (UpgradeBtnImg.color.a <= 0.5f)
            {
                ChangeColor.a = UpgradeBtnImg.color.a;
                ChangeColor.a = 1;
                UpgradeBtnImg.color = ChangeColor;
            }

            if (UpgradeBtnTxt.color.a <= 0.5f)
            {
                ChangeColor.a = UpgradeBtnTxt.color.a;
                ChangeColor.a = 1;
                UpgradeBtnTxt.color = ChangeColor;
            }


            if (UnitBtnImg.color.a > 0.5f)
            {
                ChangeColor.a = UnitBtnImg.color.a;
                ChangeColor.a = 0.5f;
                UnitBtnImg.color = ChangeColor;
            }

            if (UnitBtnTxt.color.a > 0.5f)
            {
                ChangeColor.a = UnitBtnTxt.color.a;
                ChangeColor.a = 0.5f;
                UnitBtnTxt.color = ChangeColor;
            }

            UpgradeNodeRefresh();           //업그레이드 버튼을 누르면 저장한 글로벌변수의 값들을 업그레이드변수에 적용

        }


        public void BuyUnit(UnitType a_UniType)     //노드의 유닛타입을 받아옴
        {
            if(isNetworkLock == true)
            {
                ShowDlg("잠시 후 다시 시도해 주세요.");
                return;
            }

            if (a_UniType < 0 || GlobalValue.m_ShopDataList.Count <= (int)a_UniType)
                return;


            m_UniType = a_UniType;
            string a_Mess = "";
            UnitShopState a_UniState = UnitShopState.Lock;
            bool isBuyPoss = false;
            bool isDlgType = false;
            TypeInfo a_UniInfo = GlobalValue.m_ShopDataList[(int)a_UniType];
            TurretList a_TurInfo = GlobalData.choi_m_TrList[(int)a_UniType];
            if(m_UnitNodeList != null && (int)m_UniType < m_UnitNodeList.Length)
            {
                a_UniState = m_UnitNodeList[(int)m_UniType].m_UniState;
            }
            if(a_UniState == UnitShopState.Lock)
            {
                a_Mess = "잠긴 유닛은 구입 할 수 없습니다.";
            }
            else if(a_UniState == UnitShopState.BuyBefore)
            {
                if(GlobalData.choi_userDia < a_TurInfo.m_buycost)
                {
                    a_Mess = "보유금액이 부족합니다.";
                }
                else
                {
                    a_Mess = "정말 구입하시겠습니까?";
                    isBuyPoss = true;
                }


            }
            if (isBuyPoss == true)
            {
                isDlgType = true;
                ShowDlg(a_Mess, isDlgType, TryBuyUnit);

            }

            else
                ShowDlg(a_Mess);

        }

        public void UpgradeUnit(UnitType a_UniType)
        {
            if (isNetworkLock == true)
            {
                ShowDlg("잠시 후 다시 시도해 주세요.");
                return;
            }

            if (a_UniType < 0 || GlobalValue.m_ShopDataList.Count <= (int)a_UniType)
                return;


            m_UniType = a_UniType;
            string Mess = "";
            UnitShopState a_UniState = UnitShopState.Lock;
            bool isUpgradePoss = false;
            TypeInfo a_UniInfo = GlobalValue.m_ShopDataList[(int)m_UniType];
            TurretList a_TurList = GlobalData.choi_m_TrList[(int)m_UniType];
            if(m_UpgradeNodeList != null && (int)m_UniType <m_UpgradeNodeList.Length)
            {
                a_UniState = m_UpgradeNodeList[(int)a_UniType].m_UniState;
            }
            if(a_UniState == UnitShopState.Active)
            {
                int a_Price = a_TurList.m_upgradecost;
                if (GlobalData.choi_m_TrList[(int)m_UniType].UpgradeLv >= GlobalValue.m_ShopDataList[(int)m_UniType].m_MaxLevel)      //맥스레벨 나중에 수정
                    Mess = "최대 레벨치 입니다.";
                else if (GlobalData.choi_userDia < a_Price)
                    Mess = "보유금액이 부족합니다.";
                else
                {
                    Mess = "정말로 업그레이드를 하시겠습니까?";
                    isUpgradePoss = true;
                }    
            }
            if (isUpgradePoss == true)
                ShowDlg(Mess, true, TryUpgradeUnit);
            else
                ShowDlg(Mess);




        }


        void TryBuyUnit()       //결과적으로 구매를하면 레벨이 0에서 1로 오름 
        {
            bool a_BuyOk = false;
            TypeInfo a_UnitInfo = null;
            TurretList a_TurInfo= null;
            a_SetLevel.Clear();



            for (int ii =0; ii < GlobalValue.m_ShopDataList.Count; ii++)        //터렛타입의 갯수만큼
            {
                a_UnitInfo = GlobalValue.m_ShopDataList[ii];
                a_TurInfo = GlobalData.choi_m_TrList[ii];
                //터렛타입을 넣고
                a_SetLevel.Add(GlobalData.choi_m_SaveTrList[GlobalData.choi_IsPick , ii]);
                //선택한 슬롯의 레벨들을 int리스트에 넣어준다.
                if (ii != (int)m_UniType || GlobalData.choi_m_SaveTrList[GlobalData.choi_IsPick,ii] >= a_UnitInfo.m_MaxLevel)
                    continue;

                int a_Cost = a_TurInfo.m_buycost;            //나중에 수정

                if (GlobalData.choi_userDia < a_Cost)
                    continue;

                m_SvMyGold1 = GlobalData.choi_userDia;
                m_SvMyGold1 -= a_Cost;
                a_SetLevel[ii]++;   //레벨을 올리고
                GlobalData.choi_m_SaveTrList[GlobalData.choi_IsPick, ii]++;
                GlobalData.choi_DiamondList[GlobalData.choi_IsPick] = m_SvMyGold1;
                SwitchManager.SetSwitch(GlobalData.choi_TurretNameList[(int)m_UniType], true);
                Debug.Log(SwitchManager.GetSwitch(GlobalData.choi_TurretNameList[(int)m_UniType]));
                a_BuyOk = true;
            }

            if(a_BuyOk == true)
            {
                if (a_SetLevel.Count <= 0)
                    return;
                if (a_SetLevel.Count != GlobalValue.m_ShopDataList.Count)
                    return;

                Debug.Log(a_SetLevel.Count);



                for (int ss = 1; ss < 4; ss++)
                {
                    a_MKJSON = new JSONObject();
                    JArray = new JSONArray();

                    if (ss == GlobalData.choi_IsPick)        //내가 선택한 슬롯일때는 레벨을 저장하고 
                    {
                        for (int ii = 0; ii < a_SetLevel.Count; ii++)
                        {
                            Debug.Log(JArray);

                            JArray.Add(a_SetLevel[ii]);
                        }
                        a_MKJSON.Add("TrList", JArray);
                        a_MKJSON.Add("Diamonds", GlobalData.choi_DiamondList[GlobalData.choi_IsPick]);
                        a_MKJSON.Add("Stage", GlobalData.choi_StageList[GlobalData.choi_IsPick]);
                        JsonPush(ss);
                        
                        

                    }
                    else
                    {
                        Debug.Log("확인");
                        for (int ii = 0; ii < a_SetLevel.Count; ii++)   //선택한 슬롯이 아니라면 그냥 저장되어있는 값들을 넣어준다.
                        {
                            JArray.Add(GlobalData.choi_m_SaveTrList[ss,ii]);
                        }
                        a_MKJSON.Add("TrList", JArray);
                        a_MKJSON.Add("Diamonds", GlobalData.choi_DiamondList[ss]);
                        a_MKJSON.Add("Stage", GlobalData.choi_StageList[ss]);
                        JsonPush(ss);

                    }
                }

                //for(int ii = 0; ii < 3; ii++)
                //{
                //    Debug.Log("1" + m_SvStrJson1);
                //    Debug.Log("2" + m_SvStrJson2);
                //    Debug.Log("3" + m_SvStrJson3);

                //}


                StartCoroutine(BuyRequestCo());


            }

            
        }



        void JsonPush(int idx)
        {
            if(idx == 1)
            {
                m_SvStrJson1 = a_MKJSON.ToString();
            }

            if(idx == 2)
            {
                m_SvStrJson2 = a_MKJSON.ToString();
            }
            
            if(idx == 3)
            {
                m_SvStrJson3 = a_MKJSON.ToString();
            }
        }

        void TryUpgradeUnit()
        {

            bool a_BuyOk = false;
            TypeInfo a_UnitInfo = null;
            TurretList a_TurInfo = null;
    

            for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)
            {

                a_UnitInfo = GlobalValue.m_ShopDataList[ii];
                a_TurInfo = GlobalData.choi_m_TrList[ii];

                //a_SetLevel.Add(a_UnitInfo.m_Level);

                if (ii != (int)m_UniType || GlobalValue.m_ShopDataList[(int)m_UniType].m_MaxLevel <= GlobalData.choi_m_TrList[(int)m_UniType].UpgradeLv)
                    continue;

                int a_Cost = a_TurInfo.m_upgradecost;

                if (GlobalData.choi_userDia < a_Cost)
                    continue;

                m_SvMyGold1 = GlobalData.choi_userDia;
                m_SvMyGold1 -= a_Cost;

                a_SetLevel[ii]++;
                if(a_SetLevel[ii] > GlobalValue.m_ShopDataList[ii].m_MaxLevel)
                {
                    a_SetLevel[ii] = GlobalValue.m_ShopDataList[ii].m_MaxLevel;
                }

                a_BuyOk = true;
            }

            if (a_BuyOk == true)
            {
                if (a_SetLevel.Count <= 0)
                    return;
                if (a_SetLevel.Count != GlobalValue.m_ShopDataList.Count)
                    return;

                for (int ii = 0; ii < a_SetLevel.Count; ii++)
                {
                    //JArray.Add(a_SetLevel[ii]);
                    a_MKJSON["TrList"][ii] = a_SetLevel[ii];
                }
                //a_MKJSON.Add("UnitList", JArray);
                m_SvStrJson1 = a_MKJSON.ToString();



                Debug.Log(m_SvStrJson1);
                StartCoroutine(UpgradeRequestCo());
            }

        }

        IEnumerator BuyRequestCo()
        {     
            //구매시 레벨,다이아 웹서버에 저장
            if (string.IsNullOrEmpty(m_SvStrJson1) == true)
                yield break;
            WWWForm form = new WWWForm();
            form.AddField("Account", GlobalData.choi_UniqueID, System.Text.Encoding.UTF8);//System.Text.Encoding.UTF8 아이디 한글로 입력했을때 깨지지 않도록 함
            //form.AddField("Diamonds", m_SvMyGold);
            form.AddField("Sv-slot1", m_SvStrJson1, System.Text.Encoding.UTF8);
            form.AddField("Sv-slot2", m_SvStrJson2, System.Text.Encoding.UTF8);
            form.AddField("Sv-slot3", m_SvStrJson3, System.Text.Encoding.UTF8);


            isNetworkLock = true;

            UnityWebRequest a_www = UnityWebRequest.Post(buyRequestUrl, form);

            yield return a_www.SendWebRequest();

            if (a_www.error == null) //에러 없으면
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string a_ReStr = enc.GetString(a_www.downloadHandler.data);
                Debug.Log(a_ReStr);
                //응답이 완료되면 전체 갱신
                // 전체값을 맏아 갱신하는 방법이 있고m_SvMyPoint,m_BuyCrType을 가지고 갱신하는 방법이 있다.


                if (a_ReStr.Contains("BuySuccess~") == true)
                    RefreshUnitCo();
            }
            else
                Debug.Log(a_www.error);

            isNetworkLock = false;
        }


        void RefreshUnitCo()
        {
            if (m_UniType < UnitType.Rocket || UnitType.UnCount <= m_UniType)
                return;

            GlobalData.choi_userDia = m_SvMyGold1;
            GlobalData.choi_m_TrList[(int)m_UniType].UpgradeLv = a_SetLevel[(int)m_UniType];
           

            RefreshUnitList();
        }


        IEnumerator UpgradeRequestCo()
        {
            //업그레이드시 차감된 다이아 및 레벨 웹서버에 저장
            if (string.IsNullOrEmpty(m_SvStrJson1) == true)
                yield break;
            WWWForm form = new WWWForm();
            form.AddField("Account", GlobalData.choi_UniqueID, System.Text.Encoding.UTF8);//System.Text.Encoding.UTF8 아이디 한글로 입력했을때 깨지지 않도록 함
            form.AddField("Diamonds", m_SvMyGold1);
            form.AddField("Inventory", m_SvStrJson1, System.Text.Encoding.UTF8);

            isNetworkLock = true;

            UnityWebRequest a_www = UnityWebRequest.Post(buyRequestUrl, form);

            yield return a_www.SendWebRequest();

            if (a_www.error == null) //에러 없으면
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string a_ReStr = enc.GetString(a_www.downloadHandler.data);

                //응답이 완료되면 전체 갱신
                // 전체값을 맏아 갱신하는 방법이 있고m_SvMyPoint,m_BuyCrType을 가지고 갱신하는 방법이 있다.


                if (a_ReStr.Contains("BuySuccess~") == true)
                    RefreshUpgradeCo();

            }
            else
                Debug.Log(a_www.error);

            isNetworkLock = false;

        }

        void RefreshUpgradeCo()
        {
            if (m_UniType < UnitType.Rocket || UnitType.UnCount <= m_UniType)
                return;

            GlobalData.choi_userDia = m_SvMyGold1;
            GlobalData.choi_m_TrList[(int)m_UniType].UpgradeLv = a_SetLevel[(int)m_UniType];

            UpgradeNodeRefresh();
        }

        IEnumerator SaveLevelCo()
        {
            if (string.IsNullOrEmpty(m_SvStrJson1) == true)
                yield break;


            //WWWForm form = new WWWForm();
            //form.AddField("Input_user", GlobalData.choi_UniqueID, System.Text.Encoding.UTF8);//System.Text.Encoding.UTF8 아이디 한글로 입력했을때 깨지지 않도록 함
            //form.AddField("Item_List", m_SvStrJson, System.Text.Encoding.UTF8);

            //isNetworkLock = true;

            //UnityWebRequest a_www = UnityWebRequest.Post(SaveLevelUrl, form);

            //yield return a_www.SendWebRequest();

            //if (a_www.error == null) //에러 없으면
            //{
            //    System.Text.Encoding enc = System.Text.Encoding.UTF8;
            //    string a_ReStr = enc.GetString(a_www.downloadHandler.data);
            //    Debug.Log(a_ReStr);
            //    //응답이 완료되면 전체 갱신
            //    // 전체값을 맏아 갱신하는 방법이 있고m_SvMyPoint,m_BuyCrType을 가지고 갱신하는 방법이 있다.

            //}
            //else
            //    Debug.Log(a_www.error);

            //isNetworkLock = false;
        }

        void ShowDlg(string a_Mess, bool DiaType = false,DialogCtrl.DLT_Res a_DltMtd = null)
        {
            if (string.IsNullOrEmpty(a_Mess))
                return;


            if (NoticeDialogObj.activeSelf == false)
                NoticeDialogObj.SetActive(true);

            DialogCtrl DiaCtrl = NoticeDialogObj.GetComponent<DialogCtrl>();
            if(DiaCtrl != null)
            {
                DiaCtrl.MessageSet(a_Mess,45, a_DltMtd);
                if(DiaType == false)
                {
                    if (DiaCtrl.NoticeSelect.activeSelf == false)
                        DiaCtrl.NoticeSelect.SetActive(true);
                    if (DiaCtrl.BuySelect.activeSelf == true)
                        DiaCtrl.BuySelect.SetActive(false);
                }
                else
                {
                    if (DiaCtrl.BuySelect.activeSelf == false)
                        DiaCtrl.BuySelect.SetActive(true);
                    if (DiaCtrl.NoticeSelect.activeSelf == true)
                        DiaCtrl.NoticeSelect.SetActive(false);
      
                }
            }
        }

        void NullJson()
        {
            //처음 JSON파일에 아무것도 들어있지 않을때
            if (a_MKJSON == null)
            {
                a_SetLevel.Clear();
                a_MKJSON = new JSONObject();
                JArray = new JSONArray();

                for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)
                {
                    a_SetLevel.Add(GlobalData.choi_m_SaveTrList[GlobalData.choi_IsPick,ii]);
                }

                if (a_SetLevel.Count <= 0)
                    return;
                if (a_SetLevel.Count != GlobalValue.m_ShopDataList.Count)
                    return;

                for (int ii = 0; ii < a_SetLevel.Count; ii++)
                {
                    JArray.Add(a_SetLevel[ii]);
                    Debug.Log(JArray[ii]);

                }
                a_MKJSON.Add("TrList_" + GlobalData.choi_IsPick, JArray);
                m_SvStrJson1 = a_MKJSON.ToString();
                Debug.Log(m_SvStrJson1);
            }
        }
    }


    
}

