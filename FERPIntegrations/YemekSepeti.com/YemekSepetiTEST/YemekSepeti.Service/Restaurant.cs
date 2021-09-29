/*
Architectural Design Question and Options
Best Option
Put all integration requirements here and close it until one side of integration changed.
Option 2 
For simplification put service referance into project and do everything same code with FERP
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Service.YSServiceReference;

namespace YemekSepeti.Service
{
    /// <summary>
    /// 
    /// </summary>
    public partial class YemekSepetiClient
    {
        

        /*
         * GetRestaurantList is used in order to get the restaurant list that restaurant chain contains. Takes no parameters and returns DataSet. Returns null
         * if authentication failed.
         * The RestaurantList DataSet contains only one table: Restaurants
         * Products Table:
         * Column Name     Description
         * CatalogName     Restaurant’s catalog name.
         * CategoryName    Restaurant’s category name.
         * DisplayName     Restaurant’s display name.
         */
        public IEnumerable<GetRestaurantListModel> GetRestaurantList()
        {
            GetRestaurantListRequest getRestaurantListRequest = new GetRestaurantListRequest(Auth);
            var response = client.GetRestaurantList(getRestaurantListRequest).GetRestaurantListResult;
            return Map.DatatableToClass<GetRestaurantListModel>(response.Tables[0]);
        }
    }

    public class GetRestaurantListModel
    {
        public string CatalogName { get; set; }
        public string CategoryName { get; set; }
        public string DisplayName { get; set; }
    }
}
