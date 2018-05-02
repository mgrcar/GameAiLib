﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GameAiLib
{
    public class TicTacToeNew : IGameNew
    {
        private byte[][] board
            = new byte[3][];
        private bool winningState
            = false;
        private int moves
            = 0;
        private bool color
            = true;

        public TicTacToeNew()
        {
            for (int row = 0; row < 3; row++)
            {
                board[row] = new byte[3];
            }
        }

        public bool Color
        {
            get { return color; }
        }

        private bool IsFull
        {
            get { return moves == 9; }
        }

        public bool IsTerminalState
        {
            get { return winningState || IsFull; }
        }

        public bool IsWinningState
        {
            get { return winningState; }
        }

        public IEnumerable<int> AvailableMoves
        {
            get
            {
                var moves = new int[9 - this.moves];
                int offset = 0;
                int i = 0;
                foreach (var row in board)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (row[col] == 0) { moves[i++] = offset + col; }
                    }
                    offset += 3;
                }
                return moves;
            }
        }

        public object MakeMove(int move)
        {
            int row = move / 3;
            int col = move % 3;
            board[row][col] = (byte)(color ? 1 : 2);
            moves++;
            // check if this resulted in a win
            if ((board[row][0] == board[row][1] && board[row][1] == board[row][2]) ||
                (board[0][col] == board[1][col] && board[1][col] == board[2][col]) ||
                (row == col && board[0][0] == board[1][1] && board[1][1] == board[2][2]) ||
                (row + col == 2 && board[0][2] == board[1][1] && board[1][1] == board[2][0]))
            {
                winningState = true;
            }
            color = !color;
            return new { move, color = !color };
        }

        public void UndoMove(object _undoToken)
        {
            var undoToken = (dynamic)_undoToken;
            int row = undoToken.move / 3;
            int col = undoToken.move % 3;
            board[row][col] = 0;
            color = undoToken.color;
            winningState = false;
            moves--;
        }

        public override string ToString()
        {
            var str = "";
            int i = 0;
            var moves = new string[] { "012", "345", "678" };
            foreach (var row in board)
            {
                str += row.Select(x => x == 0 ? "·" : (x == 1 ? "o" : "x")).Aggregate((x, y) => x + y) + " " + moves[i++] + Environment.NewLine;
            }
            return str.TrimEnd();
        }

        public string ToCode()
        {
            var str = "";
            foreach (var row in board)
            {
                str += row.Select(x => x == 0 ? "·" : (x == 1 ? "o" : "x")).Aggregate((x, y) => x + y) + " ";
            }
            return str.TrimEnd();
        }
    }
}
