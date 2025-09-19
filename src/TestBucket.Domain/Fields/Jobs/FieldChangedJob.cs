using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using Microsoft.Extensions.Logging;

using Quartz;

using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Jobs;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields.Jobs;

[DisallowConcurrentExecution]
internal class FieldChangedJob : UserJob
{
    private readonly ILogger<FieldChangedJob> _logger;
    private readonly IMediator _mediator;

    public FieldChangedJob(ILogger<FieldChangedJob> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async override Task Execute(IJobExecutionContext context)
    {
        var principal = GetUser(context);

        var fieldType = context.MergedJobDataMap.GetString("FieldType");
        if (fieldType is null)
        {
            _logger.LogError("Error, no FieldType specified in job");
            return;
        }

        switch (fieldType)
        {
            case "TestRunField":
                var testRunField = context.MergedJobDataMap.GetFromJson<TestRunField>("TestRunField");
                await _mediator.Publish(new TestRunFieldChangedNotification(principal, testRunField));
                break;
        }

    }
}
