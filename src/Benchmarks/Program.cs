using System.Net;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Benchmarks;

var _ = BenchmarkRunner.Run<OriginalYaDiskClientVsFork>();

[MemoryDiagnoser]
public class OriginalYaDiskClientVsFork
{
   

    [Benchmark]
    public Task Original()
    {
        return OriginalRealization.GetInfoAsync();
    }

    [Benchmark]
    public Task Fork()
    {
        return ForkRealization.GetInfoAsync();
    }
}