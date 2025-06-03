namespace TodoList.Result
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public Exception? Exception { get; private set; }
        public T? Value { get; private set; }

        public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };  
        public static Result<T> Fail(string error, Exception? ex = null) => new() {IsSuccess = false , ErrorMessage = error, Exception = ex };
    }
}
