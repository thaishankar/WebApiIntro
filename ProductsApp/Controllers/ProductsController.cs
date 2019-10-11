using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProductStore.Models;

namespace ProductStore.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        static readonly IProductRepository repository = new ProductRepository();

        [Route("")]
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return repository.GetAll();
        }

        [HttpGet]
        [Route("id/{id:int}", Name = "GetProductById")]
        public Product GetProduct(int id)
        {
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


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;
//using ProductStore.Models;

//namespace ProductsApp.Controllers
//{
//    [RoutePrefix("products")]
//    public class ProductsController : ApiController
//    {
//        private List<Product> products;

//        static readonly IProductRepository repository = new ProductRepository();

//        public ProductsController()
//        {
//            products = new List<Product>();

//            products.Add(new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 });
//            products.Add(new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M });
//            products.Add(new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M });
//        }

//        [Route("")]
//        [HttpGet]
//        public IEnumerable<Product> GetAllProducts()
//        {
//            return repository.GetAll();
//            //return products;
//        }

//        //[Route("{id:int}", Name = "GetProductsById")]
//        [Route("GetProductsById")]
//        [HttpGet]
//        public Product GetProduct(int id)
//        {
//            //var product = products.FirstOrDefault((p) => p.Id == id);
//            //if (product == null)
//            //{
//            //    return NotFound();
//            //}
//            //return Ok(product);

//            Product item = repository.Get(id);
//            if (item == null)
//            {
//                throw new HttpResponseException(HttpStatusCode.NotFound);
//            }
//            return item;
//        }

//        // http://localhost:3912/products/AddProduct?id=4&name=test&category=Groceries&prices=10
//        [Route("AddProduct")]
//        [HttpPost]
//        public Product AddProduct(int id, string name, string category, decimal price)
//        {
//            Product newProduct = new Product();
//            newProduct.Id = id;
//            newProduct.Name = name;
//            newProduct.Category = category;
//            newProduct.Price = price;

//            products.Add(newProduct);

//            return newProduct;
//        }

//        [Route("upload", Name = "UploadFile")]
//        [HttpPost]
//        public async Task<string> Upload()
//        {
//            try
//            {
//                var fileuploadPath = @"D:\testfile.txt";

//                var provider = new MultipartFormDataStreamProvider(fileuploadPath);
//                var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
//                foreach (var header in Request.Content.Headers)
//                {
//                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
//                }

//                await content.ReadAsMultipartAsync(provider);

//                return "Ok";
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("Exception: {0}", e.Message);
//                return e.Message;
//            }

//        }
//    }
//}
