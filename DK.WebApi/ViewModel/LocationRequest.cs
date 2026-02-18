using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class LocationRequest : RequestGuid
    {
        public HallwayRequest? Hallway {  get; set; }
        public ColumnRequest? Column { get; set; }
        public LevelRequest? Level { get; set; }
        public LocationStateRequest? State { get; set; }
    }
}
