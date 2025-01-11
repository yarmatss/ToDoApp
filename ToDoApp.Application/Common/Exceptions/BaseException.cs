namespace ToDoApp.Application.Common.Exceptions;

public abstract class BaseException : Exception
{
    public virtual int StatusCode { get; }

    protected BaseException(string message) : base(message) { }
}