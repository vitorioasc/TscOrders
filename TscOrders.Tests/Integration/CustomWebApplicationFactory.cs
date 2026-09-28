using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TscOrders.Data.Persistence;
using TscOrders.WebApi;

namespace TscOrders.Tests.Integration;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<TscOrdersDbContext>>();
            services.AddDbContext<TscOrdersDbContext>(options => options.UseSqlite(_connection));
        });
    }

    void IDisposable.Dispose()
    {
        _connection.Dispose();
        base.Dispose();
    }
}
