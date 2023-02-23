using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using Altair;

namespace SungJae
{
    public class Skill_Ctrl : Turret_Ctrl
    {
        float AttDelay = 0.0f;

        protected override void SetType(int ii)
        {
            base.SetType(ii);
        }

        protected override void turretAtt()
        {
            base.turretAtt();
        }

        public override void OnDamage(int dam)
        {
            base.OnDamage(dam);
        }

        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
            if (GlobalData.choi_m_TrList != null && GlobalData.Check == true && turretIdx == -1)
            {
                SetType(1);
                if (turretIdx == 1)
                {
                    AttDelay = 1.0f;
                }
            }
            

            AttDelay -= Time.deltaTime;
            if (AttDelay <= 0.0f)
            {
                if (turretIdx == 1)
                {
                    Debug.Log("Æã");
                    //ÁøÂ¥ Æã ³Ö¾îµÎ±â
                    AttDelay = 1.0f;
                }
            }

            Debug.Log(turretIdx);
        }
    }
}