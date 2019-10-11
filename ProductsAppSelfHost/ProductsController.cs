using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProductsAppSelfHost
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        static readonly IProductRepository repository = new ProductRepository();

        [Route("")]
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            Console.WriteLine("Recevied Request for all products");
            return repository.GetAll();
        }

        [HttpGet]
        [Route("id/{id:int}", Name = "GetProductById")]
        public Product GetProduct(int id)
        {
            Console.WriteLine($"Received Request for Product ID: {id}");
            Product item = repository.Get(id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return item;
        }

        [HttpGet]
        [Route("category/{category}", Name = "GetProductByCategory")]
        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            if (String.IsNullOrEmpty(category))
            {
                return repository.GetAll();
            }

            return repository.GetAll().Where(
                p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        [HttpPost]
        [Route("product")]
        public HttpResponseMessage PostProduct(Product item)
        {
            item = repository.Add(item);
            var response = Request.CreateResponse<Product>(HttpStatusCode.Created, item);

            string uri = Url.Link("GetProductById", new { id = item.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        [HttpPut]
        [Route("id/{id}/product")]
        public void PutProduct(int id, Product product)
        {
            product.Id = id;
            if (!repository.Update(product))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpDelete]
        [Route("id/{id}")]
        public void DeleteProduct(int id)
        {
            repository.Remove(id);
        }
    }
}