using AutoMapper;
using DK.Domain.RoadMaps;
using DK.Process.RoadMaps;
using DK.WebApi.ViewModel.RoadMaps;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.RoadMaps
{
    [ApiController]
    [Route("[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly IMapper _mapper;
        private DriverProcess _driverProcess;

        public DriverController(DriverProcess driverProcess, IMapper mapper)
        {
            _driverProcess = driverProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<DriverResponse>> GetAll()
        {
            var categories = await _driverProcess.GetAll();
            return _mapper.Map<IEnumerable<DriverResponse>>(categories);
        }

        [HttpGet("GetById")]
        public async Task<DriverResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _driverProcess.Get(id);
            return _mapper.Map<DriverResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<DriverResponse> Create([FromBody] DriverRequest data)
        {
            var category = await _driverProcess.Create(_mapper.Map<Driver>(data));
            return _mapper.Map<DriverResponse>(category);
        }

        [HttpPost("Update")]
        public async Task<DriverResponse> Update([FromBody] DriverRequest data)
        {
            var category = await _driverProcess.Update(_mapper.Map<Driver>(data));
            return _mapper.Map<DriverResponse>(category);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] DriverRequest data)
        {
            await _driverProcess.Delete(_mapper.Map<Driver>(data));
        }
    }
}
