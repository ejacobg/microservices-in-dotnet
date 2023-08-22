using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.Controllers
{
    [Route("/products")]
    public class ProductCatalogController : ControllerBase
    {
        private readonly IProductStore _productStore;

        public ProductCatalogController(IProductStore productStore) => _productStore = productStore;

        [HttpGet("")]
        [ResponseCache(Duration = 86400)] // Responses are valid for 24 hours.
        public IEnumerable<ProductCatalogProduct> Get([FromQuery] string productIds)
        {
            var products = _productStore.GetProductsByIds(ParseProductIdsFromQueryString(productIds));
            return products;
        }

        private static IEnumerable<int> ParseProductIdsFromQueryString(string productIdsString) => productIdsString
            .Split(',').Select(s => s.Replace("[", "").Replace("]", "")).Select(int.Parse);
    }

    /// <summary>
    /// IProductStore represents a service for managing product data.
    /// </summary>
    public interface IProductStore
    {
        IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds);
    }

    public class ProductStore : IProductStore
    {
        // GetProductsByIds generates fake products given a list of IDs.
        public IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds) =>
            productIds.Select(id => new ProductCatalogProduct(id, "foo" + id, "bar", new Money()));
    }

    public record ProductCatalogProduct(int ProductId, string ProductName, string Description, Money Price);

    public record Money;
}