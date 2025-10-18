using DK.Domain.Products;
using DK.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        [HttpGet("Get")]
        public async Task<Category> Get([FromQuery(Name = "Id")] long id, [FromServices] CategoryRepository categoryRepository)
        {
            var category = await categoryRepository.Get(id);
            return category;
        }

        [HttpPost("Create")]
        public async Task<Category> Create([FromServices] CategoryRepository categoryRepository, [FromBody] Category data)
        {
            var category = await categoryRepository.Create(data);
            return category;
        }

        [HttpPost("Update")]
        public async Task<Category> Update([FromServices] CategoryRepository categoryRepository, [FromBody] Category data)
        {
            var category = await categoryRepository.Update(data);
            return category;
        }

        [HttpPost("Delete")]
        public async Task Delete([FromServices] CategoryRepository categoryRepository, [FromBody] Category data)
        {
            await categoryRepository.Delete(data);
        }
    }
}
