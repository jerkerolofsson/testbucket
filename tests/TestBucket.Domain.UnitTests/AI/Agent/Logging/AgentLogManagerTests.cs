using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using NSubstitute;
using TestBucket.Domain.AI.Agent.Logging;
using TestBucket.Domain.AI.Billing;
using TestBucket.Domain.AI.Settings.LLM;
using TestBucket.Domain.AI.Models;
using OpenAI.Chat;
using Xunit;
using TestBucket.Domain.Settings;
using Microsoft.Extensions.AI;

namespace TestBucket.Domain.UnitTests.AI.Agent.Logging
{
    [Feature("Chat")]
    [Component("AI")]
    [UnitTest]
    [FunctionalTest]
    [EnrichedTest]
    public class AgentLogManagerTests
    {
        private readonly IAIUsageManager _mockUsageManager;
        private readonly ISettingsProvider _mockSettingsProvider;
        private readonly AgentLogManager _agentLogManager;

        public AgentLogManagerTests()
        {
            _mockUsageManager = Substitute.For<IAIUsageManager>();
            _mockSettingsProvider = Substitute.For<ISettingsProvider>();
            _agentLogManager = new AgentLogManager(_mockUsageManager, _mockSettingsProvider);
        }


        [Fact]
        public async Task LogResponseAsync_WithSemanticKernelChatMessageContentAndNoUsageContent_ReturnsNull()
        {
            // Arrange
            var teamId = 1L;
            var projectId = 2L;
            var orchestration = "abc";
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("tenant", "tenant1") }));
            var response = new Microsoft.SemanticKernel.ChatMessageContent()
            {
                Metadata = new Dictionary<string, object?>
                {
                },
            };
             

            _mockSettingsProvider.GetDomainSettingsAsync<LlmSettings>("tenant1", null).Returns(new LlmSettings { LlmModelUsdPerMillionTokens = 0.02 });

            // Act
            var result = await _agentLogManager.LogResponseAsync(teamId, projectId, orchestration, principal, response);

            // Assert
            Assert.Null(result);
            await _mockUsageManager.DidNotReceive().AddCostAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<ChatUsage>());
        }

        [Fact]
        public async Task LogResponseAsync_WithValidUsage_LogsUsageAndReturnsChatUsage()
        {
            // Arrange
            var teamId = 1L;
            var projectId = 2L;
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("tenant", "tenant1") }));
            var response = new ChatResponseUpdate(ChatRole.Assistant, [
                new UsageContent(new UsageDetails()
                { 
                    InputTokenCount = 10,
                    OutputTokenCount = 20,
                    TotalTokenCount = 30
                })
                ])
            {
                ModelId = "test-model"
            };

            _mockSettingsProvider.GetDomainSettingsAsync<LlmSettings>("tenant1", null).Returns(new LlmSettings { LlmModelUsdPerMillionTokens = 0.02 });

            // Act
            var result = await _agentLogManager.LogResponseAsync(teamId, projectId, principal, response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.InputTokenCount);
            Assert.Equal(20, result.OutputTokenCount);
            Assert.Equal(30, result.TotalTokenCount);
            await _mockUsageManager.Received(1).AddCostAsync(principal, Arg.Is<ChatUsage>(u => u.TotalTokenCount == 30));
        }

        [Fact]
        public async Task LogResponseAsync_WithNoUsageContent_ReturnsEmpty()
        {
            // Arrange
            var teamId = 1L;
            var projectId = 2L;
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("tenant", "tenant1") }));
            var response = new ChatResponseUpdate(ChatRole.Assistant, []);

            // Act
            var result = await _agentLogManager.LogResponseAsync(teamId, projectId, principal, response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalTokenCount);
            Assert.Equal(0, result.InputTokenCount);
            Assert.Equal(0, result.OutputTokenCount);
            await _mockUsageManager.DidNotReceive().AddCostAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<ChatUsage>());
        }

        [Fact]
        public async Task LogResponseAsync_WithChatResponseUpdate_AccumulatesUsage()
        {
            // Arrange
            var teamId = 1L;
            var projectId = 2L;
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("tenant", "tenant1") }));
            var response = new ChatResponseUpdate(ChatRole.Assistant, [
                new UsageContent(new UsageDetails()
                {
                    InputTokenCount = 5,
                    OutputTokenCount = 10,
                    TotalTokenCount = 15
                }),
                 new UsageContent(new UsageDetails()
                 {
                    InputTokenCount = 3,
                    OutputTokenCount = 6,
                    TotalTokenCount = 9
                })
              ])
            {
                ModelId = "test-model"
            };


            _mockSettingsProvider.GetDomainSettingsAsync<LlmSettings>("tenant1", null).Returns(new LlmSettings { LlmModelUsdPerMillionTokens = (double)0.02m });

            // Act
            var result = await _agentLogManager.LogResponseAsync(teamId, projectId, principal, response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(8, result.InputTokenCount);
            Assert.Equal(16, result.OutputTokenCount);
            Assert.Equal(24, result.TotalTokenCount);
            await _mockUsageManager.Received(2).AddCostAsync(principal, Arg.Any<ChatUsage>());
        }
    }
}
