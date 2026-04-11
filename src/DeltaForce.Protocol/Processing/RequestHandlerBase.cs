using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Processing;

public abstract class RequestHandlerBase<TRequest, TResponse>
    where TRequest : IRequestPacket
    where TResponse : IResponsePacket, new()
{
    public abstract TResponse Handle(TRequest request);

    protected static TResponse CreateSuccessResponse(TRequest request, string message = "Success")
    {
        return new TResponse
        {
            RequestId = request.RequestId,
            Success = true,
            Message = message
        };
    }

    protected static TResponse CreateFailureResponse(TRequest request, string message)
    {
        return new TResponse
        {
            RequestId = request.RequestId,
            Success = false,
            Message = message
        };
    }
}
