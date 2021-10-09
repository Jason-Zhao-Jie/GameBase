using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View
{    public enum PanelType
    {
        None,
        MessageBox,
        PlayerInfoPanel,
        SystemSettingPanel,
        MainMenu,
        GameSettingPanel
    }

    public abstract class APanel : MonoBehaviour
    {
        public abstract PanelType PanelType { get;}
        public virtual void Close()
        {
            MainScene.Instance.PopPanel();
        }
    }
}
