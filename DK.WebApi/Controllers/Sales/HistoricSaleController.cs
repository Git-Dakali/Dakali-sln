using AutoMapper;
using DK.Domain.Sales;
using DK.Process.Sales;
using DK.WebApi.ViewModel.Sales;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.Sales
{
    [ApiController]
    [Route("[controller]")]
    public class HistoricSaleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private HistoricSaleProcess _historicSaleProcess;


        public HistoricSaleController(HistoricSaleProcess  historicSaleProcess, IMapper mapper)
        {
            _historicSaleProcess = historicSaleProcess;
            _mapper = mapper;
        }
        
        [HttpGet("GetAll")]
        public async Task<IEnumerable<HistoricSaleResponse>> Get([FromQuery] long saleId, CancellationToken cancellation)
        {
            var products = await _historicSaleProcess.Get(new Sale { Id = saleId }, cancellation);
            return _mapper.Map<IEnumerable<HistoricSaleResponse>>(products);
        }
    }
}
