using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace WebTask1.Config
{
    public class ConfigStorage
    {
        private readonly Dictionary<string, string> _configResult;

        public ConfigStorage(IConfiguration config)
        {
            _configResult = new Dictionary<string, string>();

            var appSettings = config.GetSection("RabbitMQ").GetChildren().ToList();

            foreach (var setting in appSettings)
            {
                _configResult.Add(setting.Key.ToLower(), setting.Value);
            }
        }

        public Dictionary<string, string>  GetSettings()
        {
            return _configResult;
        }
    }
}
