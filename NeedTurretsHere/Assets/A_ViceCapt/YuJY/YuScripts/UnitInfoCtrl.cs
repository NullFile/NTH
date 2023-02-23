using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Altair;

namespace Yuspace
{
    public class UnitInfoCtrl : MonoBehaviour
    {
        [HideInInspector] public UnitType m_UniType = UnitType.Rocket;
        [HideInInspector] public UnitShopState m_UniState = UnitShopState.Lock;
        public Text UnitInfoName;
        public Text UnitInfoHp;
        public Text UnitInfoAtt;
        public Text UnitInfoCost;
        public Text UnitInfoArchive;
        public Button UnitBuyBtn;
        public Image UnitInfoImg;
        // Start is called before the first frame update



        public void SetType(UnitType a_UniType)
        {
            m_UniType = a_UniType;
            UnitInfoName.text = GlobalData.choi_m_TrList[(int)a_UniType].m_name;
            UnitInfoHp.text = "Hp : " + GlobalData.choi_m_TrList[(int)a_UniType].m_hp.ToString();
            UnitInfoAtt.text = "Att : " + GlobalData.choi_m_TrList[(int)a_UniType].m_dam.ToString();
            UnitInfoCost.text = "Cost : " + GlobalData.choi_m_TrList[(int)a_UniType].m_cost.ToString();
            UnitInfoArchive.text = GlobalData.choi_m_TrList[(int)a_UniType].m_Archive;
            UnitInfoImg.sprite = Resources.Load<Sprite>(GlobalData.choi_m_TrList[(int)a_UniType].m_iconRsc);

        }
    }
}
