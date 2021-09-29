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
    public class Message
    {

        [TestMethod]
        public void GetMessage()
        {
            var ysclient = new YemekSepeti.Service.YemekSepetiClient();
            var result = ysclient.GetMessage();

        }

        [TestMethod]
        public void GetAllMessages()
        {
            var ysclient = new YemekSepeti.Service.YemekSepetiClient();
            var result = ysclient.GetAllMessages();

        }



    }
}
