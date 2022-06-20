using Microsoft.Extensions.Configuration;

namespace ManyHelpers.Configuration {
    public class ConfigurationHelper {
        public T GetConfiguration<T>(string? sectionName = null, string? pathConfigFile = null, string configFileName = "appsettings.json", bool optional = false) {
            if (string.IsNullOrEmpty(sectionName)) {
                sectionName = typeof(T).Name;
            }
            if (string.IsNullOrEmpty(pathConfigFile)) {
                pathConfigFile = Directory.GetCurrentDirectory();
            }

            var builder = new ConfigurationBuilder()
                  .SetBasePath(pathConfigFile)
                  .AddJsonFile(configFileName, optional: optional);

            IConfiguration config = builder.Build();
            var configuration = config.GetSection(sectionName).Get<T>();

            return configuration;
        }

        public static string GetConfigurationValue(string key, string configFileName = "appsettings.json") {
            var builder = new ConfigurationBuilder().AddJsonFile(configFileName);
            var configuration = builder.Build();
            var configValue = configuration[key];
            return configValue;
        }

    }
}
