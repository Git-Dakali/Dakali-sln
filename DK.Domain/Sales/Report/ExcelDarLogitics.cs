namespace DK.Domain.Sales.Report
{
    public class ExcelDarLogitics
    {
        public long Id { get; set; }
        public string LogisticaInversa { get; set; }
        public string Tracking { get; set; }
        public string FechaEntrega { get; set; }
        public string Destinatario { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string CodigoPostal { get; set; }
        public string Observacion { get; set; }
        public decimal PrecioTotal { get; set; }
        public decimal ValorDeclarado { get; set; }
        public decimal Peso { get; set; }
    }
}
