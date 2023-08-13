using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PFI_MOVIE_SEARCH
{
    public class Program
    {
        public static IConfiguration configuration;
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            configuration = builder.Build();

            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                //HardwareTest.Instance.CheckConnection();
                logger.Info("[Program(PFI_MOVIE_SEARCH)][Main] Web server started.");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception: " + exception.ToString());
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
             WebHost.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((env, config) =>
                 {
                     //Configuración de proveedores
                     var ambiente = env.HostingEnvironment.EnvironmentName;
                     config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                     config.AddJsonFile($"appSettings.{ambiente}.json", optional: true, reloadOnChange: true);
                     config.AddEnvironmentVariables();
                     if (args != null)
                     {
                         config.AddCommandLine(args);
                     }
                 })
                 .UseStartup<Startup.Startup>();
    }
}
