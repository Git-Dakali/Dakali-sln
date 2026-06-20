using AutoMapper;
using DK.Domain.Sales;
using DK.Process.Sales;
using DK.WebApi.ViewModel.Sales;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.Sales
{
    [ApiController]
    [Route("[controller]")]
    public class LogisticsProviderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private LogisticsProviderProcess _logisticsProviderProcess;

        public LogisticsProviderController(LogisticsProviderProcess logisticsProviderProcess, IMapper mapper)
        {
            _logisticsProviderProcess = logisticsProviderProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<LogisticsProviderResponse>> GetAll()
        {
            var categories = await _logisticsProviderProcess.GetAll();
            return _mapper.Map<IEnumerable<LogisticsProviderResponse>>(categories);
        }

        [HttpGet("GetById")]
        public async Task<LogisticsProviderResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var category = await _logisticsProviderProcess.Get(id);
            return _mapper.Map<LogisticsProviderResponse>(category);
        }

        [HttpPost("Create")]
        public async Task<LogisticsProviderResponse> Create([FromBody] LogisticsProviderRequest data)
        {
            var category = await _logisticsProviderProcess.Create(_mapper.Map<LogisticsProvider>(data));
            return _mapper.Map<LogisticsProviderResponse>(category);
        }

        [HttpPost("Update")]
        public async Task<LogisticsProviderResponse> Update([FromBody] LogisticsProviderRequest data)
        {
            var category = await _logisticsProviderProcess.Update(_mapper.Map<LogisticsProvider>(data));
            return _mapper.Map<LogisticsProviderResponse>(category);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] LogisticsProviderRequest data)
        {
            await _logisticsProviderProcess.Delete(_mapper.Map<LogisticsProvider>(data));
        }
    }
}
