using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace LeeSpace
{
    public class BulletCtrl : MemoryPoolingFlag
    {
        float Speed = 5.0f;
        RaycastHit2D hit;
        public bool PlayerBulet = false;
        public int BulletDamage = 0;
        GameObject hitObj;
        public float bullet_life = 5.0f;

        bool Curved = false;
        float Curve_Per = 0;
        public GameObject TargetObj;
        Vector3 p1, p2;
        Vector3 r1, r2;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            Bullet_lifeTime();

            Curved_Bullet();

            Straight_Bullet();

            Hit_Bullet();
        }

        void Bullet_lifeTime()
        {
            if (bullet_life <= 0)
                ObjectReturn();

            bullet_life -= Time.deltaTime;
        }

        void Straight_Bullet()
        {
            bullet_life -= Time.deltaTime;

            if (Curved == true)
                return;

            if (PlayerBulet == false)
            {
                transform.Translate(Vector2.left * Speed * Time.deltaTime);
                Debug.DrawRay(transform.position - new Vector3(0f, 0, 0), Vector3.left * 0.1f, new Color(0, 1, 0));
                hit = Physics2D.Raycast(transform.position - new Vector3(0, 0, 0), Vector3.left, 0.1f, 1 << 7);
            }
            else
            {
                transform.Translate(Vector2.right * Speed * Time.deltaTime);
                Debug.DrawRay(transform.position - new Vector3(0f, 0, 0), Vector3.right * 0.1f, new Color(0, 1, 0));
                hit = Physics2D.Raycast(transform.position - new Vector3(0, 0, 0), Vector3.right, 0.1f, 1 << 6);
            }

        }

        public void Curved_Setting()
        {
            p1 = this.transform.position;
            p2 = TargetObj.transform.position;

            r1 = this.transform.position + new Vector3(0, 1.0f, 0);
            r2 = TargetObj.transform.position + new Vector3(0.0f, 3.0f, 0);

            Curve_Per = 0;
            Curved = true;
        }

        void Curved_Bullet()
        {
            if (Curved == false)
                return;

            Curve_Per += Time.deltaTime;

            Vector2 v1 = Vector2.Lerp(p1, r1, Curve_Per);
            Vector2 v2 = Vector2.Lerp(r1, r2, Curve_Per);
            Vector2 v3 = Vector2.Lerp(r2, p2, Curve_Per);

            Vector2 v4 = Vector2.Lerp(v1, v2, Curve_Per);
            Vector2 v5 = Vector2.Lerp(v2, v3, Curve_Per);

            Vector2 v6 = Vector2.Lerp(v4, v5, Curve_Per);

            this.transform.position = v6;

            if (Curve_Per <= 0.9)
            {
                transform.localEulerAngles += new Vector3(0, 0, 1);
            }

            if (Curve_Per >= 1.0f)
            {
                GameObject Explosionobj = MemoryPoolManager.instance.GetObject("Explosion_2", this.transform.position, Quaternion.Euler(0, 0, 0));
                ObjectReturn();
            }
        }

        void Hit_Bullet()
        {
            if (Curved == true)
            {
                if (hit.collider != null && Curve_Per >= 0.8)
                {
                    hitObj = hit.collider.gameObject;
                    TakeDamage();
                    ObjectReturn();
                }
            }
            else
            {
                if (hit.collider != null)
                {
                    hitObj = hit.collider.gameObject;
                    TakeDamage();
                    ObjectReturn();
                }
            }
        }


        void TakeDamage()
        {
            if (hitObj != null)
            {
                if (hitObj.layer == 6)
                {
                    hitObj.GetComponentInParent<MonsterCtrl>().OnDamage(30);
                }
                else if (hitObj.layer == 7)
                {
                    hitObj.GetComponentInParent<SungJae.Turret_Ctrl>().OnDamage(10);
                    //hitObj.GetComponentInParent<TestDummyCtrl>().Hp -= BulletDamage;
                    hitObj.GetComponent<SungJae.Turret_Ctrl>().OnDamage(BulletDamage);
                }
            }
        }

    }
}
