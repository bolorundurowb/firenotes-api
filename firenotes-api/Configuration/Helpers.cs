using System;
using System.Collections.Generic;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace firenotes_api.Configuration
{
    public static class Helpers
    {
        public static string GenerateToken(string key, string data, int duration = 48)
        {
            IDateTimeProvider provider = new UtcDateTimeProvider();
            var expiry = provider.GetNow().AddHours(duration);
            var unixEpoch = JwtValidator.UnixEpoch; 
            var secondsSinceEpoch = Math.Round((expiry - unixEpoch).TotalSeconds);
            
            var payload = new Dictionary<string, object>
            {
                { key, data },
                { "exp", secondsSinceEpoch }
            };
            var secret = Config.Secret;
            
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);
            
            return token;
        }

        internal static IDictionary<string, string> DecodeToken(string token)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

            return decoder.DecodeToObject<IDictionary<string, string>>(token, Config.Secret, true);
        }
    }
}