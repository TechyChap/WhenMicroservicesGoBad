using Polly;
using Polly.RateLimiting;

Console.WriteLine("Concurrency Limiter (was called bulkhead)");

var pipeline = new ResiliencePipelineBuilder()
    .AddConcurrencyLimiter(5,2)
    .Build();

Parallel.For(1, 50, (runNo) =>
    {
        try
        {
            //DoSomething(runNo);
            pipeline.Execute(() => DoSomething(runNo));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Doing thing {runNo} errored - {ex.Message}");
        }
    } );


Console.ReadKey();

void DoSomething(int runNo)
{
    Console.WriteLine($"Doing Slow Thing {runNo}");
    Thread.Sleep(2000);
}