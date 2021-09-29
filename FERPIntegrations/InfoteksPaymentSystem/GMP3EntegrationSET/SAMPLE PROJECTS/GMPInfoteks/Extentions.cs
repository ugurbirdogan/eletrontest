using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GMP3Infoteks.Extention;
using GMP3Infoteks.OKC;

namespace GMPInfoteks
{
    public static class Extentions
    {
        public static string ToTrString(this bool value)
        {
            if (value)
                return "Evet";
            return "Hayır";
        }

        public static DataTable ToDataTable<T>(this List<T> list)
        {

            Type pt = typeof(T);
            DataTable dtResult = new DataTable();

            System.Reflection.PropertyInfo[] pList = pt.GetProperties();
            foreach (System.Reflection.PropertyInfo item in pList)
            {
                dtResult.Columns.Add(item.Name, item.PropertyType);
            }

            foreach (T item in list)
            {
                DataRow drPr = dtResult.NewRow();
                foreach (System.Reflection.PropertyInfo pi in pList)
                {
                    drPr[pi.Name] = pi.GetValue(item, null);
                }
                dtResult.Rows.Add(drPr);
            }
            return dtResult;
        }
        public static List<T> ToList<T>(this DataTable dt) where T:class,new() 
        {

            Type pt = typeof(T);
            List<T> lResult = new List<T>();

            System.Reflection.PropertyInfo[] pList = pt.GetProperties();
            

            foreach (DataRow row in dt.AsEnumerable())
            {
                T newt=new T();
                foreach (System.Reflection.PropertyInfo pi in pList)
                {
                    if (!Convert.IsDBNull(row[pi.Name]) && dt.Columns.Contains(pi.Name) )
                        pi.SetValue(newt, row[pi.Name], null);
                }
                lResult.Add(newt);
            }
            return lResult;
        }

        public static decimal TotalPrice(this OKCProductBase prd)
        {
            decimal price=0;
            if (prd.ProductType == OProductType.BarcodeProduct)
                price = (prd as OKCBarcodeProduct).Price;
            else if (prd.ProductType == OProductType.PLUProduct)
                price = (prd as OKCPLUProduct).Price;
            else if (prd.ProductType == OProductType.NormalProduct)
                price = (prd as OKCProduct).Price;
            else if (prd.ProductType == OProductType.InfProduct)
                price = (prd as OKCInfProduct).Price;
            else if (prd.ProductType == OProductType.Cancel)
                price = (prd as OKCCancelProduct).Price;
            return Math.Round( price * (decimal)prd.Quantity,3);
        }
    }
}
