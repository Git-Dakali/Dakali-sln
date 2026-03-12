using AutoMapper;
using DK.Domain.RoadMaps;
using DK.Process.RoadMaps;
using DK.WebApi.ViewModel.RoadMaps;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.RoadMaps
{
    [ApiController]
    [Route("[controller]")]
    public class RoadMapSaleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private RoadMapProcess _roadMapProcess;
        private RoadMapSaleProcess _roadMapSaleProcess;

        public RoadMapSaleController(RoadMapProcess roadMapProcess, RoadMapSaleProcess roadMapSaleProcess, IMapper mapper)
        {
            _roadMapSaleProcess = roadMapSaleProcess;
            _roadMapProcess = roadMapProcess;
            _mapper = mapper;
        }

        [HttpGet("GetByRoadMap")]
        public async Task<IEnumerable<RoadMapSaleResponse>> GetByRoadMap([FromQuery(Name = "RoadMapId")] long roadMapId, CancellationToken cancellationToken = default)
        {
            var details = await _roadMapSaleProcess.Get(new RoadMap() { Id = roadMapId }, cancellationToken);
            return _mapper.Map<IEnumerable<RoadMapSaleResponse>>(details);
        }

        [HttpPost("AssignRoadMap")]
        public async Task AssignRoadMap([FromBody] RoadMapSaleRequest data, [FromQuery(Name = "RoadMapId")] long roadMapId, CancellationToken cancellationToken = default)
        {
            var roadMap = await _roadMapProcess.Get(roadMapId, cancellationToken);
            await _roadMapSaleProcess.AssignRoadMap(_mapper.Map<RoadMapSale>(data), roadMap, cancellationToken);
        }

        [HttpPost("UnassignRoadMap")]
        public async Task UnassignRoadMap([FromBody] RoadMapSaleRequest data, [FromQuery(Name = "RoadMapId")] long roadMapId, CancellationToken cancellationToken = default)
        {
            var roadMap = await _roadMapProcess.Get(roadMapId, cancellationToken);
            await _roadMapSaleProcess.UnassignRoadMap(_mapper.Map<RoadMapSale>(data), roadMap, cancellationToken);
        }
    }
}
