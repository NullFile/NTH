using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Yuspace
{
    public class TestMgr : MonoBehaviour
    {
        public Button Test;
        // Start is called before the first frame update
        void Start()
        {
            if (Test != null)
                Test.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene("ShopScene");
                });
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
