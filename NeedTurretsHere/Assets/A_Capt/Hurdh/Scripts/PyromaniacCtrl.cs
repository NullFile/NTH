using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Choi;
using SungJae;
using Altair;

namespace Hdh
{
    public class PyromaniacCtrl : Turret_Ctrl
    {
        //---------- 총알 발사 관련 변수 선언
        float m_CacAtTick = 0.0f;   //기관총 발사 틱 만들기....
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- 총알 발사 관련 변수 선언
        float CheckTime = 0.0f;

        bool ishit = false;

        //거리 체크용 변수
        float dist = 1.67f;  //한칸거리
        //float endPos = 9.0f; //끝지점
        Vector2 rayVec;
        RaycastHit2D hit;
        public LayerMask enemylayer;
        //거리 체크용 변수

        protected override void SetType(int ii)
        {
            base.SetType(ii);
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Altair.GlobalData.turretDataJson != null)
            {
                GlobalData.choi_InitData();
            }

            if (GlobalData.choi_m_TrList != null && turretIdx == -1)
            {
                SetType(36);
                CheckTime = turretAttWait;
                //Debug.Log(CheckTime.ToString());
                //Debug.Log(turretHp);
            }
            if (turretEnum == turretAction.deploy)
            {
                //생성함수호출
                turretEnum = turretAction.idle;
            }
            else if (turretEnum == turretAction.idle)
            {
                //설치후 1초 대기
                DelayAcitve();
            }
            else if (turretEnum == turretAction.attack)
            {
                CheckAttSensor();
                //공격 사거리 체크
                //Debug.Log("센서들어옴");
                if(m_CacAtTick <= 0)
                {
                    if (ishit == true)
                    {
                       m_CacAtTick = turretAttWait;
                        ishit = false;
                    }
                }
                else
                {
                    Debug.Log(m_CacAtTick);
                    m_CacAtTick -= Time.deltaTime;
                }
                
            }
            else if (turretEnum == turretAction.Destroy)
            {
                //터렛 hp = 0 파괴하기
                Destroy(this.gameObject);
            }
        }

        public void DelayAcitve()
        {
            //Debug.Log(CheckTime.ToString());
            CheckTime -= Time.deltaTime;

            if (CheckTime <= 0.0f)
                turretEnum = turretAction.attack;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector2(3, 3));
        }

        //공격 사거리 체크 함수
        public void CheckAttSensor()
        {
            
            rayVec = this.transform.position;
            float value = dist * turretSensor;//8.0f;
            if (endPos < (transform.position.x + value))
                value = endPos - transform.position.x;

            Collider2D[] colls = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(3, 3), 0, enemylayer);
            //Debug.Log(colls);
            //Debug.Log(colls.Length);
            if (m_CacAtTick <= 0 && ishit == false)
            {
                if (colls != null)
                {
                    //Debug.Log(colls.Length);
                    for (int i = 0; i < colls.Length; i++)
                    {
                        if (colls[i].TryGetComponent(out Enemy enemy))
                        {
                            enemy.hp -= 500; //(int)turretAttDamage;
                            ishit = true;

                            if (enemy.hp <= 0)
                            {
                                Destroy(enemy.gameObject);
                            }
                        }
                    }
                }
            }
        }

        protected override void turretAtt()
        {

        }
    }
}
