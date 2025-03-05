using System.Diagnostics;

namespace BoggleSolver;

internal class Program
{
    static void Main(string[] args)
    {
        var words = File.ReadAllLines("dictionary.txt");
        var boggle = new BoggleSolver(words);

        var stopwatch3 = Stopwatch.StartNew();
        var results3 = boggle.SolveBoard(3, 3, "oectammrn");
        var elapsed3 = stopwatch3.Elapsed;

        Console.WriteLine($"3x3 Solve Duration: {elapsed3}");
        Console.WriteLine($"3x3 Found Words: {results3.Length}");

        var stopwatch4 = Stopwatch.StartNew();
        var results4 = boggle.SolveBoard(4, 4, "oectammrnneersrt");
        var elapsed4 = stopwatch4.Elapsed;

        Console.WriteLine($"4x4 Solve Duration: {elapsed4}");
        Console.WriteLine($"4x4 Found Words: {results4.Length}");

        var stopwatch5 = Stopwatch.StartNew();
        var results5 = boggle.SolveBoard(5, 5, "oectwammrnneaeersrtblprto");
        var elapsed5 = stopwatch5.Elapsed;

        Console.WriteLine($"5x5 Solve Duration: {elapsed5}");
        Console.WriteLine($"5x5 Found Words: {results5.Length}");
    }
}
