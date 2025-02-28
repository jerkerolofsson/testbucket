using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Models;
public class ApplicationUserApiKey
{
    public long Id { get; set; }

    public required string Name { get; set; }
    public required string Key { get; set; }

    public required DateTimeOffset Expiry { get; set; }

    // Navigation
    public string? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }
}
