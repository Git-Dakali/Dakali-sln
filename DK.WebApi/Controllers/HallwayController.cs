using AutoMapper;
using DK.Domain.Locations;
using DK.Process.Locations;
using DK.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HallwayController : ControllerBase
    {
        private readonly IMapper _mapper;
        private HallwayProcess _hallwayProcess;

        public HallwayController(HallwayProcess hallwayProcess, IMapper mapper)
        {
            _hallwayProcess = hallwayProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<HallwayResponse>> GetAll()
        {
            var entities = await _hallwayProcess.GetAll();
            return _mapper.Map<IEnumerable<HallwayResponse>>(entities);
        }

        [HttpGet("GetById")]
        public async Task<HallwayResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _hallwayProcess.Get(id);
            return _mapper.Map<HallwayResponse>(entity);
        }

        [HttpGet("GetByCode")]
        public async Task<HallwayResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var entity = await _hallwayProcess.Get(code);
            return _mapper.Map<HallwayResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<HallwayResponse> Create([FromBody] HallwayRequest data)
        {
            var entity = await _hallwayProcess.Create(_mapper.Map<Hallway>(data));
            return _mapper.Map<HallwayResponse>(entity);
        }

        [HttpPost("Update")]
        public async Task<HallwayResponse> Update([FromBody] HallwayRequest data)
        {
            var entity = await _hallwayProcess.Update(_mapper.Map<Hallway>(data));
            return _mapper.Map<HallwayResponse>(entity);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] HallwayRequest data)
        {
            await _hallwayProcess.Delete(_mapper.Map<Hallway>(data));
        }
    }
}
