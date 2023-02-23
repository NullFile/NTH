using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;
using Altair;

namespace Enut4LJR
{
    public class LJR_BulletCtrl : MemoryPoolingFlag
    {
        public enum SplashType
        {
            NonSplash = 0,
            Splash
        }

        public enum ShotType
        {
            Up,
            Front,
            Down,
            Back,
        }

        internal Altair.AttackType attackType = Altair.AttackType.Null;
        public ShotType shotType = ShotType.Front;
        public SplashType splashType = SplashType.NonSplash;
        float bulletY;
        public int Damage;
        public bool isSlow = false;

        public bool ishit = false;
        int isBack = 1;

        public GameObject hitObj;
        public float bulletSpeed = 0.089f;
        Vector2 rayVec;

        //유도2(Chasing Bullet) 변수
        bool m_SelRandom = false;
        float r2yRandVal = 0.0f;
        float r1yRandVal = 0.0f;

        RaycastHit2D hit;
        public LayerMask enemylayer;

        //관통용 변수
        public List<GameObject> AttackList;

        //곡사용 변수
        [SerializeField] internal Vector3 p1;
        [SerializeField] internal Vector3 p2;
        [SerializeField] internal Vector3 r1;
        [SerializeField] internal Vector3 r2;
        public float value = 0;
        Vector3 BlasticVec = Vector3.zero;

        //사거리 변수
        float maxXPos = 6;

        //트레일 변수
        TrailRenderer trail;

        private void OnDrawGizmos()
        {
            if (this.splashType == SplashType.Splash)
                Gizmos.DrawCube(this.transform.position, new Vector3(3, 3));
        }

        private void Start() => StartFunc();

        private void StartFunc()
        {
            if (shotType == ShotType.Up)
                bulletY = this.transform.position.y + 1.53f;
            else if (shotType == ShotType.Down)
                bulletY = this.transform.position.y - 1.53f;
            else if (shotType == ShotType.Back)
                isBack = -1;

            maxXPos += this.transform.position.x;

            trail = GetComponentInChildren<TrailRenderer>();
        }

		private void OnEnable()
		{
            if (trail != null && trail.time <= 0.1f)
                trail.time = 0.0f;

		}

		private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
            if (pos.x >= 1.0f || pos.x <= -1.0f)
			{
                ObjectReturn();
                if (trail != null)
                    trail.time = -1.0f;
			}

            if (trail != null && trail.time <= 0.3f)
                trail.time += Time.deltaTime * 1.0f;
        }

        private void FixedUpdate()
        {
            switch (attackType)
            {
                case Altair.AttackType.Directional:
                    DirectionalBullet();
                    break;

                case Altair.AttackType.Balistic:
                    BalisticBullet();
                    break;

                case Altair.AttackType.Homing:
                    HomingBullet();
                    break;

                case Altair.AttackType.Piercing:
                    PiercingBullet();
                    break;

                case Altair.AttackType.Chasing:
                    ChasingBullet();
                    break;

            }
        }

        public void DirectionalBullet()
        {
            if (shotType == ShotType.Up)
            {
                if (this.transform.position.y <= bulletY)
                    this.transform.Translate(Vector2.up * bulletSpeed);
            }
            else if (shotType == ShotType.Down)
            {
                if (this.transform.position.y >= bulletY)
                    this.transform.Translate(Vector2.down * bulletSpeed);
            }

            this.transform.Translate((Vector2.right * isBack) * bulletSpeed);
            rayVec = new Vector2(this.transform.position.x - 0.5f, this.transform.position.y);

            if (ishit)
                return;
            hit = Physics2D.Raycast(rayVec, Vector2.right, 1.0f, enemylayer);
            
            Debug.DrawRay(rayVec, Vector2.right * 3.0f, Color.red);
            if (hit)
            {
                hitObj = hit.collider.gameObject;
                AttackEnemy();
                ishit = true;
				//this.gameObject.SetActive(false);

			}
		}

        public void BalisticBullet()
        {
            if (hitObj != null)
            {
                p2 = hitObj.transform.position;
                r2 = hitObj.transform.position;
                //r2.x -= 2.5f;
                r2.y += 5.0f;

                if (hitObj.TryGetComponent(out Choi.Enemy enemy))
                {
                    if (enemy.hp <= 0)
                        hitObj = null;
                }

                if (value > 0.95f)
                    AttackEnemy(splashType);
            }

            if (value >= 0.99f)
			{
                ObjectReturn();
                if (trail != null)
                    trail.time = -1.0f;
			}

            if (value < 0.4f)
                value += Time.deltaTime;
            else if (value <= 0.4f && 0.6f < value)
                value += Time.deltaTime * 0.4f;
            else
                value += Time.deltaTime;

            BlasticVec = BezierTest(p1, p2, r1, r2, value);

            //m_PlayerVec = (m_RefPlayer.transform.position - this.transform.position) * -1;
            Vector3 rotvec = BlasticVec - this.transform.position;
            float angle = Mathf.Atan2(rotvec.y, rotvec.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            //Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
            transform.eulerAngles = angleAxis.eulerAngles;

            this.transform.position = BlasticVec;
        }

        public void HomingBullet()
        {
            if (hitObj != null)
            {
                p2 = hitObj.transform.position;
                r2 = hitObj.transform.position;
                //r2.x -= 5.0f;
                //r2.y += 5.0f;
                if (hitObj.TryGetComponent(out Choi.Enemy enemy))
                {
                    if (enemy.hp <= 0)
                        hitObj = null;
                }

                if (value > 0.95f)
                    AttackEnemy(splashType);
            }

            if (value < 0.4f)
                value += Time.deltaTime * bulletSpeed * 15.0f;
            else if (value <= 0.4f && 0.6f < value)
                value += Time.deltaTime * 0.4f;
            else
                value += Time.deltaTime * bulletSpeed * 15.0f;

            BlasticVec = BezierTest(p1, p2, r1, r2, value);

            if (value >= 0.85f)
            {
                this.transform.Translate(Vector2.right * bulletSpeed * 2);
            }
            else
            {
                Vector3 rotvec = BlasticVec - this.transform.position;
                float angle = Mathf.Atan2(rotvec.y, rotvec.x) * Mathf.Rad2Deg;
                Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                //Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
                transform.eulerAngles = angleAxis.eulerAngles;

                this.transform.position = BlasticVec;
            }
        }

        public void ChasingBullet()
        {
            if (hitObj != null)
            {
                p2 = hitObj.transform.position;
                r2 = hitObj.transform.position;
                if (m_SelRandom == false)
                {
                    //r2xRandVal = Random.Range(-0.25f, 0.25f);
                    r2yRandVal = Random.Range(-0.25f, 0.25f);
                    r1yRandVal = Random.Range(-0.5f, 0.25f);
                    m_SelRandom = true;
                }
                //r2.x += r2xRandVal;
                r2.y += r2yRandVal;
                r1.y += r1yRandVal;

                if (hitObj.TryGetComponent(out Choi.Enemy enemy))
                {
                    if (enemy.hp <= 0)
                        hitObj = null;
                }

                //r2.y += 5.0f;
                if (value > 0.95f)
                {
                    AttackEnemy(splashType);
                    m_SelRandom = false;
                }
            }

            //if (value >= 0.99f)
            //    this.gameObject.SetActive(false);

            if (value < 0.4f)
                value += Time.deltaTime;
            else if (value <= 0.4f && 0.6f < value)
                value += Time.deltaTime * 0.4f;
            else
                value += Time.deltaTime;

            BlasticVec = BezierTest(p1, p2, r1, r2, value);

            
            if (value >= 0.85f)
            {
                this.transform.Translate(Vector2.right * bulletSpeed * 2);
            }
            else
            {
                Vector3 rotvec = BlasticVec - this.transform.position;
                float angle = Mathf.Atan2(rotvec.y, rotvec.x) * Mathf.Rad2Deg;
                Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                //Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5 * Time.deltaTime);
                transform.eulerAngles = angleAxis.eulerAngles;

                this.transform.position = BlasticVec;
            }
        }

        public void PiercingBullet()
        {
            // 쭉나가면서 데미지
            //this.transform.Translate((Vector2.right * isBack) * Time.deltaTime * bulletSpeed);
            //rayVec = new Vector2(this.transform.position.x - 0.5f, this.transform.position.y);

            //hit = Physics2D.Raycast(rayVec, Vector2.right, 1.0f, enemylayer);
            //if (hit)
            //{
            //    for (int i = 0; i < AttackList.Count; i++)
            //    {
            //        if (hit.collider.gameObject == AttackList[i])
            //            return;
            //    }

            //    hitObj = hit.collider.gameObject;
            //    AttackList.Add(hitObj);
            //    AttackEnemy();
            //}
            


            if (this.transform.position.x <= maxXPos)
                this.transform.Translate(Vector2.right * bulletSpeed * 1.5f);
            else
			{
                ObjectReturn();
                if (trail != null)
                    trail.time = -1.0f;
			}

            for (int i = 0; i < AttackList.Count; i++)
            {
                if (this.transform.position.x >= AttackList[i].transform.position.x)
                {
                    hitObj = AttackList[i];
                    AttackEnemy();
                    AttackList.RemoveAt(i);
                    i--;
                }

            }


            // 사거리 동시 데미지
        }

        public void AttackEnemy(SplashType isSplashType = SplashType.NonSplash)
        {
            if (ishit)
                return;

            if (hitObj != null)
            {
                if (isSplashType == SplashType.NonSplash)
                {
                    if (hitObj.TryGetComponent(out Choi.Enemy enemy))
                    {
                        if (hitObj.TryGetComponent(out IDamageable target))
                        {
                            target.OnDamage(Damage);
                            if (trail != null)
                                trail.time = -0.1f;
                        }

                        if (attackType != Altair.AttackType.Piercing)
                            ishit = true;

                        //if (isSlow)
                        //{
                        //    enemy.slowTimer = 2.0f;
                        //    enemy.speed = 0.5f;
                        //    enemy.slowFunc(true);
                        //}

                    }
                }
                else
                {
                    Collider2D[] colls = Physics2D.OverlapBoxAll(hitObj.transform.position, new Vector2(3, 3), 0, enemylayer);
                    if (colls != null)
                    {
                        for (int i = 0; i < colls.Length; i++)
                        {
                            if (colls[i].TryGetComponent(out Choi.Enemy enemy))
                            {
                                if (colls[i].TryGetComponent(out IDamageable target))
                                {
                                    target.OnDamage(Damage);
                                    if (trail != null)
                                        trail.time = -0.1f;
                                }
                                ishit = true;

                                //if (isSlow)
                                //{
                                //    enemy.slowTimer = 2.0f;
                                //    enemy.speed = 0.5f;
                                //}
                            }
                        }
                    }

                }

                ObjectReturn();
                if (trail != null)
                    trail.time = -0.1f;
            }
        }

        internal Vector2 BezierTest(Vector2 p1, Vector2 p2, Vector2 r1, Vector2 r2, float value)
        {
            Vector2 v1 = Vector2.Lerp(p1, r1, value);
            Vector2 v2 = Vector2.Lerp(r1, r2, value);
            Vector2 v3 = Vector2.Lerp(r2, p2, value);

            Vector2 v4 = Vector2.Lerp(v1, v2, value);
            Vector2 v5 = Vector2.Lerp(v2, v3, value);

            Vector2 v6 = Vector2.Lerp(v4, v5, value);
            return v6;
        }



        //void FindEnemy()
        //{
        //    Enemy[] a_EnemyList = GameObject.FindObjectsOfType<Enemy>();

        //    if (a_EnemyList.Length <= 0) //그냥 먼 어딘 가를 추적하게 한다.
        //        return;

        //    GameObject a_Find_Mon = null;
        //    float a_CacDist = 0.0f;
        //    Vector3 a_CacVec = Vector3.zero;
        //    for (int i = 0; i < a_EnemyList.Length; ++i)
        //    {
        //        a_CacVec = a_EnemyList[i].transform.position - transform.position;
        //        a_CacVec.z = 0.0f;
        //        a_CacDist = a_CacVec.magnitude;

        //        if (4.0f < a_CacDist) //4.0f ~ 5.0f
        //            continue;

        //        a_Find_Mon = a_EnemyList[i].gameObject;
        //        break;
        //    }//for (int i = 0; i < a_EnemyList.Length; ++i)

        //    hitObj = a_Find_Mon;
        //    if (hitObj != null)
        //        isTaget = true;
        //}//void FindEnemy()

        //void BulletHoming()
        //{
        //    enemyVec = hitObj.transform.position - transform.position;
        //    enemyVec.z = 0.0f;
        //    enemyVec.Normalize();

        //    //적을 향해 회전 이동하는 방법
        //    float angle = Mathf.Atan2(enemyVec.y, enemyVec.x) * Mathf.Rad2Deg;
        //    Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        //    transform.rotation = angleAxis;
        //    m_DirTgVec = transform.right;
        //    transform.Translate(Vector3.right * m_MoveSpeed * Time.deltaTime);
        //}
    }
}