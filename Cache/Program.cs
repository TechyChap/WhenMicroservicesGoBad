//https://github.com/App-vNext/Polly/wiki/Cache

using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;

var rnd = new Random();

Console.WriteLine("Cache Policies");

var memoryCache = new MemoryCache(new MemoryCacheOptions());
var memoryCacheProvider = new MemoryCacheProvider(memoryCache);

var policy = Policy
              .Cache(memoryCacheProvider, TimeSpan.FromSeconds(10)); //Set the timeout to control size and refresh of cache.

while(true)
{
    var key = "ThingKey";
    var ret = policy.Execute(context => DoSomething(key), new Context(key));
    Console.WriteLine($"Doing {ret}");
    Thread.Sleep(500);

    //For example you could remove the item from a cache if new data had come from a message
    if (rnd.Next(10) > 6)
    {
        memoryCache.Remove("ThingKey");
    }
}

string DoSomething(string key)
{
    Console.WriteLine("Calling API");
    return "Thing";
}