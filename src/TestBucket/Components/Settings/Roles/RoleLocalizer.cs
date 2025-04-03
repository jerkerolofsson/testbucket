using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using TestBucket.Localization;

namespace TestBucket.Domain.Identity.Permissions
{
    class RoleLocalizer
    {
        private readonly IStringLocalizer<SharedStrings> _loc;

        public RoleLocalizer(IStringLocalizer<SharedStrings> loc)
        {
            _loc = loc;
        }

        public string GetLocalizedRole(string role)
        {
            switch (role)
            {
                case Roles.SUPERADMIN:
                    return "role-site-admin";

                case Roles.REGULAR_USER:
                    return _loc["role-regular-user"];

                case Roles.ADMIN:
                    return _loc["role-tenant-admin"];

                case Roles.READ_ONLY:
                    return _loc["role-read-only"];
            }
            return role;
        }
        public string GetLocalizedPermissionLevel(PermissionLevel level)
        {
            string textLabelId = "permission-none";
            if ((level & PermissionLevel.Delete) == PermissionLevel.Delete)
            {
                textLabelId = "permission-delete";
            }
            else if ((level & PermissionLevel.Write) == PermissionLevel.Write)
            {
                textLabelId = "permission-write";
            }
            else if ((level & PermissionLevel.Read) == PermissionLevel.Read)
            {
                textLabelId = "permission-read";
            }

            return _loc[textLabelId];
        }
    }
}
