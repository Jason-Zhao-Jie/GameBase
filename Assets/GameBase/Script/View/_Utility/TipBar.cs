using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Utility
{
    public class TipBar : MonoBehaviour
    {
        public Text text;
        public bool autoRemove;
        public float secondsBeforeRemove;

        // Start is called before the first frame update
        protected void Start()
        {

        }

        // Update is called once per frame
        protected void Update()
        {
            if (autoRemove)
            {
                if (secondsBeforeRemove <= 0)
                {
                    transform.SetParent(null);
                    Destroy(gameObject);
                }
                else
                {
                    secondsBeforeRemove -= Time.deltaTime;
                }
            }
        }
    }
}