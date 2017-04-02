using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Othello
{
    class Program
    {
        Board board = new Board();

        public void Go()
        {
            Command userCommand;
            Command machineCommand;

            Console.WriteLine("Pelataanpa erä Othello-peliä (komento 'lopeta' lopettaa)");

            do
            {
                int numberOfCanceled = 0;

                // valkoisen siirto
                board.Display();
                if (board.Actions().Count > 0) // voi siirtää
                {
                    Console.Write("Anna siirto: ");
                    userCommand = new Command(Console.ReadLine());
                    if (userCommand.GetCommand() == Command.EXIT) break;
                    if (!board.IsValid(userCommand)) continue;
                    board.Move(userCommand);
                    board.Display();
                }
                else if (numberOfCanceled > 0) 
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Valkoinen ei voi siirtää.");
                    numberOfCanceled++;
                    board.SetTurn(Board.B);
                }

                // mustan (koneen, eli maxin) siirto
                if (board.Actions().Count > 0) // voi siirtää
                {
                    // lasketaan koneen siirto Minimax-algoritmilla
                    machineCommand = (Command)Minimax.Algorithm.ApplyAlphabeta(board, 8);

                    Console.WriteLine("Kone tekee siirron " + machineCommand.ToString());
                    board.Move(machineCommand);
                }
                else if (numberOfCanceled > 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Musta ei voi siirtää.");
                    numberOfCanceled++;
                    board.SetTurn(Board.W);
                }
                
            } while (true);

            Console.WriteLine("Tulos: valkoiset = " + board.Count(Board.W) +
                ", mustat = " + board.Count(Board.B));
        }

        static void Main(string[] args)
        {
            new Program().Go();
        }
    }
}
