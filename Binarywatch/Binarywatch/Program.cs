using System;
using System.Media;

namespace UR
{
    class Program
    {
        static string[,] segmentmilli = // tal=0-6 y= 0-2, x 0-2
{
         {"▀▀▀","  █","   ","   ","   ","█  ","   "," ▀ "},
         {"   ","  ▀","  █","   ","█  ","▀  ","▀▀▀"," ▀ "},
         {"   ","   ","  ▀","▀▀▀","▀  ","   ","   "," ▀ "}
        };

        static string[,] segment = // tal=0-7,  y=0-4 * x= 0-5
        {
                {"██████","    ██","      ","      ","      ","██    ","      ","  ██  "},
                {"      ","    ██","      ","      ","      ","██    ","      ","      "},
                {"      ","    ██","    ██","      ","██    ","██    ","██████","  ██  "},
                {"      ","      ","    ██","      ","██    ","      ","      ","      "},
                {"      ","      ","    ██","██████","██    ","      ","      ","  ██  "}
        };
        // rækkefølge er ikke ligegyldig med de små tal, de skal tegnes i bestemt rækkefølge
        static string[] syvsegmentnr = { "012543", "12", "03164", "06123", "6125", "06253", "062354", "012", "0612543", "06125", "7" };

        //  A         0
        //F   B     5   1
        //  G         6
        //E   C     4   2
        //  D         3

        static DateTime tid = System.DateTime.Now;
        static DateTime tid2 = System.DateTime.Now;

        static bool tick = true;    // tick false = tock
        static ConsoleKeyInfo input;
        static int urvalg = 0;
        static bool exit = false;

        static string[] watch = {
            "Pil op/ned: vælg ur       ESC: afslut",
            "          4-Bit ur                   ",
            "          8-Bit ur                   ",
            "          syvsegment ur              "}; 

        static string byte2bin(int byteTal, int bit)
        {
            string resultat = "";

            for (int n = bit; n >= 0; n--)
            {
                if (byteTal >= Math.Pow(2, n))
                {
                    resultat = resultat + "1";
                    byteTal = byteTal - Convert.ToInt32(Math.Pow(2, n));
                }
                else
                    resultat = resultat + "0";
            }
            return resultat;
        }

        static void Watchmenu()
        {
            for (int i = 0; i <= 3;i++)
            {
                Console.BackgroundColor = i-1 == urvalg ? ConsoleColor.Black : ConsoleColor.DarkBlue;
                Console.ForegroundColor = i-1 == urvalg ? ConsoleColor.DarkBlue : ConsoleColor.Black;
                Console.SetCursorPosition(20,23+i);
                Console.Write(watch[i]);
            }
            Console.BackgroundColor = ConsoleColor.Black; // nulstil
        }

        static void tidsupdate()
        {
            do
            {
                Watchmenu();

                tid2 = System.DateTime.Now;

                if (tid2.Second - tid.Second == 1 || tid2.Second - tid.Second == -59) // opdater hvert sekund   
                {
                    if (urvalg == 0) // binært ur
                    {
                        tegnbitstreng(55, 5, tid2.Second % 10,4);
                        tegnbitstreng(50, 5, tid2.Second / 10, 4);

                        tegnbitstreng(40, 5, tid2.Minute % 10, 4);
                        tegnbitstreng(35, 5, tid2.Minute / 10, 4);

                        tegnbitstreng(25, 5, tid2.Hour % 10, 4);
                        tegnbitstreng(20, 5, tid2.Hour / 10, 4);
                    }
                    if (urvalg == 1) // binært ur
                    {
                        tegnbitstreng(43, 5, tid2.Second, 8);
                        tegnbitstreng(38, 5, tid2.Minute, 8);
                        tegnbitstreng(33, 5, tid2.Hour, 8);
                    }
                    if (urvalg == 2)
                    {                    // slet tidligere tal
                        tegntal(70, 5, tid.Second % 10, segment, ConsoleColor.Black);
                        tegntal(65, 5, tid.Second / 10, segment, ConsoleColor.Black);

                        if (tid.Minute != tid2.Minute)
                        {
                            tegntal(55, 5, tid.Minute % 10, segment, ConsoleColor.Black);
                            tegntal(45, 5, tid.Minute / 10, segment, ConsoleColor.Black);
                            if (tid.Hour != tid2.Hour)
                            {
                                tegntal(30, 5, tid.Hour % 10, segment, ConsoleColor.Black);
                                tegntal(20, 5, tid.Hour / 10, segment, ConsoleColor.Black);
                            }
                        }
                        // tegn seperatorer
                        if (tick == true)
                            tegntal(37, 5, 10, segment, ConsoleColor.DarkBlue);
                        else
                            tegntal(37, 5, 10, segment, ConsoleColor.Black);

                        // tegn blå tal

                        tegntal(70, 5, tid2.Second % 10, segmentmilli);
                        tegntal(65, 5, tid2.Second / 10, segmentmilli);
                        tegntal(55, 5, tid2.Minute % 10, segment);
                        tegntal(45, 5, tid2.Minute / 10, segment);
                        tegntal(30, 5, tid2.Hour % 10, segment);
                        tegntal(20, 5, tid2.Hour / 10, segment);
                    }

                    try // så programmet ikke afslutter, hvis lydfilerne ikke kan findes
                    {
                        if (tick == true)
                            using (SoundPlayer player = new SoundPlayer(@"tick.wav")) player.PlaySync();
                        else
                            using (SoundPlayer player = new SoundPlayer(@"tock.wav")) player.PlaySync();
                    }
                    catch
                    {
                        Console.SetCursorPosition(20, 20);
                        Console.Write("Wavefiler ikke fundet. Der afspilles ikke lyd");
                    }

                    tick = tick == false ? true : false;

                    tid = System.DateTime.Now;  //nulstil
                }
                if (Console.KeyAvailable)
                {
                    input = Console.ReadKey(true);
                    if (input.Key == ConsoleKey.Escape)
                        exit = true;
                    else if (input.Key == ConsoleKey.UpArrow)
                        urvalg = urvalg > 0 ? urvalg - 1 : 2;
                    else if (input.Key == ConsoleKey.DownArrow)
                        urvalg = urvalg < 2 ? urvalg + 1 : 0;
                    Console.Clear();
                }
            } while (exit == false);
        }

        static void tegntal(int talx, int taly, int tal, string[,] grafikarray, ConsoleColor farve = ConsoleColor.DarkBlue)
        {
            Console.ForegroundColor = farve;

            for (int i = 0; i < (syvsegmentnr[tal].Length); i++)
                for (int y = 0; y < grafikarray.GetLength(0); y++)
                    for (int x = 0; x < grafikarray[0, 0].Length; x++)   // længden på en vilkårlig string i arrayet
                    {
                        if (grafikarray[y, Convert.ToInt32(syvsegmentnr[tal].Substring(i, 1))][x] != ' ')
                        {
                            Console.SetCursorPosition(talx + x, taly + y);
                            Console.Write(grafikarray[y, Convert.ToInt32(syvsegmentnr[tal].Substring(i, 1))][x]);
                        }
                    }
        }
        static void tegnbitstreng(int x, int y, int tal, int bit)
        {
            string binærtal = byte2bin(tal,bit);

            for (int i = 0; i < binærtal.Length; i++)
            {
                Console.ForegroundColor = binærtal[i] == '1'?(ConsoleColor)(tal % 4 + 4): ConsoleColor.DarkBlue;
                Console.SetCursorPosition(x, y + 2 * i);
                Console.Write("██");
            }
        }
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            tidsupdate();
        }
    }
}