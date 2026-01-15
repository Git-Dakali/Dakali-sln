using AutoMapper;
using DK.Domain.Locations;
using DK.Process.Locations;
using DK.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private LocationProcess _locationProcess;

        public LocationController(LocationProcess locationProcess, IMapper mapper)
        {
            _locationProcess = locationProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<LocationResponse>> GetAll()
        {
            var entities = await _locationProcess.GetAll();
            return _mapper.Map<IEnumerable<LocationResponse>>(entities);
        }

        [HttpGet("GetById")]
        public async Task<LocationResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _locationProcess.Get(id);
            return _mapper.Map<LocationResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<LocationResponse> Create([FromBody] LocationRequest data)
        {
            var stock = await _locationProcess.Create(_mapper.Map<Location>(data));
            return _mapper.Map<LocationResponse>(stock);
        }

        [HttpPost("Update")]
        public async Task<LocationResponse> Update([FromBody] LocationRequest data)
        {
            var stock = await _locationProcess.Update(_mapper.Map<Location>(data));
            return _mapper.Map<LocationResponse>(stock);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] LocationRequest data)
        {
            await _locationProcess.Delete(_mapper.Map<Location>(data));
        }
    }
}
