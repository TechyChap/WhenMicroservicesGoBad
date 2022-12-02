using Polly;

Console.WriteLine("Fallback Policies");

var policy = Policy
              .Handle<Exception>()
              .Fallback(() => { Console.WriteLine("Doing Fallback"); });

/* Can be combined with other policies
   https://andrewlock.net/when-you-use-the-polly-circuit-breaker-make-sure-you-share-your-policy-instances-2/
   Can pass parameters to fallback by defining a execute/fallback context
   https://stackoverflow.com/questions/69190092/send-parameters-to-fallback-action-in-polly
*/

    try
    {
        policy.Execute(() => DoSomething() );
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