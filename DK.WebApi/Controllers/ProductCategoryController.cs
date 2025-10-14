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
        public async Task<ProductCategory> Get([FromQuery(Name = "Id")] long id, [FromServices] IConfiguration configuration)
        {
            ContextManager.OpenSession(configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value ?? string.Empty);
            var categoryRepository = new ProductCategoryRepository();
            var category = await categoryRepository.Get(id);
            await ContextManager.Session.Commit();
            return category;
        }

        [HttpPost("Create")]
        public async Task<ProductCategory> Create([FromServices] IConfiguration configuration, [FromBody] ProductCategory data)
        {
            ContextManager.OpenSession(configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value ?? string.Empty);
            var categoryRepository = new ProductCategoryRepository();
            var category = await categoryRepository.Create(data);
            await ContextManager.Session.Commit();
            return category;
        }

        [HttpPost("Update")]
        public async Task<ProductCategory> Update([FromServices] IConfiguration configuration, [FromBody] ProductCategory data)
        {
            ContextManager.OpenSession(configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value ?? string.Empty);
            var categoryRepository = new ProductCategoryRepository();
            var category = await categoryRepository.Update(data);
            await ContextManager.Session.Commit();
            return category;
        }

        [HttpPost("Delete")]
        public async Task Delete([FromServices] IConfiguration configuration, [FromBody] ProductCategory data)
        {
            ContextManager.OpenSession(configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value ?? string.Empty);
            var categoryRepository = new ProductCategoryRepository();
            await categoryRepository.Delete(data);
            await ContextManager.Session.Commit();
        }
    }
}
