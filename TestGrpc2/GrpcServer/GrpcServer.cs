using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcServer
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class GrpcServer : StatelessService
    {
        public GrpcServer(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
           {
              new ServiceInstanceListener(serviceContext =>
                  new GrpcCommunicationListener(new [] { Hello.BindService(new HelloImpl(Context)) },
                      serviceContext, ServiceEventSource.Current, "ServiceEndpoint"))
            };
        }
    }
}
