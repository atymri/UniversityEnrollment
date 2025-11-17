using System;
using System.Threading.Tasks;

namespace UniversityEnrollment.Core.Common.Results
{
    // ------------------------
    // Base Result (non-generic)
    // ------------------------
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
                throw new ArgumentException("Invalid error", nameof(error));

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public static Result Success() => new Result(true, Error.None);
        public static Result Failure(Error error) => new Result(false, error);

        public static Result<T> Success<T>(T value) => new Result<T>(value, true, Error.None);
        public static Result<T> Failure<T>(Error error) => new Result<T>(default!, false, error);
    }

    // ------------------------
    // Generic Result<T>
    // ------------------------
    public class Result<T> : Result
    {
        private readonly T _value;

        internal Result(T value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public T Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("Cannot access the value of a failed result.");

        public Result ToResult() => IsSuccess ? Result.Success() : Result.Failure(Error);
    }           
}
