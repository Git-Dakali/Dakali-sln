using AutoMapper;
using DK.Domain.GeographicLocation;
using DK.Process.GeographicLocation;
using DK.WebApi.ViewModel.GeographicLocation;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.GeographicLocation
{
    [ApiController]
    [Route("[controller]")]
    public class ProvinceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ProvinceProcess _provinceProcess;

        public ProvinceController(ProvinceProcess provinceProcess, IMapper mapper)
        {
            _provinceProcess = provinceProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<ProvinceResponse>> GetAll()
        {
            var entities = await _provinceProcess.GetAll();
            return _mapper.Map<IEnumerable<ProvinceResponse>>(entities);
        }

        [HttpPost("GetByCountry")]
        public async Task<IEnumerable<ProvinceResponse>> GetByCountry([FromBody] CountryRequest data)
        {
            var entities = await _provinceProcess.Get(_mapper.Map<Country>(data));
            return _mapper.Map<IEnumerable<ProvinceResponse>>(entities);
        }

        [HttpGet("GetById")]
        public async Task<ProvinceResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _provinceProcess.Get(id);
            return _mapper.Map<ProvinceResponse>(entity);
        }

        [HttpGet("GetByCode")]
        public async Task<ProvinceResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var entity = await _provinceProcess.Get(code);
            return _mapper.Map<ProvinceResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<ProvinceResponse> Create([FromBody] ProvinceRequest data)
        {
            var entity = await _provinceProcess.Create(_mapper.Map<Province>(data));
            return _mapper.Map<ProvinceResponse>(entity);
        }

        [HttpPost("Update")]
        public async Task<ProvinceResponse> Update([FromBody] ProvinceRequest data)
        {
            var entity = await _provinceProcess.Update(_mapper.Map<Province>(data));
            return _mapper.Map<ProvinceResponse>(entity);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] ProvinceRequest data)
        {
            await _provinceProcess.Delete(_mapper.Map<Province>(data));
        }
    }
}
