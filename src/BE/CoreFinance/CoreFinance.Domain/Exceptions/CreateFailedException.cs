using System;
using System.Runtime.Serialization;

namespace CoreFinance.Domain.Exceptions;

[Serializable]
public class CreateFailedException : Exception
{
    public CreateFailedException()
    {
    }

    public CreateFailedException(string message) : base(message)
    {
    }

    public CreateFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected CreateFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
} 