using Polly;

Console.WriteLine("RateLimit Policies");

var policy = Policy.RateLimit(20, TimeSpan.FromSeconds(5));

for (var f = 1; f < 50; f++)
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
}