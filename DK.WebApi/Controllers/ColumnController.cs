using AutoMapper;
using DK.Domain.Locations;
using DK.Process.Locations;
using DK.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ColumnController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ColumnProcess _columnProcess;

        public ColumnController(ColumnProcess columnProcess, IMapper mapper)
        {
            _columnProcess = columnProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<ColumnResponse>> GetAll()
        {
            var entities = await _columnProcess.GetAll();
            return _mapper.Map<IEnumerable<ColumnResponse>>(entities);
        }

        [HttpGet("GetById")]
        public async Task<ColumnResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _columnProcess.Get(id);
            return _mapper.Map<ColumnResponse>(entity);
        }

        [HttpGet("GetByCode")]
        public async Task<ColumnResponse> Get([FromQuery(Name = "Code")] string code)
        {
            var entity = await _columnProcess.Get(code);
            return _mapper.Map<ColumnResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<ColumnResponse> Create([FromBody] ColumnRequest data)
        {
            var entity = await _columnProcess.Create(_mapper.Map<Column>(data));
            return _mapper.Map<ColumnResponse>(entity);
        }

        [HttpPost("Update")]
        public async Task<ColumnResponse> Update([FromBody] ColumnRequest data)
        {
            var entity = await _columnProcess.Update(_mapper.Map<Column>(data));
            return _mapper.Map<ColumnResponse>(entity);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] ColumnRequest data)
        {
            await _columnProcess.Delete(_mapper.Map<Column>(data));
        }
    }
}
