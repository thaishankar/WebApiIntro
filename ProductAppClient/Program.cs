#region snippet_all
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace HttpClientSample
{
    #region snippet_prod
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
    #endregion

    class Program
    {
        static int REMOTE_PORT = 3914;

        #region snippet_HttpClient
        static HttpClient client = new HttpClient();
        #endregion

        static void ShowProduct(Product product)
        {
            Console.WriteLine($"Name: {product.Name}\tPrice: " +
                $"{product.Price}\tCategory: {product.Category}");
        }

        #region snippet_CreateProductAsync
        static async Task<Uri> CreateProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "products/product", product);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
        #endregion

        #region snippet_GetProductAsync
        static async Task<Product> GetProductAsync(string path)
        {
            Product product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Product>();
            }
            return product;
        }
        #endregion

        #region snippet_UpdateProductAsync
        static async Task<Product> UpdateProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"products/{product.Id}", product);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            product = await response.Content.ReadAsAsync<Product>();
            return product;
        }
        #endregion

        #region snippet_DeleteProductAsync
        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"products/{id}");
            return response.StatusCode;
        }
        #endregion

        static void Main(string[] args)
        {
            RunAsync(args).GetAwaiter().GetResult();
        }

        #region snippet_run
        #region snippet5
        static async Task RunAsync(string[] args)
        {
            string ipAddress;

            if (args.Length >= 1)
            {
                ipAddress = args[0];
            }
            else
            {
                ipAddress = "localhost";
            }

            Console.WriteLine("IpAddress = {0}", ipAddress);

            // Update port # in the following line.
            client.BaseAddress = new Uri(string.Format("http://{0}:{1}/", ipAddress, REMOTE_PORT));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion


            try
            {

                Console.WriteLine("Get Product ID: 1");
                Product product = await GetProductAsync("products/id/1");
                ShowProduct(product);

                Product newProduct = new Product
                {
                    Name = "Gizmo",
                    Price = 100,
                    Category = "Widgets"
                };

                var url = await CreateProductAsync(newProduct);
                Console.WriteLine($"Created at {url}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.ReadLine();
        }


        //try
        //{
        //    // Create a new product
        //    Product product = new Product
        //    {
        //        Name = "Gizmo",
        //        Price = 100,
        //        Category = "Widgets"
        //    };

            //    var url = await CreateProductAsync(product);
            //    Console.WriteLine($"Created at {url}");

            // Get the product
            //        Console.WriteLine($"Url Path and Query {url.PathAndQuery}");
            //            product = await GetProductAsync(url.PathAndQuery);
            //            ShowProduct(product);

            //            Console.WriteLine("List of Products for Category: Widgets");

            //            //foreach(var product in GetProductAsync("/products/"))

            //            // Update the product
            //            Console.WriteLine("Updating price...");
            //            product.Price = 80;
            //            await UpdateProductAsync(product);

            //            // Get the updated product
            //            product = await GetProductAsync(url.PathAndQuery);
            //            ShowProduct(product);

            //            // Delete the product
            //            var statusCode = await DeleteProductAsync(product.Id);
            //            Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e.Message);
            //        }

            //        Console.ReadLine();
            //    }
            #endregion
        }
}
#endregion


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using Newtonsoft.Json;

//namespace HttpClientSample
//{
//    public class Product
//    {
//        //public string Id { get; set; }
//        public string Name { get; set; }
//        public decimal Price { get; set; }
//        public string Category { get; set; }
//    }

//    class Program
//    {
//        static void Main(string[] args)
//        {
//            using (var client = new HttpClient())
//            {
//                client.BaseAddress = new Uri("http://localhost:3912/");

//                //HTTP GET
//                var responseTask = client.GetAsync("products");
//                responseTask.Wait();

//                var result = responseTask.Result;
//                if (result.IsSuccessStatusCode)
//                {

//                    var readTask = result.Content.ReadAsAsync<Product[]>();
//                    readTask.Wait();

//                    var products = readTask.Result;

//                    foreach (var product in products)
//                    {
//                        Console.WriteLine(product.Name);
//                    }
//                }
//            }
//        }
//    }


//    //class Program
//    //{
//    //    static HttpClient client = new HttpClient();

//    //    static void ShowProduct(Product product)
//    //    {
//    //        Console.WriteLine($"Name: {product.Name}\tPrice: " +
//    //            $"{product.Price}\tCategory: {product.Category}");
//    //    }

//    //    static async Task<Uri> CreateProductAsync(Product product)
//    //    {
//    //        HttpResponseMessage response = await client.PostAsJsonAsync(
//    //            "products", product);
//    //        response.EnsureSuccessStatusCode();

//    //        // return URI of the created resource.
//    //        return response.Headers.Location;
//    //    }

//    //    static async Task<Product> GetProductAsync(string path)
//    //    {
//    //        Product product = null;
//    //        HttpResponseMessage response = await client.GetAsync(path);
//    //        if (response.IsSuccessStatusCode)
//    //        {
//    //            product = await response.Content.ReadAsAsync<Product>();
//    //        }
//    //        return product;
//    //    }

//    //    static async Task<Product> UpdateProductAsync(Product product)
//    //    {
//    //        HttpResponseMessage response = await client.PutAsJsonAsync(
//    //            $"products/{product.Id}", product);
//    //        response.EnsureSuccessStatusCode();

//    //        // Deserialize the updated product from the response body.
//    //        product = await response.Content.ReadAsAsync<Product>();
//    //        return product;
//    //    }

//    //    static async Task<HttpStatusCode> DeleteProductAsync(string id)
//    //    {
//    //        HttpResponseMessage response = await client.DeleteAsync(
//    //            $"products/{id}");
//    //        return response.StatusCode;
//    //    }

//    //    static void Main()
//    //    {
//    //        RunAsync().GetAwaiter().GetResult();
//    //    }

//    //    static async Task RunAsync()
//    //    {
//    //        // Update port # in the following line.
//    //        client.BaseAddress = new Uri("http://localhost:64195/");
//    //        client.DefaultRequestHeaders.Accept.Clear();
//    //        client.DefaultRequestHeaders.Accept.Add(
//    //            new MediaTypeWithQualityHeaderValue("application/json"));

//    //        try
//    //        {
//    //            // Create a new product
//    //            Product product = new Product
//    //            {
//    //                Name = "Gizmo",
//    //                Price = 100,
//    //                Category = "Widgets"
//    //            };

//    //            var url = await CreateProductAsync(product);
//    //            Console.WriteLine($"Created at {url}");

//    //            // Get the product
//    //            product = await GetProductAsync(url.PathAndQuery);
//    //            ShowProduct(product);

//    //            // Update the product
//    //            Console.WriteLine("Updating price...");
//    //            product.Price = 80;
//    //            await UpdateProductAsync(product);

//    //            // Get the updated product
//    //            product = await GetProductAsync(url.PathAndQuery);
//    //            ShowProduct(product);

//    //            // Delete the product
//    //            var statusCode = await DeleteProductAsync(product.Id);
//    //            Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

//    //        }
//    //        catch (Exception e)
//    //        {
//    //            Console.WriteLine(e.Message);
//    //        }

//    //        Console.ReadLine();
//    //    }
//    //}
//}