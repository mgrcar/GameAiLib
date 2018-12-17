﻿namespace GameAiLib
{
    public class SimpleMinimaxBrain : GenericMinimaxBrain
    {
        protected override double MinimaxEval(IGameOld game, Player player)
        {
            if (game.Winner == null) { return 0; }
            else if (game.Winner == player) { return 1; }
            else { return -1; }
        }
    }
}
