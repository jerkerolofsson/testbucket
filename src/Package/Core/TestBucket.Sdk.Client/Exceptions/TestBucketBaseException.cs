using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Sdk.Client.Exceptions;
public class TestBucketBaseException : Exception
{
    public TestBucketBaseException()
    {
    }

    public TestBucketBaseException(string? message) : base(message)
    {
    }

    public TestBucketBaseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
