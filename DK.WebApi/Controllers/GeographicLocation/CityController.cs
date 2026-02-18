using AutoMapper;
using DK.Domain.GeographicLocation;
using DK.Process.GeographicLocation;
using DK.WebApi.ViewModel.GeographicLocation;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.GeographicLocation
{
    public class CityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private CityProcess _cityProcess;

        public CityController(CityProcess cityProcess, IMapper mapper)
        {
            _cityProcess = cityProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<CityResponse>> GetAll()
        {
            var entities = await _cityProcess.GetAll();
            return _mapper.Map<IEnumerable<CityResponse>>(entities);
        }

        [HttpPost("GetByCity")]
        public async Task<IEnumerable<CityResponse>> GetByCity([FromBody] ProvinceRequest data)
        {
            var entities = await _cityProcess.Get(_mapper.Map<Province>(data));
            return _mapper.Map<IEnumerable<CityResponse>>(entities);
        }

        [HttpGet("GetById")]
        public async Task<CityResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _cityProcess.Get(id);
            return _mapper.Map<CityResponse>(entity);
        }

        [HttpGet("GetByCode")]
        public async Task<CityResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var entity = await _cityProcess.Get(code);
            return _mapper.Map<CityResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<CityResponse> Create([FromBody] CityRequest data)
        {
            var entity = await _cityProcess.Create(_mapper.Map<City>(data));
            return _mapper.Map<CityResponse>(entity);
        }

        [HttpPost("Update")]
        public async Task<CityResponse> Update([FromBody] CityRequest data)
        {
            var entity = await _cityProcess.Update(_mapper.Map<City>(data));
            return _mapper.Map<CityResponse>(entity);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] CityRequest data)
        {
            await _cityProcess.Delete(_mapper.Map<City>(data));
        }
    }
}
