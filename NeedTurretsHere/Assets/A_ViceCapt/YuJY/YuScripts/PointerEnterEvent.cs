using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

namespace Yuspace
{
    public class PointerEnterEvent : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler
    {
        public Image img;
        public Text txt;
        Color color = new Color(1, 1, 1, 1);
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        //void Update()
        //{
        
        //}

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (img.color == new Color(1,1,1,1))
                return;
            else
            {
                color = img.color;
                color.a = 0.99f;
                img.color = color;
                txt.color = color;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (img.color == new Color(1, 1, 1, 1))
                return;
            color.a = 0.5f;
            img.color = color;
            txt.color = color;

        }
    }
}
