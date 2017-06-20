namespace KinesisNet.Model
{
    public class Result
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public static Result Create(bool success, string message)
        {
            return new Result(success, message);
        }

        private Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
