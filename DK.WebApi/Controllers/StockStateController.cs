using AutoMapper;
using DK.Domain.Products;
using DK.Process.Product;
using DK.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockStateController : ControllerBase
    {
        private readonly IMapper _mapper;
        private StockStateProcess _stockStateProcess;

        public StockStateController(StockStateProcess stockStateProcess, IMapper mapper)
        {
            _stockStateProcess = stockStateProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<StockStateResponse>> GetAll()
        {
            var categories = await _stockStateProcess.GetAll();
            return _mapper.Map<IEnumerable<StockStateResponse>>(categories);
        }

        [HttpGet("GetById")]
        public async Task<StockStateResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var category = await _stockStateProcess.Get(id);
            return _mapper.Map<StockStateResponse>(category);
        }

        [HttpGet("GetByCode")]
        public async Task<StockStateResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var category = await _stockStateProcess.Get(code);
            return _mapper.Map<StockStateResponse>(category);
        }

        [HttpPost("Create")]
        public async Task<StockStateResponse> Create([FromBody] StockStateRequest data)
        {
            var category = await _stockStateProcess.Create(_mapper.Map<StockState>(data));
            return _mapper.Map<StockStateResponse>(category);
        }

        [HttpPost("Update")]
        public async Task<StockStateResponse> Update([FromBody] StockStateRequest data)
        {
            var category = await _stockStateProcess.Update(_mapper.Map<StockState>(data));
            return _mapper.Map<StockStateResponse>(category);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] StockStateRequest data)
        {
            await _stockStateProcess.Delete(_mapper.Map<StockState>(data));
        }
    }
}
