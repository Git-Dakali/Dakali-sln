using Dakali.Domine.Base;
using System;
using System.Collections.Generic;

namespace DK.Domain.RoadMaps
{
    public enum RoadMapState
    {
        Creado = 1,
        EnViaje = 3,
        Finalizado = 4
    }

    public class RoadMap : EntityGuid
    {
        public long Number { get; set; }
        public DateTime Date { get; set; }
        public DateTime? TravelDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public Driver Driver { get; set; }
        public RoadMapState State { get; set; }
        public IEnumerable<RoadMapSale> Sales { get; set; }
    }
}
