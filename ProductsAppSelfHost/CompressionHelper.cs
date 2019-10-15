//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http.Filters;

//namespace ProductsAppSelfHost
//{
//    public class DeflateCompressionAttribute : ActionFilterAttribute
//    {

//        //public override void OnActionExecuted(HttpActionExecutedContext actContext)
//        //{
//        //    var content = actContext.Response.Content;
//        //    var bytes = content == null ? null : content.ReadAsByteArrayAsync().Result;
//        //    var zlibbedContent = bytes == null ? new byte[0] :
//        //    CompressionHelper.DeflateByte(bytes);
//        //    actContext.Response.Content = new ByteArrayContent(zlibbedContent);
//        //    actContext.Response.Content.Headers.Remove("Content-Type");
//        //    actContext.Response.Content.Headers.Add("Content-encoding", "gzip");
//        //    actContext.Response.Content.Headers.Add("Content-Type", "application/json");
//        //    base.OnActionExecuted(actContext);
//        //}

//        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
//        {
//            //bool isCompressionSupported = CompressionHelper.IsCompressionSupported();
//            //string acceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

//            //if (isCompressionSupported)
//            //{
//            var content = actionContext.Response.Content;
//            var byteArray = content == null ? null : content.ReadAsByteArrayAsync().Result;

//            MemoryStream memoryStream = new MemoryStream(byteArray);

//            //if (acceptEncoding.Contains("gzip"))
//            //{
//            actionContext.Response.Content = new ByteArrayContent(CompressionHelper.Compress(memoryStream.ToArray(), false));
//            actionContext.Response.Content.Headers.Remove("Content-Type");
//            actionContext.Response.Content.Headers.Add("Content-encoding", "gzip");
//            actionContext.Response.Content.Headers.Add("Content-Type", "application/json");
//            Console.WriteLine("Encoding done... ");
//            //}
//            //else
//            //{
//            //    actionContext.Response.Content = new ByteArrayContent(CompressionHelper.Compress(memoryStream.ToArray()));
//            //    actionContext.Response.Content.Headers.Remove("Content-Type");
//            //    actionContext.Response.Content.Headers.Add("Content-encoding", "deflate");
//            //    actionContext.Response.Content.Headers.Add("Content-Type", "application/json");
//            //}
//            //}

//            base.OnActionExecuted(actionContext);
//        }
//    }

//    public class CompressionHelper
//    {
//        public static bool IsCompressionSupported()
//        {
//            string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

//            return ((!string.IsNullOrEmpty(AcceptEncoding) &&
//                    (AcceptEncoding.Contains("gzip") || AcceptEncoding.Contains("deflate"))));
//        }

//        public static byte[] Compress(byte[] data, bool useGZipCompression = true)
//        {
//            System.IO.Compression.CompressionLevel compressionLevel = System.IO.Compression.CompressionLevel.Fastest;

//            using (MemoryStream memoryStream = new MemoryStream())
//            {
//                if (useGZipCompression)
//                {
//                    using (System.IO.Compression.GZipStream gZipStream = new System.IO.Compression.GZipStream(memoryStream, compressionLevel, true))
//                    {
//                        gZipStream.Write(data, 0, data.Length);
//                    }
//                }
//                else
//                {
//                    using (System.IO.Compression.GZipStream gZipStream = new System.IO.Compression.GZipStream(memoryStream, compressionLevel, true))
//                    {
//                        gZipStream.Write(data, 0, data.Length);
//                    }
//                }

//                return memoryStream.ToArray();
//            }
//        }

//        public static byte[] DeflateByte(byte[] str)
//        {
//            if (str == null)
//            {
//                return null;
//            }

//            using (var output = new MemoryStream())
//            {
//                using (
//                    var compressor = new Ionic.Zlib.GZipStream(
//                    output, Ionic.Zlib.CompressionMode.Compress,
//                    Ionic.Zlib.CompressionLevel.BestSpeed))
//                {
//                    compressor.Write(str, 0, str.Length);
//                }

//                return output.ToArray();
//            }
//        }
//    }
//}
