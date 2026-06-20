using AutoMapper;
using DK.Domain.Products;
using DK.Domain.Sales;
using DK.Process.Product;
using DK.Process.Sales;
using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.Products;
using DK.WebApi.ViewModel.Sales;
using Microsoft.AspNetCore.Mvc;

namespace DK.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ProductProcess _productProcess;

        public ProductController(ProductProcess productProcess, IMapper mapper)
        {
            _productProcess = productProcess;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<ProductResponse>> GetAll()
        {
            var products = await _productProcess.GetAll();
            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

        [HttpPost("GetPage")]
        public async Task<ResultPageResponse<ProductResponse>> GetPage([FromBody] ProductFilter cityFilter)
        {
            var resultPage = await _productProcess.GetPage(cityFilter);
            return _mapper.Map<ResultPageResponse<ProductResponse>>(resultPage);
        }

        [HttpGet("GetById")]
        public async Task<ProductResponse> Get([FromQuery(Name = "Id")] long id)
        {
            var product = await _productProcess.Get(id);
            return _mapper.Map<ProductResponse>(product);
        }

        [HttpPost("Create")]
        public async Task<ProductResponse> Create([FromBody] ProductRequest data)
        {
            var product = await _productProcess.Create(_mapper.Map<Product>(data));
            return _mapper.Map<ProductResponse>(product);
        }

        [HttpPost("Update")]
        public async Task<ProductResponse> Update([FromBody] ProductRequest data)
        {
            var product = await _productProcess.Update(_mapper.Map<Product>(data));
            return _mapper.Map<ProductResponse>(product);
        }

        [HttpPost("Delete")]
        public async Task Delete([FromBody] ProductRequest data)
        {
            await _productProcess.Delete(_mapper.Map<Product>(data));
        }
    }
}
