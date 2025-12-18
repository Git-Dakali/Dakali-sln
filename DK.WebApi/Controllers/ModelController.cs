using AutoMapper;
using DK.Domain.Products;
using DK.Process.Product;
using DK.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ModelProcess _modelProcess;

        public ModelController(ModelProcess modelProcess, IMapper mapper)
        {
            _modelProcess = modelProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<ModelResponse>> GetAll()
        {
            var categories = await _modelProcess.GetAll();
            return _mapper.Map<IEnumerable<ModelResponse>>(categories);
        }

        [HttpGet("GetById")]
        public async Task<ModelResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var model = await _modelProcess.Get(id);
            return _mapper.Map<ModelResponse>(model);
        }

        [HttpGet("GetByCode")]
        public async Task<ModelResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var model = await _modelProcess.Get(code);
            return _mapper.Map<ModelResponse>(model);
        }

        [HttpPost("Create")]
        public async Task<ModelResponse> Create([FromBody] ModelRequest data)
        {
            var model = await _modelProcess.Create(_mapper.Map<Model>(data));
            return _mapper.Map<ModelResponse>(model);
        }

        [HttpPost("Update")]
        public async Task<ModelResponse> Update([FromBody] ModelRequest data)
        {
            var model = await _modelProcess.Update(_mapper.Map<Model>(data));
            return _mapper.Map<ModelResponse>(model);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] ModelRequest data)
        {
            await _modelProcess.Delete(_mapper.Map<Model>(data));
        }
    }
}
