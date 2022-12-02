using Polly;
using Polly.CircuitBreaker;

var rnd = new Random();

Console.WriteLine("Testing QOS Policies");
Console.WriteLine("Ctrl+c to terminate");

var policyHigh = Policy
              .Handle<Exception>()
              .LoggingCircuitBreaker(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(5),
                name: "High Priority Breaker"
              );

var policyMedium = Policy
              .Handle<Exception>()
              .LoggingCircuitBreaker(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(10),
                name: "Medium Priority Breaker"
              ).Wrap(policyHigh);

var policyLow = Policy
              .Handle<Exception>()
              .LoggingCircuitBreaker(
                exceptionsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromSeconds(20),
                name: "Low Priority Breaker"
              )
              .Wrap(policyMedium);

while (true)
{
    policyLow.TryCatchExecute(() => CallAPI("Low "));
    policyMedium.TryCatchExecute(() => CallAPI("Med "));
    policyHigh.TryCatchExecute(() => CallAPI("High "));
}


void CallAPI(string message)
{
    //Fake API Call
    Console.Write(message);
    if (rnd.Next(10) > 6 ) {
        throw new Exception("Rate Limiting Applied");
    }
    Thread.Sleep(100);  
}

public static class PollyExtention
{
    public static void TryCatchExecute(this Policy p, Action a)
    {
        try
        {
            p.Execute(() => a());
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

    public static CircuitBreakerPolicy LoggingCircuitBreaker(this PolicyBuilder builder, int exceptionsAllowedBeforeBreaking, TimeSpan durationOfBreak, string name)
    {
        return builder.CircuitBreaker(
                exceptionsAllowedBeforeBreaking: exceptionsAllowedBeforeBreaking,
                durationOfBreak: durationOfBreak,
                onBreak: (ex, breakDelay) =>
                {
                    Console.WriteLine($"\n{name}: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!", ex);
                },
                onReset: () =>
                {
                    Console.WriteLine($"\n{name}: Call ok! Closed the circuit again.");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine($"\n{name}: Half-open; next call is a trial.");
                });
    }
}