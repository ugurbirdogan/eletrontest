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
    public class Menu
    {

        [TestMethod]
        public void GetMenu()
        {
            var ysclient = new YemekSepeti.Service.YemekSepetiClient();
            GetMenuModel result = ysclient.GetMenu();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Products);
            Assert.IsTrue(result.Products.Any());

            Assert.IsNotNull(result.Options);
            Assert.IsTrue(result.Options.Any());
            Assert.IsTrue(result.Products.Any(t => t.Id == "3ab12f4b4032f630981b54c059e5493e")); // Adana Kebap Exist

            var adanaKebap = result.Products.First(t => t.Id == "3ab12f4b4032f630981b54c059e5493e");
            var adanaKebapOptions = result.Options.Where(t => t.ProductId == "3ab12f4b4032f630981b54c059e5493e");
            
            var adanaRoll = result.Products.First(t => t.Id == "2af660342b6aa578c483d2e1ab69ff8d");
            var adanaRollOptions = result.Options.Where(t => t.Id == "2af660342b6aa578c483d2e1ab69ff8d");

        }
        [TestMethod]
        public void GetMenu2()
        {
            //var remoteAddress = new System.ServiceModel.EndpointAddress("http://messaging.yemeksepeti.com/messagingwebservice/integration.asmx");
            //AuthHeader auth = new AuthHeader()
            //{
            //    UserName = "ultimate",
            //    Password = "36zn7mjn"
            //};
            //IntegrationSoap client = new IntegrationSoapClient(new System.ServiceModel.BasicHttpBinding(), remoteAddress);
            //...
        }


    }
}
