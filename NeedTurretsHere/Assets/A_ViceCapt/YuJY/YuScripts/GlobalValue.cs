using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;


namespace Yuspace
{

    public class TypeInfo       //터렛의 상점용 데이터 클래스
    {
        public UnitType m_UniType = UnitType.Rocket;        //유닛의 타입
        public UnitShopState m_UniState = UnitShopState.Lock;       //유닛의 상태
        public int m_Price = 0;   //유닛의 가격
        public int m_UpPrice = 0;       //업그레이드 가격
        public int m_Level = 0;     //유닛의  레벨
        public int m_MaxLevel = 3;  //유닛의 맥스레벨



        public void SetType(UnitType a_UniType)
        {
            m_UniType = a_UniType;

            if (a_UniType == UnitType.Rocket)        //첫번째 유닛은 처음시작할때 레벨1로 시작
            {
                //SetjsonData("Rocket Turret");
                GlobalData.choi_m_TrList[(int)a_UniType].UpgradeLv = 1;
            }

            //else if (a_UniType == UnitType.Air_Strike)        //두번째 유닛
            //{
            //    SetjsonData("Air Strike");
            //}

            //else if (a_UniType == UnitType.Claymore)        //세번째 유닛
            //{
            //    SetjsonData("Claymore");
            //}

            //else if (a_UniType == UnitType.Electric)        //네번째 유닛
            //{
            //    SetjsonData("Electric Turret");
            //}

            //else if (a_UniType == UnitType.DoubleRocket)        //다섯번째 유닛
            //{
            //    SetjsonData("Double Rocket Turret");
            //}
            //else if (a_UniType == UnitType.Militia)        //여섯번째 유닛
            //{
            //    SetjsonData("Militia");
            //}
            //else if (a_UniType == UnitType.Flame)        //일곱번째 유닛
            //{
            //    SetjsonData("Flame Thrower");
            //}
            //else if (a_UniType == UnitType.Sniper)        //여덟번째 유닛
            //{
            //    SetjsonData("Sniper");
            //}
            //else if (a_UniType == UnitType.EMP)        //아홉번째 유닛
            //{
            //    SetjsonData("EMP Wave");
            //}
            //else if (a_UniType == UnitType.LittleBoy)        //열번째 유닛
            //{
            //    SetjsonData("Little Boy");
            //}
            //else if (a_UniType == UnitType.KamiKaze)        //열한번째 유닛
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

        public static List<TypeInfo> m_ShopDataList = new List<TypeInfo>();      //상점에 대한 터렛 정보들...
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


