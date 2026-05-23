using AutoMapper;
using DK.Domain.RoadMaps;
using DK.Domain.Sales;
using DK.Process.RoadMaps;
using DK.Process.Sales;
using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.RoadMaps;
using DK.WebApi.ViewModel.Sales;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.RoadMaps
{
    [ApiController]
    [Route("[controller]")]
    public class RoadMapController : ControllerBase
    {
        private readonly IMapper _mapper;
        private RoadMapProcess _roadMapProcess;

        public RoadMapController(RoadMapProcess roadMapProcess, IMapper mapper)
        {
            _roadMapProcess = roadMapProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<RoadMapResponse>> GetAll()
        {
            var categories = await _roadMapProcess.GetAll();
            return _mapper.Map<IEnumerable<RoadMapResponse>>(categories);
        }

        [HttpPost("GetPage")]
        public async Task<ResultPageResponse<RoadMapResponse>> GetPage([FromBody] RoadMapFilter cityFilter)
        {
            var resultPage = await _roadMapProcess.GetPage(cityFilter);
            return _mapper.Map<ResultPageResponse<RoadMapResponse>>(resultPage);
        }

        [HttpGet("GetById")]
        public async Task<RoadMapResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var entity = await _roadMapProcess.Get(id);
            return _mapper.Map<RoadMapResponse>(entity);
        }

        [HttpPost("Create")]
        public async Task<RoadMapResponse> Create([FromBody] RoadMapRequest data)
        {
            var category = await _roadMapProcess.Create(_mapper.Map<RoadMap>(data));
            return _mapper.Map<RoadMapResponse>(category);
        }

        [HttpPost("Update")]
        public async Task<RoadMapResponse> Update([FromBody] RoadMapRequest data)
        {
            var category = await _roadMapProcess.Update(_mapper.Map<RoadMap>(data));
            return _mapper.Map<RoadMapResponse>(category);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] RoadMapRequest data)
        {
            await _roadMapProcess.Delete(_mapper.Map<RoadMap>(data));
        }

        [HttpPost("OnTrip")]
        public async Task OnTrip([FromBody] long roadMapId, CancellationToken cancellationToken = default)
        {
            var roadMap = await _roadMapProcess.Get(roadMapId, cancellationToken);
            await _roadMapProcess.OnTrip(roadMap, cancellationToken);
        }

        [HttpPost("FinishTrip")]
        public async Task FinishTrip([FromBody] long roadMapId, CancellationToken cancellationToken = default)
        {
            var roadMap = await _roadMapProcess.Get(roadMapId, cancellationToken);
            await _roadMapProcess.FinishTrip(roadMap, cancellationToken);
        }
    }
}
