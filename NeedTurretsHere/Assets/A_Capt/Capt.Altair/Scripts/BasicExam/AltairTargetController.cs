using UnityEngine;

namespace Altair
{
    public class AltairTargetController : MonoBehaviour
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

        //private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            
        }

        private void MoveObject()
        {
            rigidbody.MovePosition(rigidbody.position + (Vector2.left * moveSpeed));
        }
    }
}