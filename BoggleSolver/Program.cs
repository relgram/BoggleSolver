using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace BoggleSolver;

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
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
    public void BoggleSolver1()
    {
        _boggleSolver.SolveBoard(5, 5, "oectwammrnneaeersrtblprto");
    }
}