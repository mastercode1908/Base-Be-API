using BaseApi.DTOs.Requests;
using BaseApi.DTOs.Response;

namespace BaseApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
