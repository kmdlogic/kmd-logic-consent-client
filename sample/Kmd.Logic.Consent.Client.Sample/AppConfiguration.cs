using System;
using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.Consent.Client.Sample
{
    internal class AppConfiguration
    {
        public string ConsentKey { get; set; }
        public string ConsentKeyFormat { get; set; }
        public string ConsentScope { get; set; }

        public LogicTokenProviderOptions TokenProvider { get; set; } = new LogicTokenProviderOptions();

        public ConsentOptions Consent { get; set; } = new ConsentOptions();
    }
}