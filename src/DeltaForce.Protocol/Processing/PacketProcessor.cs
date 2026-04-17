using System.Collections.Concurrent;
using System.Reflection;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Processing;

public class PacketProcessor
{
    private readonly ConcurrentDictionary<byte, Func<INetPacket, INetPacket?>> _handlers = new();

    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<IResponsePacket>> _pendingRequests = new();

    public void Register<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        where TRequest : IRequestPacket
        where TResponse : IResponsePacket
    {
        var instance = Activator.CreateInstance<TRequest>();
        var packetId = (byte)instance.PacketID;

        _handlers[packetId] = packet =>
        {
            if (packet is TRequest request)
            {
                var response = handler(request);
                response.RequestId = request.RequestId;
                return response;
            }
            return null;
        };

        Console.WriteLine($"[PacketProcessor] Registered handler for {typeof(TRequest).Name}");
    }

    public void Register<TRequest, TResponse>(RequestHandlerBase<TRequest, TResponse> handler)
        where TRequest : IRequestPacket
        where TResponse : IResponsePacket, new()
    {
        var instance = Activator.CreateInstance<TRequest>();
        var packetId = (byte)instance.PacketID;

        _handlers[packetId] = packet =>
        {
            if (packet is TRequest request)
            {
                var response = handler.Handle(request);
                response.RequestId = request.RequestId;
                return response;
            }
            return null;
        };

        Console.WriteLine($"[PacketProcessor] Registered handler for {typeof(TRequest).Name}");
    }

    public void RegisterHandlersFromAssembly(Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.BaseType != null &&
                t.BaseType.IsGenericType &&
                t.BaseType.GetGenericTypeDefinition() == typeof(RequestHandlerBase<,>));

        foreach (var handlerType in handlerTypes)
        {
            var baseType = handlerType.BaseType!;
            var genericArgs = baseType.GetGenericArguments();
            var requestType = genericArgs[0];
            var responseType = genericArgs[1];

            var handlerInstance = Activator.CreateInstance(handlerType)!;

            var method = typeof(PacketProcessor)
                .GetMethods()
                .Where(m => m.Name == nameof(Register) && m.IsGenericMethod)
                .First(m =>
                {
                    var parameters = m.GetParameters();
                    return parameters.Length == 1 &&
                           parameters[0].ParameterType.IsGenericType &&
                           parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(RequestHandlerBase<,>);
                });

            if (method != null)
            {
                method.MakeGenericMethod(requestType, responseType).Invoke(this, new[] { handlerInstance });
                Console.WriteLine($"[PacketProcessor] Auto-registered: {handlerType.Name}");
            }
        }
    }

    public INetPacket? Process(INetPacket packet)
    {
        if (packet is IResponsePacket response)
        {
            if (_pendingRequests.TryRemove(response.RequestId, out var tcs))
            {
                tcs.TrySetResult(response);
                return null;
            }
        }

        var packetId = (byte)packet.PacketID;
        if (_handlers.TryGetValue(packetId, out var handler))
        {
            return handler(packet);
        }

        return null;
    }

    public async Task<TResponse?> RequestAsync<TRequest, TResponse>(
        TRequest request,
        Func<INetPacket, Task> sendAsync,
        int timeoutMs = 5000)
        where TRequest : IRequestPacket
        where TResponse : class, IResponsePacket
    {
        var tcs = new TaskCompletionSource<IResponsePacket>();
        _pendingRequests[request.RequestId] = tcs;

        try
        {
            await sendAsync(request);

            using var cts = new CancellationTokenSource(timeoutMs);
            await using (cts.Token.Register(() => tcs.TrySetCanceled()))
            {
                var result = await tcs.Task;
                return result as TResponse;
            }
        }
        catch (OperationCanceledException)
        {
            _pendingRequests.TryRemove(request.RequestId, out _);
            return null;
        }
    }

    public static TResponse CreateResponse<TRequest, TResponse>(TRequest request, bool success = true, string message = "")
        where TRequest : IRequestPacket
        where TResponse : IResponsePacket, new()
    {
        return new TResponse
        {
            RequestId = request.RequestId,
            Success = success,
            Message = message
        };
    }
}
