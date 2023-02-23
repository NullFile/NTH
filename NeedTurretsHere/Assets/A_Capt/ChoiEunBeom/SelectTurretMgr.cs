using UnityEngine;
using UnityEngine.UI;
using Altair;
using UnityEngine.SceneManagement;
//using Enut4LJR;

namespace Choi
{
    public class SelectTurretMgr : MonoBehaviour
    {
        public GameObject turretNode;
        public Transform nodeContent;
        bool isend = false;

        public Button m_ExitBtn;
        public Button m_ShopBtn;
        public GameObject m_MessageBoxObj;
        public GameObject m_Canvas;

        MySelectTurretMgr mgr;

        [SerializeField] private string lobbySceneName;
        [SerializeField] private string stageSceneName;
        [SerializeField] private string shopSceneName;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            mgr = FindObjectOfType<MySelectTurretMgr>();
            Time.timeScale = 1.0f;

            if (m_ExitBtn != null)
                m_ExitBtn.onClick.AddListener(ExitBtnFunc);

            if (m_ShopBtn != null)
                m_ShopBtn.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene(shopSceneName);
                });

            if (m_MessageBoxObj == null)
                m_MessageBoxObj = Resources.Load("/Prefab/LJR_Prefabs/MessBoxPanel") as GameObject;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (Altair.GlobalData.turretDataJson != null && GlobalData.choi_m_TrList != null)
            {
                GlobalData.choi_InitData();
            }

            if((GlobalData.choi_m_TrList.Count == GlobalData.choi_TurretNameList.Length) && isend == false)
            {
                test();
                isend = true;
            }
        }

        public void test()
        {
            for (int i = 0; i < GlobalData.choi_m_TrList.Count; i++)
            {
                if (!SwitchManager.GetSwitch(SwitchManager.switchName[i]))
                {
                    Debug.Log(GlobalData.choi_TurretNameList[i] + " : " + GlobalData.choi_m_TrList[i].UpgradeLv);
                    continue;
                }

                Debug.Log(GlobalData.choi_TurretNameList[i] + " : " + GlobalData.choi_m_TrList[i].UpgradeLv);


                GameObject obj = Instantiate(turretNode, nodeContent);
                if (obj.TryGetComponent(out SelectTurretNode node))
                {
                    node.turretIdx = i;
                    node.IconRSC = GlobalData.choi_m_TrList[i].m_iconRsc;
                    node.SetTurret();
                    node.mgr = this.mgr;
                }
            }
        }

        void ExitBtnFunc()
		{
            GameObject obj = (GameObject)Instantiate(m_MessageBoxObj);
            obj.transform.SetParent(m_Canvas.transform, false);

            Enut4LJR.MessageBoxCtrl a_MsgBoxCtrl = obj.GetComponent<Enut4LJR.MessageBoxCtrl>();
            if (a_MsgBoxCtrl != null)
			{
                a_MsgBoxCtrl.SetSceneName(lobbySceneName);
                a_MsgBoxCtrl.SetMessage("Return to Main Menu?");
            }

		}
    }
}