using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;

var rnd = new Random();

Console.WriteLine("Testing QOS Policies");
Console.WriteLine("Ctrl+c to terminate");

var highPriorityPipe = new ResiliencePipelineBuilder()
    .AddLoggingCircuitBreaker(new CircuitBreakerStrategyOptions()
    {
        BreakDuration = new TimeSpan(0, 0, 5),
        SamplingDuration = new TimeSpan(0, 0, 1),
        FailureRatio = 0.9,
        MinimumThroughput = 2,
        Name = "High"
    })
    .Build();

var mediumPriorityPipeline = new ResiliencePipelineBuilder()
    .AddPipeline(highPriorityPipe)
    .AddLoggingCircuitBreaker(new CircuitBreakerStrategyOptions()
        {
            BreakDuration = new TimeSpan(0, 0, 10),
            SamplingDuration = new TimeSpan(0, 0, 1),
            FailureRatio = 0.7,
            MinimumThroughput = 2,
            Name = "Medium"
        })
    .Build();

var lowPriorityPipeline = new ResiliencePipelineBuilder()
    .AddPipeline(highPriorityPipe)
    .AddPipeline(mediumPriorityPipeline)
    .AddLoggingCircuitBreaker(new CircuitBreakerStrategyOptions()
    {
        BreakDuration = new TimeSpan(0, 0, 20),
        SamplingDuration = new TimeSpan(0, 0, 1),
        FailureRatio = 0.2,
        MinimumThroughput = 2,
        Name = "Low"
    })
    .Build();


while (true)
{
    lowPriorityPipeline.TryCatchExecute(()=> CallAPI("Low "));
    mediumPriorityPipeline.TryCatchExecute(() => CallAPI("Med "));
    highPriorityPipe.TryCatchExecute(() => CallAPI("High "));
}


void CallAPI(string message)
{
    //Fake API Call
    Console.Write(message);
    if (rnd.Next(10) > 8 ) {
        throw new Exception("Rate Limiting Applied");
    }
    Thread.Sleep(100);  
}

public static class PollyExtention
{
    public static void TryCatchExecute(this ResiliencePipeline p, Action a)
    {
        try
        {
            p.Execute(a);
        }
        catch (BrokenCircuitException)
        {
            Console.Write(". "); // Operation was skipped
        }
        catch (Exception)
        {
            // Operation happened and errored
        }
    }

    public static ResiliencePipelineBuilder AddLoggingCircuitBreaker(this ResiliencePipelineBuilder builder, CircuitBreakerStrategyOptions options)
    {
        options.OnOpened += args =>
        {
            Console.WriteLine(
                $"\n{options.Name}: Breaking the circuit for {args.BreakDuration.TotalMilliseconds} ms! {args.Outcome.Exception?.Message}");
            return ValueTask.CompletedTask;
        };
        options.OnClosed += args =>
        {
            Console.WriteLine($"\n{options.Name}: Closing the circuit");
            return ValueTask.CompletedTask;
        };
        return builder.AddCircuitBreaker(options);
    }
}