using AutoMapper;
using DK.Domain.Sales;
using DK.Process.Sales;
using DK.WebApi.ViewModel.Sales;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.Sales
{
    [ApiController]
    [Route("[controller]")]
    public class OriginSaleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private OriginSaleProcess _originSaleProcess;

        public OriginSaleController(OriginSaleProcess originSaleProcess, IMapper mapper)
        {
            _originSaleProcess = originSaleProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<OriginSaleResponse>> GetAll()
        {
            var categories = await _originSaleProcess.GetAll();
            return _mapper.Map<IEnumerable<OriginSaleResponse>>(categories);
        }

        [HttpGet("GetById")]
        public async Task<OriginSaleResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var category = await _originSaleProcess.Get(id);
            return _mapper.Map<OriginSaleResponse>(category);
        }

        [HttpPost("Create")]
        public async Task<OriginSaleResponse> Create([FromBody] OriginSaleRequest data)
        {
            var category = await _originSaleProcess.Create(_mapper.Map<OriginSale>(data));
            return _mapper.Map<OriginSaleResponse>(category);
        }

        [HttpPost("Update")]
        public async Task<OriginSaleResponse> Update([FromBody] OriginSaleRequest data)
        {
            var category = await _originSaleProcess.Update(_mapper.Map<OriginSale>(data));
            return _mapper.Map<OriginSaleResponse>(category);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] OriginSaleRequest data)
        {
            await _originSaleProcess.Delete(_mapper.Map<OriginSale>(data));
        }
    }
}
