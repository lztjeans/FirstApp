using System.Threading.Tasks;

namespace FirstApp.Services;

public interface ITokenClaimsService
{
    Task<string> GetTokenAsync(string userName);
}
