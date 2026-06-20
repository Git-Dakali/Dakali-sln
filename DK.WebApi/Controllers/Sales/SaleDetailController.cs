using AutoMapper;
using DK.Domain.Sales;
using DK.Process.Locations;
using DK.Process.Product;
using DK.Process.Sales;
using DK.WebApi.ViewModel.Sales;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.Sales
{
    [ApiController]
    [Route("[controller]")]
    public class SaleDetailController : ControllerBase
    {
        private readonly IMapper _mapper;
        private StockProcess _stockProcess;
        private SaleProcess _saleProcess;
        private SaleDetailProcess _saleDetailProcess;
        private LocationStateProcess _locationStateProcess;

        public SaleDetailController(SaleProcess saleProcess, SaleDetailProcess saleDetailProcess, StockProcess stockProcess, LocationStateProcess locationStateProcess, IMapper mapper)
        {
            _saleDetailProcess = saleDetailProcess;
            _saleProcess = saleProcess;
            _stockProcess = stockProcess;
            _locationStateProcess = locationStateProcess;
            _mapper = mapper;
        }

        [HttpPost("GetBySale")]
        public async Task<IEnumerable<SaleDetailResponse>> GetBySale([FromQuery] long idSale, CancellationToken cancellation)
        {
            return _mapper.Map<IEnumerable<SaleDetailResponse>>(await _saleDetailProcess.Get(new Sale() { Id = idSale }, cancellation));
        }

        [HttpPost("Create")]
        public async Task Create([FromQuery] long idSale, [FromBody] SaleDetailRequest saleDetailRequest, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(idSale, cancellation);
            var detail = await _saleDetailProcess.Create(sale, _mapper.Map<SaleDetail>(saleDetailRequest), cancellation);
            var state = await _locationStateProcess.Get("DIS", cancellation);
            var stock = await _stockProcess.Reserve(state, detail.Product, detail.ProductSku, detail.Count);
            await _saleDetailProcess.AssignStock(sale, detail, stock, cancellation);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromQuery] long idSale, [FromBody] SaleDetailRequest saleDetailRequest, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(idSale, cancellation);
            var detail = await _saleDetailProcess.Get(sale, saleDetailRequest.Id, cancellation);

            await _stockProcess.CancelReserve(detail.Stock, detail.Count, cancellation);
            await _saleDetailProcess.UnassignStock(sale, detail, cancellation);
            await _saleDetailProcess.Delete(sale, detail, cancellation);
        }
    }
}
