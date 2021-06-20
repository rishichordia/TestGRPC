﻿using System;
using System.Threading.Tasks;
using Grpc.Core;


namespace gRPCServer
{
    class AccountsImpl : AccountService.AccountServiceBase
    {
        public AccountsImpl()
        {
        }
        // Server side handler of the GetEmployeeName RPC
        public override Task<EmployeeName> GetEmployeeName(EmployeeNameRequest request, ServerCallContext context)
        {
            EmployeeData empData = new EmployeeData();
            return Task.FromResult(empData.GetEmployeeName(request));
        }
    }
}
