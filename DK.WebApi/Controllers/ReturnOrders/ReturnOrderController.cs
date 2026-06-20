using AutoMapper;
using DK.Domain.ReturnOrders;
using DK.Process.ReturnOrders;
using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.ReturnOrders;
using DK.WebApi.ViewModel.Sales;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.ReturnOrders
{
    [ApiController]
    [Route("[controller]")]
    public class ReturnOrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ReturnOrderProcess _returnOrderProcess;

        public ReturnOrderController(ReturnOrderProcess returnOrderProcess, IMapper mapper)
        {
            _returnOrderProcess = returnOrderProcess;
            _mapper = mapper;
        }

        [HttpPost("GetPage")]
        public async Task<ResultPageResponse<ReturnOrderResponse>> GetPage([FromBody] ReturnOrderFilter filter)
        {
            var resultPage = await _returnOrderProcess.GetPage(filter);
            return _mapper.Map<ResultPageResponse<ReturnOrderResponse>>(resultPage);
        }

        [HttpPost("Create")]
        public async Task<ReturnOrderResponse> Create([FromBody] ReturnOrderRequest data, CancellationToken cancellation)
        {
            var returnOrder = await _returnOrderProcess.Create(_mapper.Map<ReturnOrder>(data), cancellation);
            return _mapper.Map<ReturnOrderResponse>(returnOrder);
        }

        [HttpPost("NotReturned")]
        public async Task<ReturnOrderResponse> NotReturned([FromBody] long saleId, CancellationToken cancellation)
        {
            var returnOrder = await _returnOrderProcess.Get(saleId, cancellation);
            var saleUpdated = await _returnOrderProcess.NotReturned(returnOrder, cancellation);

            return _mapper.Map<ReturnOrderResponse>(saleUpdated);
        }

        [HttpPost("Returned")]
        public async Task<ReturnOrderResponse> Returned([FromBody] long saleId, CancellationToken cancellation)
        {
            var returnOrder = await _returnOrderProcess.Get(saleId, cancellation);
            var saleUpdated = await _returnOrderProcess.Returned(returnOrder, cancellation);

            return _mapper.Map<ReturnOrderResponse>(saleUpdated);
        }

        [HttpPost("Stored")]
        public async Task<ReturnOrderResponse> Stored([FromBody] long saleId, CancellationToken cancellation)
        {
            var returnOrder = await _returnOrderProcess.Get(saleId, cancellation);
            var saleUpdated = await _returnOrderProcess.Stored(returnOrder, cancellation);

            return _mapper.Map<ReturnOrderResponse>(saleUpdated);
        }
    }
}
