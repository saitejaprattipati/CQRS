using System.Threading.Tasks;
using System;
using Grpc.Core;
using Serilog;
using Grpc.Net.Client;

namespace Author.Query.GRPC.Client.Services
{
    public class GrpcCallerService
    {
        public static async Task<TResponse> CallService<TResponse>(string urlGrpc, Func<GrpcChannel, Task<TResponse>> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var channel = GrpcChannel.ForAddress(urlGrpc);
           // var channel = GrpcChannel.ForAddress("http://localhost:32769");            
            /*
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            */


            Log.Information("Creating grpc client base address urlGrpc ={@urlGrpc}, BaseAddress={@BaseAddress} ", urlGrpc, channel.Target);

            try
            {
                return await func(channel);
            }
            catch (RpcException e)
            {
                Log.Error("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
                return default;
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }


        }

        public static async Task CallService(string urlGrpc, Func<GrpcChannel, Task> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            /*
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };
            */

            var channel = GrpcChannel.ForAddress(urlGrpc);

            Log.Debug("Creating grpc client base address {@httpClient.BaseAddress} ", channel.Target);

            try
            {
                await func(channel);
            }
            catch (RpcException e)
            {
                Log.Error("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }
        }
    }
}
