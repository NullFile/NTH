using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using Altair_Memory_Pool_Pro;

namespace Jang
{
    public class TurretFireCtrl : MemoryPoolingFlag
    {
        // ���� �� ������ �̻����� �ӵ�
        float mSpeed = 2.0f;

        // ����ĳ��Ʈ�� ����
        float Range = 0.05f;

        [HideInInspector] public int Damage = 2;

        // ���� ĳ��Ʈ2d
        RaycastHit2D hit2;

        private Animator animator;

        GameObject hitObj;

        // Start is called before the first frame update
        void Start()
        {
            animator = this.gameObject.GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector2.right * mSpeed * Time.deltaTime);

            Debug.DrawRay(transform.position + new Vector3(0.3f, -0.05f, 0), new Vector3(1f, 0, 0) * Range, Color.clear);
            hit2 = Physics2D.Raycast(transform.position, new Vector3(1f, 0, 0), Range, 1 << 6);

            if (hit2.collider != null)
            {
                hitObj = hit2.collider.gameObject;
                TakeDamage();
                ObjectReturn();
            }
        }

        public void TakeDamage()
        {
            if (hitObj != null)
            {
                if (hitObj.layer == 6)
                {
                    hitObj.GetComponent<LeeSpace.MonsterCtrl>().OnDamage(10);
                }
            }
        }
    }
}