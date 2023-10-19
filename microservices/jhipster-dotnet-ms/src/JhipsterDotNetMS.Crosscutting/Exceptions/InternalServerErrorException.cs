using JhipsterDotNetMS.Crosscutting.Constants;

namespace JhipsterDotNetMS.Crosscutting.Exceptions;

public class InternalServerErrorException : BaseException
{
    public InternalServerErrorException(string message) : base(ErrorConstants.DefaultType, message)
    {
    }
}
