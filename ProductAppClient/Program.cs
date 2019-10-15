#region snippet_all
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

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
        static HttpClient client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression =
    System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        });
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
            string ipAddress = "localhost";
            int port = REMOTE_PORT;

            if (args.Length >= 1)
            {
                ipAddress = args[0];
            }
            
            if (args.Length >= 2)
            {
                Int32.TryParse(args[1], out port);
            }

            Console.WriteLine("IpAddress = {0}", ipAddress);

            string baseAddress = string.Format("http://{0}:{1}/", ipAddress, port);

            // Update port # in the following line.
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            
            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion


            //try
            //{

                Console.WriteLine("Get Product ID: 1");
                Product product = await GetProductAsync("products/id/1");
                ShowProduct(product);

                //Product newProduct = new Product
                //{
                //    Name = "Gizmo",
                //    Price = 100,
                //    Category = "Widgets"
                //};

                //var url = await CreateProductAsync(newProduct);
                //Console.WriteLine($"Created at {url}");

                //string file = @"File100mb_2.zip";

                //await UploadFileToServer(file, baseAddress);

                //string downloadFile = @"File100mb_1.zip";
                await DownloadFileFromServer();
                
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}

            Console.ReadLine();
        }

        public static async Task DownloadFileFromServer()
        {
            //HttpClient client = HttpClientFactory.Create();

            string statsFile = @"WebApiStats-MaxLimit-10MB-Final.csv";

            string headers = "FileName, FileNum, Size, DownloadTime\n";
            File.AppendAllText(statsFile, headers);

            Random random = new Random();
            byte[] contents;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            while (true)
            {
                for (int i = 0; i < 2; i++)
                {
                    stopWatch.Restart();

                    int id = random.Next(10);
                    int size = (int)Math.Pow(10, i);

                    string fileName = string.Format(@"File{0}mb_{1}.zip", size, id);

                    HttpResponseMessage response = await client.GetAsync("products/download/" + fileName);

                    response.EnsureSuccessStatusCode();

                    contents = await response.Content.ReadAsByteArrayAsync();

                    long downloadTime = stopWatch.ElapsedMilliseconds;

                    File.WriteAllBytes("Out_" + fileName, contents);

                    long endTime = stopWatch.ElapsedMilliseconds;

                    string stats = string.Format("{0}, {1}, {2}, {3}\n", fileName, id, size, stopWatch.ElapsedMilliseconds);
                    File.AppendAllText(statsFile, stats);

                    //Console.WriteLine("Download time = {0}ms, WriteTime: {1}ms TotalTime: {2}ms", downloadTime, endTime - downloadTime, endTime);

                    Thread.Sleep(1000);
                }
            }
        }

        public static async Task UploadFileToServer(string fileName, string baseAddress)
        {
            const int BufferSize = 1024;
            //HttpClient client = HttpClientFactory.Create();

            // Set the request timeout as large uploads can take longer than the default 2 minute timeout
            //client.Timeout = TimeSpan.FromMinutes(20);

            string absolutePath = Path.Combine(@"D:\TestZip", fileName); 

            Console.WriteLine("Upload file: " + absolutePath);


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Open the file we want to upload and submit it
            using (FileStream fileStream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, useAsync: true))
            {
                // Create a stream content for the file
                StreamContent content = new StreamContent(fileStream, BufferSize);

                // Create Multipart form data content, add our submitter data and our stream content
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StringContent("Me"), "submitter");
                formData.Add(content, "filename", fileName);

                // Post the MIME multipart form data upload with the file
                Uri address = new Uri(baseAddress + "products/upload");
                HttpResponseMessage response = await client.PostAsync(address, formData);

                response.EnsureSuccessStatusCode();

                //FileResult result = await response.Content.ReadAsAsync<FileResult>();
                //Console.WriteLine("{0}Result:{0}  Filename:  {1}{0}  Submitter: {2}", Environment.NewLine, result.FileNames.FirstOrDefault(), result.Submitter);
            }

            Console.WriteLine("Upload time = {0}ms", stopWatch.ElapsedMilliseconds);
            //}

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