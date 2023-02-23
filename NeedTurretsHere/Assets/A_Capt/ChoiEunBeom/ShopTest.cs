using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Altair;

namespace Choi
{
    public class ShopTest : MonoBehaviour
    {
        public Button nanmolu = null;
        public Button GoldBtn = null;
        string BuyRequestUrl = "";
        string GoldUpdateUrl = "";

        string m_SvStrJson;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            BuyRequestUrl = "http://myturrets.dothome.co.kr/Buy_Request.php";
            GoldUpdateUrl = "http://myturrets.dothome.co.kr/UpdateUserDia.php";

            nanmolu.onClick.AddListener(nanmoluFunc);
            GoldBtn.onClick.AddListener(() =>
            {
                GlobalData.choi_userDia += 100;
                GlobalData.choi_Stage += 1;
                StartCoroutine(GoldFunc());
            });
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {

        }

        void nanmoluFunc()
        {
            JSONObject a_MkJSON = new JSONObject();
             //�迭�� �ʿ��� ��
            for (int ss = 1; ss < 4; ss++)
            {
                Debug.Log(ss);
                JSONArray jArray = new JSONArray();
                if (GlobalData.choi_IsPick != ss)
                {
                    Debug.Log("����");
                    for (int ii = 0; ii < GlobalData.choi_m_TrList.Count; ii++)
                    {
                        //Debug.Log(GlobalData.choi_m_SaveTrList[ss, ii].m_isUpgrade);
                        //if (GlobalData.choi_m_SaveTrList[ss, ii].m_isUpgrade == null)
                        //{
                        //    jArray.Add(GlobalData.choi_m_SaveTrList[ss, ii].m_isUpgrade);
                        //}
                        //else
                        {
                            jArray.Add(0);
                        }
                    }
                    Debug.Log("�ͷ� ��");
                    a_MkJSON.Add("TrList_" + ss, jArray); //�迭�� ����
                    //if (GlobalData.choi_DiamondList[ss] != null)
                    //{
                    //    a_MkJSON.Add("Diamonds_" + ss, GlobalData.choi_DiamondList[ss]);
                    //    Debug.Log("�پ� ��");
                    //}
                    //else
                    {
                        a_MkJSON.Add("Diamonds_" + ss, 0);
                        Debug.Log("�پ� 0");
                    }
                    //if (GlobalData.choi_StageList[ss] != null)
                    //{
                    //    a_MkJSON.Add("Stage_" + ss, GlobalData.choi_StageList[ss]);
                    //    Debug.Log("������ ��");
                    //}
                    //else
                    {
                        a_MkJSON.Add("Stage_" + ss, 0);
                        Debug.Log("������ 0");
                    }
                    Debug.Log(a_MkJSON.ToString());
                }
                else
                {
                    for(int ii = 0; ii <GlobalData.choi_m_TrList.Count;ii++)
                    {
                        jArray.Add(GlobalData.choi_m_TrList[ii].UpgradeLv);
                    }
                    a_MkJSON.Add("TrList_" + ss, jArray); //�迭�� ����
                    a_MkJSON.Add("Diamonds_" + ss, GlobalData.choi_userDia);
                    a_MkJSON.Add("Stage_" + ss, GlobalData.choi_Stage);
                }
            }
            m_SvStrJson = a_MkJSON.ToString();
            //---- JSON ����� ...

            StartCoroutine(BuyRequestCo());
        }

        IEnumerator BuyRequestCo()
        {
            WWWForm form = new WWWForm();

            Debug.Log(GlobalData.choi_UniqueID);

            form.AddField("Account", GlobalData.choi_UniqueID,
                                    System.Text.Encoding.UTF8);
            //form.AddField("Diamonds", GlobalData.choi_userDia);
            form.AddField("Inventory", m_SvStrJson, System.Text.Encoding.UTF8);

            UnityWebRequest a_www = UnityWebRequest.Post(BuyRequestUrl, form);
            yield return a_www.SendWebRequest();    //������ �ö����� ����ϱ�...

            if (a_www.error == null) //������ ���� �ʾ��� �� ����
            {

                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                string a_ReStr = enc.GetString(a_www.downloadHandler.data);
                //����Ϸᰡ �Ǹ� ��ü ����(��ü ���� �޾Ƽ� �����ϴ� ����� �ְ�,
                //m_SvMyPoint, m_BuyCrType �� ������ �����ϴ� ����� �ִ�.)
                Debug.Log(a_ReStr);
                if (a_ReStr.Contains("BuySuccess~") == true)
                    Debug.Log("���ظԾ�");
                else
                    Debug.Log(a_ReStr);
            }
            else
            {
                Debug.Log(a_www.error);
            }
        }

        IEnumerator GoldFunc()
        {
            WWWForm form = new WWWForm();

            Debug.Log(GlobalData.choi_UniqueID);

            form.AddField("Account", GlobalData.choi_UniqueID,
                                    System.Text.Encoding.UTF8);
            form.AddField("Diamonds", GlobalData.choi_userDia);
            form.AddField("Stage", GlobalData.choi_Stage.ToString());

            UnityWebRequest a_www = UnityWebRequest.Post(GoldUpdateUrl, form);
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
                    Debug.Log("���ظԾ�");
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
    }
}