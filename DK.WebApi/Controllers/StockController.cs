using AutoMapper;
using DK.Domain.Products;
using DK.Process.Product;
using DK.WebApi.Filters;
using DK.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IMapper _mapper;
        private StockProcess _stockProcess;

        public StockController(StockProcess stockProcess, IMapper mapper)
        {
            _stockProcess = stockProcess;
            _mapper = mapper;
        }

        [HttpPost("GetAll")]
        public async Task<IEnumerable<StockResponse>> GetAll([FromBody] StockFilter stockFilter)
        {
            var stocks = await _stockProcess.GetAll(stockFilter.SearchString);
            return _mapper.Map<IEnumerable<StockResponse>>(stocks);
        }

        [HttpGet("GetById")]
        public async Task<StockResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var stock = await _stockProcess.Get(id);
            return _mapper.Map<StockResponse>(stock);
        }

        [HttpPost("Create")]
        public async Task<StockResponse> Create([FromBody] StockRequest data)
        {
            var stock = await _stockProcess.Create(_mapper.Map<Stock>(data));
            return _mapper.Map<StockResponse>(stock);
        }

        [HttpPost("StockEntry")]
        public async Task StockEntry([FromBody] StockEntryRequest data)
        {
            await _stockProcess.StockEntry(_mapper.Map<Stock>(data.Stock), data.Amount);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] StockRequest data)
        {
            await _stockProcess.Delete(_mapper.Map<Stock>(data));
        }
    }
}
