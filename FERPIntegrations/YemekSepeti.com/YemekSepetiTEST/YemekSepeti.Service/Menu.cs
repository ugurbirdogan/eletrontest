using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Service.YSServiceReference;

namespace YemekSepeti.Service
{
    /*
     * GetMenu():
     * GetMenu is used in order to get authenticated restaurant’s menu. It takes no parameters and returns DataSet. 
     * Returns null if authentication failed.
     * The Menu DataSet name is “Menu” and it contains two tables: Products and Options
     * Products Table:
     * Column Name      Description
     * Id               Product’s Identifier
     * Name             Product’s Name
     * Price            Unit price for the product
     * Title            Product’s submenu title
     * Description      Product’s description
     * Options Table:
     * Column Name      Description
     * Id               Option’s Identifier
     * ProductId        Parent product’s Identifier
     * Name             Option’s Name
     * Price            Additional price for the option
     * Type             Type of the option    
     */

    public partial class YemekSepetiClient
    {
        public GetMenuModel GetMenu()
        {
            GetMenuRequest getMenuRequest = new GetMenuRequest(Auth);
            var response = client.GetMenu(getMenuRequest).GetMenuResult;
            GetMenuModel result = new GetMenuModel();
            result.Products= Map.DatatableToClass<Products>(response.Tables["Products"]).ToList();
            result.Options = Map.DatatableToClass<Options>(response.Tables["Options"]).ToList();
            return result;
        }
    }
    public class GetMenuModel
    {
        
        public List<Products> Products { get; set; } = new List<Products>();
        public List<Options> Options { get; set; } = new List<Options>();
    }
    public class Products
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Options
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
    }
}
