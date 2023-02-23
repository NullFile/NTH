using UnityEngine;
using System.Collections.Generic;

namespace Altair
{
    public class AltairTurretController : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        private IEnumerator<WaitForSeconds> wsCoroutine;

        [SerializeField] private float fireDelay = 1.0f;

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
            if (Input.GetButtonDown("Fire1")) SpawnBullet();
        }

        private void SpawnBullet()
        {
            if (!bulletPrefab) return;
            GameObject obj = Instantiate(bulletPrefab);
            obj.transform.position = transform.position;
            AltairBulletController bult = obj.GetComponent<AltairBulletController>();
        } 

        
        private IEnumerator<WaitForSeconds> SpawnBulletCoroutine()
        {
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < 12; i++)
            {
                SpawnBullet();
                yield return ws;
            }
        }
    }
}