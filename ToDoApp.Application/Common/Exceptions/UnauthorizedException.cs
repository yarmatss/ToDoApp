namespace ToDoApp.Application.Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public override int StatusCode => 401;

    public UnauthorizedException(string message) : base(message) { }
}
