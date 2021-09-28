using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Utility {
    public class PlayerInfoPanel : APanel
    {
        public Button btnAvatar;
        public Image avatar;
        public Text labelName;
        public Button btnChangeName;
        public InputField inputName;
        public Button btnSaveName;
        public Text labelGender;
        public Dropdown dropGender;
        public Text labelAge;
        public InputField inputAge;
        public InputField inputStateWords;

        public override PanelType PanelType => PanelType.MessageBox;

        public void Init(Common.Core.CharacterInfo info, bool isMe = false)
        {
            oldInfo = info;
            this.isMe = isMe;
            if (avatar != null)
            {
                SetDataAsDefault();
            }
        }

        public void OnClickEditAvatar()
        {
            MainScene.Instance.ShowTips("修改头像功能正在开发中");
        }

        public void OnClickEditName()
        {
            labelName.gameObject.SetActive(false);
            btnChangeName.gameObject.SetActive(false);
            inputName.gameObject.SetActive(true);
            btnSaveName.gameObject.SetActive(true);
        }

        public void OnClickSaveName()
        {
            labelName.gameObject.SetActive(true);
            btnChangeName.gameObject.SetActive(true);
            labelName.text = inputName.text;
            inputName.gameObject.SetActive(false);
            btnSaveName.gameObject.SetActive(false);
        }

        public void OnClickClose()
        {
            if (isMe)
            {
                oldInfo.name = labelName.text;
                oldInfo.gender = (Common.Core.Gender)dropGender.value;
                oldInfo.age = System.Convert.ToInt32(inputAge.text);
                oldInfo.stateWords = inputStateWords.text;
                Present.GameMain.Instance.Notify(Common.Core.SystemEventType.OnMyInfoChanged, oldInfo);
            }
            Close();
        }

        // Start is called before the first frame update
        protected void Awake()
        {
            if(oldInfo!= null)
            {
                SetDataAsDefault();
            }
        }

        private void SetDataAsDefault()
        {
            switch (oldInfo.imageType)
            {
                case Common.Core.SourceType.LocalId:
                    avatar.sprite = MainScene.Instance.GetPlayerHeadIcon(oldInfo.imageId);
                    break;
            }
            labelName.text = oldInfo.name;
            btnChangeName.gameObject.SetActive(isMe);
            inputName.text = oldInfo.name;
            inputName.gameObject.SetActive(false);
            btnSaveName.gameObject.SetActive(false);
            switch (oldInfo.gender)
            {
                case Common.Core.Gender.Male:
                    labelGender.text = "男";
                    break;
                case Common.Core.Gender.Female:
                    labelGender.text = "女";
                    break;
                case Common.Core.Gender.Other:
                default:
                    labelGender.text = "未知";
                    break;
            }
            labelGender.gameObject.SetActive(!isMe);
            dropGender.value = (int)oldInfo.gender;
            dropGender.gameObject.SetActive(isMe);
            if (oldInfo.birthYear != 0)
            {
                labelAge.text = (System.DateTime.Now.Year - oldInfo.birthYear).ToString();
            }
            else
            {
                labelAge.text = oldInfo.age.ToString();
                inputAge.text = oldInfo.age.ToString();
            }
            labelAge.gameObject.SetActive(!isMe);
            inputAge.gameObject.SetActive(isMe);
            inputStateWords.text = oldInfo.stateWords;
            inputStateWords.enabled = isMe;
        }

        private bool isMe;
        private Common.Core.CharacterInfo oldInfo;
    }
}