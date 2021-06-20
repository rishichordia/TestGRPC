using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Client
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary
    internal sealed class Client : StatelessService
    {
        public Client(StatelessServiceContext context)
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
        //static readonly Random mRand=new Random();
        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.
            /*ServicePartitionResolver partitionResolver = ServicePartitionResolver.("localhost:19000");
            var partition = partitionResolver.ResolveAsync(new Uri("fabric:/TestGRPC/gRPCServer"), ServicePartitionKey.Singleton, new System.Threading.CancellationToken()).Result;
            var endpoint = partition.Endpoints.ElementAt(mRand.Next(0, partition.Endpoints.Count));

            var address = endpoint.Address.Substring(endpoint.Address.IndexOf("\"\":\"") + 4);
            address = address.Substring(0, address.IndexOf("\""));

            Channel channel = new Channel(address, ChannelCredentials.Insecure);
            var client = new AccountService.AccountServiceClient(channel);*/
            ServicePartitionResolver resolver = new ServicePartitionResolver("localhost:19000");
            //var resolver = ServicePartitionResolver.GetDefault();
            var serviceUri = new Uri("fabric:/TestGRPC/gRPCServer");
            var communicationFactory = new GrpcCommunicationClientFactory<AccountService.AccountServiceClient>(null, resolver);

            var partitionClient = new ServicePartitionClient<GrpcCommunicationClient<AccountService.AccountServiceClient>>(communicationFactory, serviceUri, ServicePartitionKey.Singleton);
            ServiceEventSource.Current.ServiceMessage(this.Context, "Getting with employee ID: 1");
            var empName= partitionClient.InvokeWithRetryAsync((communicationClient) => Task.FromResult(communicationClient.Client.GetEmployeeName(new EmployeeNameRequest { EmpId = "1" })));
             if (empName.Result == null || string.IsNullOrWhiteSpace(empName.Result.FirstName) || string.IsNullOrWhiteSpace(empName.Result.LastName))
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Employee not found.");

            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Employee name is {0} {1}", empName.Result.FirstName, empName.Result.LastName);
            }
            long iterations = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
