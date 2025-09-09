using Microsoft.Extensions.Localization;
using NSubstitute;
using TestBucket.Domain.TestAccounts.Helpers;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.UnitTests.TestAccounts
{
    /// <summary>
    /// Contains unit tests for <see cref="TestAccountTemplateHelper"/> methods related to account types and field definitions.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    [Component("Test Accounts")]
    public class TestAccountTemplateHelperTests
    {
        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.GetSubTypesForAccountType(string)"/> returns all Wi-Fi subtypes for the Wi-Fi account type.
        /// </summary>
        [Fact]
        public void GetSubTypesForAccountType_ReturnsWifiSubTypes()
        {
            var result = TestAccountTemplateHelper.GetSubTypesForAccountType(PrimaryAccountTypes.Wifi);
            Assert.Equal(WifiAccountSubTypes.All, result);
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.GetSubTypesForAccountType(string)"/> returns all email subtypes for the Email account type.
        /// </summary>
        [Fact]
        public void GetSubTypesForAccountType_ReturnsEmailSubTypes()
        {
            var result = TestAccountTemplateHelper.GetSubTypesForAccountType(PrimaryAccountTypes.Email);
            Assert.Equal(EmailAccountTypes.All, result);
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.GetSubTypesForAccountType(string)"/> returns an empty array for an unknown account type.
        /// </summary>
        [Fact]
        public void GetSubTypesForAccountType_ReturnsEmptyForUnknownType()
        {
            var result = TestAccountTemplateHelper.GetSubTypesForAccountType("unknown");
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.GetFieldDefinitionsForAccount(IStringLocalizer, string, string)"/> returns SSID and password fields for Wi-Fi WPA2-PSK subtype.
        /// </summary>
        [Fact]
        public void GetFieldDefinitionsForAccount_WifiWpa2Psk_ReturnsSsidAndPassword()
        {
            var localizer = Substitute.For<IStringLocalizer>();
            var result = TestAccountTemplateHelper.GetFieldDefinitionsForAccount(localizer, PrimaryAccountTypes.Wifi, WifiAccountSubTypes.Wpa2Psk);
            Assert.Contains(result, f => f.Name == "ssid" && f.Type == FieldType.String);
            Assert.Contains(result, f => f.Name == "password" && f.Type == FieldType.String);
            Assert.Equal(2, result.Length);
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.GetFieldDefinitionsForAccount(IStringLocalizer, string, string)"/> returns only the SSID field for Wi-Fi open subtype.
        /// </summary>
        [Fact]
        public void GetFieldDefinitionsForAccount_WifiOpen_ReturnsOnlySsid()
        {
            var localizer = Substitute.For<IStringLocalizer>();
            var result = TestAccountTemplateHelper.GetFieldDefinitionsForAccount(localizer, PrimaryAccountTypes.Wifi, "open");
            Assert.Contains(result, f => f.Name == "ssid" && f.Type == FieldType.String);
            Assert.DoesNotContain(result, f => f.Name == "password");
            Assert.Single(result);
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.GetFieldDefinitionsForAccount(IStringLocalizer, string, string)"/> returns username and password fields for the Server account type.
        /// </summary>
        [Fact]
        public void GetFieldDefinitionsForAccount_Server_ReturnsUsernameAndPassword()
        {
            var localizer = Substitute.For<IStringLocalizer>();
            var result = TestAccountTemplateHelper.GetFieldDefinitionsForAccount(localizer, PrimaryAccountTypes.Server, "");
            Assert.Contains(result, f => f.Name == "username" && f.Type == FieldType.String);
            Assert.Contains(result, f => f.Name == "password" && f.Type == FieldType.String);
            Assert.Equal(2, result.Length);
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.GetFieldDefinitionsForAccount(IStringLocalizer, string, string)"/> returns all POP3-related fields for the Email POP3 subtype.
        /// </summary>
        [Fact]
        public void GetFieldDefinitionsForAccount_EmailPop3_ReturnsAllPop3Fields()
        {
            var localizer = Substitute.For<IStringLocalizer>();
            var result = TestAccountTemplateHelper.GetFieldDefinitionsForAccount(localizer, PrimaryAccountTypes.Email, EmailAccountTypes.Pop3);
            Assert.Contains(result, f => f.Name == "smtp_server");
            Assert.Contains(result, f => f.Name == "smtp_port");
            Assert.Contains(result, f => f.Name == "email");
            Assert.Contains(result, f => f.Name == "password");
            Assert.Contains(result, f => f.Name == "pop_server");
            Assert.Contains(result, f => f.Name == "pop_port");
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.GetFieldDefinitionsForAccount(IStringLocalizer, string, string)"/> returns all IMAP-related fields for the Email IMAP subtype.
        /// </summary>
        [Fact]
        public void GetFieldDefinitionsForAccount_EmailImap_ReturnsAllImapFields()
        {
            var localizer = Substitute.For<IStringLocalizer>();
            var result = TestAccountTemplateHelper.GetFieldDefinitionsForAccount(localizer, PrimaryAccountTypes.Email, EmailAccountTypes.Imap);
            Assert.Contains(result, f => f.Name == "smtp_server");
            Assert.Contains(result, f => f.Name == "smtp_port");
            Assert.Contains(result, f => f.Name == "email");
            Assert.Contains(result, f => f.Name == "password");
            Assert.Contains(result, f => f.Name == "imap_server");
            Assert.Contains(result, f => f.Name == "imap_port");
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.GetFieldDefinitionsForAccount(IStringLocalizer, string, string)"/> returns only email and password fields for the Email "other" subtype.
        /// </summary>
        [Fact]
        public void GetFieldDefinitionsForAccount_EmailOther_ReturnsEmailAndPassword()
        {
            var localizer = Substitute.For<IStringLocalizer>();
            var result = TestAccountTemplateHelper.GetFieldDefinitionsForAccount(localizer, PrimaryAccountTypes.Email, "other");
            Assert.Contains(result, f => f.Name == "email");
            Assert.Contains(result, f => f.Name == "password");
            Assert.Equal(2, result.Length);
        }

        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.Types"/> returns all essential account types.
        /// </summary>
        [Fact]
        public void Types_ReturnsEssetialAccountTypes()
        {
            var result = TestAccountTemplateHelper.Types;
            var expectedTypes = new[]
            {
                "wifi",
                "email",
                "wechat",
                "google",
                "facebook",
                "outlook",
                "server"
            };

            foreach (var type in expectedTypes)
            {
                Assert.Contains(type, result);
            }
        }


        /// <summary>
        /// Verifies that <see cref="TestAccountTemplateHelper.Types"/> returns all primary account types.
        /// </summary>
        [Fact]
        public void Types_ReturnsAllPrimaryAccountTypes()
        {
            var result = TestAccountTemplateHelper.Types;
            var expectedTypes = PrimaryAccountTypes.All;

            foreach (var type in expectedTypes)
            {
                Assert.Contains(type, result);
            }
        }
    }
}