using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Retry;
using System.Threading;

var rnd = new Random();

Console.WriteLine("Cache");

var serviceProvider = new ServiceCollection()
    .AddDistributedMemoryCache()
    .BuildServiceProvider();

var myCache = serviceProvider.GetService<IDistributedCache>();

var options = new CachingStrategyOptions(myCache, new DistributedCacheEntryOptions()
    .SetSlidingExpiration(TimeSpan.FromSeconds(5)));

var pipeline = new ResiliencePipelineBuilder<string>()
    .AddStrategy<string>(context => new CachingStrategy<string>(options, context.Telemetry), options) 
    .Build();

while (true)
{
    var key = "ThingKey";
    var ret = pipeline.Execute(_ => DoSomething(key), ResilienceContextPool.Shared.Get(operationKey: key));

    Console.WriteLine($"Doing {ret}");
    Thread.Sleep(500);

    //For example you could remove the item from a cache if new data had come from a message
    if (rnd.Next(10) > 6)
    {
        myCache.Remove("ThingKey");
    }
}

string DoSomething(string key)
{
    Console.WriteLine("Calling API");
    return "Thing" ;
}

