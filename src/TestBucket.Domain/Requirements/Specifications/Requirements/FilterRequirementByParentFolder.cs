﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications.Requirements
{
    public class FilterRequirementByParentFolder : FilterSpecification<Requirement>
    {
        private readonly long? _id;

        public FilterRequirementByParentFolder(long? id)
        {
            _id = id;
        }

        protected override Expression<Func<Requirement, bool>> GetExpression()
        {
            return x => x.RequirementSpecificationFolderId == _id;
        }
    }
}
