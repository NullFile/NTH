using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Enut4LJR
{
	public class LJR_UpBulletShoot : MonoBehaviour
	{
		public GameObject bulletPrefab;
		public GameObject firstObj;

		//유도용 변수
		public GameObject[] firstObjGroup = new GameObject[5];
		public GameObject m_MinPosObj = null;
		public static int m_MinIdx = -1;

		RaycastHit2D hit;
		RaycastHit2D[] hits;
		public LayerMask enemyLayer;

		List<GameObject> EnemyList;

		////화염방사기(관통) 관련 변수
		ParticleSystem FlameParticle;
		bool isShoot = false;

		private void Start() => StartFunc();

		private void StartFunc()
		{
			EnemyList = new List<GameObject>();
			FlameParticle = GetComponentInChildren<ParticleSystem>();
			//FlameParticle.Stop();
		}

		private void Update() => UpdateFunc();

		private void UpdateFunc()
		{
			//유도 ---------------------------------------------------------------------------
			for (int ii = 4; ii >= 0; ii--)
			{
				Vector2 RayLineVec = new Vector2(this.transform.position.x, this.transform.position.y + (ii - 2) * 1.53f);
				hit = Physics2D.Raycast(RayLineVec, Vector2.right, 20.0f, enemyLayer);
				if (hit)
				{
					if (hit.collider.gameObject != firstObjGroup[ii])
						firstObjGroup[ii] = hit.collider.gameObject;
				}
				else
				{
					firstObjGroup[ii] = null;
				}
			}

			if (Input.GetKeyDown(KeyCode.Z) && firstObjGroup != null)
			{
				CalMinPosx();
				if (m_MinPosObj == null)
					return;

				////유도샷
				//SetBullType(m_MinPosObj, Altair.AttackType.Homing, LJR_BulletCtrl.ShotType.Front,
				//	LJR_BulletCtrl.SplashType.NonSplash, 7.5f, 0.0f);

				//체이서 샷
				SetBullType(m_MinPosObj, Altair.AttackType.Chasing, LJR_BulletCtrl.ShotType.Front);
				

			}
			//유도 ---------------------------------------------------------------------------

			hit = Physics2D.Raycast(this.transform.position, Vector2.right, 20.0f, enemyLayer);
			if (hit)
			{
				if (hit.collider.gameObject != firstObj)
					firstObj = hit.collider.gameObject;
			}
			else
			{
				firstObj = null;	
			}

			if (Input.GetKeyDown(KeyCode.G))
			{
			}

			if (Input.GetKeyDown(KeyCode.Space) && firstObj != null)
			{
				////일반 샷
				//SetBullType(firstObj, Altair.AttackType.Directional, LJR_BulletCtrl.ShotType.Front);

				////트리플 샷
				//for (int ii = 0; ii < 3; ii++)
				//{
				//    SetBullType(firstObj, Altair.AttackType.Directional, (LJR_BulletCtrl.ShotType)ii);
				//}

				////백샷
				//for (int ii = 0; ii < 2; ii++)
				//{
				//    SetBullType(firstObj, Altair.AttackType.Directional, LJR_BulletCtrl.ShotType.Back);
				//}
				//SetBullType(firstObj, Altair.AttackType.Directional,
				//    LJR_BulletCtrl.ShotType.Front, LJR_BulletCtrl.SplashType.NonSplash);

				////곡사포
				//SetBullType(firstObj, Altair.AttackType.Balistic,
				//	LJR_BulletCtrl.ShotType.Front, LJR_BulletCtrl.SplashType.Splash);

				//관통샷
				if (hit)
				{
					hits = Physics2D.RaycastAll(this.transform.position, Vector2.right, Mathf.Infinity, enemyLayer);
					if (hits.Length > 0)
					{
						EnemyList.Clear();
						for (int i = 0; i < hits.Length; i++)
						{
							EnemyList.Add(hits[i].collider.gameObject);
						}
					}
					
					

					for (int ii = 0; ii < EnemyList.Count; ii++)
					{
						if (EnemyList[ii].transform.position.x < transform.position.x + 6)
						{
							isShoot = true;
						}
						else
							continue;
					}

					if (isShoot)
					{
						SetBullType(firstObj, Altair.AttackType.Piercing, LJR_BulletCtrl.ShotType.Front,
						LJR_BulletCtrl.SplashType.NonSplash, 0.0f, 5.0f, 1, false, EnemyList);
						isShoot = false;
					}
				}




				//------------------------------------------------


				//일반샷
				//GameObject obj = Instantiate(bulletPrefab);
				//obj.transform.position = this.transform.position;
				//if (obj.TryGetComponent(out LJR_BulletCtrl bull))
				//{
				//    bull.p1 = this.transform.position;
				//    bull.r1 = this.transform.position;
				//    //bull.r1.x += 2.5f;
				//    bull.r1.y += 5.0f;
				//
				//    bull.Damage = 1;
				//    bull.hitObj = firstObj;
				//    bull.attackType = Altair.AttackType.Directional;
				//    bull.shotType = LJR_BulletCtrl.ShotType.Front;  //직선 공격에 대한 보조 타입
				//    bull.splashType = LJR_BulletCtrl.SplashType.NonSplash;
				//    bull.isSlow = true;
				//}

				//유도샷
				//GameObject obj = Instantiate(bulletPrefab);
				//obj.transform.position = this.transform.position;
				//if (obj.TryGetComponent(out LJR_BulletCtrl bull))
				//{
				//    bull.p1 = this.transform.position;
				//    bull.r1 = this.transform.position;
				//    bull.r1.x += 7.5f;
				//    //bull.r1.y += 5.0f;

				//    bull.Damage = 1;
				//    bull.hitObj = firstObj;
				//    bull.attackType = Altair.AttackType.Homing;
				//    bull.shotType = LJR_BulletCtrl.ShotType.Front;  //직선 공격에 대한 보조 타입
				//    bull.splashType = LJR_BulletCtrl.SplashType.NonSplash;
				//    bull.isSlow = false;
				//}

				//백샷
				//for (int i = 0; i < 2; i++)
				//{
				//    obj = Instantiate(bulletPrefab);
				//    obj.transform.position = this.transform.position;
				//    if (obj.TryGetComponent(out LJR_BulletCtrl bull2))
				//    {
				//        bull2.p1 = this.transform.position;
				//        bull2.r1 = this.transform.position;
				//        //r1.x += 2.5f;
				//        bull2.r1.y += 5.0f;

				//        bull2.hitObj = firstObj;
				//        bull2.attackType = Altair.AttackType.Directional;
				//        bull2.shotType = LJR_BulletCtrl.ShotType.Back;
				//    }
				//}

				//트리플샷
				//for (int i = 0; i < 4; i++)
				//{
				//    GameObject obj = Instantiate(bulletPrefab);
				//    obj.transform.position = this.transform.position;
				//    if (obj.TryGetComponent(out LJR_BulletCtrl bull))
				//    {
				//        bull.p1 = this.transform.position;
				//        bull.r1 = this.transform.position;
				//        //r1.x += 2.5f;
				//        bull.r1.y += 5.0f;

				//        bull.hitObj = firstObj;
				//        bull.attackType = Altair.AttackType.Directional;
				//        bull.shotType = (LJR_BulletCtrl.ShotType)i;
				//    }
				//}

				//관통샷

				//if (hit)
				//{
				//    hits = Physics2D.RaycastAll(this.transform.position, Vector2.right, Mathf.Infinity, enemyLayer);

				//    if (hits.Length > 0)
				//    {
				//        EnemyList.Clear();
				//        for (int i = 0; i < hits.Length; i++)
				//        {
				//            EnemyList.Add(hits[i].collider.gameObject);
				//        }
				//    }

				//    Debug.Log(EnemyList.Count);
				//    GameObject obj = Instantiate(bulletPrefab);
				//    obj.transform.position = this.transform.position;
				//    if (obj.TryGetComponent(out LJR_BulletCtrl bull))
				//    {
				//        bull.p1 = this.transform.position;
				//        bull.r1 = this.transform.position;
				//        //bull.r1.x += 2.5f;
				//        bull.r1.y += 5.0f;

				//        bull.AttackList = EnemyList;
				//        bull.Damage = 500;
				//        bull.attackType = Altair.AttackType.Piercing;
				//        bull.shotType = LJR_BulletCtrl.ShotType.Front;  //직선 공격에 대한 보조 타입
				//        bull.splashType = LJR_BulletCtrl.SplashType.NonSplash;
				//        bull.isSlow = false;
				//    }
				//}



			}


		}

		void SetBullType(GameObject a_firstObj, Altair.AttackType a_AttType,
			LJR_BulletCtrl.ShotType a_ShotType, LJR_BulletCtrl.SplashType a_SplType = LJR_BulletCtrl.SplashType.NonSplash,
			float r1x = 0.0f, float r1y = 5.0f, int a_Damage = 1, bool a_isSlow = false, List<GameObject> a_EnemyList = null)
		{
			if (!MemoryPoolManager.instance) return;

			GameObject obj = MemoryPoolManager.instance.GetObject(0, this.transform.position);
			if (obj != null && obj.TryGetComponent(out LJR_BulletCtrl bull))
			{
				bull.p1 = this.transform.position;
				bull.r1 = this.transform.position;

				bull.r1.x += r1x;
				bull.r1.y += r1y;

				////bull.r1.x += 2.5f;
				//bull.r1.y += 5.0f;

				bull.ishit = false;
				bull.AttackList = a_EnemyList;
				bull.Damage = a_Damage;
				bull.hitObj = a_firstObj;
				bull.attackType = a_AttType;
				bull.shotType = a_ShotType;  //직선 공격에 대한 보조 타입
				bull.splashType = a_SplType;
				bull.isSlow = a_isSlow;
				bull.value = 0;
			}

			if (a_AttType == Altair.AttackType.Piercing)
			{
				GameObject flame = MemoryPoolManager.instance.GetObject(2, this.transform.position);
				flame.GetComponentInChildren<ParticleSystem>().Play();
			}

		}

		public void CalMinPosx()
		{
			float a_MinPosx = 12;
			int a_MinIdx = -1;
			int a_Stack = 0;
			

			for (int ii = 0; ii < 5; ii++)
			{
				if (firstObjGroup[ii] == null)
				{
					a_Stack++;
					continue;
				}

				if (firstObjGroup[ii].transform.position.x < a_MinPosx)
				{
					a_MinPosx = firstObjGroup[ii].transform.position.x;
					a_MinIdx = ii;
				}
			}

			if (a_MinIdx != -1)
			{
				m_MinPosObj = firstObjGroup[a_MinIdx];
				m_MinIdx = a_MinIdx;
			}

			if (a_Stack == 5)
			{
				m_MinPosObj = null;
				for (int ii = 0; ii < firstObjGroup.Length; ii++)
				{
					firstObjGroup[ii] = null;
				}
			}

		}

	}
}