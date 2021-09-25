using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Present
{
    public interface IGameMain
    {
        void PlayMusic(string audioKey, bool isUrl = false);
        void PlaySound(string audioKey, bool isUrl = false);
        View.MainMenuPanel ShowMainMenuPanel(View.MainMenuPanel.MenuType type);

    }

    public static class GameMain
    {
        public static IGameMain Instance = null;
    }
}
