using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.ResponseCompression;

namespace ProductsAppSelfHost
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            appBuilder.UseWebApi(config);
        }

        public static string GetLocalIpAddress()
        {
            IPAddress localIpAddress = IPAddress.Loopback;
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in adapters)
            {
                // We take the adapter as infrastructure interface if the adapter ip matches RoleInstanceIpAddress from registry
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet && (adapter.Description.StartsWith("Microsoft Hyper-V Network Adapter") ||
                                                        adapter.Description.StartsWith("Intel(R) Ethernet Connection")))
                {
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection unicastAddresses = ipProperties.UnicastAddresses;
                    if (unicastAddresses != null)
                    {
                        foreach (UnicastIPAddressInformation ipAddress in unicastAddresses)
                        {
                            localIpAddress = ipAddress.Address;
                        }
                    }
                }
            }
            return localIpAddress.ToString();
        }

        static void Main()
        {
            string baseAddress = string.Format("http://localhost:3914/");
            WebApp.Start<Startup>(url: baseAddress);

            string localIp = GetLocalIpAddress();
            Console.WriteLine("Local IP Address : {0}", localIp);

            string serverIpAddress = string.Format("http://{0}:3912/", localIp);
            WebApp.Start<Startup>(url: serverIpAddress);

            // Start OWIN host 
            //using (WebApp.Start<Startup>(url: baseAddress))
            //{
            //    // Create HttpCient and make a request to api/values 
            //    HttpClient client = new HttpClient();

            //    var response = client.GetAsync(baseAddress + "products").Result;

            //    Console.WriteLine(response);
            //    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            //    Console.ReadLine();
            //}

            Console.ReadLine();
        }
    }
}
