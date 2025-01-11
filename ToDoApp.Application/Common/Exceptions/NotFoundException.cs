namespace ToDoApp.Application.Common.Exceptions;

public class NotFoundException : BaseException
{
    public override int StatusCode => 404;

    public NotFoundException(string message) : base(message) { }
}