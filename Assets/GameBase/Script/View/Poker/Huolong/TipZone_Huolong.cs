using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Poker.Huolong
{
    public class TipZone_Huolong : MonoBehaviour
    {
        public Text content;
        public GameObject[] btns;
        public Text[] texts;

        protected void Awake()
        {
            callbacks = new System.Func<bool>[btns.Length];
        }

        public void ShowTip(string text, params (string btnText, System.Func<bool> callback)[] buttons)
        {
            gameObject.SetActive(true);
            content.text = text;
            ClearCallbacks();
            if(buttons == null || buttons.Length == 0)
            {
                foreach(var b in btns)
                {
                    b.SetActive(false);
                }
                btns[0].SetActive(true);
                callbacks[0] = () =>
                {
                    return true;
                };
            }
            else
            {
                for (int i=0;i<btns.Length;++i)
                {
                    bool show = buttons.Length > i && buttons[i].btnText != null;
                    btns[i].SetActive(show);
                    if (show)
                    {
                        texts[i].text = buttons[i].btnText;
                        callbacks[i] = buttons[i].callback;
                    }
                }
            }
        }

        public void HideTip()
        {
            gameObject.SetActive(false);
        }

        public void OnClick(GameObject caller)
        {
            for (int i = 0; i < btns.Length; ++i)
            {
                if(caller == btns[i])
                {
                    if(callbacks[i] == null || callbacks[i]())
                    {
                        HideTip();
                    }
                    break;
                }
            }
        }

        protected void ClearCallbacks()
        {
            for(int i = 0; i < callbacks.Length; ++i)
            {
                callbacks[i] = null;
            }
        }

        private System.Func<bool>[] callbacks;
    }
}