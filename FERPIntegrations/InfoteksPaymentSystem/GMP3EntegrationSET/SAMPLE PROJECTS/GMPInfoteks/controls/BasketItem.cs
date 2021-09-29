using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMP3Infoteks.OKC;

namespace GMPInfoteks.controls
{
    public class BasketItem
    {
        public BasketItem(OKCProductBase prd)
        {
            PrdIncrease = 0;
            PrdDiscount = 0;
            OKCPrd = prd;
        }

        public OKCProductBase OKCPrd { get; set; }
        public decimal PrdIncrease { get; set; }
        public decimal PrdDiscount { get; set; }

        public decimal NetAmount 
        {
            get
            {
                return OKCPrd.TotalPrice() - PrdDiscount + PrdIncrease;
            }
        }

        public string ItemName
        {
            get
            {
                switch (OKCPrd.ProductType)
                {
                    case OProductType.NormalProduct:
                        return (OKCPrd as OKCProduct).ProductName;
                    case OProductType.PLUProduct:
                        return (OKCPrd as OKCPLUProduct).PLU;
                    case OProductType.BarcodeProduct:
                        return (OKCPrd as OKCBarcodeProduct).Barcode;
                    case OProductType.InfProduct:
                        return (OKCPrd as OKCInfProduct).ProductName;
                    case OProductType.Discount:
                    case OProductType.Increase:
                        return (OKCPrd as OKCDiscount).ProductName;
                    case OProductType.Cancel:
                        return (OKCPrd as OKCCancelProduct).ProductName;
                    default:
                        break;
                }
                return "";
            }
        }

        public string[] GetLVStrings()
        {
            if (OKCPrd.ProductType == OProductType.Increase
                    || OKCPrd.ProductType == OProductType.Discount)
            {
                OKCDiscount disc = OKCPrd as OKCDiscount;

                return new string[] { disc.ProductName, "", "", (disc.DiscountMethod == DiscountMethod.Percent ? "%" : "") + disc.Discount.ToString("N2"), "" };

            }
            else if (OKCPrd.ProductType == OProductType.InfProduct)
            {
                OKCInfProduct inf = OKCPrd as OKCInfProduct;
                return new string[] { inf.ProductName, "", inf.Quantity.ToString("N3"), inf.Price.ToString("N3"), inf.TotalPrice().ToString("N3") };
            }
            else if (OKCPrd.ProductType == OProductType.NormalProduct)
            {
                OKCProduct prd = OKCPrd as OKCProduct;
                return new string[]{prd.ProductName,string.Format("%{0}",prd.VatRate),prd.Quantity.ToString("N3"),prd.Price.ToString("N3")
                        ,NetAmount.ToString("N3")};
            }
            else if (OKCPrd.ProductType == OProductType.Cancel)
            {
                OKCCancelProduct prd = OKCPrd as OKCCancelProduct;
                return new string[]{prd.ProductName,string.Format("%{0}",prd.VatRate),prd.Quantity.ToString("N3"),prd.Price.ToString("N3")
                        ,NetAmount.ToString("N3")};
            }
            else if (OKCPrd.ProductType == OProductType.PLUProduct
                || OKCPrd.ProductType == OProductType.BarcodeProduct)
            {
                OKCBarcodeProduct prd = OKCPrd as OKCBarcodeProduct;
                return new string[]{prd.Barcode,"",prd.Quantity.ToString("N3")
                    ,prd.Price.ToString("N3"),NetAmount.ToString("N3")};
            }
            return new string[] { "", "", "", "", "" };
        }

    }

    public class PayBasketItem
    {
        public PayBasketItem(OKCBankPayments pay,string appName)
        {
            Pay = pay;
            AppName = appName;
        }

        public OKCBankPayments Pay { get; set; }
        public string AppName {get; set; }

        public string[] GetLVStr()
        {
            return new string[] {AppName,Pay.Amount.ToString("N2") };
        }
    }

}
