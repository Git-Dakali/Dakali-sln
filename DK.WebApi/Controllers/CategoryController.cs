using AutoMapper;
using DK.Domain.Products;
using DK.Process.Product;
using DK.WebApi.ViewModel.Products;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private CategoryProcess _categoryProcess;

        public CategoryController(CategoryProcess categoryProcess, IMapper mapper)
        {
            _categoryProcess = categoryProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<CategoryResponse>> GetAll()
        {
            var categories = await _categoryProcess.GetAll();
            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }

        [HttpGet("GetById")]
        public async Task<CategoryResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var category = await _categoryProcess.Get(id);
            return _mapper.Map<CategoryResponse>(category);
        }

        [HttpGet("GetByCode")]
        public async Task<CategoryResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var category = await _categoryProcess.Get(code);
            return _mapper.Map<CategoryResponse>(category);
        }

        [HttpPost("Create")]
        public async Task<CategoryResponse> Create([FromBody] CategoryRequest data)
        {
            var category = await _categoryProcess.Create(_mapper.Map<Category>(data));
            return _mapper.Map<CategoryResponse>(category);
        }

        [HttpPost("Update")]
        public async Task<CategoryResponse> Update([FromBody] CategoryRequest data)
        {
            var category = await _categoryProcess.Update(_mapper.Map<Category>(data));
            return _mapper.Map<CategoryResponse>(category);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] CategoryRequest data)
        {
            await _categoryProcess.Delete(_mapper.Map<Category>(data));
        }
    }
}
