using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using YemekSepeti.Service;
using YemekSepeti.Service.YSServiceReference;

namespace YemekSepetiTEST
{

    [TestClass]
    public class Restaurant
    {

        [TestMethod]
        public void GetRestaurantList()
        {
            var ysclient = new YemekSepeti.Service.YemekSepetiClient();
            var result = ysclient.GetRestaurantList();
            Assert.IsNotNull(result);
            Assert.AreEqual("postakip", result.First().CatalogName);
            Assert.AreEqual("69f8d972-6470-4165-abc3-a7e9a6670aab", result.First().CategoryName);
        }

        [TestMethod]
        public void GetRestaurantList2()
        {
            var remoteAddress = new System.ServiceModel.EndpointAddress("http://messaging.yemeksepeti.com/messagingwebservice/integration.asmx");
            AuthHeader auth = new AuthHeader()
            {
                UserName = "ultimate",
                Password = "36zn7mjn"
            };
            IntegrationSoap client = new IntegrationSoapClient(new System.ServiceModel.BasicHttpBinding(), remoteAddress);
            GetRestaurantListRequest getRestaurantListRequest = new GetRestaurantListRequest(auth);
            var result = client.GetRestaurantList(getRestaurantListRequest).GetRestaurantListResult;
        }


    }
}
