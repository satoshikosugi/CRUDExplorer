using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CRUDExplorer.AuthServer.Data;

namespace CRUDExplorer.IntegrationTests;

/// <summary>
/// カスタムWebApplicationFactory - 統合テスト用の認証サーバー環境
/// </summary>
public class AuthServerWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 既存のDbContextDescriptorを削除
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // テスト用のIn-MemoryデータベースでDbContextを置き換え
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryTestDb");
            });

            // データベースの初期化
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AuthDbContext>();

            db.Database.EnsureCreated();
        });
    }
}
