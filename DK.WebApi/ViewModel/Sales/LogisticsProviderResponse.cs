using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Sales
{
    public class LogisticsProviderResponse : ResponseCode
    {
        public string Name { get; set; }
        public bool IsInHouse { get; set; }
    }
}
