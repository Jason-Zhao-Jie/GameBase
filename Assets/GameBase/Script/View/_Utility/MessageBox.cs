using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Utility
{
    public class MessageBox : APanel
    {
        public Text textContent;
        public Button btnLeft;
        public Text textLeft;
        public Button btnMid;
        public Text textMid;
        public Button btnRight;
        public Text textRight;

        public event System.Func<bool> OnClickLeft;
        public event System.Func<bool> OnClickMid;
        public event System.Func<bool> OnClickRight;

        public override PanelType PanelType => PanelType.MessageBox;
        
        public void Init(string contentText, string textBtnLeft, System.Func<bool> callbackLeft, string textBtnRight = null, System.Func<bool> callbackRight = null, string textBtnMid = null, System.Func<bool> callbackMid = null)
        {
            txtContent = contentText;
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
                    if (OnClickLeft())
                    {
                        Close();
                    }
                    break;
                case 1:
                    if (OnClickMid())
                    {
                        Close();
                    }
                    break;
                case 2:
                    if (OnClickRight())
                    {
                        Close();
                    }
                    break;
            }
        }

        protected void Start()
        {
            textContent.text = txtContent;
            textLeft.text = txtLeft;
            btnLeft.gameObject.SetActive(OnClickLeft != null);
            textRight.text = txtRight;
            btnRight.gameObject.SetActive(OnClickRight != null);
            textMid.text = txtMid;
            btnMid.gameObject.SetActive(OnClickMid != null);
        }

        private string txtContent;
        private string txtLeft;
        private string txtMid;
        private string txtRight;
    }
}
