using Dakali.Domine;
using Dakali.Domine.Base;
using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class ProductRepository : IRepositoryCode<Product>
    {
        private ISession _session;
        private CategoryRepository _categoryRepository;
        private FieldRepository _fieldRepository;
        private VariantRepository _variantRepository;
        private ProductColorRepository _productColorRepository;
        private ProductSkuRepository _productSkuRepository;

        public ProductRepository(ISession session, CategoryRepository categoryRepository, FieldRepository fieldRepository, VariantRepository variantRepository, ProductColorRepository productColorRepository, ProductSkuRepository productSkuRepository)
        {
            _session = session;
            _variantRepository = variantRepository;
            _productColorRepository = productColorRepository;
            _productSkuRepository = productSkuRepository;
            _fieldRepository = fieldRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Product> Create(Product entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Product (Code, Name, Description, CategoryId, Active, SalePrice, Price, Weight, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @Description, @CategoryId, @Active, @SalePrice, @Price, @Weight, @SearchString);";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(
                new CommandDefinition(query, new { entity.Code, entity.Name, entity.Description, CategoryId = entity.Category.Id, entity.Active, entity.SalePrice, entity.Price, entity.Weight, entity.SearchString }, transaction: _session.Transaction, cancellationToken: cancellation));

            var product = await Map(rowDapper, cancellation);

            if (product != null)
            {
                await _fieldRepository.SyncCollection(product, entity.Fields, cancellation);
                IEnumerable<Variant> variants = await _variantRepository.SyncCollection(product, entity.Variants, cancellation);
                IEnumerable<ProductColor> colors = await _productColorRepository.SyncCollection(product, entity.Colors, cancellation);

                await SyncCollectionSku(product, entity.Skus, colors, variants, cancellation);
            }

            return await Get(product.Id, cancellation);
        }

        public async Task Delete(Product entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Product
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<IEnumerable<Product>> GetAll(CancellationToken cancellation = default)
        {
            var query = @"
                select * 
                from dbo.Product 
                where IsDeleted = 0";
            var rows = await _session.Connection.QueryAsync(new CommandDefinition(query, new { }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rows == null)
                return Enumerable.Empty<Product>();

            var list = new List<Product>();

            foreach (var row in rows)
                list.Add(await Map(row, cancellation));

            return list;
        }

        public async Task<ResultPage<Product>> GetPage(ProductFilter productFilter, CancellationToken cancellationToken = default)
        {
            if (productFilter is null || productFilter.CountRows <= 0 || productFilter.Page <= 0)
                return new ResultPage<Product>() { Count = 0, Values = new List<Product>() };

            var query = @" select * from dbo.Product where IsDeleted = 0";
            var queryCount = @" select COUNT(*) from dbo.Product where IsDeleted = 0";

            dynamic filter = new ExpandoObject();

            if (productFilter.Id != null)
            {
                query += " AND Id = @Id";
                queryCount += " AND Id = @Id";

                filter.Id = productFilter.Id;
            }

            if (!string.IsNullOrWhiteSpace(productFilter.SearchString))
            {
                query += " AND SearchString like @SearchString";
                queryCount += " AND SearchString like @SearchString";

                filter.SearchString = $"%{productFilter.SearchString}%";
            }

            query += @$"
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                {queryCount}
            ";

            filter.Offset = (productFilter.Page - 1) * productFilter.CountRows;
            filter.PageSize = productFilter.CountRows;

            var results = await _session.Connection.QueryMultipleAsync(new CommandDefinition(query, filter as object, transaction: _session.Transaction, cancellationToken: cancellationToken));
            var rowsDapper = results.Read().ToList();
            var count = results.Read<long>().Single();

            if (rowsDapper is null)
                return new ResultPage<Product>() { Count = 0, Values = new List<Product>() };

            var sales = new List<Product>();

            foreach (var row in rowsDapper)
                sales.Add(await Map(row, cancellationToken));

            return new ResultPage<Product>() { Count = count, Values = sales };
        }

        public async Task<Product> Get(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Product 
                where IsDeleted = 0 AND Id = @Id";
            var rowProduct = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(new CommandDefinition(query, new { Id = id }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowProduct == null)
                return null;

            return await Map(rowProduct, cancellation);
        }

        public async Task<Product> GetLight(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Product 
                where IsDeleted = 0 AND Id = @Id";
            var rowProduct = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(new CommandDefinition(query, new { Id = id }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowProduct == null)
                return null;

            return await MapLight(rowProduct, cancellation);
        }

        public async Task<Product> Get(string code, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Product 
                where IsDeleted = 0 AND Code = @Code";
            var rowProduct = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(new CommandDefinition(query, new { Code = code }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowProduct == null)
                return null;

            return await Map(rowProduct, cancellation);
        }

        public async Task<Product> Update(Product entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Product
                SET 
                    Name = @Name,
                    Description = @Description,
                    CategoryId = @CategoryId,
                    Active = @Active,
                    SalePrice = @SalePrice,
                    Price = @Price,
                    Weight = @Weight,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            await _session.Connection.ExecuteAsync(
                new CommandDefinition(query, new { entity.Id, entity.Name, entity.Description, CategoryId = entity.Category.Id, entity.Active, entity.SalePrice, entity.Price, entity.SearchString }, transaction: _session.Transaction, cancellationToken: cancellation));

            await _fieldRepository.SyncCollection(entity, entity.Fields, cancellation);
            IEnumerable<Variant> variants = await _variantRepository.SyncCollection(entity, entity.Variants, cancellation);
            IEnumerable<ProductColor> colors = await _productColorRepository.SyncCollection(entity, entity.Colors, cancellation);

            await SyncCollectionSku(entity, entity.Skus, colors, variants, cancellation);

            return await Get(entity.Id, cancellation) ?? throw new KeyNotFoundException($"Product {entity.Code}-{entity.Name} no encontrado para actualizar.");
        }

        public async Task SyncCollectionSku(Product product, IEnumerable<ProductSku> productSkus, IEnumerable<ProductColor> productColors, IEnumerable<Variant> variants, CancellationToken cancellation = default)
        {
            if (product != null)
            {

                foreach (var sku in productSkus)
                {
                    if (sku.Color.Id <= 0)
                        sku.Color = productColors.First(c => string.Compare(c.Name, sku.Color.Name, true) == 0);
                    if (sku.Variant.Id <= 0)
                        sku.Variant = variants.First(v => string.Compare(v.Name, sku.Variant.Name, true) == 0);
                }
                await _productSkuRepository.SyncCollection(product, productSkus, cancellation);
            }
        }

        public async Task<Product> Map(dynamic rowDapper, CancellationToken cancellation = default)
        {
            if (rowDapper is null)
                return null;

            var product = await MapLight(rowDapper, cancellation);
            
            product.Variants = await _variantRepository.Get(product, cancellation);
            product.Colors = await _productColorRepository.Get(product, cancellation);
            product.Skus = await _productSkuRepository.Get(product, cancellation);

            return product;
        }

        public async Task<Product> MapLight(dynamic rowDapper, CancellationToken cancellation = default)
        {
            if (rowDapper is null)
                return null;

            var product = new Product();

            product.Id = rowDapper.Id;
            product.Code = rowDapper.Code;
            product.SearchString = rowDapper.SearchString;
            product.CreationDate = rowDapper.CreationDate;
            product.UpdateDate = rowDapper.UpdateDate;
            product.RemoveDate = rowDapper.RemoveDate;
            product.IsDeleted = rowDapper.IsDeleted;
            product.Guid = rowDapper.Guid;
            product.Version = rowDapper.Version;
            product.Name = rowDapper.Name;
            product.Description = rowDapper.Description;
            product.Active = rowDapper.Active;
            product.SalePrice = rowDapper.SalePrice;
            product.Price = rowDapper.Price;
            product.Weight = rowDapper.Weight;
            product.Category = await _categoryRepository.Get(rowDapper.CategoryId, cancellation);
            product.Fields = await _fieldRepository.Get(product, cancellation);
            
            return product;
        }
    }
}
