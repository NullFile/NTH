using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;
namespace Yuspace
{
    public class GrenadeCtrl : MemoryPoolingFlag
    {
        public bool isHit = false;
        bool stopgrenade = false;
        JumpingRobotCtrl JumpingRobot;
        float Delay = 1.0f;
        Rigidbody2D rig;
        // Start is called before the first frame update
        void Start()
        {
            JumpingRobot = FindObjectOfType<JumpingRobotCtrl>();
            rig = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {

            if (JumpingRobot == null)
                return;

            //if (JumpingRobot.BeforeJump.y >= transform.position.y)
            //{
            //    if(stopgrenade == false)
            //    {
            //        stopgrenade = true;
            //        Vector3 pos = transform.position;
            //        pos.y = JumpingRobot.BeforeJump.y;
            //        Debug.Log(JumpingRobot.BeforeJump);
                    

            //        transform.position = pos;
            //    }
     
            //}

        }

        private void FixedUpdate()
        {
            if(stopgrenade == false)
            {
                //transform.Translate(0.0f, -10.0f * Time.deltaTime, 0.0f);
                rig.AddForce(new Vector2(0.3f,0.5f) * 10.0f);

            }
            transform.Rotate(0, 200.0f * Time.deltaTime, 0);


            if(Delay > 0.0f)
            {
                Delay -= Time.deltaTime;
                if (Delay <= 0.0f)
                {
                    ExplosionEff();
                    ObjectReturn();

                }
            }    
        }

        void ExplosionEff()
        {
            if (!MemoryPoolManager.instance)
                return;

            GameObject obj = MemoryPoolManager.instance.GetObject(1, transform.position);
            if(obj != null && obj.TryGetComponent(out TestEff Eff))
            {

            }
        }

        private void OnEnable()
        {
            stopgrenade = false;
            Delay = 1.0f;
        }
    }
}
