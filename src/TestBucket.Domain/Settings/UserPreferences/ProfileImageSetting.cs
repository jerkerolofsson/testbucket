using Microsoft.AspNetCore.Identity;

using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Settings.Appearance
{
    /// <summary>
    /// Reads/Writes the profile image. The profile image is saved as a URI
    /// </summary>
    class ProfileImageSetting : SettingAdapter
    {
        private readonly IUserManager _userManager;
        private readonly IProfilePictureManager _profilePictureManager;

        public ProfileImageSetting(IUserManager manager, IProfilePictureManager profilePictureManager)
        {
            _userManager = manager;

            Metadata.Name = "profile-image";
            Metadata.Category.Name = "profile";
            Metadata.Category.Icon = SettingIcons.Profile;
            Metadata.Section.Name = "appearance";
            Metadata.Type = FieldType.ImageUri;
            _profilePictureManager = profilePictureManager;
        }

        public override async Task<FieldValue> ReadAsync(SettingContext context)
        {
            var principal = context.Principal;
            if(principal.Identity?.Name is null)
            {
                return FieldValue.Empty;
            }

            var tenantId = principal.GetTenantIdOrThrow();
            var user = await _userManager.FindAsync(principal);

            return new FieldValue { StringValue = user?.ProfileImageUri, FieldDefinitionId = 0 };
        }

        public override async Task WriteAsync(SettingContext context, FieldValue value)
        {
            var principal = context.Principal;
           
            var user = await _userManager.FindAsync(principal);
            if(user is null)
            {
                return;
            }
            user.ProfileImageUri = value.StringValue;

            // Clear the image cache
            if (user.UserName is not null)
            {
                _profilePictureManager.ClearCachedProfileImage(user.UserName);
            }

            await _userManager.UpdateUserAsync(principal, user);
        }
    }
}
