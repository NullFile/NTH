using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Altair_Memory_Pool_Pro;

namespace Yuspace
{
    public class TestEff : MemoryPoolingFlag
    {
        public float DestroyTime = 0.5f;
        Animator ani;
        AnimatorStateInfo info;

        // Start is called before the first frame update
        void Start()
        {
            ani = GetComponentInChildren<Animator>();
            info = ani.GetCurrentAnimatorStateInfo(0);
            DestroyTime = 0.5f;

        }

        // Update is called once per frame
        void Update()
        {
            DestroyTime -= Time.deltaTime;
            if (DestroyTime <= 0.0f)
            {
                ObjectReturn();
            }


        }

        private void OnEnable()
        {
            ani = GetComponentInChildren<Animator>();

            DestroyTime = 0.5f;

            ani.Play("Explosion2_Effect", 0, -0.0f);

        }
    }
}
