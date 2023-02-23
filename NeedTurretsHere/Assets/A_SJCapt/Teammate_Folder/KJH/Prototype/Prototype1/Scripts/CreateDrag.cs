using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Altair_Memory_Pool_Pro;
using Choi;
using SungJae;
using Altair;

namespace KJH
{
    public class CreateDrag : Turret_Ctrl
    {
        public GameObject dragTower;
        public Image DelayImg;
        public Image NameImg;

        Vector3[] v = new Vector3[4];
        Vector3 mousePos;

        bool isPick = false;
        float Check = 0.0f;

        [SerializeField] float Delay = 0.0f;

        public int num = 0;

        protected override void SetType(int ii)
        {
            base.SetType(ii);
        }

        void Start()
        {
            GetComponent<RectTransform>().GetWorldCorners(v);

            //Debug.Log(turretAttDamage);
        }

        bool isUp;

        void Update()
        {
            if (Altair.GlobalData.turretDataJson != null)
            {
                GlobalData.choi_InitData();
            }

            if (GlobalData.choi_m_TrList != null && turretIdx == -1)
            {
                SetType(num);
                //CheckTime = turretAttWait;
                //Debug.Log(turretHp);

                NameImg.GetComponentInChildren<Text>().text = GlobalData.choi_m_TrList[num].m_name;

                //Debug.Log(GlobalData.choi_m_TrList[num].m_name);
            }

            isUp = ButtonInside();

            if (isUp)
                NameImg.gameObject.SetActive(true);
            else
                NameImg.gameObject.SetActive(false);

            if (0.0f < Check)
            {
                Check -= Time.deltaTime;

                if (DelayImg != null)
                    DelayImg.fillAmount = Check / Delay;

                if (Check <= 0.0f)
                    Check = 0.0f;
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isPick = ButtonInside();

                    if (isPick == true)
                    {
                        Create();
                    }
                }
            }
        }

        bool ButtonInside()
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (v[0].x <= mousePos.x && mousePos.x <= v[2].x &&
                v[0].y <= mousePos.y && mousePos.y <= v[2].y)
            {
                return true;
            }

            return false;
        }

        void Create()
        {
            Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vec.z = 0.0f;

            GameObject go = MemoryPoolManager.instance.GetObject(num, vec);

            Drag d = go.GetComponent<Drag>();

            if (d != null)
                d.createDrag = this;
        }

        public void SetDelay()
        {
            Check = Delay;
        }
    }
}