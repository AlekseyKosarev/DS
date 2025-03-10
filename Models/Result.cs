namespace DS.Models
{
    public class Result {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        protected Result(bool isSuccess, string errorMessage) {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new Result(true, null);
        public static Result Failure(string errorMessage) => new Result(false, errorMessage);
    }

    public class Result<T> : Result {
        public T Data { get; }

        private Result(bool isSuccess, T data, string errorMessage)
            : base(isSuccess, errorMessage) {
            Data = data;
        }

        public static Result<T> Success(T data) => new Result<T>(true, data, null);
        public new static Result<T> Failure(string errorMessage) => new Result<T>(false, default, errorMessage);
    }
}