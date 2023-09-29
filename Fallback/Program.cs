using Polly;
using Polly.Fallback;

Console.WriteLine("Fallback Policies");

var pipeline = new ResiliencePipelineBuilder<string>()
        .AddFallback(new FallbackStrategyOptions<string>
        {
            FallbackAction = args =>
                { Console.WriteLine("Doing Fallback"); 
                  return Outcome.FromResultAsValueTask("SomeDefaultValue");
                }
        })
        .Build();

    try
    {
        var result = pipeline.Execute(() => DoSomething() );
    }
    catch
    {
        Console.WriteLine("Doing thing errored");
    }



string DoSomething()
{
    Console.WriteLine("Doing Thing");
    throw new Exception();
}