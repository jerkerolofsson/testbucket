using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts.Mapping
{
    public static class TestAccountMapping
    {
        public static TestAccountDto ToDto(this TestAccount source)
        {
            return new TestAccountDto
            {
                Name = source.Name,
                Variables = source.Variables,
                Type = source.Type,
                SubType = source.SubType,
                Owner = source.Owner,
                Tenant = source.TenantId,
                Enabled = source.Enabled,
                Locked = source.Locked,
                ModifiedBy = source.ModifiedBy,
                CreatedBy = source.CreatedBy,
                Created = source.Created,
                Modified = source.Modified,
                LockExpires = source.LockExpires,
                LockOwner = source.LockOwner,
            };
        }
        public static TestAccount ToDbo(this TestAccountDto source)
        {
            return new TestAccount
            {
                Name = source.Name,
                Variables = source.Variables,
                Type = source.Type,
                SubType = source.SubType,
                Owner = source.Owner ?? "internal",
                TenantId = source.Tenant,
                Enabled = source.Enabled,
                Locked = source.Locked,
                ModifiedBy = source.ModifiedBy,
                CreatedBy = source.CreatedBy,
                Created = source.Created,
                Modified = source.Modified,
                LockExpires = source.LockExpires,
                LockOwner = source.LockOwner,
            };
        }
    }
}
