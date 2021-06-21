using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace GrpcClient
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class GrpcClient : StatelessService
    {
        public GrpcClient(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.
            Environment.SetEnvironmentVariable("GRPC_DNS_RESOLVER", "native");
            long iterations = 0;
            var resolver = ServicePartitionResolver.GetDefault();
            string s = FabricRuntime.GetActivationContext().ApplicationName;
            var serviceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/GrpcServer");
            var communicationFactory = new GrpcCommunicationClientFactory<Hello.HelloClient>(null, resolver);

            var partitionClient = new ServicePartitionClient<GrpcCommunicationClient<Hello.HelloClient>>(communicationFactory, serviceUri, ServicePartitionKey.Singleton);

            while (!cancellationToken.IsCancellationRequested)
            {
                var reply = partitionClient.InvokeWithRetryAsync((communicationClient) => Task.FromResult(communicationClient.Client.SayHello(new HelloRequest { Name = $"{++iterations}" })));

                ServiceEventSource.Current.ServiceMessage(this.Context, "Client Received: {0}", reply.Result.Message);

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}
