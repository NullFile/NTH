using UnityEngine;
using Altair_Memory_Pool_Pro; //메모리풀을 사용하려면 using 해야한다.

namespace Altair
{
    public class AltairEnemySpawner : MonoBehaviour
    {
        
        [SerializeField] private Vector2 spawnPos;
        //private void Start() => StartFunc();

        private void StartFunc()
        {
        
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (SwitchManager.GetSwitch("Switch0")) EnemySpawn();
        }

        private void EnemySpawn()
        {
            MemoryPoolManager.instance.BringObject("Enemy", spawnPos);
            SwitchManager.SetSwitch("Switch0", false);
           
        }
    }
}