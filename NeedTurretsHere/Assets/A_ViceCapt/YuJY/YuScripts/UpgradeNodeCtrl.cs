using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Altair;

namespace Yuspace
{
    public class UpgradeNodeCtrl : MonoBehaviour
    {
        [HideInInspector] public UnitType m_UniType = UnitType.Rocket;
        [HideInInspector] public UnitShopState m_UniState = UnitShopState.Lock;
        public Text m_UnitName;
        public int m_UnitLevel;
        public Button m_UpgradeBtn;
        public Text UpgradeBtnText;
        public Image UpgradeImg;
        public Button m_UpNodeBtn;
        public Sprite[] LevelSprite;
        public Image TurretImg;
        UnitInfoCtrl UnitInfoCtrl;
  

        private void Start()
        {
            if (m_UpgradeBtn != null)
                m_UpgradeBtn.onClick.AddListener(UpgradeBtnFunc);
            if (m_UpNodeBtn != null)
                m_UpNodeBtn.onClick.AddListener(UpNodeBtn);



            UpdateType(m_UniType);


        }
        
        public void UpdateType(UnitType a_UniType)
        {
            if (GlobalData.choi_m_TrList[(int)a_UniType].UpgradeLv <= 0)
                return;

            m_UniType = a_UniType;
            m_UnitLevel = GlobalData.choi_m_TrList[(int)a_UniType].UpgradeLv;
            m_UnitName.text = GlobalData.choi_m_TrList[(int)a_UniType].m_name + "(Lv " + m_UnitLevel + ")";
            UpgradeBtnText.text = GlobalData.choi_m_TrList[(int)a_UniType].m_upgradecost + " Dia";
            TurretImg.sprite = Resources.Load<Sprite>(GlobalData.choi_m_TrList[(int)a_UniType].m_iconRsc);
            if (GlobalData.choi_m_TrList[(int)a_UniType].UpgradeLv == GlobalValue.m_ShopDataList[(int)m_UniType].m_MaxLevel)      //유닛의 레벨이 맥스레벨까지 도달하면
            {
                UpgradeBtnText.text = "Max Level";
            }

            if (GlobalData.choi_m_TrList[(int)a_UniType].UpgradeLv <= GlobalValue.m_ShopDataList[(int)m_UniType].m_MaxLevel)
            {
                
                int LevelCnt = GlobalData.choi_m_TrList[(int)a_UniType].UpgradeLv;
                UpgradeImg.sprite = LevelSprite[LevelCnt - 1];

            }

        }

        public void SetState(UnitShopState a_UniState)
        {
            m_UniState = a_UniState;        //유저노드리스트에서 상태값을 받아온다.
        }

        void UpgradeBtnFunc()
        {
            StoreMgr.Inst.UpgradeUnit(m_UniType);
        }
        void UpNodeBtn()
        {
            UnitInfoCtrl = StoreMgr.Inst.UnitInfoobj.GetComponent<UnitInfoCtrl>();
            //if (UniCtrl.gameObject.activeSelf == false)
            //    UniCtrl.gameObject.SetActive(true);

            //UniCtrl.SetType(m_UniType);
            if (StoreMgr.Inst.UnitInfoobj.activeSelf == false)
                StoreMgr.Inst.UnitInfoobj.SetActive(true);
            UnitInfoCtrl.SetType(m_UniType);
        }
    }
}
