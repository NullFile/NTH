using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Altair
{
    public class AltairMemoryBulletController : MemoryPoolingFlag //�޸�Ǯ�� �� ������Ʈ�� MemoryPoolingFlag�� ��� �޾ƾ� �Ѵ�.
    {
        internal bool isHit = false;

        [SerializeField] private float bulletSpeed = .0f;
        [SerializeField] private int damage = 8;

        internal float spanTime = 5.0f;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (bulletSpeed <= 0) bulletSpeed = .25f;
            isHit = false;
        }

        private void FixedUpdate()
        {
            if (isHit) return;
            MoveObject();
            spanTime -= Time.deltaTime;
            if (spanTime <= .0f)
                //������Ʈ�� Destroy() ��� ObjectReturn()�� ����� �޸�Ǯ�� ��ȯ�Ѵ�.
                ObjectReturn();
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (isHit) return;
            GameObject target = CheckCollider();

            if (target)
                isHit = HitCheck(target);
        }

        private void MoveObject()
        {
            if (isHit) return;
            transform.Translate(Vector2.right * bulletSpeed);
        }

        private GameObject CheckCollider()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity);
            if (hit.collider)
                return hit.collider.gameObject;
            else return null;
        }

        private bool HitCheck(GameObject target)
        {
            Vector2 origin = transform.position;
            Vector2 dest = target.transform.position;
            Vector2 dir = dest - origin;

            float distance = dir.sqrMagnitude;

            if(distance <= .1f)
            {
                OnDamage(target);
                return true;
            }
            return false;
        }

        private void OnDamage(GameObject target)
        {
            if (!target) return;

            if (!target.TryGetComponent(out AltairMemoryTargetController targetController)) return;

            targetController.hp -= damage;
            if (targetController.hp <= 0) targetController.Dead();

            //������Ʈ�� Destroy() ��� ObjectReturn()�� ����� �޸�Ǯ�� ��ȯ�Ѵ�.
            ObjectReturn();
        }
    }
}