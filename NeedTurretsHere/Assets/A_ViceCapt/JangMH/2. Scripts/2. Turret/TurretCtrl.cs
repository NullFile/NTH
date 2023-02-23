using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Jang
{
    // �ͷ��� �����ϰų� �װų��� Ÿ���� �����ߴ�.
    public enum TurretType
    {
        T_Attack,
        T_Death
    }

    public class TurretCtrl : MemoryPoolingFlag
    {
        // �⺻ ���� ����� ��.
        TurretType m_TurType = TurretType.T_Attack;

        private Animator[] animator;

        #region // ------------- ���ݿ� ���� ����

        // ���ݿ� ���ӿ�����Ʈ(�̻���)
        public GameObject TurretFire;

        // ���ݿ� ����
        Vector2 FirePos;

        // �ͷ� ���ݼӵ�
        float T_ASpeed = 1.0f;

        // ���� ��Ÿ��
        float AttackTime = 0.5f;

        // ���� ����
        float AttackRange = 15.0f;

        // ����ĳ��Ʈ
        RaycastHit hit;
        #endregion // ------------- ���ݿ� ���� ����

        // Update is called once per frame
        void Update()
        {
            TurretAI();

            AttackCheck();
        }

        // TurretAI ���� �Լ�
        void TurretAI()
        {
            if (0.0f < AttackTime)
                AttackTime -= Time.deltaTime;

            // �ͷ� Ÿ�� ����(����, ����)
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
                // �ͷ��� ü���� ���� ��

            }
        }

        #region // ------------- ���� ���� �Լ���
        // �ͷ� ���ݿ� �Լ�
        void TurretAttack()
        {
            FirePos = this.transform.position;
            FirePos.x += 1.0f;
            FirePos.y += 0.2f;
            
            // �޸�Ǯ obj
            GameObject obj = MemoryPoolManager.instance.GetObject(2, FirePos);
            if (obj != null && obj.TryGetComponent(out TurretFireCtrl bullets))
            {
                
            }
        }

        // ���� ������ �Դ��� �Ǻ��� �Լ�
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
        #endregion // ------------- ���� ���� �Լ���
    }
}