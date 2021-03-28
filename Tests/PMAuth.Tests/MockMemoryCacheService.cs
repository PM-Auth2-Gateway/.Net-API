using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace PMAuth.Tests
{
    public static class MockMemoryCacheService
    {
        public static IMemoryCache GetMemoryCache(object key, object expectedValue)
        {
            var mockMemoryCache = new Mock<IMemoryCache>();
            mockMemoryCache
                .Setup(x => x.TryGetValue(key, out expectedValue))
                .Returns(true);
            return mockMemoryCache.Object;
        }
    }
}
