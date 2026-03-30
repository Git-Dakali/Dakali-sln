using AutoMapper;
using DK.Domain.GeographicLocation;
using DK.Domain.Sales;
using DK.Process.GeographicLocation;
using DK.Process.Sales;
using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.GeographicLocation.Cities;
using DK.WebApi.ViewModel.Sales;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.Sales
{
    [ApiController]
    [Route("[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private SaleProcess _saleProcess;
        

        public SaleController(SaleProcess saleProcess, IMapper mapper)
        {
            _saleProcess = saleProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<SaleResponse>> GetAll(CancellationToken cancellation)
        {
            var products = await _saleProcess.GetAll(cancellation);
            return _mapper.Map<IEnumerable<SaleResponse>>(products);
        }

        [HttpPost("GetPage")]
        public async Task<ResultPageResponse<SaleResponse>> GetPage([FromBody] SaleFilter cityFilter)
        {
            var resultPage = await _saleProcess.GetPage(cityFilter);
            return _mapper.Map<ResultPageResponse<SaleResponse>>(resultPage);
        }

        [HttpGet("GetById")]
        public async Task<SaleResponse> Get([FromQuery(Name = "Id")] long id, CancellationToken cancellation)
        {
            var product = await _saleProcess.Get(id, cancellation);
            return _mapper.Map<SaleResponse>(product);
        }

        [HttpGet("GetByNumber")]
        public async Task<SaleResponse> GetByNumber([FromQuery(Name = "number")] long number, CancellationToken cancellation)
        {
            var product = await _saleProcess.GetByNumber(number, cancellation);
            return _mapper.Map<SaleResponse>(product);
        }

        [HttpPost("Create")]
        public async Task<SaleResponse> Create([FromBody] SaleRequest data, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Create(_mapper.Map<Sale>(data), cancellation);
            return _mapper.Map<SaleResponse>(sale);
        }

        [HttpPost("Update")]
        public async Task<SaleResponse> Update([FromBody] SaleRequest data, CancellationToken cancellation)
        {
            var product = await _saleProcess.Update(_mapper.Map<Sale>(data), cancellation);
            return _mapper.Map<SaleResponse>(product);
        }

        [HttpPost("AddLocation")]
        public async Task AddLocation([FromQuery] long SaleId, [FromQuery] decimal longitude, [FromQuery] decimal latitude, CancellationToken cancellation)
        {
            await _saleProcess.AddLocation(new Sale() { Id = SaleId, Longitude = longitude, Latitude = latitude }, cancellation);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] SaleRequest data, CancellationToken cancellation)
        {
            await _saleProcess.Delete(_mapper.Map<Sale>(data), cancellation);
        }
    }
}
