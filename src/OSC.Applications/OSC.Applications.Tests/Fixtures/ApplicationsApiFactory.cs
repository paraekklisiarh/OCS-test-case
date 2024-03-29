using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using OSC.Applications.Api;

namespace OSC.Applications.Tests.Fixtures;

public class ApplicationsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public HttpClient Client { get; private set; }
    public PostgresContainerFixture ContainerFixture { get; } = new();
    private string ConnectionString => ContainerFixture.ConnectionString;


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // In order to make this work, you have to provide your configuration values twice
        // both before and after the minimal API Program.cs runs.
        var values = new Dictionary<string, string?>
            { ["ConnectionStrings:ApplicationsApi"] = ConnectionString };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        builder
            .UseConfiguration(configuration)
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddInMemoryCollection(values);
            });
    }

    public Task DisposeAsync() => ContainerFixture.DisposeAsync();

    public async Task InitializeAsync()
    {
        await ContainerFixture.InitializeAsync();

        Client = CreateClient();
    }
}