using Polly;
using Polly.Fallback;
using Polly.Retry;

Console.WriteLine("Chained Pipelines");

var retryPipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions()
    {
        Delay = TimeSpan.FromSeconds(5)
    }) 
    .Build();  

var fallbackPipeline = new ResiliencePipelineBuilder<string>()
    .AddFallback(new FallbackStrategyOptions<string>
    {
        FallbackAction = args =>
        {
            Console.WriteLine("Doing Fallback");
            return Outcome.FromResultAsValueTask("SomeDefaultValue");
        }
    })
    .Build();

var pipeline = new ResiliencePipelineBuilder<string>()
    .AddPipeline(fallbackPipeline)
    .AddPipeline(retryPipeline)     // Ordering is important
    .Build();

    try
    {
        var result = pipeline.Execute(() => DoSomething());
        Console.WriteLine(result);
    }
    catch
    {
        Console.WriteLine("Doing thing errored");
    }

string DoSomething()
{
    Console.WriteLine("Doing Thing");
    throw new Exception();
}