﻿using System;
using System.Collections.Generic;

namespace GameAiLib
{
    public abstract class GenericBrain : IBrain
    {
        private static Random rng 
            = new Random();

        public void MakeMove(IGame game, Player player)
        {
            double bestScore = double.MinValue;
            var bestMoves = new List<int>();
            foreach (int move in game.AvailableMoves(player))
            {
                var undoToken = game.MakeMove(move, player);
                double score = EvalGame(game, player);
                Console.WriteLine($"{move}: {score}");
                if (score > bestScore) { bestScore = score; bestMoves.Clear(); }
                if (score == bestScore) { bestMoves.Add(move); }
                game.UndoMove(move, player, undoToken);
            }
            game.MakeMove(bestMoves[rng.Next(bestMoves.Count)], player);
        }

        protected abstract double EvalGame(IGame game, Player player);
    }
}
