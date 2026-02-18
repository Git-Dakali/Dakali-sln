using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class LocationResponse : ResponseGuid
    {
        public HallwayResponse? Hallway { get; set; }
        public ColumnResponse? Column { get; set; }
        public LevelResponse? Level { get; set; }
        public LocationStateResponse? State { get; set; }
    }
}
