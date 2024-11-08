using System;
using FluentResults;

namespace Pages.Utils.FluentResults;

public class ArgumentError(string message) : ExceptionalError(message, new ArgumentException(message));

public class RecordNotFoundError(string message) : Error(message);

public class GatewayError(int statusCode, string message) : Error(message)
{
    public int StatusCode { get; } = statusCode;
}