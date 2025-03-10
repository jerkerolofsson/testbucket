using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats.Ctrf.Models;

namespace TestBucket.Formats.Ctrf
{
    public class CtrfXunitSerializer : CtrfSerializer<XunitEnvironmentExtra, XunitTestsExtra>
    {
        protected override void ParseTestExtra(CtrfTest<XunitTestsExtra> test, TestCaseRunDto testDto)
        {
            if(test.Extra is not null)
            {
                if(test.Extra.Attachments is not null)
                {
                    foreach(var attachment in test.Extra.Attachments)
                    {
                        var name = attachment.Key;
                        var value = attachment.Value;
                        if(name is not null && value is not null)
                        {
                            if(value.GetValueKind() == System.Text.Json.JsonValueKind.Object)
                            {
                                // Parse as attachment
                            }
                            else if(value.GetValueKind() == System.Text.Json.JsonValueKind.String)
                            {
                                // Simple key-value
                                testDto.Traits.Add(new TestTrait(name, value.ToString()));
                            }
                        }
                    }
                }
            }
        }
    }
}
