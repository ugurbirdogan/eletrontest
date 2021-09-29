using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Service.YSServiceReference;

namespace YemekSepeti.Service
{
    /*
     * GetPaymentTypes()
     * GetPaymentTypes is used in order to get Yemeksepeti Payment Methods. 
     * Takes no parameters and returns DataSet. Returns null if authentication failed.
     * The PaymentTypes DataSet contains only one table: PaymentMethods
     * Products Table:
     * Column Name      Description 
     * Id               Payment Type’s identifier
     * Name             Payment Type’s name
     * Description      Payment Type’s description
     */

    public partial class YemekSepetiClient
    {
        public IEnumerable<GetPaymentTypesModel> GetPaymentTypes()
        {
            GetPaymentTypesRequest getPaymentTypesRequest = new GetPaymentTypesRequest(Auth);
            var response = client.GetPaymentTypes(getPaymentTypesRequest).GetPaymentTypesResult;
            var result = Map.DatatableToClass<GetPaymentTypesModel>(response.Tables["PaymentMethods"]).ToList();
            return result;
        }
    }
    public class GetPaymentTypesModel
    {

        public int Id { get; set; }
        public string name { get; set; }
        public string commerceName { get; set; }
        public string description { get; set; }
    }


}
