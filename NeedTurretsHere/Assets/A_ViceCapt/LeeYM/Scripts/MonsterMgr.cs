using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;

namespace LeeSpace
{
    public class MonsterMgr : MonoBehaviour
    {
        private void Awake()
        {
            if (GlobalData.enemyData == null)
                if (!JSONParser.DataValidation(Altair.GlobalData.enemyDataJson, out GlobalData.enemyData)) return;
        }
    }
}