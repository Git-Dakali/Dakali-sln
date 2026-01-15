using AutoMapper;
using DK.Domain.Locations;
using DK.Process.Locations;
using DK.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LevelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private LevelProcess _levelProcess;

        public LevelController(LevelProcess levelProcess, IMapper mapper)
        {
            _levelProcess = levelProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<LevelResponse>> GetAll()
        {
            var entities = await _levelProcess.GetAll();
            return _mapper.Map<IEnumerable<LevelResponse>>(entities);
        }

        [HttpGet("GetById")]
        public async Task<LevelResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _levelProcess.Get(id);
            return _mapper.Map<LevelResponse>(entity);
        }

        [HttpGet("GetByCode")]
        public async Task<LevelResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var entity = await _levelProcess.Get(code);
            return _mapper.Map<LevelResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<LevelResponse> Create([FromBody] LevelRequest data)
        {
            var entity = await _levelProcess.Create(_mapper.Map<Level>(data));
            return _mapper.Map<LevelResponse>(entity);
        }

        [HttpPost("Update")]
        public async Task<LevelResponse> Update([FromBody] LevelRequest data)
        {
            var entity = await _levelProcess.Update(_mapper.Map<Level>(data));
            return _mapper.Map<LevelResponse>(entity);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] LevelRequest data)
        {
            await _levelProcess.Delete(_mapper.Map<Level>(data));
        }
    }
}
