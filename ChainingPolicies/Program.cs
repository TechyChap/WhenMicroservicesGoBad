using Polly;

Console.WriteLine("Chained Policies");

var retrypolicy = Policy
              .Handle<Exception>()
              .Retry();

var retryThenFallbackPolicy = Policy
              .Handle<Exception>()
              .Fallback(() => { Console.WriteLine("Doing Fallback"); })
              .Wrap(retrypolicy);

//See https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry for a more sophisticate retry

    try
    {
        retryThenFallbackPolicy.Execute(() => DoSomething());
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