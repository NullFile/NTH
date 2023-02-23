using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Choi
{
    public class EnemyGenerator : MonoBehaviour
    {//Choi

        public GameObject enemyprefab;

        private void Start() => StartFunc();

        private void StartFunc()
        {
         
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameObject obj = MemoryPoolManager.instance.GetObject(1, this.transform.position);
            }
        }
    }
}