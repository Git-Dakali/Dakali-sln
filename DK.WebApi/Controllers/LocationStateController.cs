using AutoMapper;
using DK.Domain.Locations;
using DK.Process.Locations;
using DK.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationStateController : ControllerBase
    {
        private readonly IMapper _mapper;
        private LocationStateProcess _stockStateProcess;

        public LocationStateController(LocationStateProcess stockStateProcess, IMapper mapper)
        {
            _stockStateProcess = stockStateProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<LocationStateResponse>> GetAll()
        {
            var categories = await _stockStateProcess.GetAll();
            return _mapper.Map<IEnumerable<LocationStateResponse>>(categories);
        }

        [HttpGet("GetById")]
        public async Task<LocationStateResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var category = await _stockStateProcess.Get(id);
            return _mapper.Map<LocationStateResponse>(category);
        }

        [HttpGet("GetByCode")]
        public async Task<LocationStateResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var category = await _stockStateProcess.Get(code);
            return _mapper.Map<LocationStateResponse>(category);
        }

        [HttpPost("Create")]
        public async Task<LocationStateResponse> Create([FromBody] LocationStateRequest data)
        {
            var category = await _stockStateProcess.Create(_mapper.Map<LocationState>(data));
            return _mapper.Map<LocationStateResponse>(category);
        }

        [HttpPost("Update")]
        public async Task<LocationStateResponse> Update([FromBody] LocationStateRequest data)
        {
            var category = await _stockStateProcess.Update(_mapper.Map<LocationState>(data));
            return _mapper.Map<LocationStateResponse>(category);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] LocationStateRequest data)
        {
            await _stockStateProcess.Delete(_mapper.Map<LocationState>(data));
        }
    }
}
