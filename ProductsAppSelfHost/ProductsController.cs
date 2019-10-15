using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;
using System.Net.Http.Headers;

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

        [Route("upload", Name = "UploadFile")]
        [HttpPost]
        public async Task<string> Upload()
        {
            //try
            //{
            var fileuploadPath = @"D:\";

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //byte[] contents = await Request.Content.ReadAsByteArrayAsync();

            //File.WriteAllBytes(@"D:\readAsBytes.zip", contents);

            //Console.WriteLine("File uploaded... ");

            //return "Ok";

            
            MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(fileuploadPath);

            // Read the MIME multipart asynchronously content using the stream provider we just created.

            await Request.Content.ReadAsMultipartAsync(streamProvider);

            string originalFileName = null, uploadingFileName = null;

            foreach (MultipartFileData file in streamProvider.FileData)
            {
                if (file.Headers.ContentDisposition.FileName != null)
                {
                    originalFileName = file.Headers.ContentDisposition.FileName.Trim(new Char[] { '"' });
                }

                uploadingFileName = file.LocalFileName;
            }

            if (String.IsNullOrEmpty(originalFileName) || String.IsNullOrEmpty(uploadingFileName))
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            Console.WriteLine($"Original File: {originalFileName} , Uploading File: {uploadingFileName}");

            //string uploadingFileName = streamProvider.FileData.Select(x => x.LocalFileName).FirstOrDefault();
            //string originalFileName = String.Concat(fileuploadPath, "\\" + (streamProvider.Contents[0].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' }));

            originalFileName = Path.Combine(fileuploadPath, originalFileName);
            if (File.Exists(originalFileName))
            {
                File.Delete(originalFileName);
            }
            Console.Write($"File Copied to {originalFileName}");
            File.Move(uploadingFileName, originalFileName);

            return "Ok";
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Exception: {0}", e.Message);
            //    return e.Message;
            //}
            
        }


        [Route("download/{fileName}", Name = "DownloadFile")]
        //[DeflateCompression]
        [HttpGet]
        public HttpResponseMessage Download(string fileName)
        {
            var fileuploadPath = @"D:\TestZip";
            string path = Path.Combine(fileuploadPath, fileName);

            if (!File.Exists(path))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            //try
            //{
            bool fullContent = true;
            MemoryStream responseStream = new MemoryStream();
            Stream fileStream = File.Open(path, FileMode.Open);

            if (this.Request.Headers.Range != null)
            {
                fullContent = false;

                // Currently we only support a single range.
                RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                // From specified, so seek to the requested position.
                if (range.From != null)
                {
                    fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                    // In this case, actually the complete file will be returned.
                    if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                    {
                        fileStream.CopyTo(responseStream);
                        fullContent = true;
                    }
                }
                if (range.To != null)
                {
                    // 10-20, return the range.
                    if (range.From != null)
                    {
                        long? rangeLength = range.To - range.From;
                        int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                        byte[] buffer = new byte[length];
                        fileStream.Read(buffer, 0, length);
                        responseStream.Write(buffer, 0, length);
                    }
                    // -20, return the bytes from beginning to the specified value.
                    else
                    {
                        int length = (int)Math.Min(range.To.Value, fileStream.Length);
                        byte[] buffer = new byte[length];
                        fileStream.Read(buffer, 0, length);
                        responseStream.Write(buffer, 0, length);
                    }
                }
                // No Range.To
                else
                {
                    // 10-, return from the specified value to the end of file.
                    if (range.From != null)
                    {
                        if (range.From < fileStream.Length)
                        {
                            int length = (int)(fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                }
            }
            // No Range header. Return the complete file.
            else
            {
                fileStream.CopyTo(responseStream);
            }
            fileStream.Close();
            responseStream.Position = 0;

            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent;
            response.Content = new StreamContent(responseStream);
            return response;
            //}
            //catch (IOException)
            //{
            //    throw new HttpResponseException(HttpStatusCode.InternalServerError);
            //}
        }
    }
}