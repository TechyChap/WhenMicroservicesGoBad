using Polly;

Console.WriteLine("RateLimit Policies");

var policy = Policy.Bulkhead(5);

    Parallel.For(1, 50, (runNo) =>
    {
        try
        {
            //DoSomething(runNo);
            policy.Execute(() => DoSomething(runNo));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Doing thing {runNo} errored - {ex.Message}");
        }
    } );


Console.ReadKey();

void DoSomething(int runNo)
{
    Console.WriteLine($"Doing Slow Thing {runNo}");
    Thread.Sleep(2000);
}