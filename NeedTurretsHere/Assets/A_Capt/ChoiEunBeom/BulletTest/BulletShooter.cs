using UnityEngine;

namespace Choi
{
    public class BulletShooter : MonoBehaviour
    {
        public GameObject bulletPrefab;

        private void Start() => StartFunc();

        private void StartFunc()
        {

        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject obj = Instantiate(bulletPrefab);
                obj.transform.position = this.transform.position;
            }
        }
    }
}