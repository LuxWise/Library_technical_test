namespace Library.Exeptions
{
    public abstract class AppException : Exception
    {
        public int Status { get; }
        public string Code { get; }

        protected AppException(int status, string code, string? message=null) : base(message)
        {
            Status = status;
            Code = code;
        }
    }
    
    public sealed class NotFoundException     : AppException { public NotFoundException(string m) : base(404,"not_found",m) {} }
    public sealed class ConflictException     : AppException { public ConflictException(string m) : base(409,"conflict",m) {} }
    public sealed class UnauthorizedAppException : AppException { public UnauthorizedAppException(string m="Unauthorized") : base(401,"unauthorized",m) {} }
    public sealed class ForbiddenException    : AppException { public ForbiddenException(string m="Forbidden") : base(403,"forbidden",m) {} }

    public sealed class ValidationAppException : AppException
    {
        public Dictionary<string, string[]> Errors { get; }
        public ValidationAppException(Dictionary<string,string[]> errors, string m="Validation failed")
            : base(400, "validation_error", m) => Errors = errors;
    }
}
