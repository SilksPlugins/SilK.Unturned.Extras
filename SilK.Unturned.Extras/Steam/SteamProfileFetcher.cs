using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilK.Unturned.Extras.Steam
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class SteamProfileFetcher : ISteamProfileFetcher, IDisposable
    {
        private readonly ILogger<SteamProfileFetcher> _logger;

        private readonly XmlSerializer _serializer;
        private readonly HttpClient _httpClient;

        private readonly Dictionary<CSteamID, ISteamProfile> _cachedProfiles;

        public SteamProfileFetcher(ILogger<SteamProfileFetcher> logger)
        {
            _logger = logger;

            _serializer = new XmlSerializer(typeof(SteamProfile));
            _httpClient = new HttpClient();

            _cachedProfiles = new Dictionary<CSteamID, ISteamProfile>();
        }

        private static string GetProfileDataUrl(CSteamID steamId) => $"https://steamcommunity.com/profiles/{steamId}?xml=1";

        public async Task<ISteamProfile?> GetSteamProfile(CSteamID steamId, bool forceRefresh = false)
        {
            if (!forceRefresh && _cachedProfiles.TryGetValue(steamId, out var profile))
                return profile;

            var url = GetProfileDataUrl(steamId);

            try
            {
                var profileData = await _httpClient.GetStringAsync(url);

                using var reader = new StringReader(profileData);

                profile = (SteamProfile)_serializer.Deserialize(reader);

                if (!_cachedProfiles.ContainsKey(steamId))
                    _cachedProfiles.Add(steamId, profile);
                else
                    _cachedProfiles[steamId] = profile;

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error occurred when fetching steam profile data");
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
