﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace GameAiLib
{
    public class Connect4New : IGameNew
    {
        public struct UndoToken
        {
            public ulong position;
            public ulong mask;
        }

        public class NegamaxBrain : GenericNegamaxBrain
        {
            private int[] order
                = new[] { 5, 3, 1, 0, 2, 4, 6 };

            public NegamaxBrain(int maxDepth = int.MaxValue) : base(maxDepth)
            {
            }

            protected override double NegamaxEval(IGameNew _game)
            {
                var game = (Connect4New)_game;
                if (game.Winner == null)
                {
                    int scoreP1 = PopCount(ComputeWinningPositions(game.position, game.mask));
                    int scoreP2 = PopCount(ComputeWinningPositions(game.position ^ game.mask, game.mask));
                    return game.currentPlayer == Player.Player2 ? scoreP1 - scoreP2 : scoreP2 - scoreP1;
                }
                if (game.Winner == Player.Player1)
                {
                    return 6 * 7;
                }
                else
                {
                    return -6 * 7;
                }
            }

            protected override IEnumerable<int> OrderedMoves(IGameNew game)
            {
                return game.AvailableMoves().OrderBy(m => order[m]);
            }

            private ulong ComputeWinningPositions(ulong position, ulong mask)
            {
                const int HEIGHT = 6;

                // vertical
                ulong r = (position << 1) & (position << 2) & (position << 3);

                // horizontal
                ulong p = (position << (HEIGHT + 1)) & (position << 2 * (HEIGHT + 1));
                r |= p & (position << 3 * (HEIGHT + 1));
                r |= p & (position >> (HEIGHT + 1));
                p = (position >> (HEIGHT + 1)) & (position >> 2 * (HEIGHT + 1));
                r |= p & (position << (HEIGHT + 1));
                r |= p & (position >> 3 * (HEIGHT + 1));

                // diagonal 1
                p = (position << HEIGHT) & (position << 2 * HEIGHT);
                r |= p & (position << 3 * HEIGHT);
                r |= p & (position >> HEIGHT);
                p = (position >> HEIGHT) & (position >> 2 * HEIGHT);
                r |= p & (position << HEIGHT);
                r |= p & (position >> 3 * HEIGHT);

                // diagonal 2
                p = (position << (HEIGHT + 2)) & (position << 2 * (HEIGHT + 2));
                r |= p & (position << 3 * (HEIGHT + 2));
                r |= p & (position >> (HEIGHT + 2));
                p = (position >> (HEIGHT + 2)) & (position >> 2 * (HEIGHT + 2));
                r |= p & (position << (HEIGHT + 2));
                r |= p & (position >> 3 * (HEIGHT + 2));

                return r & (boardMask ^ mask);
            }

            private int PopCount(ulong m)
            {
                int c = 0;
                for (c = 0; m != 0; c++) { m &= m - 1; }
                return c;
            }
        }

        private const ulong bottomMask
            = 0b0000001_0000001_0000001_0000001_0000001_0000001_0000001ul;
        private const ulong boardMask
            = bottomMask * ((1ul << 6) - 1);

        public ulong position;
        public ulong mask;
        private int moves;
        private Player? winner;
        private Player currentPlayer;

        public Connect4New(Player startingPlayer = Player.Player1)
        {
            currentPlayer = startingPlayer;
        }

        public Player? Winner
        {
            get { return winner; }
        }

        public Player CurrentPlayer
        {
            get { return currentPlayer; }
        }

        private bool IsFull
        {
            get { return moves == 6 * 7; }
        }

        public bool IsTerminalState
        {
            get { return winner != null || IsFull; }
        }

        public IEnumerable<int> AvailableMoves()
        {
            var list = new List<int>(7);
            for (int col = 0; col < 7; col++)
            {
                if (((1ul << 5 << (col * 7)) & mask) == 0)
                {
                    list.Add(col);
                }
            }
            return list;
        }

        public object MakeMove(int move)
        {
            var undoToken = new UndoToken {
                position = position,
                mask = mask
            };
            mask |= mask + (1ul << (move * 7));
            position ^= mask;
            moves++;
            // check if this resulted in a win
            // horizontal
            ulong m = position & (position >> 7);
            if ((m & (m >> 14)) != 0) { winner = currentPlayer; }
            // diag 1
            m = position & (position >> 6);
            if ((m & (m >> 12)) != 0) { winner = currentPlayer; }
            // diag 2
            m = position & (position >> 8);
            if ((m & (m >> 16)) != 0) { winner = currentPlayer; }
            // vertical
            m = position & (position >> 1);
            if ((m & (m >> 2)) != 0) { winner = currentPlayer; }
            currentPlayer = currentPlayer.OtherPlayer();
            return undoToken;
        }

        public void UndoMove(object _undoToken)
        {
            var undoToken = (UndoToken)_undoToken;
            position = undoToken.position;
            mask = undoToken.mask;
            winner = null;
            moves--;
            currentPlayer = currentPlayer.OtherPlayer();
        }

        public override string ToString()
        {
            var posPlayer1 = Convert.ToString((long)position, 2).PadLeft(49, '0');
            var posPlayer2 = Convert.ToString((long)(position ^ mask), 2).PadLeft(49, '0');
            var board = new char[49, 49];
            for (int i = 0, j = 48; i < 49; i++, j--)
            {
                int row = i % 7;
                int col = i / 7;
                board[row, col] = posPlayer1[j] == '1' ? 'o' : (posPlayer2[j] == '1' ? 'x' : '·');
            }
            var boardStr = "";
            for (int row = 5; row >= 0; row--)
            {
                for (int col = 0; col < 7; col++)
                {
                    boardStr += board[row, col] + " ";
                }
                boardStr += Environment.NewLine;
            }
            return boardStr + "0 1 2 3 4 5 6";
        }
    }
}