using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Models
{
    public class SeedConfiguration
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Tenant { get; set; }
        public string? SymmetricKey { get; set; }
        public string? Audience { get; set; }
        public string? Issuer { get; set; }
        public string? AccessToken { get; set; }

        public string? PublicEndpointUrl { get; set; }
    }
}
