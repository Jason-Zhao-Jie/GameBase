using UnityEngine;
using UnityEngine.UI;

using CharacterInfo = GameBase.Common.Core.CharacterInfo;

namespace GameBase.View.Poker.Huolong
{
    public class PlayerHead_Huolong : MonoBehaviour
    {
        public Image avatarIcon;
        public Text playerName;
        public GameObject mainFlag;
        public Text campIndexTitle;
        public Image campIndexImage;
        public Text level;
        public Text score;

        public CharacterInfo Info
        {
            get
            {
                return info;
            }
            set
            {
                info = value;
                avatarIcon.sprite = MainScene.Instance.GetPlayerHeadIcon(info.imageId);
                playerName.text = info.name;
                mainFlag.SetActive(false);
            }
        }

        public bool IsMain
        {
            get
            {
                return mainFlag.activeSelf;
            }
            set
            {
                mainFlag.SetActive(value);
            }
        }

        public void SetCampIndex(int index)
        {
            campIndexTitle.text = "����:";
            playerName.color = indexColor[index];
            campIndexTitle.color = indexColor[index];
            campIndexImage.color = indexColor[index];
            level.color = indexColor[index];
            score.color = indexColor[index];
        }

        public void SetLevel(int level)
        {
            this.level.text = string.Format("�ȼ�:{0}", level);
        }

        public void SetScore(int score)
        {
            this.score.text = string.Format("�÷�:{0}", score);
        }

        public void OnClick()
        {
            MainScene.Instance.ShowPlayerInfoPanel(info);
        }

        private CharacterInfo info;

        private static readonly Color[] indexColor = new Color[]{
            Color.blue,
            Color.red,
            Color.green,
            Color.magenta,
            Color.cyan,
            Color.yellow,
        };
    }
}