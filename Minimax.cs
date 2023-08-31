using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimax
{
    // Node represents the current game state
    // * board
    // * what are the all possible children states (after all possible different possible moves) for the node
    // * what move resulted the current state
    // * who's turn is it in the current game state
    // * calculate heuristic estimate for the current state
    // Two algorithm versions are being supported:
    // Move ApplyMinimax(Node node, int depth) - calculate move using the minimax
    // Move ApplyAlphaBeta(Node node, int depth) - calculate move using the minimax and alpha-beta-prunings

    public interface Node
    {
        bool IsTerminal();          // Game is over in that point
        List<Node> GetChildren();   // Get all possible child state nodes for the current state node
        Move GetLastMove();         // Get the last move which resulted the current state
        int Heuristic();            // Get heuristic estimate for given state node

        int Player();               // The player is either Algorithm.MIN or Algorithm.MAX
    }

    // Move is a command resulting children nodes for a state.
    // The actual implementation depends on the game implemented.
    public interface Move
    {
        string ToString();
    }

    public class Algorithm
    {
        // players are mister MAX, and mister MIN
        public const int MAX = 1;
        public const int MIN = 0;

        public static Move? ApplyMinimax(Node node, int depth)
        {
            int value = int.MinValue;       // current max value
            Move? bestMove = null;

            foreach (Node child in node.GetChildren())
            {
                int newValue = Minimax(child, depth - 1);
                if (newValue > value)
                {
                    bestMove = child.GetLastMove();
                    value = newValue;
                }
            }

            return bestMove;
        }

        protected static int Minimax(Node node, int depth)
        {
            int value;                  // result of this function

            if (depth == 0 || node.IsTerminal())
                return node.Heuristic();

            if (node.Player() == MAX)             // MAXIMIZING
            {
                value = int.MinValue;

                foreach (Node child in node.GetChildren())
                {
                    value = Math.Max(value, Minimax(child, depth - 1));
                }
            }

            else                        // MINIMAZING
            {
                value = int.MaxValue;

                foreach (Node child in node.GetChildren())
                {
                    value = Math.Min(value, Minimax(child, depth - 1));
                }
            }

            return value;
        }

        public static Move? ApplyAlphabeta(Node node, int depth)
        {
            int value = int.MinValue;       // current max value
            Move? bestMove = null;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            foreach (Node child in node.GetChildren()) {
                int newValue = Alphabeta(child, depth - 1, alpha, beta);
                if (newValue > value) {
                    bestMove = child.GetLastMove();
                    value = newValue;
                }
            }

            return bestMove;
        }

        protected static int Alphabeta(Node node, int depth, int alpha, int beta)
        {
            int value;                  // result of this function

            if (depth == 0 || node.IsTerminal())
                return node.Heuristic();

            if (node.Player() == MAX)             // MAXIMIZING
            {
                value = int.MinValue;

                foreach (Node child in node.GetChildren())
                {
                    value = Math.Max(value, Alphabeta(child, depth - 1, alpha, beta));
                    alpha = Math.Max(alpha, value);

                    if (beta <= alpha)  // alpha-beta-pruning
                        break;          // the foreach
                }
            }

            else                        // MINIMAZING
            {
                value = int.MaxValue;

                foreach (Node child in node.GetChildren())
                {
                    
                    value = Math.Min(value, Alphabeta(child, depth - 1, alpha, beta));
                    beta = Math.Min(beta, value);

                    if (beta <= alpha)  // alpha-beta-pruning
                        break;          // the foreach
                }
            }

            return value;
        }
    }
}
