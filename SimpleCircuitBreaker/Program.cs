using Polly;

Console.WriteLine("Circuit Breaker Policies");

//https://martinfowler.com/bliki/CircuitBreaker.html
//https://github.com/App-vNext/Polly/wiki/Circuit-Breaker

var policy = Policy
              .Handle<Exception>()
              .CircuitBreaker(
                exceptionsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromMinutes(1)
              );

for (var f = 1; f < 10; f++)
{
    try
    {
        Console.WriteLine($"Run #{f}");
        policy.Execute(() => DoSomething());
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