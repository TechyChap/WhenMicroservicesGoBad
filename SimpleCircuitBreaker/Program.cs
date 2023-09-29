using Polly;
using Polly.CircuitBreaker;

Console.WriteLine("Circuit Breaker");

//https://martinfowler.com/bliki/CircuitBreaker.html
//https://github.com/App-vNext/Polly/wiki/Circuit-Breaker

var stateProvider = new CircuitBreakerStateProvider();

ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions()
    {  
        BreakDuration = new TimeSpan(0, 0, 2),
        SamplingDuration = new TimeSpan(0,0,1),
        MinimumThroughput = 2,
        StateProvider = stateProvider
    })
    .Build();

for (var f = 1; f < 10; f++)
{
    try
    {
        Console.WriteLine($"Run #{f}");
        Console.WriteLine(stateProvider.CircuitState.ToString());
        pipeline.Execute(() => DoSomething());
        Thread.Sleep(1000);
    }
    catch
    {
        Console.WriteLine("Doing thing errored");
    }
}

void DoSomething()
{
    Console.WriteLine("Doing Thing");
    throw new Exception();
}