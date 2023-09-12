using System.Threading.Tasks;

namespace FirstApp.Service;

public interface ITokenClaimsService
{
    Task<string> GetTokenAsync(string userName);
}
