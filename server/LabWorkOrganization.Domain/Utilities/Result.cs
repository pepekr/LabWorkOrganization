namespace LabWorkOrganization.Domain.Utilities
{
    public class Result<T>
    {
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
        public T? Data { get; init; }

        public static Result<T> Success(T data)
        {
            return new Result<T> { IsSuccess = true, Data = data };
        }
        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T> { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}
