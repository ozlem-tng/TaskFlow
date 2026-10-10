using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RegisterAsync(RegisterDto request)
    {
        // 1. Email daha önce kayıt edilmiş mi?
        bool emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Bu e-posta adresi zaten kayıtlı."
            );
        }

        // 2. Şifreyi hashle
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Yeni kullanıcı nesnesini oluştur
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        // 4. Kullanıcıyı EF Core'a ekle
        _context.Users.Add(user);

        // 5. Değişiklikleri veritabanına kaydet
        await _context.SaveChangesAsync();
    }
}