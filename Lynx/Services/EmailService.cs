using Lynx.Core.Entities;
using Lynx.IServices;

namespace Lynx.Services;

public class EmailService : IEmailService
{
private readonly HttpClient _httpClient;

    public EmailService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<User> GetUser(int id)
    {
        return (await _httpClient.GetFromJsonAsync<User>($"user?q={id}"))!;
    }
}