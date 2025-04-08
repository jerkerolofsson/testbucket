using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Models
{
    public class SeedConfiguration
    {
        /// <summary>
        /// Default admin email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Default admin password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Tenant
        /// </summary>
        public string? Tenant { get; set; }

        /// <summary>
        /// JWT symmetric key
        /// </summary>
        public string? SymmetricKey { get; set; }

        /// <summary>
        /// JWT audience
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// JWT issuer
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Default admin access toekn
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Publicly accessible URL (in case the service is running behind a reverse-proxy)
        /// </summary>
        public string? PublicEndpointUrl { get; set; }
    }
}
