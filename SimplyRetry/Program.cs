using Polly;

Console.WriteLine("Retry Policies");

var policy = Policy
              .Handle<Exception>()
              .Retry();

    try
    {
        policy.Execute(() => DoSomething());
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


