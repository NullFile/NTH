using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enut4LJR
{
    public class DroneCtrl : MonoBehaviour
    {
        //이동용 변수
        [SerializeField] internal Vector3 p1;
        [SerializeField] internal Vector3 p2;
        [SerializeField] internal Vector3 r1;
        [SerializeField] internal Vector3 r2;
        public float value = 0.0f;

        // Start is called before the first frame update
        //void Start()
        //{
        //    
        //}
        //
        //// Update is called once per frame
        //void Update()
        //{
        //    
        //}

        public void MoveDrone(Vector3 a_StartPos, Vector3 a_ArrivePos, bool isDown)
		{
            if(isDown)
			{
                p1 = a_StartPos;
                r1 = a_ArrivePos;
                p2 = a_ArrivePos;
                r2 = a_ArrivePos;

                r1.y -= 0.5f;
                r2.y -= 0.5f;

                
                value += Time.deltaTime; 

                if (value >= 1.0f)
				{
                    value = 1.0f;
                    this.transform.position = a_ArrivePos;
				}
			}
            else
			{
                p1 = a_StartPos;
                r1 = a_StartPos;
                p2 = a_ArrivePos;
                r2 = a_StartPos;

                r1.y -= 0.5f;
                r2.y -= 0.5f;

                value += Time.deltaTime;

                if (value >= 1.0f)
                {
                    value = 1.0f;
                    this.transform.position = a_ArrivePos;
                }
            }

            this.transform.position = BezierTest(p1, p2, r1, r2, value);
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
    }
}
