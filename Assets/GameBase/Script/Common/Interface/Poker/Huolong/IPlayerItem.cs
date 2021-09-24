using GameBase.Common.Core;

namespace GameBase.Common.Interface.Poker.Huolong
{
    public interface IPlayerItem: Poker.IPlayerItem
    {
        public void SetVector(IPlayerVector_Item vector);
        public void OnGameStart();
        public void OnMatchStart();
        public void OnGetOneCard(int card);
        public void OnGetAllCards(int[] cards);
        public void OnPlayerShow(int player, int[] cards);
        public void OnPlayerShowResult(int player, int[] jokers, int target);
        public void OnMatchAborted();
        public void OnAskForLastCards(int[] mainPlayerLastCards);
        public void OnLastCardsOver(Core.Poker.Huolong.LastCardsReport report);
        public void OnPlayerThrew(int player, int[] threw);
        public void OnAskForThrow(int[] leaderCards);
        public void OnRoundOver(Core.Poker.Huolong.RoundReport report);
        public void OnMatchOver(Core.Poker.Huolong.MatchReport report);
        public void OnGameOver(Core.Poker.Huolong.GameReport report);
        public void OnGameAborted();
        public void OnPlayerInfoChanged(int report, CharacterInfo oldInfo, CharacterInfo newInfo);
        public void OnResponse(Core.Poker.Huolong.GameOperationEvent _event, Core.Poker.Huolong.GameOperationResponse response);
    }
}