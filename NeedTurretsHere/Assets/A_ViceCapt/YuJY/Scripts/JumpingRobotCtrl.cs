using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Yuspace
{
    public class JumpingRobotCtrl : MonoBehaviour
    {
        Animator ani;
        [HideInInspector] public bool isJump = false;
        bool isWalk = true;
        [SerializeField] internal Vector3 p1;
        [SerializeField] internal Vector3 p2;
        [SerializeField] internal Vector3 r1;
        [SerializeField] internal Vector3 r2;

        [SerializeField] [Range(0, 1)] private float value;

        RaycastHit2D lefthit;
        RaycastHit2D bottomhit;
        // Start is called before the first frame update
        void Start()
        {
            ani = GetComponentInChildren<Animator>();

        }

        // Update is called once per frame
        void Update()
        {


            if (Input.GetKeyDown(KeyCode.Space))
            {
                isWalk = false;
                ani.SetTrigger("Jump");
                ani.SetTrigger("Up");
                ani.SetTrigger("Jumping");


            }

            if (value >= 0.8f && value < 1.0f)
            {
                ani.SetTrigger("Landing");
            }

            if (value >= 1.0f)
            {
                VezierSet();
                value = 0.0f;
                isJump = false;
                isWalk = true;
                ani.SetTrigger("Walk");

            }
            Vector3 raypos = transform.position;
            raypos.x -= 1.0f;
            raypos.y += 0.7f;
            lefthit = Physics2D.Raycast(raypos, Vector2.left, 1);
            Debug.DrawRay(raypos, Vector2.left,Color.blue);


        }


        private void FixedUpdate()
        {
            if (isWalk == true)
            {
                VezierSet();
                transform.Translate(Vector2.left * 0.03f * Time.deltaTime);
            }
            if (isJump == true)
            {
                value += 0.02f;
                transform.position = BezierTest(p1, p2, r1, r2, value);

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



        void VezierSet()
        {
            p1 = transform.position;
            r1 = p1;
            r1.y = p1.y + 4.0f;
            p2.x = p1.x - 3.0f;
            p2.y = p1.y;
            r2.x = p2.x;
            r2.y = p2.y + 4.0f;
        }
    }


    //[CanEditMultipleObjects]
    //[CustomEditor(typeof(JumpingRobotCtrl))]
    //public class VezierEditor : Editor
    //{
    //    private void OnSceneGUI()
    //    {
    //        JumpingRobotCtrl gen = (JumpingRobotCtrl)target;

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
