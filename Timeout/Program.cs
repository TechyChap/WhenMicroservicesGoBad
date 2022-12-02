using Polly;
using Polly.Timeout;

Console.WriteLine("Timeout Policies");

var policy = Policy
              .Timeout(5, TimeoutStrategy.Pessimistic);

var cooperativePolicy = Policy
              .TimeoutAsync(5, TimeoutStrategy.Optimistic);

try
{
    policy.Execute(() => DoSomethingForever());
    Console.WriteLine("Stopped");
}
catch (TimeoutRejectedException)
{
    Console.WriteLine("Failed to Stop");
}

try
{
    await cooperativePolicy.ExecuteAsync(async ct => await DoSomethingCancellable(ct),CancellationToken.None);
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


void DoSomethingForever()
{
    //TimeoutStrategy.Pessimistic is needed if we are going to do this.
    while (true)
    {
        Console.Write(".");
        Thread.Sleep(100);
    }
}