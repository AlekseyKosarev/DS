namespace _Project.System.DS.Models
{
    public class Result
    {
        protected Result(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        public static Result Success()
        {
            return new Result(true, null);
        }

        public static Result Failure(string errorMessage)
        {
            return new Result(false, errorMessage);
        }
    }

    public class Result<T> : Result
    {
        private Result(bool isSuccess, T data, string errorMessage)
            : base(isSuccess, errorMessage)
        {
            Data = data;
        }

        public T Data { get; }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, null);
        }

        public new static Result<T> Failure(string errorMessage)
        {
            return new Result<T>(false, default, errorMessage);
        }
    }
}