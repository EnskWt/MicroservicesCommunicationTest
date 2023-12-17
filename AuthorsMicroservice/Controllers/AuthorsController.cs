using Contracts;
using Contracts.ApiContracts;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace AuthorsMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorsController : ControllerBase, IAuthorsMicroservice
    {
        //private readonly UniversalClient<IProductsMicroservice> _productsMicroservice;

        private readonly IProductsMicroservice _productsMicroservice;

        //public AuthorsController(UniversalClient<IProductsMicroservice> productsMicroservice)
        //{
        //    _productsMicroservice = productsMicroservice;
        //}

        public AuthorsController(IProductsMicroservice productsMicroservice)
        {
            _productsMicroservice = productsMicroservice;
        }

        [HttpGet("/test")]
        public async Task<ActionResult<List<int>>>? GetProducts(int value, int method)
        {
            if (method == 1)
            {
                var result = await _productsMicroservice.GetProductsAsyncGet(value);
                return result;
            }
            else
            {
                var result = await _productsMicroservice.GetProductsAsyncPost(new Test { X = value });
                return Ok(result);
            }
        }
    }
}
