using UnityEngine;
using System.Collections.Generic;
using Altair_Memory_Pool_Pro; //메모리풀을 사용하려면 using 해야한다.

namespace Altair
{
    public class AltairMemoryTurretController : MonoBehaviour
    {
        private IEnumerator<WaitForSeconds> wsCoroutine;

        [SerializeField] private float fireDelay = 1.0f;

        [SerializeField] private Vector2 gunPivot;

        private WaitForSeconds ws;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (fireDelay <= .0f) fireDelay = 1f;
            ws = new WaitForSeconds(fireDelay);
            wsCoroutine = SpawnBulletCoroutine();
            StartCoroutine(wsCoroutine);
        }

        //private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            
        }

        private void SpawnBullet()
        {
            if (!MemoryPoolManager.instance) return;

            GameObject obj = MemoryPoolManager.instance.GetObject(0, gunPivot);
            if (obj != null && obj.TryGetComponent(out AltairMemoryBulletController bullet))
            {
                bullet.spanTime = 5.0f;
                bullet.isHit = false;
            }
        } 

        
        private IEnumerator<WaitForSeconds> SpawnBulletCoroutine()
        {
            yield return new WaitForSeconds(1f);

            while (true)
            {
                SpawnBullet();
                yield return ws;
            }
        }
    }
}