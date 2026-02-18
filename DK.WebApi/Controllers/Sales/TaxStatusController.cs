using AutoMapper;
using DK.Domain.Sales;
using DK.Process.Sales;
using DK.WebApi.ViewModel.Sales;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.Sales
{
    [ApiController]
    [Route("[controller]")]
    public class TaxStatusController : ControllerBase
    {
        private readonly IMapper _mapper;
        private TaxStatusProcess _taxStatusProcess;

        public TaxStatusController(TaxStatusProcess taxStatusProcess, IMapper mapper)
        {
            _taxStatusProcess = taxStatusProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<TaxStatusResponse>> GetAll()
        {
            var entities = await _taxStatusProcess.GetAll();
            return _mapper.Map<IEnumerable<TaxStatusResponse>>(entities);
        }

        [HttpGet("GetById")]
        public async Task<TaxStatusResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _taxStatusProcess.Get(id);
            return _mapper.Map<TaxStatusResponse>(entity);
        }

        [HttpGet("GetByCode")]
        public async Task<TaxStatusResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var entity = await _taxStatusProcess.Get(code);
            return _mapper.Map<TaxStatusResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<TaxStatusResponse> Create([FromBody] TaxStatusRequest data)
        {
            var entity = await _taxStatusProcess.Create(_mapper.Map<TaxStatus>(data));
            return _mapper.Map<TaxStatusResponse>(entity);
        }

        [HttpPost("Update")]
        public async Task<TaxStatusResponse> Update([FromBody] TaxStatusRequest data)
        {
            var entity = await _taxStatusProcess.Update(_mapper.Map<TaxStatus>(data));
            return _mapper.Map<TaxStatusResponse>(entity);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] TaxStatusRequest data)
        {
            await _taxStatusProcess.Delete(_mapper.Map<TaxStatus>(data));
        }
    }
}
