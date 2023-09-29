using Polly;
using Polly.Retry;

Console.WriteLine("Retry");

/*
// For advanced control over the retry behavior, including the number of attempts,
 
// delay between retries, and the types of exceptions to handle.
new ResiliencePipelineBuilder().AddRetry(new RetryStrategyOptions
{
    ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
    BackoffType = DelayBackoffType.Exponential,
    UseJitter = true,  // Adds a random factor to the delay
    MaxRetryAttempts = 4,
    Delay = TimeSpan.FromSeconds(3),
});
*/

ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions()) // Add retry using the default options
    .Build();

    try
    {
        pipeline.Execute(() => DoSomething());
    }
    catch
    {
        Console.WriteLine("Doing thing errored");
    }


void DoSomething()
{
    Console.WriteLine("Doing Thing");
    throw new Exception();
}


