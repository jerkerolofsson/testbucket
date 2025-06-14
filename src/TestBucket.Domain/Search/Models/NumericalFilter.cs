using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Search.Models;
public record class NumericalFilter(FilterOperator Operator, double Value);
