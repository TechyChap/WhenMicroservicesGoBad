using Polly;
using System.Threading.RateLimiting;

Console.WriteLine("RateLimit");

//Note the change from previous Polly being that it's now using a window rather than looking for the frequency of updates.
var pipeline = new ResiliencePipelineBuilder()
    .AddRateLimiter(new SlidingWindowRateLimiter(
                    new SlidingWindowRateLimiterOptions() { SegmentsPerWindow = 20, PermitLimit =10, Window = new TimeSpan(0,0,10)}))
    .Build();

for (var f = 1; f < 50; f++)
{
    try
    {
        Console.WriteLine($"Run #{f}");
        pipeline.Execute(() => DoSomething());
        Thread.Sleep(100);
    }
    catch
    {
        Console.WriteLine("Doing thing errored");
    }
}

void DoSomething()
{
    Console.WriteLine("Doing Thing");
}