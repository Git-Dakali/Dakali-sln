using AutoMapper;
using DK.Domain.GeographicLocation;
using DK.Domain.Sales;
using DK.Process.Sales;
using DK.WebApi.ViewModel.Base;
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

        [HttpPost("UpdateIsPrinted")]
        public async Task UpdateIsPrinted([FromQuery] long saleId, [FromQuery] bool isPrinted, CancellationToken cancellation)
        {
            await _saleProcess.UpdateIsPrinted(saleId, isPrinted, cancellation);
        }

        [HttpPost("AddLocation")]
        public async Task AddLocation([FromBody] SaleLocationRequest request, CancellationToken cancellation)
        {
            await _saleProcess.AddLocation(new Sale() { Id = request.SaleId, Longitude = request.Longitude, Latitude = request.Latitude, Address = request.Address, City = _mapper.Map<City>(request.City), Observation = request.Observation }, cancellation);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] SaleRequest data, CancellationToken cancellation)
        {
            await _saleProcess.Delete(_mapper.Map<Sale>(data), cancellation);
        }

        [HttpPost("Confirm")]
        public async Task<SaleResponse> Confirm([FromBody] long saleId, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(saleId, cancellation);
            var saleUpdated = await _saleProcess.Confirm(sale, cancellation);

            return _mapper.Map<SaleResponse>(saleUpdated);
        }

        [HttpPost("Prepared")]
        public async Task<SaleResponse> Prepared([FromBody] long saleId, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(saleId, cancellation);
            var saleUpdated = await _saleProcess.Prepared(sale, cancellation);

            return _mapper.Map<SaleResponse>(saleUpdated);
        }

        [HttpPost("PendingDispatch")]
        public async Task<SaleResponse> PendingDispatch([FromBody] long saleId, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(saleId, cancellation);
            var saleUpdated = await _saleProcess.PendingDispatch(sale, cancellation);

            return _mapper.Map<SaleResponse>(saleUpdated);
        }

        [HttpPost("OnTrip")]
        public async Task<SaleResponse> OnTrip([FromBody] long saleId, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(saleId, cancellation);
            var saleUpdated = await _saleProcess.OnTrip(sale, cancellation);

            return _mapper.Map<SaleResponse>(saleUpdated);
        }

        [HttpPost("Deliver")]
        public async Task<SaleResponse> Deliver([FromBody] long saleId, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(saleId, cancellation);
            var saleUpdated = await _saleProcess.Deliver(sale, cancellation);

            return _mapper.Map<SaleResponse>(saleUpdated);
        }

        [HttpPost("PartialDeliver")]
        public async Task<SaleResponse> PartialDeliver([FromBody] long saleId, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(saleId, cancellation);
            var saleUpdated = await _saleProcess.PartialDelivered(sale, cancellation);

            return _mapper.Map<SaleResponse>(saleUpdated);
        }

        [HttpPost("Reject")]
        public async Task<SaleResponse> Reject([FromBody] long saleId, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(saleId, cancellation);
            var saleUpdated = await _saleProcess.Reject(sale, cancellation);

            return _mapper.Map<SaleResponse>(saleUpdated);
        }

        [HttpPost("Cancel")]
        public async Task<SaleResponse> Cancel([FromBody] long saleId, CancellationToken cancellation)
        {
            var sale = await _saleProcess.Get(saleId, cancellation);
            var saleUpdated = await _saleProcess.Cancel(sale, cancellation);

            return _mapper.Map<SaleResponse>(saleUpdated);
        }

        [HttpGet("ReportExcelDarLogitics")]
        public async Task<string> ReportExcelDarLogitics([FromQuery(Name = "SaleIds")] string saleIds, CancellationToken cancellation)
        {
            var ids = saleIds.Split(",").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => long.Parse(x));
            var bytes = await _saleProcess.GetReportExcelDarLogitics(ids, cancellation);

            return Convert.ToBase64String(bytes);
        }
    }
}
