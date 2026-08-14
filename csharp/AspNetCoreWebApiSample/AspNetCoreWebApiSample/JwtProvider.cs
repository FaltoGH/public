using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Security.Cryptography;

namespace AspNetCoreWebApiSample
{
    public class JwtProvider
    {
        IJwtEncoder encoder;
        IJwtDecoder decoder;
        public JwtProvider()
        {
            RSA privateKey = RSA.Create();
            // openssl genrsa -out privkey.pem
            string privkey = File.ReadAllText("privkey.pem");
            privateKey.ImportFromPem(privkey);

            RSA publicKey = RSA.Create();
            // openssl rsa -in privkey.pem -pubout -out pubkey.pem
            string pubkey = File.ReadAllText("pubkey.pem");
            publicKey.ImportFromPem(pubkey);
            
            IJwtAlgorithm algorithm = new RS256Algorithm(publicKey, privateKey);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJsonSerializer serializer = new JsonNetSerializer();
            encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
        }

        public string Encode(Dictionary<string, object> payload)
        {
            string token = encoder.Encode(payload, (string?)null);
            return token;
        }

        public string Decode(string token)
        {
            string payload = decoder.Decode(token, true);
            return payload;
        }

    }
}
