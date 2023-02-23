using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using Altair_Memory_Pool_Pro;

namespace Jang
{
    public class EnemyBulletCtrl : MemoryPoolingFlag
    {
        // ���� �� ������ �̻����� �ӵ�
        float mSpeed = 2.0f;

        // ����ĳ��Ʈ�� ����
        float Range = 0.05f;

        // ���� ĳ��Ʈ2d
        RaycastHit2D hit2;

        private Animator[] animator;

        // Start is called before the first frame update
        void Start()
        {
            animator = this.gameObject.GetComponentsInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector2.left * mSpeed * Time.deltaTime);

            Debug.DrawRay(transform.position + new Vector3(-0.3f, 0, 0), new Vector3(-1f, 0, 0) * Range, Color.clear);
            hit2 = Physics2D.Raycast(transform.position, new Vector3(-1f, 0, 0), Range, 1 << 7);

            if (hit2.collider != null)
            {
                ObjectReturn();
            }
        }
    }
}