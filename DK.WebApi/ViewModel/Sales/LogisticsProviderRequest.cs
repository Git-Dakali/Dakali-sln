using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Sales
{
    public class LogisticsProviderRequest : RequestCode
    {
        public string Name { get; set; }
        public bool IsInHouse { get; set; }
    }
}
