using Testcontainers.PostgreSql;

namespace OSC.Applications.Tests.Fixtures;

public class PostgresContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer Container { get; } = new PostgreSqlBuilder()
        .WithDatabase("osc-integration-tests" + Guid.NewGuid())
        .WithUsername("osc")
        .WithPassword("osc")
        .Build();

    public string ConnectionString => Container.GetConnectionString();

    public Task InitializeAsync() => Container.StartAsync();

    public Task DisposeAsync() => Container.DisposeAsync().AsTask();
}