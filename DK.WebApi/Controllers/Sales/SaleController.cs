using AutoMapper;
using DK.Domain.Sales;
using DK.Process.Sales;
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

        [HttpPost("Delete")]
        public async Task Delete([FromBody] SaleRequest data, CancellationToken cancellation)
        {
            await _saleProcess.Delete(_mapper.Map<Sale>(data), cancellation);
        }
    }
}
