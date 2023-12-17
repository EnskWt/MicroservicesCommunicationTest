using Contracts.ApiContracts;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace PublicationsMicroservice.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase, IProductsMicroservice
    {
        // if get method you must use [FromBody] for now
        [HttpGet("/productsGet")]
        public async Task<ActionResult> GetProductsAsyncGet([FromBody] int test)
        {
            if (test == 1)
            {
                //return Ok(new List<int> { 1, 2, 3 });
                return Ok();
            }
            else
            {
                return Problem("hello");
            }
        }

        // if post method you should use [FromBody] for now
        [HttpPost("/productsPost")]
        public async Task<ActionResult<List<int>>> GetProductsAsyncPost([FromBody] Test test)
        {
            if (test.X == 1)
            {
                return Ok(new List<int> { 4, 5, 6 });
            }
            else
            {
                return Problem("hello");
            }
        }
    }
}
