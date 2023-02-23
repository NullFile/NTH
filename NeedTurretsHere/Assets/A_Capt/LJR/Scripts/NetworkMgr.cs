using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using UnityEngine.Networking;
using SimpleJSON;

namespace Enut4LJR
{
    public enum PacketType
	{
        Slot1Update,
        Slot2Update,
        Slot3Update,
        Dia1Update,
        Dia2Update,
        Dia3Update,

	}

	public class NetworkMgr : MonoBehaviour
    {
        bool isNetworkLock = false;
        List<PacketType> m_PacketBuff = new List<PacketType>();

        [HideInInspector] public string m_TempStrBuff = "";

        public static NetworkMgr Inst = null;

        string updateSlotUrl = "";
        string updateDiaUrl = "";
        string keyStr = "";
        string m_SvStrJson = "";

        private void Awake()
		{
            Inst = this;
		}

		// Start is called before the first frame update
		void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (isNetworkLock == false)
			{
                if (0 < m_PacketBuff.Count)
				{
                    Req_Network();
				}
			}
        }

        void Req_Network()
        {
            if (m_PacketBuff[0] == PacketType.Slot1Update)
                StartCoroutine(Slot1UpdateCo());
            else if (m_PacketBuff[0] == PacketType.Slot2Update)
                StartCoroutine(Slot2UpdateCo());
            else if (m_PacketBuff[0] == PacketType.Slot3Update)
                StartCoroutine(Slot3UpdateCo());
            else if (m_PacketBuff[0] == PacketType.Dia1Update)
                StartCoroutine(Dia1UpdateCo());
            else if (m_PacketBuff[0] == PacketType.Dia2Update)
                StartCoroutine(Dia2UpdateCo());
            else if (m_PacketBuff[0] == PacketType.Dia3Update)
                StartCoroutine(Dia3UpdateCo());

            m_PacketBuff.RemoveAt(0);
        }

        IEnumerator Slot1UpdateCo()
		{
            if (GlobalData.choi_UniqueID == "")
                yield break;

            updateSlotUrl = "http://myturrets.dothome.co.kr/Sv_slot1.php";
            keyStr = "Sv_slot1";
            SlotCreateFunc();
            StartCoroutine(SlotUpdateCo());
		}

        IEnumerator Slot2UpdateCo()
		{
            if (GlobalData.choi_UniqueID == "")
                yield break;

            updateSlotUrl = "http://myturrets.dothome.co.kr/Sv_slot2.php";
            keyStr = "Sv_slot2";
            SlotCreateFunc();
            StartCoroutine(SlotUpdateCo());
        }

        IEnumerator Slot3UpdateCo()
		{
            if (GlobalData.choi_UniqueID == "")
                yield break;

            updateSlotUrl = "http://myturrets.dothome.co.kr/Sv_slot3.php";
            keyStr = "Sv_slot3";
            SlotCreateFunc();
            StartCoroutine(SlotUpdateCo());
        }

        IEnumerator SlotUpdateCo()
        {
            if (string.IsNullOrEmpty(keyStr) || string.IsNullOrEmpty(updateSlotUrl) || string.IsNullOrEmpty(m_SvStrJson))
                yield break;

            WWWForm form = new WWWForm();
            form.AddField("Account", GlobalData.choi_UniqueID, System.Text.Encoding.UTF8);
            form.AddField(keyStr, m_SvStrJson, System.Text.Encoding.UTF8);
            UnityWebRequest a_www = UnityWebRequest.Post(updateSlotUrl, form);
            yield return a_www.SendWebRequest();    //응답이 올때까지 대기하기...

            if (a_www.error == null) //에러가 나지 않았을 때 동작
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string a_ReStr = enc.GetString(a_www.downloadHandler.data);
                //응답완료가 되면 전체 갱신(전체 값을 받아서 갱신하는 방법이 있고,
                //m_SvMyPoint, m_BuyCrType 를가지고 갱신하는 방법이 있다.)
                if (a_ReStr.Contains("Save Success!!") == true)
                    Debug.Log("성공");
                else
                    Debug.Log(a_ReStr);
            }
            else
            {
                Debug.Log(a_www.error);
            }
        }

        
        void SlotCreateFunc()
        {
            for (int i = 0; i < GlobalData.choi_m_TrList.Count; i++)
            {
                GlobalData.choi_m_SaveTrList[GlobalData.choi_IsPick, i] = GlobalData.choi_m_TrList[i].UpgradeLv;
                
            }
            GlobalData.choi_StageList[GlobalData.choi_IsPick] = GlobalData.choi_Stage;
            //GlobalData.choi_DiamondList[GlobalData.choi_IsPick] = GlobalData.choi_userDia;


            JSONObject a_MkJSON = new JSONObject();
            //배열이 필요할 때
            JSONArray jArray = new JSONArray();
            
            for (int ii = 0; ii < GlobalData.choi_m_TrList.Count; ii++)
            {
                jArray.Add(GlobalData.choi_m_TrList[ii].UpgradeLv);
            }
            Debug.Log("터렛 업");
            a_MkJSON.Add("TrList", jArray); //배열을 넣음
            //a_MkJSON.Add("Diamonds", GlobalData.choi_userDia);
            //Debug.Log("다야 업");
            a_MkJSON.Add("Stage", GlobalData.choi_Stage);
            Debug.Log("스테지 업");
            Debug.Log(a_MkJSON.ToString());
           
            m_SvStrJson = a_MkJSON.ToString();
        }

        IEnumerator Dia1UpdateCo()
        {
            if (GlobalData.choi_UniqueID == "")
                yield break;

			updateDiaUrl = "http://myturrets.dothome.co.kr/Sv_dia1.php";
            keyStr = "Sv_dia1";
            GlobalData.choi_DiamondList[GlobalData.choi_IsPick] = GlobalData.choi_userDia;
            StartCoroutine(DiaUpdateCo());
            

        }

        IEnumerator Dia2UpdateCo()
        {
            if (GlobalData.choi_UniqueID == "")
                yield break;

            updateDiaUrl = "http://myturrets.dothome.co.kr/Sv_dia2.php";
            keyStr = "Sv_dia2";
            GlobalData.choi_DiamondList[GlobalData.choi_IsPick] = GlobalData.choi_userDia;
            StartCoroutine(DiaUpdateCo());
        }

        IEnumerator Dia3UpdateCo()
        {
            if (GlobalData.choi_UniqueID == "")
                yield break;

            updateDiaUrl = "http://myturrets.dothome.co.kr/Sv_dia3.php";
            keyStr = "Sv_dia3";
            GlobalData.choi_DiamondList[GlobalData.choi_IsPick] = GlobalData.choi_userDia;
            StartCoroutine(DiaUpdateCo());
        }

        IEnumerator DiaUpdateCo()
        {
            if (string.IsNullOrEmpty(keyStr) || string.IsNullOrEmpty(updateDiaUrl))
                yield break;

            WWWForm form = new WWWForm();
            form.AddField("Account", GlobalData.choi_UniqueID, System.Text.Encoding.UTF8);
            form.AddField(keyStr, GlobalData.choi_userDia);
            UnityWebRequest a_www = UnityWebRequest.Post(updateDiaUrl, form);
            yield return a_www.SendWebRequest();    //응답이 올때까지 대기하기...

            if (a_www.error == null) //에러가 나지 않았을 때 동작
            {
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string a_ReStr = enc.GetString(a_www.downloadHandler.data);
                //응답완료가 되면 전체 갱신(전체 값을 받아서 갱신하는 방법이 있고,
                //m_SvMyPoint, m_BuyCrType 를가지고 갱신하는 방법이 있다.)
                if (a_ReStr.Contains("Save Success!!") == true)
                    Debug.Log("성공");
                else
                    Debug.Log(a_ReStr);
            }
            else
            {
                Debug.Log(a_www.error);
            }
        }

        public void PushPacket(PacketType a_PType)
        {
            bool a_isExist = false;
            for (int ii = 0; ii < m_PacketBuff.Count; ii++)
            {
                if (m_PacketBuff[ii] == a_PType) //아직 처리 되지 않은 패킷이 존재하면
                    a_isExist = true;
                //또 추가하지 않고 기존 버퍼의 패킷으로 업데이트한다.
            }

            if (a_isExist == false)
                m_PacketBuff.Add(a_PType);
            //대기 중인 이 타입의 패킷이 없으면 새로 추가한다.
        }
    }    
}
