using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Players.Chat.Events;
using SilK.Unturned.Extras.Configuration;
using SilK.Unturned.Extras.Events;
using SilK.Unturned.Extras.Localization;

namespace UnturnedExtrasTest
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class TestService : ITestService,
        IInstanceEventListener<UnturnedPlayerChattingEvent>
    {
        private readonly ILogger<TestService> _logger;
        private readonly IStringLocalizerAccessor<UnturnedExtrasTestPlugin> _stringLocalizerAccessor;
        private readonly IConfigurationAccessor<UnturnedExtrasTestPlugin> _configurationAccessor;

        public TestService(
            ILogger<TestService> logger,
            IStringLocalizerAccessor<UnturnedExtrasTestPlugin> stringLocalizerAccessor,
            IConfigurationAccessor<UnturnedExtrasTestPlugin> configurationAccessor)
        {
            _logger = logger;
            _stringLocalizerAccessor = stringLocalizerAccessor;
            _configurationAccessor = configurationAccessor;

            TestMethod();
        }

        public UniTask HandleEventAsync(object? sender, UnturnedPlayerChattingEvent @event)
        {
            _logger.LogInformation("Player is chatting: " + @event.Message);

            return UniTask.CompletedTask;
        }

        public void TestMethod()
        {
            var stringLocalizer = _stringLocalizerAccessor.GetNullableInstance();
            _logger.LogInformation($"Is {nameof(IStringLocalizer)} currently null?: {stringLocalizer == null}");

            var configuration = _configurationAccessor.GetNullableInstance();
            _logger.LogInformation($"Is {nameof(IConfiguration)} currently null?: {configuration == null}");
        }
    }
}
