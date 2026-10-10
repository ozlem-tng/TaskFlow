using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto request);
}