using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Altair;


namespace Yuspace
{
    public enum UnitShopState
    {
        Lock,
        BuyBefore,
        Active
    }

    public class UnitNodeCtrl : MonoBehaviour
    {
        [HideInInspector] public UnitType m_UniType = UnitType.Rocket;
        [HideInInspector] public UnitShopState m_UniState = UnitShopState.Lock;
        string[] TurretText = new string[(int)UnitType.UnCount];     //유닛타입의 갯수만큼 배열생성
        public Image UnitImg;
        public Text UnitName;
        public Text UnitAtt;
        public Text UnitPrice;
        public Button UnitInfoBtn;
        public Button UnitBuyBtn;
        public Text BuyText;
        public GameObject in_Poss;
        public GameObject LockPanel;


        UnitInfoCtrl UniCtrl;
        // Start is called before the first frame update
        void Start()
        {
            if (UnitInfoBtn != null)
                UnitInfoBtn.onClick.AddListener(UnitInfoFunc);

            if (UnitBuyBtn != null)
                UnitBuyBtn.onClick.AddListener(UnitBuyFunc);


        }

        // Update is called once per frame
        //void Update()
        //{

        //}


        public void InitData(UnitType a_UniType)
        {
            if (a_UniType < UnitType.Rocket || UnitType.UnCount <= a_UniType)
                return;

            m_UniType = a_UniType;
            UnitImg.sprite = Resources.Load<Sprite>(GlobalData.choi_m_TrList[(int)m_UniType].m_iconRsc);
            Debug.Log(GlobalData.choi_TurretNameList[(int)m_UniType]);
            UnitName.text = GlobalData.choi_m_TrList[(int)a_UniType].m_name;
            UnitAtt.text = "공격력 : " + GlobalData.choi_m_TrList[(int)a_UniType].m_dam.ToString();
            UnitPrice.text = GlobalData.choi_m_TrList[(int)a_UniType].m_buycost + "Dia";
            
        }

        public void SetState(UnitShopState a_UniState, int a_Price)
        {
            m_UniState = a_UniState;
            if (a_UniState == UnitShopState.Lock)
            {
                UnitName.color = new Color32(110, 110, 110, 255);
                UnitAtt.color = new Color32(110, 110, 110, 255);
                UnitPrice.color = new Color32(110, 110, 110, 255);
                UnitPrice.text = a_Price.ToString();
                if(LockPanel.activeSelf == false)
                    LockPanel.SetActive(true);
                BuyText.text = "잠금";
            }
            else if (a_UniState == UnitShopState.BuyBefore)
            {
                UnitName.color = new Color32(180, 180, 180, 255);
                UnitAtt.color = new Color32(180, 180, 180, 255);
                UnitPrice.color = new Color32(180, 180, 180, 255);
                UnitPrice.text = a_Price.ToString();
                if (LockPanel.activeSelf == true)
                    LockPanel.SetActive(false);
                BuyText.text = "구매";
            }
            else if (a_UniState == UnitShopState.Active)
            {
                UnitName.color = new Color32(255, 255, 255, 255);
                UnitAtt.color = new Color32(255, 255, 255, 255);
                UnitPrice.color = new Color32(255, 255, 255, 255);
                UnitPrice.text = a_Price.ToString();
                if (in_Poss.activeSelf == false)
                    in_Poss.SetActive(true);
                if (LockPanel.activeSelf == true)
                    LockPanel.SetActive(false);

                BuyText.text = "구매완료";

            }
        }


        void UnitInfoFunc()
        {
            //Debug.Log("인포창");
            //UniCtrl = GameObject.Find("UnitInfo").GetComponent<UnitInfoCtrl>();
            UniCtrl = StoreMgr.Inst.UnitInfoobj.GetComponent<UnitInfoCtrl>();
            //if (UniCtrl.gameObject.activeSelf == false)
            //    UniCtrl.gameObject.SetActive(true);

            //UniCtrl.SetType(m_UniType);
            if (StoreMgr.Inst.UnitInfoobj.activeSelf == false)
                StoreMgr.Inst.UnitInfoobj.SetActive(true);
            UniCtrl.SetType(m_UniType);
            

       
        }

        void UnitBuyFunc()      //유닛노드이 버튼을 누르면
        {

            StoreMgr.Inst.BuyUnit(m_UniType);

        }
    }
}


