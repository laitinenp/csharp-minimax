using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimax;


namespace Othello
{
    class Board : Node
    {
        public const int _ = 2;
        public const int B = Algorithm.MAX;
        public const int W = Algorithm.MIN;
        protected const int BSymbol = '*';
        protected const int WSymbol = 'O';

        Move lastMove = null;

        // the state S of the game is defined by board, turn and depth
        protected int[,] board;
        protected int turn;

        // Node implementation methods
        //****************************

        // is this terminal node in the game, no further moves available
        public bool IsTerminal() { return Actions().Count == 0; }

        public Move GetLastMove() { return lastMove;  }

        public List<Node> GetChildren()
        {
            List<Node> result = new List<Node>();

            foreach (Command move in Actions())
            {
                result.Add(Result(move));
            }

            return result;
        }

        public int Player()
        {
            return turn;
        }

        // minimax heuristics function
        // calculated by number of blacks - number of whites
        // weighted with weights array
        public int Heuristic()
        {
            int result = 0;
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    if (board[r, c] == Board.B)
                        result += weights[r, c];
                    else if (board[r, c] == Board.W)
                        result -= weights[r, c];
                }

            return result;
        }

        static private int[,] weights = new int[8, 8] 
            {   // corners and borders are the most invaluable
                {8,0,5,4,4,5,0,8},      // this is the line "1"
                {0,0,0,0,0,0,0,0},      // this is the line "2"
                {5,0,1,1,1,1,0,5},      // etc...
                {4,0,1,1,1,1,0,4},
                {4,0,1,1,1,1,0,4},
                {5,0,1,1,1,1,0,5},
                {0,0,0,0,0,0,0,0},
                {8,0,5,4,4,5,0,8}
            };




        internal Board()
        {
            // note: when accessing the board, the row is always the first: board[2,1] = board["B3"]
            board = new int[8, 8] 
            {   
                {_,_,_,_,_,_,_,_},      // this is the line "1"
                {_,_,_,_,_,_,_,_},      // this is the line "2"
                {_,_,_,_,_,_,_,_},      // etc...
                {_,_,_,B,W,_,_,_},
                {_,_,_,W,B,_,_,_},
                {_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_}
            };

            // assumption: white (user) starts
            turn = W;
        }

        internal Board(Board o)
        {
            board = (int[,])o.GetBoard().Clone();
            turn = o.GetTurn();
        }

        internal int GetTurn() { return turn; }
        internal void SetTurn(int newturn) { turn = newturn; }
        internal static int opposite(int color) { if (color == B) return W; else return B; }
        internal int GetValue(int column, int row) {return board[row, column];}
        internal int[,] GetBoard() { return board; }
        internal int Count(int color)
        {
            int count = 0;
            for (int c = 0; c <= 7; c++)
                for (int r = 0; r <= 7; r++)
                    if (board[c, r] == color)
                        count++;
            return count;
        }


        // Display the board
        internal void Display()
        {
            if (turn == B) Console.WriteLine("Vuoro: MUSTAT(*)");
            else Console.WriteLine("Vuoro: VALKOISET(O)");

            Console.WriteLine("  A B C D E F G H");
            for (int r = 7; r >= 0; r--)
            {
                Console.Write("" + (r + 1));
                for (int c = 0; c < 8; c++)
                {
                    Console.Write(" ");
                    Display(c, r);
                }
                Console.WriteLine("");
            }
        }

        // display the item in the board
        internal void Display(int c, int r)
        {
            if (board[r, c] == _) Console.Write('.');
            else if (board[r, c] == B) Console.Write((char)BSymbol);
            else if (board[r, c] == W) Console.Write((char)WSymbol);
        }

        // check if the move is applicable
        internal bool IsValid(Command m)
        {
            if (board[m.row, m.column] != _) return false;
            bool possible = false;

            // there has to be different color next to at least one of N|NE|E|SE|S|SW|W|NW direction
            // let's create an array of deltax, deltay encoding the differences to each direction
            // and check wether the move is possible
            int[,] dirs = new int[8, 2] {
                {0,1},{1,1},{1,0},{1,-1},{0,-1},{-1,-1},{-1,0},{-1,1}
            };
            for (int i = 0; i < 8; i++) // check all the 8 directios, the result is disjunction of the all
            {
                possible |= checkDirection(m.column, m.row, dirs[i, 0], dirs[i, 1]);
            }

            return possible;
        }

        // Generate result for given state (actually board) after command
        internal void Move(Command m)
        {
            if (m.GetCommand() == Command.MOVE)
            {
                if (board[m.row, m.column] != _) return;
                bool possible = false;

                //if (m.column == 3 && m.row == 0) Console.WriteLine("HEP!!!!!!!");

                // there has to be different color next to at least one of N|NE|E|SE|S|SW|W|NW direction
                // let's create an array of deltax, deltay encoding theAct differences to each direction
                // and check wether the move is possible
                int[,] dirs = new int[8, 2] {
                    {0,1},{1,1},{1,0},{1,-1},{0,-1},{-1,-1},{-1,0},{-1,1}
                };
                for (int i = 0; i < 8; i++) // check all the 8 directios, the result is disjunction of the all
                {
                    possible |= checkDirectionW(m.column, m.row, dirs[i, 0], dirs[i, 1]);
                }

                if (possible)
                {
                    board[m.row, m.column] = turn;
                    turn = Board.opposite(turn);
                }
            }
            return;
        }

        // Generate result for given state (actually board) after command
        // return NEW COPY of the board
        public Board Result(Command com)
        {
            Board b = new Board(this);
            b.Move(com);
            b.lastMove = com;
            return b;
        }

        // Get all possible actions which can be taken from the given state (board+turn)
        // if now moves can not be taken, the Cancel comamnd is being returned
        public List<Move> Actions()
        {
            List<Move> result = new List<Move>();
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    Command m = new Command(c, r);
                    if (IsValid(m))
                        result.Add(m);
                }
            return result;
        }

        // checkDirectionW() checks if the move is possible in terms of direction dx,dy=N|NE|E|SE|S|SW|W|NW
        // if it is, then the move will be written into the board
        // check is done in three phases
        // check = 0: nothing has been checked
        // check = 1: is there a button with different colour inbetween
        // check = 2: is there a button with same color at the end
        protected bool checkDirectionW(int c, int r, int dx, int dy)
        {
            int c0; int r0;
            c0 = c + dx;
            r0 = r + dy;
            int counter = 0;

            // check 1: is there a button with different colour inbetween
            for (; c0 >= 0 && c0 <= 7 && r0 >= 0 && r0 <= 7 && board[r0, c0] == opposite(turn); c0 += dx, r0 += dy, counter++) ;
            if (c0 < 0 || c0 > 7 || r0 < 0 || r0 > 7 || counter == 0) return false;

            // check 2: is there a button with same color at the end
            if (board[r0, c0] != turn) return false;

            // move is possible
            // => change the buttons color to turn
            for (c0 = c + dx, r0 = r + dy; board[r0, c0] == opposite(turn); c0 += dx, r0 += dy)
            {
                board[r0, c0] = turn;
            }
            return true;
        }
        // this version of the same operation does not make the move, just checks if it is possible
        protected bool checkDirection(int c, int r, int dx, int dy)
        {
            int c0; int r0;
            c0 = c + dx;
            r0 = r + dy;
            int counter = 0;

            // check 1: is there a button with different colour inbetween
            for (; c0 >= 0 && c0 <= 7 && r0 >= 0 && r0 <= 7 && board[r0, c0] == opposite(turn); c0 += dx, r0 += dy, counter++) ;
            if (c0 < 0 || c0 > 7 || r0 < 0 || r0 > 7 || counter == 0) return false;

            // check 2: is there a button with same color at the end
            if (board[r0, c0] != turn) return false;

            // move is possible
            // => just do nothing in the board
            return true;
        }
    }

    class Command : Move
    {
        public const int EXIT = 0;
        public const int MOVE = 1;
        public const int CANCEL = 2;
        public const int INVALID = 3;
        internal int command;
        internal int column;
        internal int row;

        public Command(int c)
        {
            command = c;
        }

        public int GetCommand() { return command; }

        public Command(string s)
        {
            if (s == "lopeta") command = EXIT;
            else if (s.Length == 2)
            {
                int c = (int)(s[0] - 'a');
                int r = (int)(s[1] - '1');
                if (c < 0 || c > 7) { command = INVALID; return; }
                if (r < 0 || r > 7) { command = INVALID; return; }
                column = c;
                row = r;
                command = MOVE;
            }
            else
            {
                command = INVALID;
                return;
            }
        }

        public string ToString()
        {
            string result = "";
            if (command == MOVE) {
                result = ""
                            + Convert.ToChar(column + Convert.ToInt16('a'))
                            + Convert.ToChar(row + Convert.ToInt16('1'));
            }
            else if (command == EXIT)
            {
                result = "exit";
            }

            return result;
        }

        public Command(int c, int r)
        {
            column = c;
            row = r;
            command = MOVE;
        }

        public int GetColumn()
        {
            if (command != MOVE) throw new Exception("Invalid accessor call.");
            else return column;
        }

        public int GetRow()
        {
            if (command != MOVE) throw new Exception("Invalid accessor call.");
            else return row;
        }
    }


}
