using Dakali.Domine.Base;
using DK.Domain.Locations;

namespace DK.Domain.Products
{
    public class Stock: EntityGuid
    {
        public Product Product {  get; set; }
        public Variant Variant { get; set; }
        public ProductColor Color { get; set; }
        public long Physical { get; set; }
        public long Reserved { get; set; }
        public long Transit { get; set; }
        public long Free { get; set; }
        public long Minimum { get; set; }
        public long Maximum { get; set; }
        public Location Location { get; set; }

        public Stock()
        { 
            Physical = 0;   
            Reserved = 0;
            Transit = 0;
            Free = 0;
            Minimum = 0;
            Maximum = 0;
        }

        public override string ToString() 
        {
            return $"{Product.Model.Code} {Product.Name} {Variant.Name} {Color.Name} {Location.ToString()}";
        }
    }
}
