using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;


namespace Yuspace
{

    public class TypeInfo       //�ͷ��� ������ ������ Ŭ����
    {
        public UnitType m_UniType = UnitType.Rocket;        //������ Ÿ��
        public UnitShopState m_UniState = UnitShopState.Lock;       //������ ����
        public int m_Price = 0;   //������ ����
        public int m_UpPrice = 0;       //���׷��̵� ����
        public int m_Level = 0;     //������  ����
        public int m_MaxLevel = 3;  //������ �ƽ�����



        public void SetType(UnitType a_UniType)
        {
            m_UniType = a_UniType;

            if (a_UniType == UnitType.Rocket)        //ù��° ������ ó�������Ҷ� ����1�� ����
            {
                //SetjsonData("Rocket Turret");
                GlobalData.choi_m_TrList[(int)a_UniType].UpgradeLv = 1;
            }

            //else if (a_UniType == UnitType.Air_Strike)        //�ι�° ����
            //{
            //    SetjsonData("Air Strike");
            //}

            //else if (a_UniType == UnitType.Claymore)        //����° ����
            //{
            //    SetjsonData("Claymore");
            //}

            //else if (a_UniType == UnitType.Electric)        //�׹�° ����
            //{
            //    SetjsonData("Electric Turret");
            //}

            //else if (a_UniType == UnitType.DoubleRocket)        //�ټ���° ����
            //{
            //    SetjsonData("Double Rocket Turret");
            //}
            //else if (a_UniType == UnitType.Militia)        //������° ����
            //{
            //    SetjsonData("Militia");
            //}
            //else if (a_UniType == UnitType.Flame)        //�ϰ���° ����
            //{
            //    SetjsonData("Flame Thrower");
            //}
            //else if (a_UniType == UnitType.Sniper)        //������° ����
            //{
            //    SetjsonData("Sniper");
            //}
            //else if (a_UniType == UnitType.EMP)        //��ȩ��° ����
            //{
            //    SetjsonData("EMP Wave");
            //}
            //else if (a_UniType == UnitType.LittleBoy)        //����° ����
            //{
            //    SetjsonData("Little Boy");
            //}
            //else if (a_UniType == UnitType.KamiKaze)        //���ѹ�° ����
            //{
            //    SetjsonData("Kamikaze");
            //}
        }

        void SetjsonData(string key)
        {
            //if (!JSONParser.DataValidation(GlobalData.turretData[key]["name"], out m_Name)) return;
            //if (!JSONParser.DataValidation(GlobalData.turretData[key]["hp"], out m_Hp)) return;
            //if (!JSONParser.DataValidation(GlobalData.turretData[key]["dam"], out m_Att)) return;
            //if (!JSONParser.DataValidation(GlobalData.turretData[key]["Archive"], out m_Archive)) return;
            //if (!JSONParser.DataValidation(GlobalData.turretData[key]["cost"], out m_Cost)) return;
            //if (!JSONParser.DataValidation(GlobalData.turretData[key]["buyCost"], out m_Price)) return;
            //if (!JSONParser.DataValidation(GlobalData.turretData[key]["upgradeCost"], out m_UpPrice)) return;


            //Debug.Log(m_Name + ":" + m_Hp + ":" + m_Att + " : " + m_Archive);
            

        }

    }

    public class GlobalValue
    {
        // Start is called before the first frame update
        //public static string g_ID = "";
        //public static int g_UserGold = 0;

        public static List<TypeInfo> m_ShopDataList = new List<TypeInfo>();      //������ ���� �ͷ� ������...
        public static void InitData()
        {
            if (0 < m_ShopDataList.Count)
                return;

            TypeInfo a_UnitNd;
            for (int ii = 0; ii < (int)UnitType.UnCount; ii++)
            {
                a_UnitNd = new TypeInfo();
                a_UnitNd.SetType((UnitType)ii);
                m_ShopDataList.Add(a_UnitNd);
            }
        }


    }
}


