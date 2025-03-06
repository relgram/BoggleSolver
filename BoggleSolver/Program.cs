using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BoggleSolver;

internal class Program
{
    //static void Main(string[] args)
    //{
    //    BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    //}

    static void Main(string[] args)
    {
        var words = File.ReadAllLines("dictionary.txt");
        var boggle = new BoggleSolver(words);

        var stopwatch1 = Stopwatch.StartNew();

        for (int i = 0; i < 1_000; ++i)
        {
            var results1 = boggle.SolveBoard(3, 3, "yoxrbaved");
        }

        var elapsed1 = stopwatch1.Elapsed;

        Console.WriteLine($"Duration: {elapsed1 / 1_000}");
    }
}

[SimpleJob(RuntimeMoniker.Net80, baseline: true)]
[RPlotExporter]
public class BoggleExperiment
{
    private BoggleSolver _boggleSolver;

    [Params(1000, 10000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        var words = File.ReadAllLines("dictionary.txt");
        _boggleSolver = new BoggleSolver(words);
    }

    [Benchmark]
    public void BoggleSolver()
    {
        _boggleSolver.SolveBoard(5, 5, "oectwammrnneaeersrtblprto");
    }
}