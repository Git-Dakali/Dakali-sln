using Dakali.Domine;
using Dakali.Domine.Base;
using System;

namespace DK.Domain.Sales
{
    public class HistoricSale : Entity
    {
        public DateTime CreationDate { get; set; }
        public SaleState State { get; set; }
        public string Description { get; set; }
        public StoredFile StoredFile { get; set; }
    }
}
