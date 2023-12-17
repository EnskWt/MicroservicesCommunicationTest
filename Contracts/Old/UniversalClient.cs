using Contracts.ApiContracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Old
{
    public class UniversalClient<T> where T : IClient
    {
        private readonly HttpClient _httpClient;

        public UniversalClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResult?> InvokeMethod<TResult>(Expression<Func<T, Task<TResult>>> expression)
        {
            var methodCall = (MethodCallExpression)expression.Body;
            var interfaceMethod = methodCall.Method;

            var httpMethodAttribute = interfaceMethod.GetCustomAttributes(typeof(HttpMethodAttribute), false).FirstOrDefault() as HttpMethodAttribute;
            var routeAttribute = interfaceMethod.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

            if (httpMethodAttribute == null || routeAttribute == null)
            {
                throw new InvalidOperationException("The method must have HttpMethod and Route attributes.");
            }

            var httpMethod = httpMethodAttribute.HttpMethods.FirstOrDefault();
            var route = routeAttribute.Template;

            var response = await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), route));
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(jsonString);
        }
    }
}
