using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace Pages.Utils.FluentResults;

public static class FluentResultsExtensions
{
    private static string CombineErrorMessages(this IList<IError> errors)
        => string.Join(", ", errors.Select(s => s.Message));

    private static Exception? GetFirstException(this IList<IError> errors)
        => errors.OfType<ExceptionalError>().FirstOrDefault()?.Exception;

    public static void ThrowOnError<T>(this Result<T> result)
        => result.ToResult().ThrowOnError();

    public static void ThrowOnError(this Result result)
    {
        if (result.IsSuccess) return;

        var firstException = result.Errors.GetFirstException();
        if (firstException != null)
            throw firstException;

        if (result.HasError<ArgumentError>())
            throw new ArgumentException(result.Errors.CombineErrorMessages());

        throw new Exception(result.Errors.CombineErrorMessages());
    }

    public static void InvokeOnError<T>(this Result<T> result, Action<string> errorAction)
    {
        if (result.IsSuccess) return;
        errorAction(result.Errors.CombineErrorMessages());
    }

    public static void InvokeOnError(this Result result, Action<string> errorAction)
    {
        if (result.IsSuccess) return;
        errorAction(result.Errors.CombineErrorMessages());
    }

    public static void ThrowOnSpecificError<TError>(this Result result) where TError : IError
    {
        if (result.IsSuccess) return;
        if (!result.HasError<TError>(out var errors)) return;

        var firstException = errors.OfType<IError>().ToList().GetFirstException();
        if (firstException != null)
            throw firstException;
    }

    public static void ThrowOnSpecificError<T, TError>(this Result<T> result) where TError : IError =>
        result.ToResult().ThrowOnSpecificError<TError>();
}