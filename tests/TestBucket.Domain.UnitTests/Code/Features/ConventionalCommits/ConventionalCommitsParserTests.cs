using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Code.Features.ConventionalCommits;
using TestBucket.Domain.Code.Features.ConventionalCommits.Parser;

namespace TestBucket.Domain.UnitTests.Code.Features.ConventionalCommits
{
    /// <summary>
    /// Convential commit parsing tests
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    public class ConventionalCommitsParserTests
    {
        /// <summary>
        /// Verifies that the parsed commit message contains the complete message as there is no type or footer
        /// 
        /// ```
        /// This is a commit message without a type or footer.
        /// ```
        /// </summary>
        [Fact]
        public void Parse_SingleLineWithNoType_NoTypeReturnedAllIsMessage()
        {
            var message = "This is a commit message without a type or footer.";

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.Equal(message, conventionalMessage.Description);
        }

        /// <summary>
        /// Verifies that breaking change is false when there is no breaking change token
        /// 
        /// ```
        /// This is a commit message without a type or footer.
        /// ```
        /// </summary>
        [Fact]
        public void Parse_SingleLineWithNoType_BreakingChangeIsFalse()
        {
            var message = "This is a commit message without a type or footer.";

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.False(conventionalMessage.IsBreakingChange);
        }

        /// <summary>
        /// Verifies that breaking change is true when there is type followed by exclamation point
        /// 
        /// ```
        /// feat!: send an email to the customer when a product is shipped
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-10")]
        public void Parse_WithExclamationPointForType_BreakingChangeIsTrue()
        {
            var message = """
                feat!: send an email to the customer when a product is shipped
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.True(conventionalMessage.IsBreakingChange);
        }


        /// <summary>
        /// Verifies that breaking change is true when there is type with scope followed by exclamation point
        /// 
        /// ```
        /// feat(api)!: send an email to the customer when a product is shipped
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-10")]
        public void Parse_WithExclamationPointForTypeWithScope_BreakingChangeIsTrue()
        {
            var message = """
                feat(api)!: send an email to the customer when a product is shipped
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.True(conventionalMessage.IsBreakingChange);
        }


        /// <summary>
        /// Verifies that breaking change is true when there is breaking change in the footer
        /// 
        /// ```
        /// feat: allow provided config object to extend other configs
        /// 
        /// Longer description here
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-5")]
        public void Parse_WithLongerDescriptionNoFooter_LongerDescriptionCorrect()
        {
            var message = """
                feat: allow provided config object to extend other configs

                Longer description here
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);
            Assert.Equal("Longer description here", conventionalMessage.LongerDescription);
        }

        /// <summary>
        /// Verifies that breaking change is true when there is breaking change in the footer
        /// 
        /// ```
        /// feat: allow provided config object to extend other configs
        /// 
        /// BREAKING CHANGE: `extends` key in config file is now used for extending other config files
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-9")]
        public void Parse_WithBreakingChangeFooterAndNoDescription_BreakingChangeIsTrue()
        {
            var message = """
                feat: allow provided config object to extend other configs

                BREAKING CHANGE: `extends` key in config file is now used for extending other config files
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.True(conventionalMessage.IsBreakingChange);
        }


        /// <summary>
        /// Verifies that BREAKING-CHANGE is synonymous with BREAKING CHANGE
        /// 
        /// ```
        /// feat: allow provided config object to extend other configs
        /// 
        /// BREAKING-CHANGE: `extends` key in config file is now used for extending other config files
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-13")]
        public void Parse_WithBreakingChangeWithHyphenFooterAndNoDescription_BreakingChangeIsTrue()
        {
            var message = """
                feat: allow provided config object to extend other configs

                BREAKING-CHANGE: `extends` key in config file is now used for extending other config files
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.True(conventionalMessage.IsBreakingChange);
        }



        /// <summary>
        /// Verifies that breaking change is true when there is breaking change in the footer and there is a longer description
        /// on a single line
        /// 
        /// ```
        /// feat: allow provided config object to extend other configs
        /// 
        /// Longer description
        /// 
        /// BREAKING CHANGE: `extends` key in config file is now used for extending other config files
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-13")]
        public void Parse_WithBreakingChangeFooterAndSingleLineDescription_BreakingChangeIsTrue()
        {
            var message = """
                feat: allow provided config object to extend other configs

                Longer description

                BREAKING CHANGE: `extends` key in config file is now used for extending other config files
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.True(conventionalMessage.IsBreakingChange);
        }

        /// <summary>
        /// Verifies that breaking change is true when there is breaking change in the footer and a type with exclamation mark
        /// 
        /// ```
        /// feat!: allow provided config object to extend other configs
        /// 
        /// BREAKING CHANGE: `extends` key in config file is now used for extending other config files
        /// ```
        /// </summary>
        [Fact]
        public void Parse_WithBothBreakingChangeFooterAndExclamationPoint_BreakingChangeIsTrue()
        {
            var message = """
                feat!: allow provided config object to extend other configs

                BREAKING CHANGE: `extends` key in config file is now used for extending other config files
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.True(conventionalMessage.IsBreakingChange);
        }

        /// <summary>
        /// Verifies that the parsed commit message contains the type "fix" when there is no scope defined
        /// 
        /// ```
        /// fix: this is a commit message without a type or footer.
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-3")]
        public void Parse_SingleLineWithFix_FixItemAdded()
        {
            var message = "fix: this is a commit message without a type or footer.";

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.Single(conventionalMessage.Types);
            Assert.Equal("fix", conventionalMessage.Types[0].Type);
            Assert.Null(conventionalMessage.Types[0].Scope);
        }

        /// <summary>
        /// Verifies that the parsed commit message with a type and scope but not body contains
        /// the type, scope and description when the scope is a numeric value
        /// 
        /// ```
        /// feat(123): add Swedish language
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-3")]
        public void Parse_WithNumericScope_ScopeParsedCorrectly()
        {
            var message = "feat(123): add Swedish language";

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.Equal("add Swedish language", conventionalMessage.Description);
            Assert.Single(conventionalMessage.Types);
            Assert.Equal("feat", conventionalMessage.Types[0].Type);
            Assert.Equal("123", conventionalMessage.Types[0].Scope);
        }

        /// <summary>
        /// Verifies that the parsed commit message with a type and scope but not body contains
        /// the type, scope and description when the scope is a string value
        /// 
        /// ```
        /// feat(lang): add Swedish language
        /// ```
        /// </summary>        
        [Fact]
        [CoveredRequirement("CC-3")]
        public void Parse_WithNoBody_TypeParsedCorrectly()
        {
            var message = "feat(lang): add Swedish language";

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.Equal("add Swedish language", conventionalMessage.Description);
            Assert.Single(conventionalMessage.Types);
            Assert.Equal("feat", conventionalMessage.Types[0].Type);
            Assert.Equal("lang", conventionalMessage.Types[0].Scope);
        }

        /// <summary>
        /// Verifies breaking change can be identified when there are multiple footers including one BREAKING CHANGE
        /// 
        /// ```
        /// fix: prevent racing of requests
        /// 
        /// Introduce a request id and a reference to latest request. Dismiss
        /// incoming responses other than from latest request.
        /// 
        /// Remove timeouts which were used to mitigate the racing issue but are
        /// obsolete now.
        /// 
        /// Reviewed-by: Z
        /// BREAKING CHANGE: ABC
        /// Refs: #123
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-7")]
        public void Parse_WithMultipleFootersAndBreakingChange_FootersParsedCorrectly()
        {
            var message = """
                fix: prevent racing of requests

                Introduce a request id and a reference to latest request. Dismiss
                incoming responses other than from latest request.

                Remove timeouts which were used to mitigate the racing issue but are
                obsolete now.

                Reviewed-by: Z
                BREAKING CHANGE: ABC
                Refs: #123
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.Equal("prevent racing of requests", conventionalMessage.Description);
            Assert.True(conventionalMessage.IsBreakingChange);

            Assert.NotNull(conventionalMessage.LongerDescription);
            Assert.Single(conventionalMessage.Types);
            Assert.Equal(3, conventionalMessage.Footer.Count);
            Assert.Equal("fix", conventionalMessage.Types[0].Type);

            Assert.Equal("Reviewed-by", conventionalMessage.Footer[0].Type);
            Assert.Equal("Z", conventionalMessage.Footer[0].Description);

            Assert.Equal("BREAKING CHANGE", conventionalMessage.Footer[1].Type);
            Assert.Equal("ABC", conventionalMessage.Footer[1].Description);

            Assert.Equal("Refs", conventionalMessage.Footer[2].Type);
            Assert.Equal("#123", conventionalMessage.Footer[2].Description);
        }

        /// <summary>
        /// Verifies that multiple footers can be parsed correctly. This checks the type and scope.
        /// 
        /// ```
        /// fix: prevent racing of requests
        /// 
        /// Introduce a request id and a reference to latest request. Dismiss
        /// incoming responses other than from latest request.
        /// 
        /// Remove timeouts which were used to mitigate the racing issue but are
        /// obsolete now.
        /// 
        /// Reviewed-by: Z
        /// Refs: #123
        /// ```
        /// 
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-7")]
        public void Parse_WithMultipleFooters_FootersParsedCorrectly()
        {
            var message = """
                fix: prevent racing of requests

                Introduce a request id and a reference to latest request. Dismiss
                incoming responses other than from latest request.

                Remove timeouts which were used to mitigate the racing issue but are
                obsolete now.

                Reviewed-by: Z
                Refs: #123
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.Equal("prevent racing of requests", conventionalMessage.Description);
            Assert.NotNull(conventionalMessage.LongerDescription);
            Assert.Single(conventionalMessage.Types);
            Assert.Equal(2, conventionalMessage.Footer.Count);
            Assert.Equal("fix", conventionalMessage.Types[0].Type);

            Assert.Equal("Reviewed-by", conventionalMessage.Footer[0].Type);
            Assert.Equal("Z", conventionalMessage.Footer[0].Description);
            Assert.Equal("Refs", conventionalMessage.Footer[1].Type);
            Assert.Equal("#123", conventionalMessage.Footer[1].Description);
        }

        /// <summary>
        /// Verifies that a footer can contain white spaces and new lines
        /// 
        /// ```
        /// fix: prevent racing of requests
        /// 
        /// Custom-footer-with-new-line: Hello
        /// World
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-8")]
        public void Parse_WithFooterValueNewLine_FooterValueParsedCorrectly()
        {
            var message = """
                fix: prevent racing of requests

                Custom-footer-with-new-line: Hello
                World
                """;

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.Equal("Custom-footer-with-new-line", conventionalMessage.Footer[0].Type);
            Assert.Equal("Hello\nWorld", conventionalMessage.Footer[0].Description);
        }

        /// <summary>
        /// Verifies that a types other than fix and feat are supported
        /// 
        /// ```
        /// xyz: Value
        /// ```
        /// </summary>
        [Fact]
        [CoveredRequirement("CC-11")]
        public void Parse_WithTypeExtensions_ScopeParsedCorrectly()
        {
            var message = "xyz: Value";

            // Act
            var conventionalMessage = new ConventionalCommitParser().Parse(message);

            Assert.Equal("Value", conventionalMessage.Description);
            Assert.Single(conventionalMessage.Types);
            Assert.Equal("xyz", conventionalMessage.Types[0].Type);
            Assert.Equal("Value", conventionalMessage.Types[0].Description);
        }
    }
}
