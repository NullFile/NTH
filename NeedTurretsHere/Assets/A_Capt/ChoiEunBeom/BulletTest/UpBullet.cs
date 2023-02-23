using System.Collections;
using UnityEngine;

namespace Choi
{
    public class UpBullet : MonoBehaviour
    {
        

        public GameObject TargetObj;

        [SerializeField] internal Vector3 p1;
        [SerializeField] internal Vector3 p2;
        [SerializeField] internal Vector3 r1;
        [SerializeField] internal Vector3 r2;

        private float value = 0;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            p1 = this.transform.position;
            r1 = this.transform.position;
            //r1.x += 2.5f;
            r1.y += 5.0f;
        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
            if (TargetObj != null)
            {
                p2 = TargetObj.transform.position;
                r2 = TargetObj.transform.position;
                r2.x -= 2.5f;
                r2.y += 5.0f;

                if (value > 0.95f)
                    AttackEnemy();
            }

            if (value >= 0.99f)
                this.gameObject.SetActive(false);

            if (value < 0.4f)
                value += Time.deltaTime;
            else if (value <= 0.4f && 0.6f < value)
                value += Time.deltaTime * 0.4f;
            else
                value += Time.deltaTime;

            transform.position = BezierTest(p1, p2, r1, r2, value);
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


        public void AttackEnemy()
        {
            if (TargetObj != null)
            {
                if (TargetObj.TryGetComponent(out Enemy enemy))
                {
                    enemy.hp -= 1;
                    this.gameObject.SetActive(false);
                    Debug.Log(TargetObj.name + "에게 데미지 1 부여 남은 체력 : " + enemy.hp);

                    if (enemy.hp <= 0)
                    {
                        Destroy(enemy.gameObject);
                    }
                }
            }
        }
    }
}