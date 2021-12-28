using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManyHelpers.Configuration {
    public class ConfigurationHelper {
        public static string GetConfigurationValue(string key) {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var configValue = configuration[key];
            return configValue;
        }
    }
}
