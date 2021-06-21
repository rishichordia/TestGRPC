using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcServer
{
    class HelloImpl: Hello.HelloBase
    { 
        private readonly StatelessServiceContext _context;
        public HelloImpl(StatelessServiceContext context)
        {
            _context = context;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "Server Received: {0}", request.Name);
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
    }
}
