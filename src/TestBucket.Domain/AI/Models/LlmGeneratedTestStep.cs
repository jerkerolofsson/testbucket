﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Models;
public record class GeneratedTestStep
{
    public string? Action { get; set; }
    public string? ExpectedResult { get; set; }
}
