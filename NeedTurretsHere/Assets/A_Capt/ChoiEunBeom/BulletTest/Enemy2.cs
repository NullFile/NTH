using UnityEngine;

namespace Choi
{
    public class Enemy2 : MonoBehaviour
    {
        public int hp = 5;
        public int speed = 1;

        private void Start() => StartFunc();

        private void StartFunc()
        {

        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            this.transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
    }
}