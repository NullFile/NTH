using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair;

namespace Yuspace
{
    
    public class SelfDestroyCtrl : MonoBehaviour
    {
        public GameObject InitData;
        public GameObject DestroyEff;
        public GameObject RobotObj;
        int DataCheck = 0;
        RaycastHit2D lefthit;
        int hp = 0;
        int sheidhp = 0;
        int dmg = 0;
        float moveSpeed = 0.0f;
        Animator ani;
        bool robotDestroy = false;
        bool DieCheck = false;
        Color color = new Color(1, 1, 1, 1);
        MeshRenderer[] rend;
        // Start is called before the first frame update
        void Start()
        {
            ani = GetComponentInChildren<Animator>();
            rend = GetComponentsInChildren<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if(InitData.activeSelf == false)
            {
                if(DataCheck == 0)
                {

                    DataInitFunc();
                    DataCheck++;
                }
            }

            Vector3 pos = transform.position;
            pos.x -= 1.0f;
            pos.y += 0.5f;
            lefthit = Physics2D.Raycast(pos, Vector2.left, 0.5f);
            Debug.DrawRay(pos, Vector2.left, Color.blue);

            if(lefthit.collider != null)
            {
                if (lefthit.collider.tag == "Tower")
                {
                    if(robotDestroy == false)
                    {
                        robotDestroy = true;
                        StartCoroutine(DestroyRobotFunc());
                    }
                }
            }

            if(robotDestroy == true && DieCheck == true)
            {
               



            }

        }


        private void FixedUpdate()
        {
            if(robotDestroy == false)
                transform.Translate(Vector2.left * 0.2f * Time.deltaTime);

        }

        private void OnDestroy()
        {
            
        }


        void DataInitFunc()
        {

            if (!JSONParser.DataValidation(GlobalData.enemyData["Jack-in-the-box Zombie"]["hp"], out hp)) return;
            if (!JSONParser.DataValidation(GlobalData.enemyData["Jack-in-the-box Zombie"]["dam"], out dmg)) return;
            if (!JSONParser.DataValidation(GlobalData.enemyData["Jack-in-the-box Zombie"]["moveSpeed"], out moveSpeed)) return;


        }

        IEnumerator DestroyRobotFunc()
        {
            ani.SetTrigger("Die");
            yield return new WaitForSeconds(1.0f); 
            while(true)
            {
                if (color.g > 0)
                {
                    color.g -= (Time.deltaTime / 2.0f);
                }
                if (color.b > 0)
                    color.b -= (Time.deltaTime / 2.0f);

                for (int ii = 0; ii < rend.Length; ii++)
                {
                    rend[ii].material.color = color;
                }

                if (color.g <= 0 && color.b <= 0)
                {
                    RobotObj.SetActive(false);
                    GameObject eff = Instantiate(DestroyEff) as GameObject;
                    eff.transform.position = transform.position;
                    Destroy(this.gameObject, 2.0f);
                    yield break;
                }

                yield return null;

            }
            
        }
    }
}
