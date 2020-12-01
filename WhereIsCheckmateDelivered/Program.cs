using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public class Program
{
    private const int BoardSize = 8;
    private static readonly int[,] whiteCheckmates = new int[8, 8];
    private static readonly int[,] blackCheckmates = new int[8, 8];

    private static long LineCount = 0;

    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: WhereisChekmateDelivered.exe [game folder] [output folder]");
            return;
        }

        string folderPath = args[0];
        Stopwatch stopwatch = Stopwatch.StartNew();
        foreach (var filePath in Directory.EnumerateFiles(folderPath, "*.pgn"))
        {
            ReadFile(filePath);
        }

        stopwatch.Stop();

        Console.WriteLine();
        Console.WriteLine("Elapsed " + stopwatch.ElapsedMilliseconds + "ms");

        string outputFolder = args[1];
        WriteResults(outputFolder + "/white_checkmates.txt", whiteCheckmates);
        WriteResults(outputFolder + "/black_checkmates.txt", blackCheckmates);
    }

    private static void ReadFile(string filePath)
    {
        using (StreamReader sr = File.OpenText(filePath))
        {
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                LineCount++;

                if (LineCount % (int)1e6 == 0)
                    Console.WriteLine($"Processed {LineCount / (int)1e6}m lines");

                ParseLine(line);
            }
        }
    }

    private static void ParseLine(string line)
    {
        if (line.Length == 0 || line[0] == '[')
            return;

        bool inComment = false;
        for (int i = line.Length - 1; i >= 0; i--)
        {
            char current = line[i];
            if (inComment)
            {
                if (current == '{')
                {
                    inComment = false;
                    continue;
                }
            }
            else
            {
                if (current == '}')
                {
                    inComment = true;
                    continue;
                }
                else if (current == '#')
                {
                    bool whiteMoved = line.EndsWith("1-0");
                    int spaceIdx = line.LastIndexOf(' ', i);
                    string move = line.Substring(spaceIdx + 1, i - spaceIdx - 1);
                    int len = move.Length;

                    int x = 0, y = 0;

                    // Its a normal move
                    if (move[0] != 'O')
                    {
                        // Its a promotion so skip over the "=X"
                        if (move[len - 2] == '=')
                        {
                            x = move[len - 4] - 'a';
                            y = move[len - 3] - '1';
                        }
                        else
                        {
                            x = move[len - 2] - 'a';
                            y = move[len - 1] - '1';
                        }
                    }
                    // Theses moves are really rare to be checkmate, but they need to be handled
                    // Kingside castle
                    else if (move == "O-O")
                    {
                        x = 5;
                        y = whiteMoved ? 0 : 7;
                    }
                    //Queenside castle
                    else if (move == "O-O-O")
                    {
                        x = 3;
                        y = whiteMoved ? 0 : 7;
                    }

                    if (whiteMoved)
                        whiteCheckmates[x, y]++;
                    else
                        blackCheckmates[x, y]++;
                }
            }
        }
    }

    private static void WriteResults(string path, int[,] data)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                sb.AppendLine(data[i, j].ToString());
            }
        }

        File.WriteAllText(path, sb.ToString());
    }
}
