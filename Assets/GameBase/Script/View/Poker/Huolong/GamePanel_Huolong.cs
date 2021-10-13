using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameBase.Common.Interface.Poker.Huolong;
using GameBase.Common.Core.Poker;
using GameBase.Common.Core.Poker.Huolong;

namespace GameBase.View.Poker.Huolong
{
    public class GamePanel_Huolong : APokerGamePanel, IPlayerItem<IPlayerVector_Item>
    {
        #region Inspector Fields

        public GameObject gameStateInfo;
        public GameObject[] headInfo;
        public GameObject myStateInfo;

        #endregion Inspector Fields

        #region Intefaces

        public override int GameSubType => (int)Common.Core.Poker.GameSubType.Huolong;

        GameSubType Common.Interface.Poker.IPlayerItem<IPlayerVector_Item>.GameSubType => Common.Core.Poker.GameSubType.Huolong;

        public void SetVector(IPlayerVector_Item vector)
        {
            this.vector = vector;
        }

        public void OnResponse(GameOperationEvent _event, GameOperationResponse response)
        {

        }

        public void OnGameStart()
        {

        }

        public void OnMatchStart()
        {

        }

        public void OnGetOneCard(int card)
        {

        }

        public void OnPlayerShow(int player, int[] cards)
        {

        }

        public void OnPlayerShowResult(int player, int[] jokers, int target)
        {

        }

        public void OnGetAllCards(int[] cards)
        {

        }

        public void OnLastCardsOver(LastCardsReport report)
        {

        }

        public void OnAskForLastCards(int[] mainPlayerLastCards)
        {

        }

        public void OnAskForThrow(int[] leaderCards)
        {

        }

        public void OnPlayerThrew(int player, int[] threw)
        {

        }

        public void OnMatchAborted()
        {

        }

        public void OnGameAborted()
        {

        }

        public void OnRoundOver(RoundReport report)
        {

        }

        public void OnMatchOver(MatchReport report)
        {

        }

        public void OnGameOver(GameReport report)
        {

        }

        public void OnPlayerInfoChanged(int report, Common.Core.CharacterInfo oldInfo, Common.Core.CharacterInfo newInfo)
        {

        }

        #endregion Interfaces

        #region View Callbacks



        #endregion View Callbacks

        #region Unity Events



        #endregion Unity Events

        private IPlayerVector_Item vector = null;
    }
}