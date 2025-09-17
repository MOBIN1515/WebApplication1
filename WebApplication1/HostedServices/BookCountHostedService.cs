using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebApplication1.AppDbContextEF;

namespace WebApplication1.HostedServices;

public class BookCountHostedService : BackgroundService
{
    public readonly ILogger<BookCountHostedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public BookCountHostedService(ILogger<BookCountHostedService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var count = await db.Books.CountAsync(stoppingToken);
            _logger.LogInformation("تعداد کتاب‌ها در DB: {Count}", count);

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

}
