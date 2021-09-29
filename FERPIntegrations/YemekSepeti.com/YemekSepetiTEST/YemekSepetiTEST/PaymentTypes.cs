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
    public class GetPaymentType
    {

        [TestMethod]
        public void GetPaymentTypes()
        {
            var ysclient = new YemekSepeti.Service.YemekSepetiClient();
            List<GetPaymentTypesModel> result = ysclient.GetPaymentTypes().ToList();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            Assert.IsTrue(result.First(t => t.Id == 1).name == "Peşin"); // Cash Payment Type
        }

        [TestMethod]
        public void GetPaymentTypes2()
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
