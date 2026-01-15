using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class StockResponse : ResponseGuid
    {
        public ProductResponse Product { get; set; }
        public VariantResponse Variant { get; set; }
        public ColorResponse Color { get; set; }
        public LocationResponse Location { get; set; }
        public long Physical { get; set; }
        public long Reserved { get; set; }
        public long Transit { get; set; }
        public long Free { get; set; }
        public long Minimum { get; set; }
        public long Maximum { get; set; }
    }
}
