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
    public enum UnitType            //�ͷ����� Ÿ��(���⼭ �ּ��� ���ָ� ������ �ش� �ͷ��� ����)
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

        public GameObject NoticeDialogObj;              //�˸��� ���̾�α� ������
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
        //JSON �����
        JSONObject a_MKJSON;
        JSONArray JArray;     //JSON �迭

        string buyRequestUrl = "";
        string goldUpdateUrl = "";
        string SaveLevelUrl = "";
        bool isNetworkLock = false;

        //--------------------------------�׽�Ʈ�� ��ư(���̾�)
        public Button TestMoneyBtn;
        //--------------------------------�׽�Ʈ�� ��ư(���̾�)


        //���� �߰�
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


            ChangeColor = new Color(1, 1, 1, 1);    //������ �������ֱ� ���� �÷���
            buyRequestUrl = "http://myturrets.dothome.co.kr//Buy_Request.php";
            //������ ���Ž� ��������
            goldUpdateUrl = "http://myturrets.dothome.co.kr/UpdateUserDia.php";

            //SaveLevelUrl = "http://appyzs.dothome.co.kr/ShopLevelLoad.php";
            //������ ���ý� ���� ���� ����

            UnitBtnImg = UnitBtn.GetComponent<Image>();
            UpgradeBtnImg = UpgradeBtn.GetComponent<Image>();



            StartInitList(GlobalData.choi_IsPick);

            //-------������ ��� �߰�----------

            for(int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)
            {
                Debug.Log("���� : " + a_SetLevel[ii]);
            }


            //-------�� Ȯ��
            GameMoney.text  = GlobalData.choi_userDia.ToString();
            //-------�� Ȯ��

           
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
            yield return a_www.SendWebRequest();    //������ �ö����� ����ϱ�...


            if (a_www.error == null) //������ ���� �ʾ��� �� ����
            {

                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string a_ReStr = enc.GetString(a_www.downloadHandler.data);
                //����Ϸᰡ �Ǹ� ��ü ����(��ü ���� �޾Ƽ� �����ϴ� ����� �ְ�,
                //m_SvMyPoint, m_BuyCrType �� ������ �����ϴ� ����� �ִ�.)
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

        //-----------------------------�׽�Ʈ�� �Լ�--------------------------------
        void StageParse()
        {
            //int Chapter = (int)GlobalData.choi_Stage;
            //
            //int a_Stage = (int)((GlobalData.choi_Stage - Chapter) * 100);

            int Chapter = GlobalData.choi_Stage / 100;

            int a_Stage = GlobalData.choi_Stage % 100;
            //= GlobalData.choi_Stage - Chapter;

            Debug.Log("������ " + Chapter + "-" + a_Stage + "�Դϴ�..");
        }
        //-----------------------------�׽�Ʈ�� �Լ�--------------------------------


        void StartInitList(int isPick)
        {
            //-------������ ��� �߰�----------
            if (GlobalData.choi_m_TrList.Count > 0)        //�̹� �۷ι������Ϳ� ����Ʈ�� ������ �Ǿ��ִٸ� ��ŸƮ�Լ����� ó��
            {
                //GlobalValue.InitData();
                GameObject a_UnitObj = null;
                UnitNodeCtrl a_UnitNode = null;
                GameObject a_UpgradeObj = null;
                UpgradeNodeCtrl a_UpNode = null;
                for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)  //������Ʈ�� �ͷ�Ÿ���� ������ŭ ����ִ�.
                {  //
                    a_UnitObj = Instantiate(m_UnitNodeObj) as GameObject;
                    a_UnitNode = a_UnitObj.GetComponent<UnitNodeCtrl>();

                    a_UnitNode.InitData(GlobalValue.m_ShopDataList[ii].m_UniType);

                    a_UnitObj.transform.SetParent(m_Unit_ScrollContent.transform, false);
                }
                RefreshUnitList();                           //��������Ʈ ����
                if (UpgradeShop.activeSelf == false)         //���׷��̵� ������ �����ֱ⶧���� ��� Ų���� �ʱ�ȭ�������ְ� �ٽ� ��
                {
                    UpgradeShop.SetActive(true);
                }

                for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ++ii)
                {
                    a_UpgradeObj = Instantiate(m_UpgradeNodeObj) as GameObject;
                    a_UpNode = a_UpgradeObj.GetComponent<UpgradeNodeCtrl>();
                    a_UpNode.UpdateType(GlobalValue.m_ShopDataList[ii].m_UniType);
                    a_UpgradeObj.transform.SetParent(m_Upgrade_ScrollContent.transform, false);
                    if (GlobalData.choi_m_TrList[ii].UpgradeLv >= GlobalValue.m_ShopDataList[ii].m_MaxLevel)      //�α����Ҷ� �ҷ��� ������ ���� ������ �ִ뷹���� �Ѿ��
                        GlobalData.choi_m_TrList[ii].UpgradeLv = GlobalValue.m_ShopDataList[ii].m_MaxLevel;       //������ ������ �ִ뷹���� ������ ����
                    a_SetLevel.Add(GlobalData.choi_m_SaveTrList[isPick,ii]);      //���ֵ��� ���������� ��������Ʈ�� �־��ش�.
                    Debug.Log("startInit" + a_SetLevel[ii]);
                }

                GlobalData.choi_userDia = GlobalData.choi_DiamondList[GlobalData.choi_IsPick];


                NullJson();     //JSON�� null�̸� �ʱ�ȭ

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
 
            if (UnitShop.activeSelf == true)        //ó�� ����Ʈ������ ���ַ��� Update���� ó�� ���־���ҵ� Json������ �� �ǰ��� ���� ����Ʈ ����
            {
                if (GlobalData.choi_m_TrList.Count <= 0)           //ó�� �۷ι� ���������͸���Ʈ�� ���ٸ� �ʱ�ȭ����
                {
                    
                    GlobalData.choi_InitData();         //��ü �ͷ��� ������ �ʱ�ȭ
                    GlobalValue.InitData();             //������ ���� �ͷ� Ÿ���� �ʱ�ȭ 
                    GameObject a_UnitObj = null;
                    UnitNodeCtrl a_UnitNode = null;
                    GameObject a_UpgradeObj = null;
                    UpgradeNodeCtrl a_UpNode = null;
                    for (int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)       //�ͷ� Ÿ���� ������ŭ�� ����
                    {
                        Debug.Log("zz");
                        a_UnitObj = Instantiate(m_UnitNodeObj) as GameObject;
                        a_UnitNode = a_UnitObj.GetComponent<UnitNodeCtrl>();

                        a_UnitNode.InitData(GlobalValue.m_ShopDataList[ii].m_UniType);

                        a_UnitObj.transform.SetParent(m_Unit_ScrollContent.transform, false);
                    }
                    RefreshUnitList();
                    if(UpgradeShop.activeSelf == false)         //���׷��̵� ������ �����ֱ⶧���� ��� Ų���� �ʱ�ȭ�������ְ� �ٽ� ��
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

                    NullJson();     //JSon���� null�̶�� ����Ʈ�� �������� �޾Ƽ� Json���� �ҷ���

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
            for(int ii = 0; ii < GlobalValue.m_ShopDataList.Count; ii++)                                 //�۷ι������� ����Ʈ�� ����� ������ ���׷��̵��帮��Ʈ�� ����
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


            //if (UnitBtnImg.sprite == SelectBtnOnOff[1]) //���ֹ�ư�� ��������Ʈ�� �������¶��
            //    UnitBtnImg.sprite = SelectBtnOnOff[0];  //������ �ٲ���

            //if (UpgradeBtnImg.sprite == SelectBtnOnOff[0])       //���׷��̵� ��ư�� �»��¶��
            //    UpgradeBtnImg.sprite = SelectBtnOnOff[1];       //������ �ٲ���

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


            //if (UnitBtnImg.sprite == SelectBtnOnOff[0]) //���ֹ�ư�� ��������Ʈ�� �»��¶��
            //    UnitBtnImg.sprite = SelectBtnOnOff[1];  //�������� �ٲ���

            //if (UpgradeBtnImg.sprite == SelectBtnOnOff[1])       //���׷��̵� ��ư�� �������¶��
            //    UpgradeBtnImg.sprite = SelectBtnOnOff[0];       //�·� �ٲ���

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

            UpgradeNodeRefresh();           //���׷��̵� ��ư�� ������ ������ �۷ι������� ������ ���׷��̵庯���� ����

        }


        public void BuyUnit(UnitType a_UniType)     //����� ����Ÿ���� �޾ƿ�
        {
            if(isNetworkLock == true)
            {
                ShowDlg("��� �� �ٽ� �õ��� �ּ���.");
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
                a_Mess = "��� ������ ���� �� �� �����ϴ�.";
            }
            else if(a_UniState == UnitShopState.BuyBefore)
            {
                if(GlobalData.choi_userDia < a_TurInfo.m_buycost)
                {
                    a_Mess = "�����ݾ��� �����մϴ�.";
                }
                else
                {
                    a_Mess = "���� �����Ͻðڽ��ϱ�?";
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
                ShowDlg("��� �� �ٽ� �õ��� �ּ���.");
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
                if (GlobalData.choi_m_TrList[(int)m_UniType].UpgradeLv >= GlobalValue.m_ShopDataList[(int)m_UniType].m_MaxLevel)      //�ƽ����� ���߿� ����
                    Mess = "�ִ� ����ġ �Դϴ�.";
                else if (GlobalData.choi_userDia < a_Price)
                    Mess = "�����ݾ��� �����մϴ�.";
                else
                {
                    Mess = "������ ���׷��̵带 �Ͻðڽ��ϱ�?";
                    isUpgradePoss = true;
                }    
            }
            if (isUpgradePoss == true)
                ShowDlg(Mess, true, TryUpgradeUnit);
            else
                ShowDlg(Mess);




        }


        void TryBuyUnit()       //��������� ���Ÿ��ϸ� ������ 0���� 1�� ���� 
        {
            bool a_BuyOk = false;
            TypeInfo a_UnitInfo = null;
            TurretList a_TurInfo= null;
            a_SetLevel.Clear();



            for (int ii =0; ii < GlobalValue.m_ShopDataList.Count; ii++)        //�ͷ�Ÿ���� ������ŭ
            {
                a_UnitInfo = GlobalValue.m_ShopDataList[ii];
                a_TurInfo = GlobalData.choi_m_TrList[ii];
                //�ͷ�Ÿ���� �ְ�
                a_SetLevel.Add(GlobalData.choi_m_SaveTrList[GlobalData.choi_IsPick , ii]);
                //������ ������ �������� int����Ʈ�� �־��ش�.
                if (ii != (int)m_UniType || GlobalData.choi_m_SaveTrList[GlobalData.choi_IsPick,ii] >= a_UnitInfo.m_MaxLevel)
                    continue;

                int a_Cost = a_TurInfo.m_buycost;            //���߿� ����

                if (GlobalData.choi_userDia < a_Cost)
                    continue;

                m_SvMyGold1 = GlobalData.choi_userDia;
                m_SvMyGold1 -= a_Cost;
                a_SetLevel[ii]++;   //������ �ø���
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

                    if (ss == GlobalData.choi_IsPick)        //���� ������ �����϶��� ������ �����ϰ� 
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
                        Debug.Log("Ȯ��");
                        for (int ii = 0; ii < a_SetLevel.Count; ii++)   //������ ������ �ƴ϶�� �׳� ����Ǿ��ִ� ������ �־��ش�.
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
            //���Ž� ����,���̾� �������� ����
            if (string.IsNullOrEmpty(m_SvStrJson1) == true)
                yield break;
            WWWForm form = new WWWForm();
            form.AddField("Account", GlobalData.choi_UniqueID, System.Text.Encoding.UTF8);//System.Text.Encoding.UTF8 ���̵� �ѱ۷� �Է������� ������ �ʵ��� ��
            //form.AddField("Diamonds", m_SvMyGold);
            form.AddField("Sv-slot1", m_SvStrJson1, System.Text.Encoding.UTF8);
            form.AddField("Sv-slot2", m_SvStrJson2, System.Text.Encoding.UTF8);
            form.AddField("Sv-slot3", m_SvStrJson3, System.Text.Encoding.UTF8);


            isNetworkLock = true;

            UnityWebRequest a_www = UnityWebRequest.Post(buyRequestUrl, form);

            yield return a_www.SendWebRequest();

            if (a_www.error == null) //���� ������
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string a_ReStr = enc.GetString(a_www.downloadHandler.data);
                Debug.Log(a_ReStr);
                //������ �Ϸ�Ǹ� ��ü ����
                // ��ü���� ���� �����ϴ� ����� �ְ�m_SvMyPoint,m_BuyCrType�� ������ �����ϴ� ����� �ִ�.


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
            //���׷��̵�� ������ ���̾� �� ���� �������� ����
            if (string.IsNullOrEmpty(m_SvStrJson1) == true)
                yield break;
            WWWForm form = new WWWForm();
            form.AddField("Account", GlobalData.choi_UniqueID, System.Text.Encoding.UTF8);//System.Text.Encoding.UTF8 ���̵� �ѱ۷� �Է������� ������ �ʵ��� ��
            form.AddField("Diamonds", m_SvMyGold1);
            form.AddField("Inventory", m_SvStrJson1, System.Text.Encoding.UTF8);

            isNetworkLock = true;

            UnityWebRequest a_www = UnityWebRequest.Post(buyRequestUrl, form);

            yield return a_www.SendWebRequest();

            if (a_www.error == null) //���� ������
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string a_ReStr = enc.GetString(a_www.downloadHandler.data);

                //������ �Ϸ�Ǹ� ��ü ����
                // ��ü���� ���� �����ϴ� ����� �ְ�m_SvMyPoint,m_BuyCrType�� ������ �����ϴ� ����� �ִ�.


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
            //form.AddField("Input_user", GlobalData.choi_UniqueID, System.Text.Encoding.UTF8);//System.Text.Encoding.UTF8 ���̵� �ѱ۷� �Է������� ������ �ʵ��� ��
            //form.AddField("Item_List", m_SvStrJson, System.Text.Encoding.UTF8);

            //isNetworkLock = true;

            //UnityWebRequest a_www = UnityWebRequest.Post(SaveLevelUrl, form);

            //yield return a_www.SendWebRequest();

            //if (a_www.error == null) //���� ������
            //{
            //    System.Text.Encoding enc = System.Text.Encoding.UTF8;
            //    string a_ReStr = enc.GetString(a_www.downloadHandler.data);
            //    Debug.Log(a_ReStr);
            //    //������ �Ϸ�Ǹ� ��ü ����
            //    // ��ü���� ���� �����ϴ� ����� �ְ�m_SvMyPoint,m_BuyCrType�� ������ �����ϴ� ����� �ִ�.

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
            //ó�� JSON���Ͽ� �ƹ��͵� ������� ������
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

