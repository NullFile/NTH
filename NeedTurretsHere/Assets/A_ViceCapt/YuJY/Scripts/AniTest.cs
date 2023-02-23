using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yuspace
{
    public class AniTest : MonoBehaviour
    {
        JumpingTest Jump;
        // Start is called before the first frame update
        void Start()
        {
            Jump = GetComponentInParent<JumpingTest>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void Jumping()
        {
            Debug.Log("ÂÀÇª");
            Jump.isJump = true;
        }
    }
}
