using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Utility
{
    public class SystemSettingPanel : APanel
    {
        public Toggle musicMute;
        public Slider musicVolume;
        public Toggle soundMute;
        public Slider soundVolume;
        public InputField inputAIDelay;

        public override PanelType PanelType => PanelType.MessageBox;

        // Start is called before the first frame update
        protected void Start()
        {
            var settings = Present.GameMain.Instance.SystemSettings;
            musicMute.isOn = settings.musicMute;
            musicVolume.value = settings.musicVolume;
            soundMute.isOn = !settings.soundMute;
            soundVolume.value = settings.soundVolume;
            inputAIDelay.text = settings.aiDelay.ToString();
        }

        public void OnSaveSetting()
        {
            var settings = Present.GameMain.Instance.SystemSettings;
            settings.musicMute = musicMute.isOn;
            settings.musicVolume = musicVolume.value;
            settings.soundMute = !soundMute.isOn;
            settings.soundVolume = soundVolume.value;
            settings.aiDelay = System.Convert.ToInt32(inputAIDelay.text);
            Present.GameMain.Instance.Notify(Common.Core.SystemEventType.OnSystemSettingChanged, settings);
        }
    }
}