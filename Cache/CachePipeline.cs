using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.Telemetry;

// Strategies should be internal and not exposed in the library's public API.
// Use extension methods and options to configure the strategy.
internal sealed class CachingStrategy<T> : ResilienceStrategy<T> where T : class
{
    private readonly CachingStrategyOptions _options;
    private readonly ResilienceStrategyTelemetry _telemetry;

    public CachingStrategy(
        CachingStrategyOptions options,
        ResilienceStrategyTelemetry telemetry)
    {
        _options = options;
        _telemetry = telemetry;
    }

    protected override async ValueTask<Outcome<T>> ExecuteCore<TState>(
        Func<ResilienceContext, TState, ValueTask<Outcome<T>>> callback,
        ResilienceContext context,
        TState state)
    {
        Outcome<T> outcome;

        var key = context.OperationKey ?? "defaultKey";

        var cached = await _options.CacheProvider.GetAsync(key);
        if (cached != null)
        {
            var result = cached.FromByteArray<T>();
            outcome = Outcome.FromResult<T>(result);
            _telemetry.Report(new ResilienceEvent(ResilienceEventSeverity.Information, "CacheHit"), context, outcome);
        }
        else
        {
            outcome = await callback(context, state).ConfigureAwait(context.ContinueOnCapturedContext);
            _telemetry.Report(new ResilienceEvent(ResilienceEventSeverity.Information, "CacheMiss"), context, outcome);
            if (outcome.Result != null)
            {
                await _options.CacheProvider.SetAsync(key, outcome.Result.ToByteArray());
            }
        }
        return outcome;
    }
}

public class CachingStrategyOptions : ResilienceStrategyOptions
{
    public DistributedCacheEntryOptions CacheOptions { get; }
    public IDistributedCache CacheProvider { get; }

    public CachingStrategyOptions(IDistributedCache cacheProvider, DistributedCacheEntryOptions cacheOptions)
    {
        CacheOptions = cacheOptions;
        CacheProvider = cacheProvider;
    }
}

internal static class Serialization
{
    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }

    public static byte[] ToByteArray<T>(this T obj)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj, GetJsonSerializerOptions()));

    }
    public static T FromByteArray<T>(this byte[] byteArray)
    {
        return JsonSerializer.Deserialize<T>(byteArray, GetJsonSerializerOptions());
    }

}



  