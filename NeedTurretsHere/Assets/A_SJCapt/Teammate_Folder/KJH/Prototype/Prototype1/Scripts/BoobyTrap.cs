using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using SungJae;
using Altair;

namespace MySpace
{
    public class BoobyTrap : Turret_Ctrl
    {
        int damage = 1;

        float check = 0.0f;
        float delay = 0.0f;

        protected override void SetType(int ii)
        {
            base.SetType(ii);
        }

        private void Update()
        {
            if (Altair.GlobalData.turretDataJson != null)
            {
                GlobalData.choi_InitData();
            }

            if (GlobalData.choi_m_TrList != null && turretIdx == -1)
            {
                SetType(15);
                damage = (int)turretAttDamage;
                delay = turretAttSpeed;
                //Debug.Log(CheckTime.ToString());
                //Debug.Log(turretHp);
            }

            if (0.0f < check)
            {
                check -= Time.deltaTime;

                if (check <= 0.0f)
                    check = 0.0f;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("dummy"))
            {
                if (0.0f < check)
                    return;

                //DmgTEnemy dte = collision.GetComponent<DmgTEnemy>();

                //dte.hp -= damage;
                //check = delay;

                //Debug.Log(dte.hp);

                //if (dte.hp <= 0)
                //{
                //    Destroy(dte.gameObject);
                //}
            }
        }
    }
}
