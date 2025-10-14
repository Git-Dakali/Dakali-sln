using Dakali;
using DK.Model;
using DK.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        [HttpGet("Get")]
        public async Task<ProductCategory> Get([FromQuery(Name = "Id")] long id, [FromServices] ProductCategoryRepository categoryRepository)
        {
            var category = await categoryRepository.Get(id);
            return category;
        }

        [HttpPost("Create")]
        public async Task<ProductCategory> Create([FromServices] ProductCategoryRepository categoryRepository, [FromBody] ProductCategory data)
        {
            var category = await categoryRepository.Create(data);
            return category;
        }

        [HttpPost("Update")]
        public async Task<ProductCategory> Update([FromServices] ProductCategoryRepository categoryRepository, [FromBody] ProductCategory data)
        {
            var category = await categoryRepository.Update(data);
            return category;
        }

        [HttpPost("Delete")]
        public async Task Delete([FromServices] ProductCategoryRepository categoryRepository, [FromBody] ProductCategory data)
        {
            await categoryRepository.Delete(data);
        }
    }
}
