using AutoMapper;
using DK.Process.Product;
using DK.WebApi.ViewModel.Products;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers.Products
{
    [ApiController]
    [Route("[controller]")]
    public class ProductSkuController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ProductSkuProcess _productSkuProcess;

        public ProductSkuController(ProductSkuProcess productSkuProcess, IMapper mapper)
        {
            _productSkuProcess = productSkuProcess;
            _mapper = mapper;
        }

        [HttpGet("GetBySku")]
        public async Task<ProductSkuResponse> GetBySku([FromQuery(Name = "Sku")] string sku)
        {
            var productSku = await _productSkuProcess.GetBySku(sku);
            return _mapper.Map<ProductSkuResponse>(productSku);
        }

    }
}
