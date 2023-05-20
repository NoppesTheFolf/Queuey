using System.Security.Cryptography;
using System.Text;

namespace Noppes.Queuey.Api.Authentication;

public class ApiKeyChecker
{
    private readonly ICollection<byte[]> _apiKeys;

    public ApiKeyChecker(IEnumerable<string> apiKeys)
    {
        _apiKeys = apiKeys.Select(x => Encoding.UTF8.GetBytes(x)).ToList();
    }

    public bool IsAuthorized(string apiKey)
    {
        return _apiKeys
            .Select(x => CryptographicOperations.FixedTimeEquals(x, Encoding.UTF8.GetBytes(apiKey)))
            .Any(x => x);
    }
}
