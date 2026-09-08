using Microsoft.Extensions.Caching.Memory;
using StyleCart.Application.Interfaces;

namespace StyleCart.Infrastructure.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(key, out T? value);

        return Task.FromResult(value);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        _cache.Set(key, value, expiration);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);

        return Task.CompletedTask;
    }
}