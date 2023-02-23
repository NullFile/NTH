using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;
using Altair_Memory_Pool_Pro;


namespace LeeSpace
{
    public class TestDummyCtrl : MonoBehaviour
    {
        public int Hp = 100;
        float AttackDelay = 1.0f;
        public GameObject Bullet;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            AttackDelay -= Time.deltaTime;

            //if (0 >= AttackDelay)
            //{
            //    Debug.Log("Shoot");
            //    Shoot();
            //    AttackDelay = 1.0f;
            //}

            if (Input.GetKeyDown(KeyCode.K))
            {
                Shoot();
            }
        }

        void Shoot()
        {
            GameObject Bulletobj = MemoryPoolManager.instance.GetObject(0, this.transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0));
            if (Bulletobj != null && Bulletobj.TryGetComponent(out BulletCtrl bullet))
            {
                bullet.PlayerBulet = true;
                bullet.transform.localEulerAngles = new Vector3(0, 0, 0);
                bullet.bullet_life = 5.0f;
            }
        }
    }
}
