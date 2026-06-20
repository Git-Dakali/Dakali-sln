using DK.Domain.Products;
using DK.Domain.Sales;
using DK.Repositories.Sales;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Sales
{
    public class SaleDetailValidator
    {
        public SaleDetailRepository _saleDetailRepository;

        public SaleDetailValidator(SaleDetailRepository saleDetailRepository)
        {
            _saleDetailRepository = saleDetailRepository ?? throw new ArgumentNullException("SaleDetailRepository");
        }

        public async Task Create(Sale sale, SaleDetail saleDetail, CancellationToken cancellationToken = default)
        {
            await Product(saleDetail, cancellationToken);
            await ProductSku(saleDetail, cancellationToken);
            await Count(saleDetail, cancellationToken);
            await Price(saleDetail, cancellationToken);
            await Location(saleDetail, cancellationToken);
        }

        public async Task Update(Sale sale, SaleDetail saleDetail, CancellationToken cancellationToken = default)
        {
            await Product(saleDetail, cancellationToken);
            await ProductSku(saleDetail, cancellationToken);
            await Count(saleDetail, cancellationToken);
            await Price(saleDetail, cancellationToken);
            await Location(saleDetail, cancellationToken);
        }

        public async Task Delete(Sale sale, SaleDetail saleDetail, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(sale, saleDetail, cancellationToken)))
                throw new Exception($"No existe el producto {saleDetail.Product.Name}-{saleDetail.ProductSku.Variant.Name}-{saleDetail.ProductSku.Color.Name}");
        }

        public async Task AssignStock(Sale parent, SaleDetail saleDetail, Stock stock, CancellationToken cancellation = default)
        {
            if (parent is null)
                throw new Exception("La venta esta vacio.");

            if (saleDetail is null)
                throw new Exception("El detalle esta vacio.");

            var detailPersisted = await _saleDetailRepository.Get(parent, saleDetail.Id, cancellation);

            if (detailPersisted is null)
                throw new Exception($"El detalle no existe para la venta {parent.Number}.");

            if(detailPersisted.Stock != null)
                throw new Exception($"El detalle ya posee un stock reservado.");

            if (stock is null)
                throw new Exception("El stock esta vacio.");
        }

        public async Task UnassignStock(Sale parent, SaleDetail saleDetail, CancellationToken cancellation = default)
        {
            if (parent is null)
                throw new Exception("La venta esta vacio.");

            if (saleDetail is null)
                throw new Exception("El detalle esta vacio.");

            var detailPersisted = await _saleDetailRepository.Get(parent, saleDetail.Id, cancellation);
            
            if (detailPersisted is null)
                throw new Exception($"El detalle no existe para la venta {parent.Number}.");

            if(detailPersisted.Stock is null)
                throw new Exception($"El detalle {detailPersisted.Product.Name}-{detailPersisted.ProductSku.Variant.Name}-{detailPersisted.ProductSku.Color.Name} no posee stock reservado para desasignar.");

            await _saleDetailRepository.UnassignStock(parent, saleDetail, cancellation);
        }

        public async Task<bool> Exist(Sale sale, SaleDetail saleDetail, CancellationToken cancellationToken = default)
        {
            return (await _saleDetailRepository.Get(sale, saleDetail.Id, cancellationToken)) != null;
        }

        public async Task Product(SaleDetail saleDetail, CancellationToken cancellationToken = default)
        {
            if (saleDetail.Product is null)
                throw new Exception("El producto esta vacio.");
        }

        public async Task ProductSku(SaleDetail saleDetail, CancellationToken cancellationToken = default)
        {
            if (saleDetail.ProductSku?.Variant is null)
                throw new Exception("La Variante esta vacio.");

            if (saleDetail.ProductSku?.Color is null)
                throw new Exception("El Color esta vacio.");

        }

        public async Task Count(SaleDetail saleDetail, CancellationToken cancellationToken = default)
        {
            if (saleDetail.Count <= 0)
                throw new Exception("Debe ingresar una cantidad");
        }

        public async Task Price(SaleDetail saleDetail, CancellationToken cancellationToken = default)
        {
            if (saleDetail.Price <= 0)
                throw new Exception("Debe ingresar un precio.");
            
        }

        public async Task Location(SaleDetail saleDetail, CancellationToken cancellationToken = default)
        {
            
        }
    }
}
