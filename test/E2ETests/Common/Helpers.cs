using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.Extensions.Logging;

namespace E2ETests
{
    public class Helpers
    {
        public static string GetApplicationPath(ApplicationType applicationType)
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "MusicStore.sln")))
                {
                    break;
                }
                current = current.Parent;
            }

            if (current == null)
            {
                throw new InvalidOperationException("Could not find the solution directory");
            }

            return Path.GetFullPath(Path.Combine(current.FullName, "samples", "MusicStore"));
        }

        public static void SetInMemoryStoreForIIS(DeploymentParameters deploymentParameters, ILogger logger)
        {
            if (deploymentParameters.ServerType == ServerType.IIS)
            {
                // Can't use localdb with IIS. Setting an override to use InMemoryStore.
                logger.LogInformation("Creating configoverride.json file to override default config.");

                var compileRoot = Path.GetFullPath(
                    Path.Combine(
                        deploymentParameters.ApplicationPath,
                        "..", "approot", "packages", "MusicStore"));

                // We don't know the exact version number with which sources are built.
                string overrideConfig = Path.Combine(Directory.GetDirectories(compileRoot).First(), "root", "configoverride.json");


                File.WriteAllText(overrideConfig, "{\"UseInMemoryDatabase\": \"true\"}");
            }
        }

        public static string GetCurrentBuildConfiguration()
        {
            var configuration = "Debug";
            if (string.Equals(Environment.GetEnvironmentVariable("Configuration"), "Release", StringComparison.OrdinalIgnoreCase))
            {
                configuration = "Release";
            }

            return configuration;
        }

        public static bool PreservePublishedApplicationForDebugging
        {
            get
            {
                var deletePublishedFolder = Environment.GetEnvironmentVariable("ASPNETCORE_DELETEPUBLISHEDFOLDER");

                if (string.Equals("false", deletePublishedFolder, StringComparison.OrdinalIgnoreCase)
                    || string.Equals("0", deletePublishedFolder, StringComparison.OrdinalIgnoreCase))
                {
                    // preserve the published folder and do not delete it
                    return true;
                }

                // do not preserve the published folder and delete it
                return false;
            }
        }
    }
}