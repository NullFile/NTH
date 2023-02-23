using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;
using SungJae;

namespace KJH
{
    public enum TowerType
    {
        Normal,
        Shovel,
    }

    public class Drag : Turret_Ctrl
    {
        public TowerType type;
        public GameObject tower;

        [HideInInspector] public CreateDrag createDrag;
        public float delay = 5.0f;

        GameObject ground;
        bool isCreate = false;
        Vector3 vec;

        public int num = 0;

        void Update()
        {                
            // 왼쪽 버튼 클릭
            if (Input.GetMouseButtonDown(1))
            {
                ObjectReturn();
            }

            // 드래그 상태
            if (Input.GetMouseButton(0))
            {
                Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                vec.z = 0.0f;

                this.transform.position = vec;
            }

            // 삭제
            if (Input.GetMouseButtonUp(0))
            {
                if (isCreate == true)
                {
                    if (type == TowerType.Shovel)
                        Delete();
                    else
                        Create();
                }

                ObjectReturn();
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name.Contains("In_Node"))
            {
                isCreate = true;
                vec = collision.transform.position;
                ground = collision.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.name.Contains("In_Node"))
            {
                isCreate = false;
                vec = Vector3.zero;
                ground = null;
            }
        }

        void Create()
        {
            if (ground != null)
            {
                In_Node node = ground.GetComponent<In_Node>();

                if (node != null)
                {
                    if (node.tower != null)
                        return;
                    else
                    {
                        if (createDrag != null)
                            createDrag.SetDelay();

                        vec.z = 0.0f;

                        GameObject go = MemoryPoolManager.instance.GetObject(num, vec);
                        go.GetComponent<Turret_Ctrl>().ShotPoint = vec;
                        node.tower = go;
                    }
                }
            }
        }

        void Delete()
        {
            if (ground != null)
            {
                In_Node node = ground.GetComponent<In_Node>();

                if (node != null)
                {
                    if (node.tower != null)
                    {
                        node.tower.GetComponent<Turret_Ctrl>().turretObjectReturn();
                        node.tower = null;
                    }
                    else
                        return;
                }
            }
        }
    }
}