using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SemperPrecisStageTracker.Domain.Configurations;
using SemperPrecisStageTracker.Mocks.Scenarios;
using SemperPrecisStageTracker.Models.Diagnostics.Tracers;
using ZenProgramming.Chakra.Core.Configurations;
using ZenProgramming.Chakra.Core.Configurations.Utils;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.Diagnostic;
using ZenProgramming.Chakra.Core.Mocks.Data;

namespace SemperPrecisStageTracker.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Configurazione del tracer
            Tracer.Append(typeof(Log4NetTracer));
            Tracer.Info($"[Settings] Working on environment '{ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.EnvironmentName}' (from configuration factory)");

            //Select provider for data storage
            SettingsUtils.Switch(ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.Storage.Provider, new Dictionary<string, Action>
            {
                { "Mockup", SessionFactory.RegisterDefaultDataSession<MockDataSession<SimpleScenario>> },
                //{ "Sql", SessionFactory.RegisterDefaultDataSession<EntityFrameworkDataSession<BonebatContext>>  }
            });

            // identity client
            //SettingsUtils.Switch(ConfigurationFactory<SemperPrecisStageTrackerConfiguration>.Instance.Storage.Provider, new Dictionary<string, Action>
            //{
            //    { "Mockup", ServiceResolver.Register<IIdentityClient, MockIdentityClient> },
            //    { "Sql", ServiceResolver.Register<IIdentityClient, SqlIdentityClient> }
            //});

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
