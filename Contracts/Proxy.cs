using Contracts.ApiContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;

namespace Contracts
{
    public class Proxy<T> : DispatchProxy where T : IClient
    {
        private HttpClient? _httpClient;

        // TODO: divide into multiple methods
        // TODO: add support for GET request query parameters
        protected override object Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            var httpMethodAttribute = targetMethod!.GetCustomAttributes(typeof(HttpMethodAttribute), false).FirstOrDefault() as HttpMethodAttribute;

            if (httpMethodAttribute == null)
            {
                throw new InvalidOperationException("The method must have an HttpMethod attribute.");
            }

            var httpMethod = httpMethodAttribute.HttpMethods.FirstOrDefault();
            var route = httpMethodAttribute.Template;

            // *** maybe use it for query parameters generation ***
            //var parameters = targetMethod.GetParameters()
            //    .Select((param, index) => new KeyValuePair<string, object>(param.Name, args[index]))
            //    .ToDictionary(pair => pair.Key, pair => pair.Value);

            //var json = JsonConvert.SerializeObject(parameters);

            // TODO: change for GET requests
            if (args?.Length != 1 || args[0] == null)
            {
                throw new InvalidOperationException("The method must have a single argument.");
            }

            // converts the argument object to json for post request
            var json = JsonConvert.SerializeObject(args[0]);

            // TODO: change for GET requests to query not body
            var request = new HttpRequestMessage(new HttpMethod(httpMethod!), route)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = _httpClient!.SendAsync(request).Result;

            //***********To handle answer from microservice***********
            var message = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(message);
            }


            response.EnsureSuccessStatusCode();

            //var response = _httpClient!.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod!), requestUri)).Result;
            //response.EnsureSuccessStatusCode();

            var jsonString = response.Content.ReadAsStringAsync().Result;
            var resultType = targetMethod.ReturnType.GetGenericArguments()[0];

            // *********** For ActionResult<T> ***********
            var returnTypeIsActionResult = resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ActionResult<>);
            if (returnTypeIsActionResult)
            {
                resultType = resultType.GetGenericArguments()[0];
            }

            var result = JsonConvert.DeserializeObject(jsonString, resultType);

            //TODO: check and validate result

            // *********** For ActionResult<T> ***********
            if (returnTypeIsActionResult)
            {
                dynamic actionResult = Activator.CreateInstance(typeof(ActionResult<>).MakeGenericType(resultType), result)!;

                dynamic taskCompletionSourceWithActionResult = Activator.CreateInstance(
                typeof(TaskCompletionSource<>).MakeGenericType(typeof(ActionResult<>).MakeGenericType(resultType)))!;
                taskCompletionSourceWithActionResult.SetResult(actionResult);
                return taskCompletionSourceWithActionResult.Task;
            }

            dynamic taskCompletionSource = Activator.CreateInstance(
                typeof(TaskCompletionSource<>).MakeGenericType(resultType))!;
            taskCompletionSource.SetResult((dynamic)result!);
            return taskCompletionSource.Task;
        }

        public void SetHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
