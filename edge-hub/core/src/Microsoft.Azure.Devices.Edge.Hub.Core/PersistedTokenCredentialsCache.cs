// Copyright (c) Microsoft. All rights reserved.
namespace Microsoft.Azure.Devices.Edge.Hub.Core
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Edge.Hub.Core.Identity;
    using Microsoft.Azure.Devices.Edge.Storage;
    using Microsoft.Azure.Devices.Edge.Util;
    using Microsoft.Azure.Devices.Edge.Util.Json;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class PersistedTokenCredentialsCache : ICredentialsCache
    {
        readonly IKeyValueStore<string, string> encryptedStore;

        public PersistedTokenCredentialsCache(IKeyValueStore<string, string> encryptedStore)
        {
            this.encryptedStore = Preconditions.CheckNotNull(encryptedStore, nameof(encryptedStore));
        }

        public async Task Add(IClientCredentials clientCredentials)
        {
            if (clientCredentials is ITokenCredentials tokenCredentials)
            {
                string targetId = AuthChainHelpers.GetAuthTarget(tokenCredentials.AuthChain).GetOrElse(tokenCredentials.Identity.Id);

                try
                {
                    var tokenCredentialsData = new TokenCredentialsData(tokenCredentials.Token, tokenCredentials.IsUpdatable, tokenCredentials.AuthChain);
                    string tokenCredentialsString = JsonConvert.SerializeObject(tokenCredentialsData);
                    await this.encryptedStore.Put(targetId, tokenCredentialsString);
                    Events.Stored(clientCredentials.Identity.Id);
                }
                catch (Exception e)
                {
                    Events.ErrorStoring(e, clientCredentials.Identity.Id);
                }
            }
        }

        public async Task<Option<IClientCredentials>> Get(IIdentity identity)
        {
            Option<string> tokenCredentialsDataOption = await this.encryptedStore.Get(identity.Id);
            return tokenCredentialsDataOption.FlatMap(
                t =>
                {
                    Events.Retrieved(identity.Id);
                    try
                    {
                        return Option.Some(
                            ParseTokenCredentialsData(identity, t)
                                .GetOrElse(() => new TokenCredentials(identity, t, string.Empty, Option.None<string>(), Option.None<string>(), false) as IClientCredentials));
                    }
                    catch (Exception e)
                    {
                        Events.ErrorGetting(e, identity.Id);
                        return Option.None<IClientCredentials>();
                    }
                });
        }

        static Option<IClientCredentials> ParseTokenCredentialsData(IIdentity identity, string json)
        {
            try
            {
                var tokenCredentialsData = json.FromJson<TokenCredentialsData>();
                Events.Retrieved(identity.Id);
                return Option.Some(new TokenCredentials(identity, tokenCredentialsData.Token, string.Empty, Option.None<string>(), tokenCredentialsData.AuthChain, tokenCredentialsData.IsUpdatable) as IClientCredentials);
            }
            catch (Exception e)
            {
                Events.ErrorParsingData(identity.Id, e);
                return Option.None<IClientCredentials>();
            }
        }

        static class Events
        {
            const int IdStart = HubCoreEventIds.TokenCredentialsStore;
            static readonly ILogger Log = Logger.Factory.CreateLogger<PersistedTokenCredentialsCache>();

            enum EventIds
            {
                ErrorStoring = IdStart,
                ErrorRetrieving,
                Retrieved,
                Stored,
                ErrorParsingData
            }

            public static void Stored(string id)
            {
                Log.LogDebug((int)EventIds.Stored, $"Stored token for - {id}");
            }

            public static void ErrorStoring(Exception ex, string id)
            {
                Log.LogWarning((int)EventIds.ErrorStoring, ex, $"Error storing token for - {id}");
            }

            public static void Retrieved(string id)
            {
                Log.LogDebug((int)EventIds.Retrieved, $"Retrieved token data for - {id}");
            }

            public static void ErrorGetting(Exception ex, string id)
            {
                Log.LogWarning((int)EventIds.ErrorRetrieving, ex, $"Error storing token for - {id}");
            }

            public static void ErrorParsingData(string id, Exception ex)
            {
                Log.LogDebug((int)EventIds.ErrorParsingData, ex, $"Error parsing persisted token credentials data for {id}, treating it as the token string instead.");
            }
        }

        class TokenCredentialsData
        {
            [JsonConstructor]
            public TokenCredentialsData(string token, bool isUpdatable, string authChain)
            {
                this.Token = token;
                this.IsUpdatable = isUpdatable;
                this.AuthChain = Option.Maybe(authChain);
            }

            public TokenCredentialsData(string token, bool isUpdatable, Option<string> authChain)
            {
                this.Token = token;
                this.IsUpdatable = isUpdatable;
                this.AuthChain = authChain;
            }

            [JsonProperty("isUpdatable")]
            public bool IsUpdatable { get; }

            [JsonProperty("token")]
            public string Token { get; }

            [JsonConverter(typeof(OptionConverter<string>))]
            [JsonProperty(PropertyName = "authChain")]
            public Option<string> AuthChain { get; }
        }
    }
}
