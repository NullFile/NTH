using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Yuspace
{
    public class JumpingTest : MonoBehaviour
    {
        Animator ani;
        public bool isJump = false;
        bool isWalk = true;
        [SerializeField] internal Vector3 p1;
        [SerializeField] internal Vector3 p2;
        [SerializeField] internal Vector3 r1;
        [SerializeField] internal Vector3 r2;

        [SerializeField] [Range(0, 1)] private float value;
        // Start is called before the first frame update
        void Start()
        {
            ani = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if(isWalk == true)
            {
                p1 = transform.position;
                r1 = p1;
                r1.y = p1.y + 4.0f;
                p2.x = p1.x - 3.0f;
                p2.y = p1.y;
                r2.x = p2.x;
                r2.y = p2.y + 4.0f;
            }
            transform.position = BezierTest(p1, p2, r1, r2, value);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isWalk = false;
                ani.SetTrigger("Jump");
                ani.SetTrigger("Up");
                ani.SetTrigger("Jumping");


            }

            if(value >= 0.8f && value < 1.0f)
            {
                ani.SetTrigger("Landing");
            }

            if (value >= 1.0f)
            {
                p1 = transform.position;
                r1 = p1;
                r1.y = p1.y + 4.0f;
                p2.x = p1.x - 3.0f;
                p2.y = p1.y;
                r2.x = p2.x;
                r2.y = p2.y + 4.0f;
                value = 0.0f;
                isJump = false;
                ani.SetTrigger("Walk");

            }


        }

        private void FixedUpdate()
        {
            if (isJump == true)
            {
                value += 0.02f;
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


    }


    //[CanEditMultipleObjects]
    //[CustomEditor(typeof(JumpingTest))]
    //public class VezierEditor : Editor
    //{
    //    private void OnSceneGUI()
    //    {
    //        JumpingTest gen = (JumpingTest)target;

    //        gen.p1 = Handles.PositionHandle(gen.p1, Quaternion.identity);
    //        gen.p2 = Handles.PositionHandle(gen.p2, Quaternion.identity);
    //        gen.r1 = Handles.PositionHandle(gen.r1, Quaternion.identity);
    //        gen.r2 = Handles.PositionHandle(gen.r2, Quaternion.identity);

    //        Handles.DrawLine(gen.p1, gen.r1);
    //        Handles.DrawLine(gen.p2, gen.r2);

    //        int arc = 20;
    //        for (float i = 0; i < arc; i++)
    //        {
    //            float value1 = i / arc;
    //            float value2 = (i + 1) / arc;
    //            Vector2 pos1 = gen.BezierTest(gen.p1, gen.p2, gen.r1, gen.r2, value1);
    //            Vector2 pos2 = gen.BezierTest(gen.p1, gen.p2, gen.r1, gen.r2, value2);
    //            Handles.DrawLine(pos1, pos2);
    //        }
    //    }
    //}
}
