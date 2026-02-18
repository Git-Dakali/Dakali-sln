using AutoMapper;
using DK.Domain.GeographicLocation;
using DK.Process.GeographicLocation;
using DK.WebApi.ViewModel.GeographicLocation;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.GeographicLocation
{
    [ApiController]
    [Route("[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private CountryProcess _countryProcess;

        public CountryController(CountryProcess categoryProcess, IMapper mapper)
        {
            _countryProcess = categoryProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<CountryResponse>> GetAll()
        {
            var categories = await _countryProcess.GetAll();
            return _mapper.Map<IEnumerable<CountryResponse>>(categories);
        }

        [HttpGet("GetById")]
        public async Task<CountryResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _countryProcess.Get(id);
            return _mapper.Map<CountryResponse>(entity);
        }

        [HttpGet("GetByCode")]
        public async Task<CountryResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var entity = await _countryProcess.Get(code);
            return _mapper.Map<CountryResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<CountryResponse> Create([FromBody] CountryRequest data)
        {
            var entity = await _countryProcess.Create(_mapper.Map<Country>(data));
            return _mapper.Map<CountryResponse>(entity);
        }

        [HttpPost("Update")]
        public async Task<CountryResponse> Update([FromBody] CountryRequest data)
        {
            var entity = await _countryProcess.Update(_mapper.Map<Country>(data));
            return _mapper.Map<CountryResponse>(entity);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] CountryRequest data)
        {
            await _countryProcess.Delete(_mapper.Map<Country>(data));
        }
    }
}
