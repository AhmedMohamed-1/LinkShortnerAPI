using LinkShorterAPI.Models;
using LinkShorterAPI.Services.EmailServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace LinkShorterAPI.Background_Service
{
    public class ExpirationCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpirationCheckService> _logger;

        public ExpirationCheckService(IServiceProvider serviceProvider, ILogger<ExpirationCheckService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<LinkShortnerContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var now = DateTime.UtcNow;

                    var expiredLinks = await dbContext.ShortLinks
                        .Where(l => l.IsActive == true && l.ExpireAt != null && l.ExpireAt <= now)
                        .ToListAsync(stoppingToken);

                    if (expiredLinks.Any())
                    {
                        foreach (var link in expiredLinks)
                        {
                            link.IsActive = false;

                            var user = await dbContext.Users
                                .FirstOrDefaultAsync(u => u.Id == link.CreatedBy, stoppingToken);

                            if (user != null)
                            {
                                await emailService.SendEmailAsync(
                                    user.Email,
                                    "Your link has expired",
                                    $"Your short link <b>{link.Slug}</b> expired on {link.ExpireAt:yyyy-MM-dd}."
                                );
                            }
                        }
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Marked {Count} links as inactive.", expiredLinks.Count);
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking for expired links.");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}
