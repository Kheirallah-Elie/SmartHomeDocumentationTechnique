using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.Devices;
using System;
using System.Threading.Tasks;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Register SignalR ServiceManager
        services.AddSingleton(sp =>
        {
            return (IServiceManager)new ServiceManagerBuilder()
                .WithOptions(option =>
                {
                    option.ConnectionString = Environment.GetEnvironmentVariable("AzureSignalRConnectionString");
                })
                .BuildServiceManager();
        });

        // Register ServiceHubContext
        services.AddSingleton(sp =>
        {
            var manager = sp.GetRequiredService<IServiceManager>();
            return manager.CreateHubContextAsync("deviceHub").GetAwaiter().GetResult();
        });

        // Register IoT Hub ServiceClient
        services.AddSingleton(sp =>
        {
            var connectionString = Environment.GetEnvironmentVariable("IOT_HUB_CONNECTION_STRING");
            return ServiceClient.CreateFromConnectionString(connectionString);
        });
    })
    .Build();

await host.RunAsync();
