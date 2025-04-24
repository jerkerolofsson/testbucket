using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.TestResources;

namespace TestBucket.Domain.UnitTests.TestResources
{
    [EnrichedTest]
    [UnitTest]
    public class TestCaseDependencyMergerTests
    {
        [Component("TestResources")]
        [Fact]
        public void MergeTwoLists_WithBothThatHasAttributeRequirements_ResultAttributeContains()
        {
            // Input each has one resource, the result should be merged into a single one
            List<TestCaseDependency> a = [
                    new TestCaseDependency
                    {
                        ResourceType = "phone",
                        AttributeRequirements =
                        [
                            new AttributeRequirement() { Name = "model", Operator = AttributeOperator.Equals, Value = "Pixel 8 Pro"}
                        ]
                    }
                ];
            List<TestCaseDependency> b = [
                    new TestCaseDependency
                    {
                        ResourceType = "phone",
                        AttributeRequirements =
                        [
                            new AttributeRequirement() { Name = "manufacturer", Operator = AttributeOperator.Equals, Value = "Google"}
                        ]
                    }
                ];

            var result = TestCaseDependencyMerger.Merge(a, b);
            Assert.Single(result);
            Assert.Equal("phone", result[0].ResourceType);
            Assert.NotNull(result[0].AttributeRequirements);
            Assert.Equal(2, result[0].AttributeRequirements!.Count);
            Assert.Equal("model", result[0].AttributeRequirements![0].Name);
            Assert.Equal("Pixel 8 Pro", result[0].AttributeRequirements![0].Value);
            Assert.Equal(AttributeOperator.Equals, result[0].AttributeRequirements![0].Operator);

            Assert.Equal("manufacturer", result[0].AttributeRequirements![1].Name);
            Assert.Equal("Google", result[0].AttributeRequirements![1].Value);
            Assert.Equal(AttributeOperator.Equals, result[0].AttributeRequirements![1].Operator);
        }

        [Component("TestResources")]
        [Fact]
        public void MergeTwoLists_WithOneThatHasAttributeRequirements_ResultAttributeContains()
        {
            // Input each has one resource, the result should be merged into a single one
            List<TestCaseDependency> a = [
                    new TestCaseDependency 
                    { 
                        ResourceType = "phone", 
                        AttributeRequirements = 
                        [ 
                            new AttributeRequirement() { Name = "model", Operator = AttributeOperator.Equals, Value = "Pixel 8 Pro"} 
                        ] 
                    }
                ];
            List<TestCaseDependency> b = [new TestCaseDependency { ResourceType = "phone" }];

            var result = TestCaseDependencyMerger.Merge(a, b);
            Assert.Single(result);
            Assert.Equal("phone", result[0].ResourceType);
            Assert.NotNull(result[0].AttributeRequirements);
            Assert.Single(result[0].AttributeRequirements!);
            Assert.Equal("model", result[0].AttributeRequirements![0].Name);
            Assert.Equal("Pixel 8 Pro", result[0].AttributeRequirements![0].Value);
            Assert.Equal(AttributeOperator.Equals, result[0].AttributeRequirements![0].Operator);
        }

        [Component("TestResources")]
        [Fact]
        public void MergeTwoLists_WithSameNumberOfResources_ResourceCountCorrect()
        {
            // Input each has one resource, the result should be merged into a single one
            List<TestCaseDependency> a = [new TestCaseDependency { ResourceType = "phone" }];
            List<TestCaseDependency> b = [new TestCaseDependency { ResourceType = "phone" }];

            var result = TestCaseDependencyMerger.Merge(a, b);
            Assert.Single(result);
            Assert.Equal("phone", result[0].ResourceType);
        }

        [Component("TestResources")]
        [Fact]
        public void MergeTwoLists_WithDifferentNumberOfResources_ResourceCountCorrect()
        {
            // a requires one phone, b requires two. The result should be two
            List<TestCaseDependency> a = [new TestCaseDependency { ResourceType = "phone" }];
            List<TestCaseDependency> b = [new TestCaseDependency { ResourceType = "phone" }, new TestCaseDependency { ResourceType = "phone" }];

            var result = TestCaseDependencyMerger.Merge(a, b);
            Assert.Equal(2, result.Count);
            Assert.Equal("phone", result[0].ResourceType);
            Assert.Equal("phone", result[1].ResourceType);
        }
    }
}
