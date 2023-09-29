using Polly;
using Polly.Retry;
using Polly.Timeout;

Console.WriteLine("Timeout Policies");

ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
    .AddTimeout(new TimeSpan(0,0,5))  //Timeout after 5s.
    .Build();

//Only co-operative mode now supported.

try
{
    await pipeline.ExecuteAsync(async ct => await DoSomethingCancellable(ct),CancellationToken.None);
    Console.WriteLine("Stopped");
}
catch (TimeoutRejectedException)
{
    Console.WriteLine("Failed to Stop");
}

async Task DoSomethingCancellable(CancellationToken cancellationToken)
{
    while (!cancellationToken.IsCancellationRequested)
    {
        Console.Write(".");
        Thread.Sleep(100);
    }
}
