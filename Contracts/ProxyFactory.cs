using Contracts.ApiContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class ProxyFactory<T> where T : IClient
    {
        public static T Create(HttpClient httpClient)
        {
            object proxy = DispatchProxy.Create<T, Proxy<T>>();
            ((Proxy<T>)proxy).SetHttpClient(httpClient);

            return (T)proxy;
        }
    }
}
