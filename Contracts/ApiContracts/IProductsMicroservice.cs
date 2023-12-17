using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.ApiContracts
{
    public interface IProductsMicroservice : IClient
    {
        [HttpGet("/productsGet")]
        Task<ActionResult> GetProductsAsyncGet(int test);

        [HttpPost("/productsPost")]
        Task<ActionResult<List<int>>> GetProductsAsyncPost([FromBody] Test test);
    }

    public class Test
    {
        public int X { get; set; }
    }

    public class Test2
    {
        public int Y { get; set; }
    }
}
