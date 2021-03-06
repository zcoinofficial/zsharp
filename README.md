# Zsharp
[![codecov](https://codecov.io/gh/zcoinofficial/zsharp/branch/master/graph/badge.svg)](https://codecov.io/gh/zcoinofficial/zsharp)

This is a high-quality .NET library for working with Zcoin. Almost all of the code have test coverage. Some of available features are:

- Classes and functions to deal with Blockchain primitive based on [NBitcoin](https://github.com/MetacoSA/NBitcoin) (e.g. block, transaction, etc.).
- RPC client to communicate with Zcoin daemon.
- Block indexer for implementing any kind of application on top of it (e.g. explorer).
- Custom type mapper for Entity Framework to allow using blockchain primitive directly on the model (e.g. use `uint256` directly on the EF model).

Most of the code was migrated from [ZTM](https://github.com/zcoinofficial/ztm).

## Installation

All of Zsharp packages can be install from [NuGet](https://www.nuget.org/packages?q=Zsharp).

## Examples
### Parsing block

```csharp
var block = Block.Parse("HEX", Networks.Default.Mainnet);
```

### Block indexer

```csharp
void ConfigureServices(IServiceCollection services)
{
    // Application Services.
    services.AddHostedService<DatabaseMigrator>();

    // Zsharp Services.
    services.AddSingleton<IServiceExceptionHandler, ServiceExceptionLogger>();
    services.AddZcoin(NetworkType.Mainnet);
    services.AddElysiumSerializer();
    services.AddZcoinRpcClient(options =>
    {
        options.ServerUrl = new Uri("http://127.0.0.1:8888");
        options.Credential = RPCCredentialString.Parse("rpcuser:rpcpassword");
    });
    services.AddLightweightIndexer(options =>
    {
        options.BlockPublisherAddress = "tcp://127.0.0.1:ZMQPORT";
    });
    services.AddLightweightIndexerEntityRepository();
    services.AddLightweightIndexerPostgresDbContext(options =>
    {
        options.ConnectionString = "Host=127.0.0.1;Database=postgres;Username=postgres;Password=postgres";
    });
}
```

```csharp
class DatabaseMigrator : IHostedService
{
    readonly ILogger logger;
    readonly IDbContextFactory<Zsharp.LightweightIndexer.Entity.DbContext> chain;

    public DatabaseMigrator(
        ILogger<DatabaseMigrator> logger,
        IDbContextFactory<Zsharp.LightweightIndexer.Entity.DbContext> chain)
    {
        this.logger = logger;
        this.chain = chain;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Blockchain.
        this.logger.LogInformation("Migrating blockchain database.");

        await using (var db = await this.chain.CreateAsync(cancellationToken))
        {
            await db.Database.MigrateAsync(cancellationToken);
        }

        this.logger.LogInformation("All database migrations completed successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

## Development
### Requirements

- .NET Core 3.1
- Docker Compose

### Build

```sh
dotnet build src/Zsharp.sln
```

### Running tests

Start the required services with Docker Compose:

```sh
docker-compose up -d
```

Then execute:

```sh
dotnet test src/Zsharp.sln
```

### Updating version

The right way to update `Version` in the `.csproj` file on each project is:

1. Increase the version number **only** on the first commit that introduce changes to that project since the latest release.
2. Follow [SemVer](https://semver.org/) for how to increase version number.

With this way we will always know which project need to publish a new version when we are going to release by comparing the version in the repository against the latest version that was published. Another benefits is we don't need to publish the project that don't have any changes since the latest release.
