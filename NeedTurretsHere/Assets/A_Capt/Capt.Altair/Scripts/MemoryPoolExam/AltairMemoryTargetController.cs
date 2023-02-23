using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Altair
{
    public class AltairMemoryTargetController : MemoryPoolingFlag //메모리풀에 들어갈 오브젝트는 MemoryPoolingFlag를 상속 받아야 한다.
    {
        [SerializeField] internal int hp = 92;
        private new Rigidbody2D rigidbody;

        [SerializeField] private float moveSpeed = .005f;

        private void FixedUpdate()
        {
            if (moveSpeed <= 0) moveSpeed = .005f;
            MoveObject();
        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (!TryGetComponent(out rigidbody))
                rigidbody = gameObject.AddComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            hp = 92;
        }

        //private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            
        }

        private void MoveObject()
        {
            rigidbody.MovePosition(rigidbody.position + (Vector2.left * moveSpeed));
        }

        internal void Dead()
        {
            if (hp > 0) return;

            //오브젝트를 Destroy() 대신 ObjectReturn()를 사용해 메모리풀로 반환한다.
            ObjectReturn();
        }
    }
}