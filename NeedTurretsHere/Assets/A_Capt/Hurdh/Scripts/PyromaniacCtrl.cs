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
        //---------- �Ѿ� �߻� ���� ���� ����
        float m_CacAtTick = 0.0f;   //����� �߻� ƽ �����....
        GameObject a_NewObj = null;
        GameObject findObj = null;
        //---------- �Ѿ� �߻� ���� ���� ����
        float CheckTime = 0.0f;

        bool ishit = false;

        //�Ÿ� üũ�� ����
        float dist = 1.67f;  //��ĭ�Ÿ�
        //float endPos = 9.0f; //������
        Vector2 rayVec;
        RaycastHit2D hit;
        public LayerMask enemylayer;
        //�Ÿ� üũ�� ����

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
                //�����Լ�ȣ��
                turretEnum = turretAction.idle;
            }
            else if (turretEnum == turretAction.idle)
            {
                //��ġ�� 1�� ���
                DelayAcitve();
            }
            else if (turretEnum == turretAction.attack)
            {
                CheckAttSensor();
                //���� ��Ÿ� üũ
                //Debug.Log("��������");
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
                //�ͷ� hp = 0 �ı��ϱ�
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

        //���� ��Ÿ� üũ �Լ�
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
