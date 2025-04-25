using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Sdk.Client.Exceptions;
public class CreationException : TestBucketBaseException
{
    public CreationException()
    {
    }

    public CreationException(string? message) : base(message)
    {
    }

    public CreationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
