using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class StockRequest: RequestGuid
    {
        public ProductRequest Product { get; set; }
        public VariantRequest Variant { get; set; }
        public ColorRequest Color { get; set; }
        public long Physical { get; set; }
        public long Reserved { get; set; }
        public long Transit { get; set; }
        public long Free { get; set; }
        public long Minimum { get; set; }
        public long Maximum { get; set; }
        public StockStateRequest State { get; set; }
    }
}
