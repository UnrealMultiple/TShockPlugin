namespace Economics.Core.Model;

public abstract class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !this.IsSuccess;
    public string? Error { get; protected set; }

    public static Result<T> Success<T>(T value)
    {
        return new(value);
    }

    public static Result<T> Failure<T>(string error)
    {
        return new(error);
    }

    public static Result Success()
    {
        return new SuccessResult();
    }

    public static Result Failure(string error)
    {
        return new FailureResult(error);
    }
}

public class Result<T> : Result
{
    public T? Value { get; }

    public Result(T value)
    {
        this.Value = value;
        this.IsSuccess = true;
    }

    public Result(string error)
    {
        this.Error = error;
        this.IsSuccess = false;
    }

    public T GetValueOrThrow()
    {
        return this.IsFailure ? throw new InvalidOperationException(this.Error) : this.Value!;
    }

    public static implicit operator Result<T>(T value) => new(value);
}

public class SuccessResult : Result
{
    public SuccessResult()
    {
        this.IsSuccess = true;
    }
}

public class FailureResult : Result
{
    public FailureResult(string error)
    {
        this.Error = error;
        this.IsSuccess = false;
    }
}

public readonly struct DeductResult
{
    public bool IsSuccess { get; }
    public long BalanceAfter { get; }

    private DeductResult(bool isSuccess, long balanceAfter)
    {
        this.IsSuccess = isSuccess;
        this.BalanceAfter = balanceAfter;
    }

    public static DeductResult Success(long balanceAfter)
    {
        return new(true, balanceAfter);
    }

    public static DeductResult Failure(long currentBalance)
    {
        return new(false, currentBalance);
    }
}
