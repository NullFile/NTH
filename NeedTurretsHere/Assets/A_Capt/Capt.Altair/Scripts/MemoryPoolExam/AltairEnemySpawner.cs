using UnityEngine;
using Altair_Memory_Pool_Pro; //�޸�Ǯ�� ����Ϸ��� using �ؾ��Ѵ�.

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