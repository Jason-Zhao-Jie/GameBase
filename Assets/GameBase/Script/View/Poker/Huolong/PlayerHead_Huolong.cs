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
        public Text campIndex;
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
            campIndex.text = string.Format("队伍{0}", index);
        }

        public void SetLevel(int level)
        {
            this.level.text = string.Format("等级{0}", level);
        }

        public void SetScore(int score)
        {
            this.score.text = string.Format("得分{0}", score);
        }

        public void OnClick()
        {
            MainScene.Instance.ShowPlayerInfoPanel(info);
        }

        private CharacterInfo info;
    }
}