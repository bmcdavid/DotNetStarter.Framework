using DotNetStarter.Abstractions;

namespace DotNetStarter.Framework.Tests
{
    [StartupModule(typeof(RegisterConfiguration))]
    public class _TestLocatorSetup : ILocatorConfigure
    {
        private ILocator _Locator;

        public void Configure(ILocatorRegistry locator, IStartupEngine engine)
        {
            _Locator = locator;

#if NETCOREAPP1_1
            var mOptions = new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() { CompactOnMemoryPressure = true, ExpirationScanFrequency = new System.TimeSpan(0, 0, 10) };
            locator.Add(typeof(Microsoft.Extensions.Options.IOptions<Microsoft.Extensions.Caching.Memory.MemoryCacheOptions>), new Microsoft.Extensions.Options.OptionsWrapper<Microsoft.Extensions.Caching.Memory.MemoryCacheOptions>(mOptions));
#endif
        }
    }
}