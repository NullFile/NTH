using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Jang
{
    // 터렛은 공격하거나 죽거나로 타입을 설정했다.
    public enum TurretType
    {
        T_Attack,
        T_Death
    }

    public class TurretCtrl : MemoryPoolingFlag
    {
        // 기본 상태 무브로 함.
        TurretType m_TurType = TurretType.T_Attack;

        private Animator[] animator;

        #region // ------------- 공격용 변수 관련

        // 공격용 게임오브젝트(미사일)
        public GameObject TurretFire;

        // 공격용 벡터
        Vector2 FirePos;

        // 터렛 공격속도
        float T_ASpeed = 1.0f;

        // 공격 쿨타임
        float AttackTime = 0.5f;

        // 공격 범위
        float AttackRange = 15.0f;

        // 레이캐스트
        RaycastHit hit;
        #endregion // ------------- 공격용 변수 관련

        // Update is called once per frame
        void Update()
        {
            TurretAI();

            AttackCheck();
        }

        // TurretAI 조정 함수
        void TurretAI()
        {
            if (0.0f < AttackTime)
                AttackTime -= Time.deltaTime;

            // 터렛 타입 변경(공격, 죽음)
            if (m_TurType == TurretType.T_Attack)
            {
                if (AttackTime <= 0.0f)
                {
                    TurretAttack();
                    AttackTime = T_ASpeed;
                }
            }
            else if (m_TurType == TurretType.T_Death)
            {
                // 터렛의 체력이 없을 때

            }
        }

        #region // ------------- 공격 관련 함수들
        // 터렛 공격용 함수
        void TurretAttack()
        {
            FirePos = this.transform.position;
            FirePos.x += 1.0f;
            FirePos.y += 0.2f;
            
            // 메모리풀 obj
            GameObject obj = MemoryPoolManager.instance.GetObject(2, FirePos);
            if (obj != null && obj.TryGetComponent(out TurretFireCtrl bullets))
            {
                
            }
        }

        // 공격 범위에 왔는지 판별용 함수
        void AttackCheck()
        {
            if (Physics.Raycast(transform.position + new Vector3(1f, 0, 0), Vector3.right, out hit, AttackRange, 1 << 7))
            {
                Debug.DrawRay(transform.position + new Vector3(0.75f, 0.1f, 0), Vector3.right * AttackRange, Color.clear);

                if (hit.collider != null)
                {
                    if (m_TurType != TurretType.T_Attack && hit.collider.tag == "Enemy")
                    {
                        m_TurType = TurretType.T_Attack;
                    }
                }
                else
                {
                    m_TurType = TurretType.T_Death;
                }
            }            
        }
        #endregion // ------------- 공격 관련 함수들
    }
}