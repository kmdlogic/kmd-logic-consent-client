﻿using Serilog;

namespace Kmd.Logic.ConsentService.ConsoleSample
{
    internal class ConfigurationValidator
    {
        private readonly AppConfiguration _configuration;

        public ConfigurationValidator(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        public enum Result { Valid, Invalid }

        public Result Validate()
        {
            if (_configuration.LogicAccount == null
                || string.IsNullOrWhiteSpace(_configuration.LogicAccount?.ClientId)
                || string.IsNullOrWhiteSpace(_configuration.LogicAccount?.ClientSecret)
                || _configuration.LogicAccount?.SubscriptionId == null)
            {
                Log.Error("Invalid `LogicAccount` configuration. Please provide proper information to `appsettings.json`. Current data is: {@LogicAccount}",
                    _configuration.LogicAccount);

                return Result.Invalid;
            }

            return Result.Valid;
        }
    }
}