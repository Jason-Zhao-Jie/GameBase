using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Component
{
    public class SoundPlayer : MonoBehaviour
    {
        public Button[] buttons;
        public AudioClip sound;

        // Start is called before the first frame update
        private void Start()
        {
            foreach(var b in buttons)
            {
                b.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            Present.GameMain.Instance.PlaySound(sound);
        }
    }
}
