using BaseApi.Models;

namespace BaseApi.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}

