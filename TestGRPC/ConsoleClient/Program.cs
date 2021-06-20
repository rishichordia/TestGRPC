using Grpc.Core;
using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Linq;

namespace ConsoleClient
{
    class Program
    {
        static Random mRand = new Random();
        static void Main(string[] args)
        {
            ServicePartitionResolver partitionResolver = new ServicePartitionResolver("localhost:19000");
            var partition = partitionResolver.ResolveAsync(new Uri("fabric:/gRPCApplication/gRPCServer"),
                ServicePartitionKey.Singleton, new System.Threading.CancellationToken()).Result;
            var endpoint = partition.Endpoints.ElementAt(mRand.Next(0, partition.Endpoints.Count));

            var address = endpoint.Address.Substring(endpoint.Address.IndexOf("\"\":\"") + 4);
            address = address.Substring(0, address.IndexOf("\""));

            Channel channel = new Channel(address, ChannelCredentials.Insecure);
            var client=new AccountService.AccountServiceClient(channel);
            EmployeeName empName = client.GetEmployeeName(new EmployeeNameRequest { EmpId = "1" });
            if (empName == null || string.IsNullOrWhiteSpace(empName.FirstName) || string.IsNullOrWhiteSpace(empName.LastName))
            {
                Console.WriteLine("Employee not found.");
            }
            else
            {
                Console.WriteLine($"The employee name is {empName.FirstName} {empName.LastName}.");
            }
        }
    }
}
