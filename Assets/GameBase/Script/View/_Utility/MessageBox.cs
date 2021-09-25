using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Utility
{
    public class MessageBox : MonoBehaviour
    {
        public Text textContent;
        public Button btnLeft;
        public Text textLeft;
        public Button btnMid;
        public Text textMid;
        public Button btnRight;
        public Text textRight;

        public event System.Action OnClickLeft;
        public event System.Action OnClickMid;
        public event System.Action OnClickRight;
        
        public void Init(string textBtnLeft, System.Action callbackLeft, string textBtnRight = null, System.Action callbackRight = null, string textBtnMid = null, System.Action callbackMid = null)
        {
            txtLeft = textBtnLeft;
            OnClickLeft = callbackLeft;
            txtRight = textBtnRight;
            OnClickRight = callbackRight;
            txtMid = textBtnMid;
            OnClickMid = callbackMid;
            if (textLeft != null)
            {
                textLeft.text = txtLeft;
            }
            if (btnLeft != null)
            {
                btnLeft.gameObject.SetActive(callbackLeft != null);
            }
            if (textRight != null)
            {
                textRight.text = txtRight;
            }
            if (btnRight != null)
            {
                btnRight.gameObject.SetActive(callbackRight != null);
            }
            if (textMid != null)
            {
                textMid.text = txtMid;
            }
            if (btnMid != null)
            {
                btnMid.gameObject.SetActive(callbackMid != null);
            }
        }

        public void OnClick(int index)
        {
            switch (index)
            {
                case 0:
                    OnClickLeft();
                    break;
                case 1:
                    OnClickMid();
                    break;
                case 2:
                    OnClickRight();
                    break;
            }
        }

        protected void Start()
        {
            textLeft.text = txtLeft;
            btnLeft.gameObject.SetActive(OnClickLeft != null);
            textRight.text = txtRight;
            btnRight.gameObject.SetActive(OnClickRight != null);
            textMid.text = txtMid;
            btnMid.gameObject.SetActive(OnClickMid != null);
        }

        private string txtLeft;
        private string txtMid;
        private string txtRight;
    }
}
