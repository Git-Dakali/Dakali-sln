using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Sales
{
    public class HistoricSaleResponse : Response
    {
        public DateTime? CreationDate { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public StoredFileResponse StoredFile { get; set; }
    }
}
