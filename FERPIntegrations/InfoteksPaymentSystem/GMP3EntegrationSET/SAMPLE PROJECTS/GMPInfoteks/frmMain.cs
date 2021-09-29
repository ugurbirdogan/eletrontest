using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;
using GMP3Infoteks.OKC;
using GMP3Infoteks.Extention;
using System.Diagnostics;
using System.Collections;

namespace GMPInfoteks
{
    public partial class frmMain : Form
    {
        private OKCAPI _OKCAPI = null;
        private int _UniqueId;
        private int _EkuNo;
        private Stopwatch _Watch = new Stopwatch();
        public frmMain()
        {
            InitializeComponent();

            CreateAPIInstance();
        }
        private string VendorSerial
        {
            get
            {
                return OKCAPI.VendorSerial;
            }
        }

        private void CreateAPIInstance()
        {

            OKCAPI.CreateInstnce("infoteks", "410PC");
            _OKCAPI = OKCAPI.GetInstance();
            InitOKCAPI();
        }
        private void InitOKCAPI()
        {
            lstTestResult.Items.Clear();
            AddTestListItem("", "--------OKC API INIT----------");
            _Watch.Restart();
            try
            {

                _OKCAPI.Init("", "");                       //OKC seri no belli seri noya göre portu bul
                //_OKCAPI.Init("TEST00002471", "COM4");      //Seri ve Port Belli
                //_OKCAPI.Init("","COM4");                   //Sadece Portu biliyorum
                //_OKCAPI.Init("","");                       //Seri ve Port bilmiyorum bağlı okc yi bul veya son eşleşme yapılan OKC bilgisini kullan
            }
            catch (OKCException ex)
            {
                AddTestListItem("", "OKC API INIT FAIL");
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("", "OKC API INIT FAIL");
                AddTestListItem("Hata", ex.Message);
            }
            _Watch.Stop();
            AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
            AddTestListItem("Init Başarılı", _OKCAPI.IsInited.ToTrString());

            if (_OKCAPI.IsInited && _OKCAPI.CurrentOKC != null)
            {
                AddTestListItem("OKC SeriNo", _OKCAPI.CurrentOKC.SerialNo);
                AddTestListItem("İletişim PORT", _OKCAPI.CurrentOKC.Port);
            }
            

        }

        private void AddTestListItem(string col1, object col2)
        {
            string scol2 = "";
            if (col2.GetType() == typeof(bool))
                scol2 = ((bool)col2).ToTrString();
            else
                scol2 = col2.ToString();


            ListViewItem lvi = new ListViewItem(new string[] { col1, scol2 });
            lvi.SubItems[0].BackColor = lstTestResult.BackColor;
            lvi.SubItems[1].BackColor = Color.FromArgb(255, 255, 192);
            lvi.UseItemStyleForSubItems = false;
            lstTestResult.Items.Add(lvi);
        }
        private void AddProgLog(string col1, object col2)
        {
            string scol2 = "";
            if (col2.GetType() == typeof(bool))
                scol2 = ((bool)col2).ToTrString();
            else
                scol2 = col2.ToString();


            ListViewItem lvi = new ListViewItem(new string[] { col1, scol2 });
            lvi.SubItems[0].BackColor = lstTestResult.BackColor;
            lvi.SubItems[1].BackColor = Color.FromArgb(255, 255, 192);
            lvi.UseItemStyleForSubItems = false;
            lstProgLog.Items.Add(lvi);
        }



        string GetPayTypeName(int nPayType)
        {
            PAYMENT_TYPE payType = (PAYMENT_TYPE)nPayType;
            switch (payType)
            {
                case PAYMENT_TYPE.Cash:
                    return "NAKİT";
                case PAYMENT_TYPE.CCard:
                    return "KREDİ KARTI";
                case PAYMENT_TYPE.Credit:
                    return "KREDİLİ SATIŞ";
                case PAYMENT_TYPE.Eft:
                    return "HAVALE/EFT";
                case PAYMENT_TYPE.Document:
                    return "EVRAKLI SATIŞ";
                case PAYMENT_TYPE.FOOD:
                    return "YEMEK_FISI_KARTI";
                case PAYMENT_TYPE.Point:
                    return "PUAN İLE ÖDEME";
                case PAYMENT_TYPE.Other:
                    return "DİĞER ÖDEME";
            }
            return "";
        }
        private void AddErrorList(MsgResult mret)
        {
            if (mret.LocalError != LocalErrors.ER_SUCCESS)
                AddTestListItem("Hata", mret.LocalError.ToString());

            else
            {

                AddTestListItem("Cevap Kodu ", mret.RespCode);
                AddTestListItem("Hata Kodu  ", mret.ErrCode.ToString());
                AddTestListItem("Hata       ", mret.ErrDescription);
            }
        }
        private void cmdUniqueId_Click(object sender, EventArgs e)
        {
            UniqueResult uret = null;
            LastCountersResult lcRet = null;
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            AddTestListItem(" ", " ----OKC UNIQUEID AL--- ");
            _Watch.Restart();
            try
            {
                uret = _OKCAPI.GetUniqueId();
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (uret.HasError)
                {
                    AddErrorList(uret);
                }
                else
                {
                    lcRet = _OKCAPI.GetLastCounters();
                    _EkuNo = lcRet.EkuNo;
                    _UniqueId = uret.UniqueId;
                    if ((_UniqueId & 0xFFFF0000) >> 16 != _EkuNo)
                    {
                        _EkuNo= (_EkuNo & 0x0000FFFF)<<16;
                        _UniqueId = _EkuNo + (_UniqueId & 0x0000FFFF);
                    }
                    
                    AddTestListItem("UniqueId ", _UniqueId.ToString());
                    
                    AddTestListItem(" ", " --------------------------- ");
                }
                if (uret.OKCStatus != null)
                    DisplayOKCStatus(uret.OKCStatus);



            }
            catch (OKCException oex)
            {
                AddTestListItem("Hata Kodu:" + oex.ErrorCode.ToString(), oex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }
        private void cmdUniqueStatus_Click(object sender, EventArgs e)
        {
            UniqueStatusResult uret = null;
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            if (_UniqueId == 0)
            {
                AddTestListItem(" ", " ----UNIQUEID 0--- ");
                return;
            }
            AddTestListItem(" ", " ----OKC UNIQUEID DURUM--- ");
            _Watch.Restart();
            try
            {
                uret = _OKCAPI.GetUniqueIdStatus(_UniqueId);
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (uret.HasError)
                {
                    AddErrorList(uret);
                }
                AddTestListItem("UniqueId Status", uret.UniqueStatus.ToString());

            }
            catch (OKCException oex)
            {
                AddTestListItem("Hata Kodu:" + oex.ErrorCode.ToString(), oex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }
        private bool CheckAPIInit()
        {
            if (_OKCAPI.IsInited)
                return true;

            try
            {
                _OKCAPI.Init("", "");
            }
            catch (Exception ex)
            {

            }
            return _OKCAPI.IsInited;
        }
        private void cmdAvans_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            AddTestListItem(" ", " ----AVANS--- ");
            if (_UniqueId == 0)
            {
                AddTestListItem(" ", " ----Unique Id Alınmalı--- ");
                return;
            }

            OKCSalesInfo salesInfo = new OKCSalesInfo();
            salesInfo.SalesType = SalesTYPE.AVANS;
            salesInfo.SlipMessages.TopMsg1 = "MÜŞTERİ TCKN : 12345678901<n>"
                + "<b>ADI SOYADI/UNVANI</b> : Muzaffer Zorlu<n>"
                + "Açıklama Kısmı";
            salesInfo.SlipMessages.DownMsg1 = "TEŞEKKÜRLER";
            salesInfo.SlipMessages.DownMsg2 = "1 EUR 3.7085 TRY";

            salesInfo.AddProduct(new OKCInfProduct() { Price = 25, ProductName = "Tahsil Ed. Avans Tutarı", Quantity = 1, UnitTypeId = (int)UnitType.Adet });   //Avans 

            salesInfo.PayWithCCard = false;
            salesInfo.TotalAmount = (decimal)25;        //Toplam Tutar = + Ürün Tutarları - Ürün İndirmi-Genel İndirim + Matrahsız Ürün tutarları
            salesInfo.UniqueId = _UniqueId;


            SalesResult sret = _OKCAPI.Sales(salesInfo);

            if (sret.HasError)
            {
                AddErrorList(sret);
            }
            else
            {
                AddTestListItem("Z NO     ", sret.ZNo);
                AddTestListItem("EKÜ NO   ", sret.EkuNo);
                AddTestListItem("Fiş No   ", sret.ReceiptNo);
                AddTestListItem(" ", "------Ödeme Bilgileri------");
                if (sret.CashAmt > 0)
                    AddTestListItem("Nakit   ", sret.CashAmt.ToString("N2"));
                if (sret.CCardAmt > 0)
                    AddTestListItem("Kredi Kartı ", sret.CCardAmt.ToString("N2"));
                if (sret.CCardPayments != null && sret.CCardPayments.Count > 0)
                {
                    foreach (CCardPayment item in sret.CCardPayments)
                    {
                        AddTestListItem("Banka Id ", item.AcquirerId);
                        AddTestListItem("Tutar ", item.Amount);
                        AddTestListItem("--------", "");
                    }
                }
                _UniqueId = 0;
                AddTestListItem(" ", "---------------------------");
            }
        }
        private void cmdInvCollect_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            AddTestListItem(" ", " ----FATURA TAHSİLATI--- ");
            if (_UniqueId == 0)
            {
                AddTestListItem(" ", " ----Unique Id Alınmalı--- ");
                return;
            }

            OKCSalesInfo salesInfo = new OKCSalesInfo();
            salesInfo.SalesType = SalesTYPE.InvoiceCollect;

            //Fatura Kurum ve Müşteri Bilgileri bu alana yazılır.
            salesInfo.SlipMessages.TopMsg1 = "<b>Tahsil Edilen Faturanın</b><n>Kurumu : TEDAŞ <n>Tarihi : 2016-12-05<n>No  : 121251<n>Abone No : 14565";
            salesInfo.SlipMessages.DownMsg1 = "TEŞEKKÜRLER";	//Fiş Alt Açıklama Alanı MF sembolunun üstü

            //1. Normal Ürün Ekleme
            OKCProduct product = new OKCProduct();
            product.Price = 1;
            product.ProductName = "Komisyon Ücreti";
            product.Quantity = 1;
            product.VatRate = 18;
            product.DEPNo = 4;

            salesInfo.AddProduct(product);

            //2.Matrahsız Kalem Ekleme
            OKCInfProduct prdTaxfree = new OKCInfProduct();
            prdTaxfree.Price = 15;						 //15 Tllik Fatura Tahsilatı
            prdTaxfree.Quantity = 1;
            prdTaxfree.InfType = (int)InfReceiptType.InvoiceCollect; //
            prdTaxfree.ProductName = "Tedaş Fatura ";
            salesInfo.AddProduct(prdTaxfree);


            salesInfo.PayWithCCard = false;	//Kredi Kartlı Ödeme alınmak isteniyor ise true

            salesInfo.TotalAmount = 15 + 1;	//Toplam Ödeme Tutarı 
            salesInfo.UniqueId = _UniqueId; //Önceden Alınan UniqueId



            SalesResult sret = _OKCAPI.Sales(salesInfo);

            if (sret.HasError)
            {
                AddErrorList(sret);
            }
            else
            {
                AddTestListItem("Z NO     ", sret.ZNo);
                AddTestListItem("EKÜ NO   ", sret.EkuNo);
                AddTestListItem("Fiş No   ", sret.ReceiptNo);
                AddTestListItem(" ", "------Ödeme Bilgileri------");
                if (sret.CashAmt > 0)
                    AddTestListItem("Nakit   ", sret.CashAmt.ToString("N2"));
                if (sret.CCardAmt > 0)
                    AddTestListItem("Kredi Kartı ", sret.CCardAmt.ToString("N2"));
                if (sret.CCardPayments != null && sret.CCardPayments.Count > 0)
                {
                    foreach (CCardPayment item in sret.CCardPayments)
                    {
                        AddTestListItem("Banka Id ", item.AcquirerId);
                        AddTestListItem("Tutar ", item.Amount);
                        AddTestListItem("--------", "");
                    }
                }
                _UniqueId = 0;
                AddTestListItem(" ", "---------------------------");
            }
        }

        private void cmdEczn_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            AddTestListItem(" ", " ----NAKİT SATIŞ--- ");
            if (_UniqueId == 0)
            {
                AddTestListItem(" ", " ----Unique Id Alınmalı--- ");
                return;
            }
            OKCSalesInfo sales = new OKCSalesInfo();
            sales.SalesType = SalesTYPE.Fis;

            OKCSalesInfo salesInfo = new OKCSalesInfo();
            salesInfo.SlipMessages.TopMsg1 = "HOŞGELDİNİZ<n>Satış Test1";
            salesInfo.SlipMessages.DownMsg1 = "TEŞEKKÜRLER";
            salesInfo.SlipMessages.DownMsg2 = "1 EUR 2.7985 TRY";

            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Karışık Kek", Quantity = 1, VatRate = 18, UnitTypeId = (int)UnitType.Rulo });   //Normal Ürün:Birim Fiyat, Ürün adı,kdv + kısım veya kdv belirtilerek işlem yapılır
            salesInfo.AddProduct(new OKCPLUProduct() { Price = (decimal)1.5, PLU = "12300001", Quantity = 1 });        //OKC de kayıtlı PLU numarası ile ürün satış,Birim Fiyat,PLU no,ve miktar beliritilir, PLU okc de tanımlı değil ise OKC hata döner
            salesInfo.AddProduct(new OKCBarcodeProduct() { Price = (decimal)4.99, Barcode = "86900100001", Quantity = 1 }); //OKC de kayıtlı barcod numarası ile ürün satış,Birim Fiyat,barcod no,ve miktar beliritilir, barcod okc de tanımlı değil ise OKC hata döner
            salesInfo.AddProduct(new OKCDiscount() { Discount = (decimal)0.99, DiscountType = DiscountType.Product, DiscountMethod = DiscountMethod.Const, ProductName = "Ürün İndirimi" }); //Ürün indirimi: indirim yöntemi(%,sabit) ve indirim miktarı ile birlikte bir önceki ürüne yapılacak indirimi belirtir, matrahsız kaleme indirim uygulanmaz
            salesInfo.AddProduct(new OKCInfProduct() { Price = (decimal)5, ProductName = "Eczane Katılım Payı", Quantity = 1, InfType = (int)InfReceiptType.Ilac_Hastane_Katilim }); //Eczane Katılım Payı Vergi matrahına dahil edilmez, KDV si yoktur. 
            salesInfo.AddProduct(new OKCDiscount() { Discount = (decimal)10, DiscountType = DiscountType.Total, DiscountMethod = DiscountMethod.Percent, ProductName = "Toplam İndirim(%10)" });    //Toplam İndirim: İndirim listeye eklendiği noktaya kadarki ürünlere % veya sabit indirim uygular, Matrahsız kalemler indirime dahil edilmez.

            salesInfo.ForeignPayments.Add(new OKCForeignPayments() { LocalAmt = (decimal)2, CurNo = 949, CurCode = "TRY" });    //alınan nakit tl ödeme
            salesInfo.PayWithCCard = false;
            salesInfo.TotalAmount = (decimal)10.85;     //Toplam Tutar = + Ürün Tutarları - Ürün İndirmi-Genel İndirim + Matrahsız Ürün tutarları
            salesInfo.UniqueId = _UniqueId;
            salesInfo.CasierName = "İnfoteks";

            SalesResult sret = _OKCAPI.Sales(salesInfo);

            if (sret.HasError)
            {
                AddErrorList(sret);
            }
            else
            {
                AddTestListItem("Z NO     ", sret.ZNo);
                AddTestListItem("EKÜ NO   ", sret.EkuNo);
                AddTestListItem("Fiş No   ", sret.ReceiptNo);
                AddTestListItem(" ", "------Ödeme Bilgileri------");
                if (sret.CashAmt > 0)
                    AddTestListItem("Nakit   ", sret.CashAmt.ToString("N2"));
                if (sret.CCardAmt > 0)
                    AddTestListItem("Kredi Kartı ", sret.CCardAmt.ToString("N2"));
                if (sret.CCardPayments != null && sret.CCardPayments.Count > 0)
                {
                    foreach (CCardPayment item in sret.CCardPayments)
                    {
                        AddTestListItem("Banka Id ", item.AcquirerId);
                        AddTestListItem("Tutar ", item.Amount);
                        AddTestListItem("--------", "");
                    }
                }
                _UniqueId = 0;
                AddTestListItem(" ", "---------------------------");
            }
        }
        private void cmdSaleTest_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            AddTestListItem(" ", " ----NAKİT SATIŞ--- ");
            if (_UniqueId == 0)
            {
                AddTestListItem(" ", " ----Unique Id Alınmalı--- ");
                return;
            }

            OKCSalesInfo salesInfo = new OKCSalesInfo();
            salesInfo.SalesType = SalesTYPE.Fis;
            salesInfo.SlipMessages.TopMsg1 = "HOŞGELDİNİZ<n>Satış Test1";
            salesInfo.SlipMessages.DownMsg1 = "TEŞEKKÜRLER";
            salesInfo.SlipMessages.DownMsg2 = "1 EUR 2.7985 TRY";
            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1.25, ProductName = "Sucuklu Pide", Quantity = 2, VatRate = 8 });
            int nTestPrdCnt = 0;
            for (int i = 0; i < nTestPrdCnt; i++)
            {
                salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Ürünüm Pide " + i.ToString(), Quantity = 1, VatRate = 1 });
            }

            //salesInfo.ForeignPayments.Add(new OKCForeignPayments() { LocalAmt = (decimal)2, CurNo = 949, CurCode = "TRY" });

            salesInfo.PayWithCCard = false;
            salesInfo.CasierName = "İnfoteks Kasiyer";
            

            salesInfo.TotalAmount = (decimal)8.24 + nTestPrdCnt * 1;
            salesInfo.TotalAmount = (decimal)5.03;
            salesInfo.TotalAmount = (decimal)2.50;
            salesInfo.UniqueId = _UniqueId;


            _Watch.Restart();
            try
            {
                SalesResult sret = _OKCAPI.Sales(salesInfo);
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (sret.HasError)
                {
                    AddErrorList(sret);
                }
                else
                {
                    AddTestListItem("Z NO     ", sret.ZNo);
                    AddTestListItem("EKÜ NO   ", sret.EkuNo);
                    AddTestListItem("Fiş No   ", sret.ReceiptNo);
                    AddTestListItem("Fiş Tarihi   ", System.Text.Encoding.UTF8.GetString(sret.SalesDate));
                    AddTestListItem(" ", "------Ödeme Bilgileri------");
                    if (sret.CashAmt > 0)
                        AddTestListItem("Nakit   ", sret.CashAmt.ToString("N2"));
                    if (sret.CCardAmt > 0)
                        AddTestListItem("Kredi Kartı ", sret.CCardAmt.ToString("N2"));
                    if (sret.CCardPayments != null && sret.CCardPayments.Count > 0)
                    {
                        foreach (CCardPayment item in sret.CCardPayments)
                        {
                            AddTestListItem("Banka Id ", item.AcquirerId);
                            AddTestListItem("Tutar ", item.Amount);
                            AddTestListItem("--------", "");
                        }
                    }
                    _UniqueId = 0;
                    AddTestListItem(" ", "---------------------------");
                }
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }

        }
        private void cmdInvoice_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            AddTestListItem(" ", " ----FATURA SATIŞ--- ");
            if (_UniqueId == 0)
            {
                AddTestListItem(" ", " ----Unique Id Alınmalı--- ");
                return;
            }

            OKCSalesInfo salesInfo = new OKCSalesInfo();
            salesInfo.SalesType = SalesTYPE.Fatura;
            salesInfo.SlipMessages.TopMsg1 = "HOŞGELDİNİZ<n>Fatura Satış Test1";
            salesInfo.SlipMessages.DownMsg1 = "TEŞEKKÜRLER";
            salesInfo.SlipMessages.DownMsg2 = "1 EUR 3.1185 TRY";

            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Karışık Kek", Quantity = 1, VatRate = 18 });
            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1.5, ProductName = "Kola 1 LT.", Quantity = 1.5, VatRate = 8 });
            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)4.99, ProductName = "Sucuklu Pide", Quantity = 1, VatRate = 8 });


            /*Fatura Bilgileri Start*/
            salesInfo.InvoiceInfo = new OKCInvoiceInfo();
            salesInfo.InvoiceInfo.InvoiceDate = DateTime.Now;
            salesInfo.InvoiceInfo.InvoiceNo = "A145236";
            salesInfo.InvoiceInfo.IsIrsaliye = false;
            salesInfo.InvoiceInfo.TCNo = "12345678901";
            /*Fatura Bilgileri END*/

            salesInfo.ForeignPayments.Add(new OKCForeignPayments() { LocalAmt = (decimal)2, CurNo = 949, CurCode = "TRY" });
            salesInfo.PayWithCCard = false;
            salesInfo.TotalAmount = (decimal)8.24;
            salesInfo.UniqueId = _UniqueId;
            salesInfo.CasierName = "Kasiyer";

            SalesResult sret = _OKCAPI.Sales(salesInfo);

            if (sret.HasError)
            {
                AddErrorList(sret);
            }
            else
            {
                AddTestListItem("Z NO     ", sret.ZNo);
                AddTestListItem("EKÜ NO   ", sret.EkuNo);
                AddTestListItem("Fiş No   ", sret.ReceiptNo);
                AddTestListItem(" ", "------Ödeme Bilgileri------");
                if (sret.CashAmt > 0)
                    AddTestListItem("Nakit   ", sret.CashAmt.ToString("N2"));
                if (sret.CCardAmt > 0)
                    AddTestListItem("Kredi Kartı ", sret.CCardAmt.ToString("N2"));
                if (sret.CCardPayments != null && sret.CCardPayments.Count > 0)
                {
                    foreach (CCardPayment item in sret.CCardPayments)
                    {
                        AddTestListItem("Banka Id ", item.AcquirerId);
                        AddTestListItem("Tutar ", item.Amount);
                        AddTestListItem("--------", "");
                    }
                }
                _UniqueId = 0;
                AddTestListItem(" ", "---------------------------");
            }
        }
        private void cmdEFatura_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            AddTestListItem(" ", " ----EFATURA SATIŞ--- ");
            if (_UniqueId == 0)
            {
                AddTestListItem(" ", " ----Unique Id Alınmalı--- ");
                return;
            }

            OKCSalesInfo salesInfo = new OKCSalesInfo();
            salesInfo.SalesType = SalesTYPE.EFatura;
            salesInfo.SlipMessages.TopMsg1 = "HOŞGELDİNİZ<n>EFatura Satış Test1";
            salesInfo.SlipMessages.DownMsg1 = "TEŞEKKÜRLER";
            salesInfo.SlipMessages.DownMsg2 = "1 EUR 3.1185 TRY";

            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Karışık Kek", Quantity = 1, VatRate = 18 });
            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1.5, ProductName = "Kola 1 LT.", Quantity = 1.5, VatRate = 8 });
            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)4.99, ProductName = "Sucuklu Pide", Quantity = 1, VatRate = 1 });


            /*Fatura Bilgileri Start*/
            salesInfo.InvoiceInfo = new OKCInvoiceInfo();
            salesInfo.InvoiceInfo.InvoiceDate = DateTime.Now;
            salesInfo.InvoiceInfo.InvoiceNo = "A145236";
            salesInfo.InvoiceInfo.IsIrsaliye = false;
            salesInfo.InvoiceInfo.TCNo = "12345678901";
            /*Fatura Bilgileri END*/

            salesInfo.ForeignPayments.Add(new OKCForeignPayments() { LocalAmt = (decimal)2, CurNo = 949, CurCode = "TRY" });
            salesInfo.PayWithCCard = false;
            salesInfo.TotalAmount = (decimal)8.24;
            salesInfo.UniqueId = _UniqueId;


            SalesResult sret = _OKCAPI.Sales(salesInfo);

            if (sret.HasError)
            {
                AddErrorList(sret);
            }
            else
            {
                AddTestListItem("Z NO     ", sret.ZNo);
                AddTestListItem("EKÜ NO   ", sret.EkuNo);
                AddTestListItem("Fiş No   ", sret.ReceiptNo);
                AddTestListItem(" ", "------Ödeme Bilgileri------");
                if (sret.CashAmt > 0)
                    AddTestListItem("Nakit   ", sret.CashAmt.ToString("N2"));
                if (sret.CCardAmt > 0)
                    AddTestListItem("Kredi Kartı ", sret.CCardAmt.ToString("N2"));
                if (sret.CCardPayments != null && sret.CCardPayments.Count > 0)
                {
                    foreach (CCardPayment item in sret.CCardPayments)
                    {
                        AddTestListItem("Banka Id ", item.AcquirerId);
                        AddTestListItem("Tutar ", item.Amount);
                        AddTestListItem("--------", "");
                    }
                }
                _UniqueId = 0;
                AddTestListItem(" ", "---------------------------");
            }
        }
        private void SaleCCTest(int nBankAppId1 = 0, decimal dBankAppAmount1 = 0, int nBankAppId2 = 0, decimal dBankAppAmount2 = 0)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            AddTestListItem(" ", " ----Kredi Kartlı SATIŞ--- ");
            if (_UniqueId == 0)
            {
                AddTestListItem(" ", " ----Unique Id Alınmalı--- ");
                return;
            }
            OKCSalesInfo sales = new OKCSalesInfo();
            sales.SalesType = SalesTYPE.Fis;

            OKCSalesInfo salesInfo = new OKCSalesInfo();
            salesInfo.SlipMessages.TopMsg1 = "HOŞGELDİNİZ<n>Satış Test1";
            salesInfo.SlipMessages.DownMsg1 = "TEŞEKKÜRLER";
            salesInfo.SlipMessages.DownMsg2 = "1 EUR 2.7985 TRY";

            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Karışık Kek", Quantity = 1, VatRate = 18 });
            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1.5, ProductName = "Kola 1 LT.", Quantity = 1.5, VatRate = 8 });
            salesInfo.AddProduct(new OKCProduct() { Price = (decimal)4.99, ProductName = "Sucuklu Pide", Quantity = 1, VatRate = 1 });

            salesInfo.ForeignPayments.Add(new OKCForeignPayments() { LocalAmt = (decimal)0.25, CurNo = 949, CurCode = "TRY" });
            if (nBankAppId1 > 0 && dBankAppAmount1 > 0)
                salesInfo.BankPayments.Add(new OKCBankPayments() { Amount = dBankAppAmount1, AppId = nBankAppId1 });            //nBankAppId1 Ödeme Uygulamasından dBankAppAmount1 miktarında çekilsin
            if (nBankAppId2 > 0 && dBankAppAmount2 > 0)
                salesInfo.BankPayments.Add(new OKCBankPayments() { Amount = dBankAppAmount2, AppId = nBankAppId2 });            //nBankAppId2 Ödeme Uygulamasından dBankAppAmount2 miktarında çekilsin


            salesInfo.PayWithCCard = true;      //Kredi Kartlı Ödeme Yapılsın

            salesInfo.TotalAmount = (decimal)8.24;
            salesInfo.UniqueId = _UniqueId;


            SalesResult sret = _OKCAPI.Sales(salesInfo);

            if (sret.HasError)
            {
                AddErrorList(sret);
            }
            else
            {
                AddTestListItem("Z NO     ", sret.ZNo);
                AddTestListItem("EKÜ NO   ", sret.EkuNo);
                AddTestListItem("Fiş No   ", sret.ReceiptNo);
                AddTestListItem(" ", "------Ödeme Bilgileri------");
                if (sret.CashAmt > 0)
                    AddTestListItem("Nakit   ", sret.CashAmt.ToString("N2"));
                if (sret.CCardAmt > 0)
                    AddTestListItem("Kredi Kartı ", sret.CCardAmt.ToString("N2"));
                if (sret.CCardPayments != null && sret.CCardPayments.Count > 0)
                {
                    foreach (CCardPayment item in sret.CCardPayments)
                    {
                        AddTestListItem("OKC App Id ", item.BankAppId);
                        AddTestListItem("Banka Id ", item.AcquirerId);
                        AddTestListItem("Tutar ", item.Amount);
                        AddTestListItem("--------", "");
                    }
                }
                _UniqueId = 0;
                AddTestListItem(" ", "---------------------------");
            }
        }
        private void cmdCCSales_Click(object sender, EventArgs e)
        {
            SaleCCTest();//Banka Seçimini OKC de yapılsın
        }

        private void cmdCCSalesWithApp_Click(object sender, EventArgs e)
        {
            SaleCCTest(7, (decimal)1.5);//7 BKM 1.5 TL çekilsin
        }

        private void cmdCCSalesWithFoodApp_Click(object sender, EventArgs e)
        {
            SaleCCTest(7, (decimal)1.5, 75, (decimal)1.25);//7 BKM 1.5 TL çekilsin, 75 Yemek uygulamasından 1.25 çekilsin
        }

        private void DisplayZRep(ZXResult zret)
        {
            if (zret.HasError)
            {
                AddErrorList(zret);
            }
            else
            {
                ZReport zrep = zret.ZReport;
                if (zrep == null)
                {
                    AddTestListItem("Hata", zret.LocalError.ToString());
                    AddTestListItem("", "Z Raporu Çözümleme Hatası");
                    return;
                }
                AddTestListItem("Z NO", zrep.ZNo);
                AddTestListItem("EKÜ NO", zrep.EKUNo);
                AddTestListItem("Fiş No", zrep.ReceiptNo);
                AddTestListItem("Z Tarihi", zrep.ZDate.ToString("yyyy-MM-dd hh:mm:ss"));

                AddTestListItem("Artırım", zrep.IncreaseCnt);
                AddTestListItem("İndirim", zrep.DiscountCnt);
                AddTestListItem("Hata Düzelt", zrep.RemoveCnt);
                AddTestListItem("Mali Fiş", zrep.ReceiptCntFisc);
                AddTestListItem("Mali Olmayan Fiş", zrep.ReceiptCntNotFisc);
                AddTestListItem("Toplam Fiş", zrep.ReceiptCntFisc + zrep.ReceiptCntNotFisc);
                AddTestListItem("Fiş Satış", zrep.VoucherCount);
                AddTestListItem("Fiş İptal", zrep.VoidCnt);
                AddTestListItem("Artırım Toplam", zrep.IncreaseAmt);
                AddTestListItem("İndirim Toplam", zrep.DiscountAmt);
                AddTestListItem("Satış Toplam", zrep.VoucherAmt.ToString("N2"));
                AddTestListItem("Fatura Toplam", zrep.InvoiceAmt.ToString("N2"));
                AddTestListItem("İptal Toplam", zrep.VoidAmt.ToString("N2"));
                AddTestListItem("Hata Düzelt Toplam", zrep.RemoveAmt.ToString("N2"));

                if (zrep.InfRcpCounts != null)
                {
                    bool bTemp = false;
                    for (int i = 0; i < zrep.InfRcpCounts.Length; i++)
                    {
                        if (zrep.InfRcpCounts[i] > 0)
                        {
                            if (!bTemp)
                            {
                                bTemp = true;
                                AddTestListItem("Bilgi Fişleri", "");
                            }
                            AddTestListItem(zrep.InfRcpCounts[i].ToString(), zrep.InfRcpAmt[i].ToString("N2"));
                        }
                    }
                    if (!bTemp)
                        AddTestListItem("-------------", "");
                }
                AddTestListItem("", "DEPARTMAN SATIŞLARI");
                if (zrep.DepTotal != null)
                {
                    for (int i = 0; i < Math.Min(zrep.DepTotal.Length, consts.DEP_CNT); i++)
                    {
                        if (zrep.DepTotal[i] > 0)
                        {
                            AddTestListItem(zrep.DepNames[i], "%" + (zrep.VatRates[zrep.DepRate[i] - 1] / 100).ToString());
                            AddTestListItem("", "TOPLAM            " + zrep.DepTotal[i].ToString("N2"));
                            AddTestListItem("", "ADET             " + zrep.DepQuantity[i].ToString("N2"));
                        }
                    }
                }
                AddTestListItem("", "-------------------");
                AddTestListItem("", "SATIŞ VE KDV TOPLAMLARI");
                decimal totalKDV = 0;
                decimal totalAmt = 0;
                decimal cumTotalamt = 0;
                decimal totalCumkdv = 0;
                if (zrep.VatTotal != null)
                {
                    for (int i = 0; i < Math.Min(zrep.VatTotal.Length, consts.VAT_CNT); i++)
                    {
                        if (zrep.VatTotal[i] > 0)
                        {
                            totalAmt += zrep.VatTotal[i];
                            totalKDV += zrep.VatCalcTotal[i];
                            AddTestListItem("%" + (zrep.VatRates[i] / 100).ToString(), "");
                            AddTestListItem("", "TOPLAM            " + zrep.VatTotal[i].ToString("N2"));
                            AddTestListItem("", "KDV              " + zrep.VatCalcTotal[i].ToString("N2"));
                        }
                        cumTotalamt += zrep.VatCumTotal[i];
                        totalCumkdv += zrep.VatCalcCumTotal[i];
                    }
                }
                AddTestListItem("", "-------------------");
                AddTestListItem("", "MALİ VERİ");
                AddTestListItem("TOPLAM", zrep.VoucherAmt.ToString("N2"));
                AddTestListItem("KDV", totalKDV.ToString("N2"));
                AddTestListItem("", "---------");
                AddTestListItem("", "ÇEKMECE TOPLAMLARI");
                if (zrep.PayAmounts != null)
                {
                    for (int i = 0; i < zrep.PayAmounts.Length; i++)
                    {
                        if (zrep.PayAmounts[i] > 0)
                            AddTestListItem(GetPayTypeName(zrep.PayTypes[i]), zrep.PayAmounts[i].ToString("N2"));
                    }
                }
                AddTestListItem("", "-------------------");
                AddTestListItem("KU TOPLAM", cumTotalamt.ToString("N2"));
                AddTestListItem("KU KDV", totalCumkdv.ToString("N2"));
                AddTestListItem("", "-------------------");

            }
        }
        private void DisplayXRep(ZXResult zret)
        {
            if (zret.HasError)
            {
                AddErrorList(zret);
            }
            else
            {
                XReport zrep = zret.XReport;
                if (zrep == null)
                {
                    AddTestListItem("Hata", zret.LocalError.ToString());
                    AddTestListItem("", "Z Raporu Çözümleme Hatası");
                    return;
                }
                AddTestListItem("Z NO", zrep.ZNo);
                AddTestListItem("X NO", zrep.XNo);
                AddTestListItem("EKÜ NO", zrep.EKUNo);
                AddTestListItem("Fiş No", zrep.ReceiptNo);
                AddTestListItem("Z Tarihi", zrep.ZDate.ToString("yyyy-MM-dd hh:mm:ss"));

                AddTestListItem("Artırım", zrep.IncreaseCnt);
                AddTestListItem("İndirim", zrep.DiscountCnt);
                AddTestListItem("Hata Düzelt", zrep.RemoveCnt);
                AddTestListItem("Mali Fiş", zrep.ReceiptCntFisc);
                AddTestListItem("Mali Olmayan Fiş", zrep.ReceiptCntNotFisc);
                AddTestListItem("Toplam Fiş", zrep.ReceiptCntFisc + zrep.ReceiptCntNotFisc);
                AddTestListItem("Fiş Satış", zrep.VoucherCount);
                AddTestListItem("Fiş İptal", zrep.VoidCnt);
                AddTestListItem("Artırım Toplam", zrep.IncreaseAmt);
                AddTestListItem("İndirim Toplam", zrep.DiscountAmt);
                AddTestListItem("Satış Toplam", zrep.VoucherAmt.ToString("N2"));
                AddTestListItem("Fatura Toplam", zrep.InvoiceAmt.ToString("N2"));
                AddTestListItem("İptal Toplam", zrep.VoidAmt.ToString("N2"));
                AddTestListItem("Hata Düzelt Toplam", zrep.RemoveAmt.ToString("N2"));

                if (zrep.InfRcpCounts != null)
                {
                    bool bTemp = false;
                    for (int i = 0; i < zrep.InfRcpCounts.Length; i++)
                    {
                        if (zrep.InfRcpCounts[i] > 0)
                        {
                            if (!bTemp)
                            {
                                bTemp = true;
                                AddTestListItem("Bilgi Fişleri", "");
                            }
                            AddTestListItem(zrep.InfRcpCounts[i].ToString(), zrep.InfRcpAmt[i].ToString("N2"));
                        }
                    }
                    if (!bTemp)
                        AddTestListItem("-------------", "");
                }
                AddTestListItem("", "DEPARTMAN SATIŞLARI");
                if (zrep.DepTotal != null)
                {
                    for (int i = 0; i < Math.Min(zrep.DepTotal.Length, consts.DEP_CNT); i++)
                    {
                        if (zrep.DepTotal[i] > 0)
                        {
                            AddTestListItem(zrep.DepNames[i], "%" + (zrep.VatRates[zrep.DepRate[i] - 1] / 100).ToString());
                            AddTestListItem("", "TOPLAM            " + zrep.DepTotal[i].ToString("N2"));
                            AddTestListItem("", "ADET             " + zrep.DepQuantity[i].ToString("N2"));
                        }
                    }
                }
                AddTestListItem("", "-------------------");
                AddTestListItem("", "SATIŞ VE KDV TOPLAMLARI");
                decimal totalKDV = 0;
                decimal totalAmt = 0;
                decimal cumTotalamt = 0;
                decimal totalCumkdv = 0;
                if (zrep.VatTotal != null)
                {
                    for (int i = 0; i < Math.Min(zrep.VatTotal.Length, consts.VAT_CNT); i++)
                    {
                        if (zrep.VatTotal[i] > 0)
                        {
                            totalAmt += zrep.VatTotal[i];
                            totalKDV += zrep.VatCalcTotal[i];
                            AddTestListItem("%" + (zrep.VatRates[i] / 100).ToString(), "");
                            AddTestListItem("", "TOPLAM            " + zrep.VatTotal[i].ToString("N2"));
                            AddTestListItem("", "KDV              " + zrep.VatCalcTotal[i].ToString("N2"));
                        }
                        cumTotalamt += zrep.VatCumTotal[i];
                        totalCumkdv += zrep.VatCalcCumTotal[i];
                    }
                }
                AddTestListItem("", "-------------------");
                AddTestListItem("", "MALİ VERİ");
                AddTestListItem("TOPLAM", zrep.VoucherAmt.ToString("N2"));
                AddTestListItem("KDV", totalKDV.ToString("N2"));
                AddTestListItem("", "---------");
                AddTestListItem("", "ÇEKMECE TOPLAMLARI");
                if (zrep.PayAmounts != null)
                {
                    for (int i = 0; i < zrep.PayAmounts.Length; i++)
                    {
                        if (zrep.PayAmounts[i] > 0)
                            AddTestListItem(GetPayTypeName(zrep.PayTypes[i]), zrep.PayAmounts[i].ToString("N2"));
                    }
                }
                AddTestListItem("", "-------------------");
                AddTestListItem("KU TOPLAM", cumTotalamt.ToString("N2"));
                AddTestListItem("KU KDV", totalCumkdv.ToString("N2"));
                AddTestListItem("", "-------------------");

            }
        }

        private void DisplayFirmInfo(FirmInfoResult fir)
        {
            if (fir.HasError)
            {
                AddErrorList(fir);
            }
            else
            {
                AddTestListItem("İşyeri Id", fir.MerchantId);
                AddTestListItem("Firma Tipi", fir.FirmType);
                AddTestListItem("Firma Adı", fir.FirmName);
                AddTestListItem("TCKN", fir.TCKN);
                AddTestListItem("VKN", fir.VKNo);
                AddTestListItem("Vergi Dairesi", fir.TaxOffice);
                AddTestListItem("Fiş Başlığı", fir.ReceiptTitle);
                AddTestListItem("1.Adres Satırı", fir.AddressLine_1);
                AddTestListItem("2.Adres Satırı", fir.AddressLine_2);
                AddTestListItem("3.Adres Satırı", fir.AddressLine_3);
                AddTestListItem("Üst Slip Notu", fir.TopSlipNote);
                AddTestListItem("Alt Slip Notu", fir.DownSlipNote);
                AddTestListItem("İnternet Sitesi", fir.WebSite);
                AddTestListItem("Mersis No", fir.MersisNo);
            }
        }

        private int CheckMasa()
        {
            OleDbConnection con2 = new OleDbConnection(sqlCon);
            OleDbCommand cmd2 = new OleDbCommand("Select * from MasaDurumu where DURUM=2", con2);
            dr2 = null;
            if (con2.State == ConnectionState.Closed)
            {
                con2.Open();
            }
            dr2 = cmd2.ExecuteReader();
            if (dr2.Read())
            {
                
                con2.Close();
                con2.Dispose();
                dr2.Close();
                return 1;
            }
            con2.Close();
            con2.Dispose();
            dr2.Close();
            return 0;
        }
        private void cmdTakeZReport_Click(object sender, EventArgs e)
        {
            // Masaları kontrol et Açık adisyon varsa uyarı ver
            if (CheckMasa() != 0)
            { 
                // Açık Masa var önce onları kapat
                MessageBox.Show("Açık Masanız Bulunmaktadır Lütfen Öncelikle Onları Kapatın", "UYARI",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            
            }
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            _Watch.Restart();
            try
            {
                ZXResult zret = _OKCAPI.TakeZReport();
                AddTestListItem(" ", " ----Z RAPORU--- ");
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                DisplayZRep(zret);
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }

        }

        private void cmdTakeXReport_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }

            _Watch.Restart();

            try
            {
                ZXResult zret = _OKCAPI.TakeXReport();
                AddTestListItem(" ", " ----X RAPORU--- ");
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                DisplayXRep(zret);
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
        }

        private void cmdInfRecipt_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            try
            {
                _Watch.Restart();
                string infReceipt = "Stok Sayım Raporu   <i01><i02><a1>Test Fiş   {f}</a>Yeni satır<n>Z no :{z}<n>Ekü No :{e}<n>yeni satıra otomatik olarak geçecek satır burası. <b>bu kısım bold olarak yazılır</b><n><pr p:5,45,100,180,230,250;>pnt1;pnt2;pnt3;pnt4;p5;p6#pn11;pn12;pn13;pn14;p15;p16#pn21;pn22;pn23;pn24;p25;p26#pn31;pn32;pn33;pn34;p35;p36#pn41;pn42;pn43;pn44;p45;p46</pr><rw rt:211;a:021;>kolon11;kolon12;kolon13#kolon21;kolon22;kolon23#kolon31;kolon32;kolon33</rw><ft we:6;h:34;></ft>buradan itibaren font değişir<n><rw rt:211;a:021;>kolon1-1;kolon2-1;kolon3-1;</rw>Şimdi iki kolonlu bir örnek<n><rw rt:120;a:020;>kolon1-1;kolon1-2#kolon2-1;kolon2-2#kolon3-1;kolon3-2#kolon4-1;kolon4-2</rw><rw rt:120;a:020;>kolon1-3;kolon2-3;;</rw><a1>-----------------------</a><f>30</f><i03>";
                InfReceiptResult iresult = _OKCAPI.PrintInfReceipt(InfReceiptType.Report, 0, infReceipt);

                AddTestListItem(" ", " ----BİLGİ FİŞİ YAZDIR--- ");
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (iresult.HasError)
                {
                    AddErrorList(iresult);
                }
                else
                {
                    AddTestListItem("Z NO", iresult.ZNo);
                    AddTestListItem("EKÜ NO", iresult.EkuNo);
                    AddTestListItem("Fiş No", iresult.ReceiptNo);
                }
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
        }
        private void PrintCopyReceipt(int nZNo, int nReciptNo)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }

            try
            {
                _Watch.Restart();
                MsgResult cresult = _OKCAPI.PrintCopyReceipt(nZNo, nReciptNo);

                AddTestListItem(" ", " ----FİŞ KOPYASI--- ");
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (cresult.HasError)
                {
                    if (cresult.LocalError != LocalErrors.ER_SUCCESS)
                        AddTestListItem("Hata", cresult.LocalError.ToString());

                    else
                    {

                        AddTestListItem("Cevap Kodu ", cresult.RespCode);
                        AddTestListItem("Hata Kodu  ", cresult.ErrCode.ToString());
                        AddTestListItem("Hata       ", cresult.ErrDescription);
                    }
                }
                else
                {
                    AddTestListItem(" ", " ----KOPYA YAZDIRILDI--- ");
                }
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }
        private void cmdCopyLastR_Click(object sender, EventArgs e)
        {
            PrintCopyReceipt(0, 0);
        }

        private void cmdCopyReceipt_Click(object sender, EventArgs e)
        {
            int nReciptNo = 0;
            int nZNo = 0;
            int.TryParse(txtCopyReciptNo.Text, out nReciptNo);
            int.TryParse(txtCopyZNo.Text, out nZNo);
            if (nZNo == 0 || nReciptNo == 0)
            {
                lstTestResult.Items.Clear();
                AddTestListItem(" ", " ----BİLGİ FİŞİ YAZDIR--- ");
                AddTestListItem("Hata", "Z NO ve Fiş No Giriniz");
            }
            else
            {
                PrintCopyReceipt(nZNo, nReciptNo);
            }
        }

        private void cmdCounters_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            _Watch.Restart();
            try
            {
                LastCountersResult cresult = _OKCAPI.GetLastCounters();

                AddTestListItem(" ", " ----OKC SAYAÇ SORGU--- ");
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (cresult.HasError)
                {
                    AddErrorList(cresult);
                }
                else
                {
                    AddTestListItem("Z NO", cresult.ZNo);
                    AddTestListItem("EKÜ NO", cresult.EkuNo);
                    AddTestListItem("Fiş No", cresult.ReceiptNo + 1);
                    AddTestListItem(" ", " ---------------- ");
                }
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }

        private void cmdGetZData_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            try
            {
                _Watch.Restart();
                int nZNo = 0;
                int.TryParse(txtZNoGetZ.Text, out nZNo);


                if (nZNo == 0)
                {
                    AddTestListItem(" ", " ----Z VERİSİ SORGU--- ");
                    AddTestListItem("Hata", "Z NO Giriniz");
                }
                else
                {

                    ZXResult zresult = _OKCAPI.GetZReportWithNo(nZNo);
                    AddTestListItem(" ", " ----Z RAPORU VERİSİ--- ");
                    _Watch.Stop();
                    AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                    DisplayZRep(zresult);
                }
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }

        private void cmdPayApps_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            try
            {
                _Watch.Restart();
                AppListResult apResult = _OKCAPI.GetPayAppList();

                AddTestListItem(" ", " ----ÖDEME UYGULAMA LİSTESİ--- ");
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (apResult.HasError)
                {
                    AddErrorList(apResult);
                }
                else
                {
                    if (apResult.AppList != null)
                    {

                        for (int i = 0; i < apResult.AppList.Count; i++)
                        {
                            AddTestListItem(apResult.AppList[i].AppName, "");
                            AddTestListItem("   ID", apResult.AppList[i].AppId);
                            AddTestListItem("   Tip", (AppType)apResult.AppList[i].AppType);
                            AddTestListItem("   Tanımlı Versiyon", apResult.AppList[i].Version.ToString());
                            AddTestListItem("   Cihazdaki Versiyon", apResult.AppList[i].VersionLocal.ToString());
                        }
                        salesS1.SetPayApps(apResult.AppList);
                    }

                }
                AddTestListItem(" ", " ----------------------- ");
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }
        private void cmdAppList_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            try
            {
                _Watch.Restart();
                AppListResult apResult = _OKCAPI.GetAppList();

                AddTestListItem(" ", " ----UYGULAMA LİSTESİ--- ");
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (apResult.HasError)
                {
                    AddErrorList(apResult);
                }
                else
                {
                    if (apResult.AppList != null)
                    {

                        for (int i = 0; i < apResult.AppList.Count; i++)
                        {


                            AddTestListItem(apResult.AppList[i].AppName, "");
                            AddTestListItem("   ID", apResult.AppList[i].AppId);
                            AddTestListItem("   Tip", (AppType)apResult.AppList[i].AppType);
                            AddTestListItem("   Tanımlı Versiyon", apResult.AppList[i].Version.ToString());
                            AddTestListItem("   Cihazdaki Versiyon", apResult.AppList[i].VersionLocal.ToString());
                        }
                    }

                }
                AddTestListItem(" ", " ----------------------- ");
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }

        private void cmdDepAndVat_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            AddTestListItem(" ", " ----KDV VE KISIMLAR--- ");

            _Watch.Restart();
            try
            {
                KISIMListResult depResult = _OKCAPI.GetKISIMList();
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (depResult.HasError)
                {
                    AddErrorList(depResult);
                }
                else
                {
                    if (depResult.VATRates != null)
                    {
                        AddTestListItem(" ", " ----KDV ORANLARI--- ");
                        foreach (int item in depResult.VATRates)
                        {
                            AddTestListItem(" ", "% " + item.ToString());
                        }
                        AddTestListItem(" ", " ------------------ ");
                    }
                    if (depResult.KISIMList != null)
                    {
                        AddTestListItem(" ", " ----KISIMLAR--- ");

                        int nKISIMNo = 1;
                        foreach (OKCKISIM item in depResult.KISIMList)
                        {

                            AddTestListItem("KISIM " + nKISIMNo.ToString(), item.KISIMName);
                            AddTestListItem("   KDV ", "%" + item.VatRate.ToString());
                            AddTestListItem("   Birim ", ((UnitType)item.UnitTypeId).ToString());
                            AddTestListItem("   Limit ", item.LimitValue == 0 ? "Sınırsız" : item.LimitValue.ToString("N2"));
                            AddTestListItem("   ID ", item.KISIMId);
                            nKISIMNo++;
                        }
                        AddTestListItem(" ", " --------------- ");
                        salesS1.SetKISIMList(depResult.KISIMList);
                    }

                }
                AddTestListItem(" ", " ----------------------- ");
                FillKISIMGrid(depResult);
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }

        private void cmdPing_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            _Watch.Restart();
            try
            {
                string serialNo = "";
                LocalErrors lerror = _OKCAPI.Ping(out serialNo);
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                AddTestListItem(" ", " ----PING--- ");
                if (lerror != LocalErrors.ER_SUCCESS)
                    AddTestListItem("Hata", lerror.ToString());
                else
                {
                    AddTestListItem("OKC Seri No", serialNo);
                }
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }

        private void cmdPair_Click(object sender, EventArgs e)
        {

            lstTestResult.Items.Clear();
            AddTestListItem(" ", " ----EŞLEŞTİRME--- ");
            _Watch.Restart();
            try
            {
                _OKCAPI.Pair();
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            _Watch.Stop();
            AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
            AddTestListItem("Eşleştirme Başarılı", _OKCAPI.IsInited.ToTrString());

            if (_OKCAPI.IsInited && _OKCAPI.CurrentOKC != null)
            {
                AddTestListItem("OKC SeriNo", _OKCAPI.CurrentOKC.SerialNo);
                AddTestListItem("İletişim PORT", _OKCAPI.CurrentOKC.Port);
            }


        }
        private void DisplayOKCStatus(OKCStatus status)
        {
            AddTestListItem(" ", " ----OKC DURUM BİLGİSİ--- ");
            AddTestListItem("Eşleşme Yapılmış", status.IsPaired.ToTrString());
            AddTestListItem("Eşleşme Gerekli", status.NeedPair.ToTrString());
            AddTestListItem("Mali Modda", status.IsFiscal.ToTrString());
            AddTestListItem("Kullanıcı Modunda", status.IsUserMode.ToTrString());
            AddTestListItem("Parametre indirilmiş", status.IsParameterDown.ToTrString());
            AddTestListItem(" ", " -------------------- ");
            AddTestListItem("Z Zorunlu ", status.IsZRepNeed.ToTrString());
            AddTestListItem("Z Raporu Alınmalı ", status.OnlyZReport.ToTrString());
            AddTestListItem("Başka Ciahza Ait FM  ", status.OtherDeviceFM.ToTrString());
            AddTestListItem("Cihaz Kullanım Dışı ", status.DeviceOutOfUse.ToTrString());
            AddTestListItem("Servis Girişi Yapılmış ", status.ServiceUserEntered.ToTrString());
            AddTestListItem("EKÜ Takılı Değil", status.NoEKUInserted.ToTrString());
            AddTestListItem("Zaman Değiştirilmiş", status.TimeChanged.ToTrString());
            AddTestListItem("Başka Cihaza Ait EKÜ", status.OtherDeviceEKU.ToTrString());
            AddTestListItem("Eski EKÜ Takılı", status.OldEKUInserted.ToTrString());
            AddTestListItem("EKÜ İletişim Hatası", status.EKUCOMMError.ToTrString());
            AddTestListItem("DB Bütünlüğü yok", status.DBIntegrity.ToTrString());
            AddTestListItem("Parametre Bütünlük Yok", status.ParamIntegrity.ToTrString());
            AddTestListItem("Servis Modunda", status.OnService.ToTrString());
            AddTestListItem("Günlük Hafıza Bozuk", status.DMEraseOrIntegrity.ToTrString());
            AddTestListItem("FM Bütünlüğü Yok", status.FMIntegrity.ToTrString());
            AddTestListItem("Fişe Devam Edip Z Alınacak", status.ResumeRcpAndTakeZ.ToTrString());
            AddTestListItem("Olay Kayıt Bütünlüğü Yok", status.EvtDBIntegrity.ToTrString());
            AddTestListItem("Kapaklar Açılmış", status.CoversOpened.ToTrString());
            AddTestListItem("Boş EKÜ Takılı", status.EmptyEKUInserted.ToTrString());
        }
        private void cmdEcho_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            try
            {
                string serialNo = "";
                _Watch.Restart();
                EchoResult eret = _OKCAPI.Echo();

                AddTestListItem(" ", " ----ECHO--- ");
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);

                if (eret.HasError)
                {
                    AddErrorList(eret);
                }
                else
                {
                    if (eret.OKCStatus == null)
                    {
                        AddTestListItem("Hata : ", "okc durum bilgisi alınamadı");
                        return;
                    }
                    DisplayOKCStatus(eret.OKCStatus);
                }
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }

        #region Products
        private bool _KisimLoaded = false;
        private bool _GroupLoaded = false;
        private void BindUnitTypeComboColumn(DataGridViewComboBoxColumn comboboxColumn)
        {
            UnitType[] unittypes = (UnitType[])Enum.GetValues(typeof(UnitType));
            string[] unitNames = Enum.GetNames(typeof(UnitType));
            DataTable dtUnit = new DataTable();
            dtUnit.Columns.Add("UnitTypeId", typeof(int));
            dtUnit.Columns.Add("UnitTypeName", typeof(string));
            for (int i = 0; i < unittypes.Length; i++)
            {
                DataRow drUnit = dtUnit.NewRow();
                drUnit["UnitTypeId"] = (int)unittypes[i];
                drUnit["UnitTypeName"] = unitNames[i];
                dtUnit.Rows.Add(drUnit);
            }
            comboboxColumn.DataSource = dtUnit;
            comboboxColumn.ValueMember = "UnitTypeId";
            comboboxColumn.DisplayMember = "UnitTypeName";
        }
        private void BindPrdGrpComboColumn(DataGridViewComboBoxColumn comboboxColumn, DataTable dtPrdGrp)
        {

            //dt.Columns.Add("GroupDesc", typeof(string));
            //dt.Columns.Add("SortIx", typeof(int));
            //dt.Columns.Add("GrCode", typeof(string));
            //dt.Columns.Add("KISIMNo", typeof(int));


            DataTable dtPg = new DataTable();
            dtPg.Columns.Add("GrCode", typeof(string));
            dtPg.Columns.Add("GroupDesc", typeof(string));
            foreach (DataRow drPg in dtPrdGrp.AsEnumerable())
            {

                DataRow drPgNew = dtPg.NewRow();
                drPgNew["GrCode"] = drPg["GrCode"];
                drPgNew["GroupDesc"] = drPg["GroupDesc"];
                dtPg.Rows.Add(drPgNew);
            }
            comboboxColumn.DataSource = dtPg;
            comboboxColumn.ValueMember = "GrCode";
            comboboxColumn.DisplayMember = "GroupDesc";
        }

        private void BindVATComboColumn(DataGridViewComboBoxColumn comboboxColumn, List<int> vatRates)
        {

            DataTable dtVat = new DataTable();
            dtVat.Columns.Add("VATRate", typeof(int));
            dtVat.Columns.Add("VATRateName", typeof(string));
            bool isZeroAdd = false;
            for (int i = 0; i < vatRates.Count; i++)
            {
                if (vatRates[i] == 0 && isZeroAdd)
                    continue;
                DataRow drVat = dtVat.NewRow();
                drVat["VATRate"] = vatRates[i];
                drVat["VATRateName"] = string.Format("% {0}", vatRates[i]);
                dtVat.Rows.Add(drVat);
                if (vatRates[i] == 0)
                    isZeroAdd = true;
            }
            comboboxColumn.DataSource = dtVat;
            comboboxColumn.ValueMember = "VATRate";
            comboboxColumn.DisplayMember = "VATRateName";
        }
        private void BindKISIMComboColumn(DataGridViewComboBoxColumn comboboxColumn, List<OKCKISIM> kisimList)
        {


            DataTable dtKisim = new DataTable();
            dtKisim.Columns.Add("KISIMNo", typeof(int));
            dtKisim.Columns.Add("KISIMName", typeof(string));


            for (int i = 0; i < kisimList.Count; i++)
            {
                DataRow drKisim = dtKisim.NewRow();
                drKisim["KISIMNo"] = i + 1;
                drKisim["KISIMName"] = kisimList[i].KISIMName;
                dtKisim.Rows.Add(drKisim);

            }
            comboboxColumn.DataPropertyName = "KISIMNo";
            comboboxColumn.DataSource = dtKisim;
            comboboxColumn.ValueMember = "KISIMNo";
            comboboxColumn.DisplayMember = "KISIMName";
        }
        void FillKISIMGrid(KISIMListResult kret)
        {
            lstProgLog.Items.Clear();


            BindUnitTypeComboColumn(colUnitTypeId);

            grdOKCVatList.Rows.Clear();
            try
            {
                if (!kret.HasError)
                {
                    BindVATComboColumn(colVATRate1, kret.VATRates);
                    grdOKCDepList.DataSource = kret.KISIMList;

                    if (kret.VATRates != null)
                    {
                        foreach (int item in kret.VATRates)
                        {
                            grdOKCVatList.Rows.Add("% " + item.ToString());
                        }
                    }
                    _KisimLoaded = true;
                }
                else
                {
                    AddProgLog("Hata kodu:", kret.ErrCode);
                    AddProgLog("Hata    :", kret.ErrDescription);
                }

            }
            catch (Exception ex)
            {
                AddProgLog("Hata    :", ex.Message);
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMain.SelectedTab == tpProgramlama)
            {

                if (!CheckAPIInit())
                {
                    lstTestResult.Items.Clear();
                    AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                    return;
                }

                if (!_KisimLoaded)
                {
                    lstProgLog.Items.Clear();
                    lstTestResult.Items.Clear();
                    _Watch.Restart();
                    try
                    {
                        KISIMListResult kret = _OKCAPI.GetKISIMList();
                        _Watch.Stop();
                        AddProgLog("", "OKC KISIM Bilgileri Yükleme");
                        AddProgLog("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                        AddTestListItem("", "OKC KISIM Bilgileri Yükleme");
                        AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                        FillKISIMGrid(kret);
                        if (!kret.HasError && kret.KISIMList != null)
                            salesS1.SetKISIMList(kret.KISIMList);
                    }
                    catch (Exception ex)
                    {
                        AddProgLog("Hata    :", ex.Message);
                    }
                }
            }
            else if (tabMain.SelectedTab == tpSales)
            {
                salesS1.Select();
            }
        }
        private void FillPrdGroupGrid(ProductGroupResult gret)
        {

            List<OKCKISIM> kisimlist = grdOKCDepList.DataSource as List<OKCKISIM>;
            if (kisimlist == null)
                return;
            BindKISIMComboColumn(colPgKISIMNo, kisimlist);

            DataTable dt = new DataTable();
            dt.Columns.Add("GroupDesc", typeof(string));
            dt.Columns.Add("SortIx", typeof(int));
            dt.Columns.Add("GrCode", typeof(string));
            dt.Columns.Add("KISIMNo", typeof(int));


            try
            {
                foreach (var item in gret.ProductGroupList)
                {
                    DataRow drPg = dt.NewRow();
                    drPg["GroupDesc"] = item.GroupDesc;
                    drPg["SortIx"] = item.SortIx;
                    drPg["GrCode"] = item.GrCode;
                    drPg["KISIMNo"] = item.KISIMNo;
                    dt.Rows.Add(drPg);

                }

                dgProductGroup.DataSource = dt;
            }
            catch (Exception ex)
            {
                AddProgLog("Hata    :", ex.Message);
            }
        }
        private void cmdUpdateKISIM_Click(object sender, EventArgs e)
        {
            lstProgLog.Items.Clear();
            if (!_OKCAPI.IsInited)
            {
                AddProgLog("Hata : ", "OKC API Init Edilmeli");
                return;
            }
            _Watch.Restart();
            List<OKCKISIM> kisimList = grdOKCDepList.DataSource as List<OKCKISIM>;
            if (kisimList == null)
            {
                AddProgLog("Hata : ", "Önce KISIM Listesi Yüklenmeli");
                return;
            }

            try
            {
                MsgResult mret = _OKCAPI.UpdateKISIMList(kisimList);
                _Watch.Stop();
                AddProgLog("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (mret.HasError)
                {
                    AddProgLog("Hata kodu:", mret.ErrCode);
                    AddProgLog("Hata : ", mret.ErrDescription);
                }
                else
                {
                    AddProgLog("", "KISIM Listesi Güncellendi");
                    salesS1.SetKISIMList(kisimList);
                }
            }
            catch (OKCException oex)
            {
                AddProgLog("Hata kodu:", oex.ErrorCode);
                AddProgLog("Hata : ", oex.Message);
            }
            catch (Exception ex)
            {
                AddProgLog("Hata : ", ex.Message);
            }


        }

        private void cmdGetPrdGroup_Click(object sender, EventArgs e)
        {
            try
            {
                lstProgLog.Items.Clear();
                _Watch.Restart();


                if (!_OKCAPI.IsInited)
                {
                    AddProgLog(" ", "OKC API İnit Edilmeli");
                    return;
                }

                ProductGroupResult pret = _OKCAPI.GetProductGroupList();
                _Watch.Stop();

                if (pret.HasError)
                {
                    AddProgLog("Hata kodu:", pret.ErrCode);
                    AddProgLog("Hata : ", pret.ErrDescription);
                }
                else
                {
                    _GroupLoaded = true;
                    FillPrdGroupGrid(pret);
                }
                AddProgLog("", "Ürün Grupları Yüklendi");
                AddProgLog("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
            }
            catch (OKCException ex)
            {
                AddProgLog("Hata Kodu", ex.ErrorCode);
                AddProgLog("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddProgLog("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }

        private void cmdUpdatePrdGroup_Click(object sender, EventArgs e)
        {
            lstProgLog.Items.Clear();
            if (!_OKCAPI.IsInited)
            {
                AddProgLog(" ", "OKC API İnit Edilmeli");
                return;
            }


            DataTable dtPrdGroup = dgProductGroup.DataSource as DataTable;
            List<OKCProductGroupDef> prdGroupList = new List<OKCProductGroupDef>();
            foreach (DataGridViewRow item in dgProductGroup.Rows)
            {
                if (string.IsNullOrEmpty(item.ErrorText) && !string.IsNullOrEmpty(item.Cells["colPgGroupDesc"].Value + ""))
                {
                    prdGroupList.Add(new OKCProductGroupDef()
                    {
                        GroupDesc = item.Cells["colPgGroupDesc"].Value.ToString(),
                        GrCode = item.Cells["colPgGrCode"].Value.ToString(),
                        KISIMNo = item.Cells["colPgKISIMNo"].Value.Convert<int>(),
                        SortIx = item.Cells["colPgSortIx"].Value.Convert<int>()
                    });
                }
            }

            try
            {

                _Watch.Restart();

                MsgResult mret = _OKCAPI.UpdateProductGroupList(prdGroupList, chkPrdGrpReset.Checked);
                _Watch.Stop();
                AddProgLog("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (mret.HasError)
                {
                    AddProgLog("Hata kodu:", mret.ErrCode);
                    AddProgLog("Hata : ", mret.ErrDescription);
                }
                else
                {
                    AddProgLog(" ", "Ürün Grupları Güncellendi");
                }

            }
            catch (OKCException ex)
            {
                AddProgLog("Hata Kodu", ex.ErrorCode);
                AddProgLog("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddProgLog("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }

        }

        private void tabProg_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabProg.SelectedTab == tbpPrdGorup)
            {
                if (!_GroupLoaded)
                {
                    cmdGetPrdGroup_Click(null, null);
                }
            }
        }
        private void dgProductGroup_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {

            DataTable dtPrdGroup = dgProductGroup.DataSource as DataTable;
            if (dtPrdGroup == null)
                return;
            if (e.RowIndex >= (dgProductGroup.Rows.Count - 1))
                return;
            DataGridViewRow drRor = dgProductGroup.Rows[e.RowIndex];
            //dgProductGroup.Columns[

            string error = string.Empty;
            if (string.IsNullOrEmpty(drRor.Cells["colPgGroupDesc"].Value + ""))
                error = "Grup Adı Girilmeli";
            else if (string.IsNullOrEmpty(drRor.Cells["colPgGrCode"].Value + ""))
                error = "Grup Kodu Girilmeli";
            else if (drRor.Cells["colPgGrCode"].Value.ToString().Length != 4)
                error = "Grup Kodu 4 Haneli Olmalı";
            else if (drRor.Cells["colPgKISIMNo"].Value.Convert<int>() == 0)
                error = "Gruba Ait KISIM Seçilmeli";
            else if (drRor.Cells["colPgSortIx"].Value == null)
                error = "Grup sıra numarası girilmeli";
            lstProgLog.Items.Clear();
            if (!string.IsNullOrEmpty(error))
            {
                e.Cancel = true;
                dgProductGroup.Rows[e.RowIndex].ErrorText = error;
                AddProgLog(string.Format("Satır {0} Hata", e.RowIndex + 1), error);
            }
            else
                dgProductGroup.Rows[e.RowIndex].ErrorText = string.Empty;

        }
        private void FillPrdListGrid(List<OKCProductDef> productList)
        {
            List<OKCKISIM> kisimlist = grdOKCDepList.DataSource as List<OKCKISIM>;
            if (kisimlist == null)
                return;
            DataTable dtPrdGrp = dgProductGroup.DataSource as DataTable;
            if (dtPrdGrp == null)
                return;

            BindKISIMComboColumn(colPrdKISIM, kisimlist);
            BindUnitTypeComboColumn(colPrdUnitTypeId);
            BindPrdGrpComboColumn(colPrdGroupCode, dtPrdGrp);

            DataTable dtPr = productList.ToDataTable<OKCProductDef>();
            //Type pt= typeof(OKCProductDef);
            //DataTable dtPr = new DataTable();

            //System.Reflection.PropertyInfo[] pList = pt.GetProperties();
            //foreach (System.Reflection.PropertyInfo item in pList)
            //{
            //    dtPr.Columns.Add(item.Name, item.PropertyType);
            //}


            try
            {
                //foreach (var item in pret.ProductList)
                //{
                //    DataRow drPr = dtPr.NewRow();
                //    foreach (System.Reflection.PropertyInfo pi in pList)
                //    {
                //        drPr[pi.Name] = pi.GetValue(item,null);
                //    }
                //    dtPr.Rows.Add(drPr);
                //}
                dgProducts.DataSource = dtPr;
            }
            catch (Exception ex)
            {
                AddProgLog("Hata    :", ex.Message);
            }
        }
        private void cmdGetProducts_Click(object sender, EventArgs e)
        {
            try
            {
                lstProgLog.Items.Clear();
                _Watch.Restart();


                if (!_OKCAPI.IsInited)
                {
                    AddProgLog(" ", "OKC API İnit Edilmeli");
                    return;
                }

                ProductListResult pret = _OKCAPI.GetProductList();
                _Watch.Stop();

                if (pret.HasError)
                {
                    AddProgLog("Hata kodu:", pret.ErrCode);
                    AddProgLog("Hata : ", pret.ErrDescription);
                }
                else
                {
                    AddProgLog("", "Ürünler Yüklendi");
                    FillPrdListGrid(pret.ProductList);
                }

                AddProgLog("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
            }
            catch (OKCException ex)
            {
                AddProgLog("Hata Kodu", ex.ErrorCode);
                AddProgLog("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddProgLog("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }

        private void cmdUpdateProducts_Click(object sender, EventArgs e)
        {
            lstProgLog.Items.Clear();
            if (!_OKCAPI.IsInited)
            {
                AddProgLog(" ", "OKC API İnit Edilmeli");
                return;
            }

            DataTable dtPrd = dgProducts.DataSource as DataTable;
            if (dtPrd == null)
                return;
            dtPrd.AcceptChanges();
            _Watch.Restart();
            List<OKCProductDef> lstPrd = dtPrd.ToList<OKCProductDef>();

            try
            {
                MsgResult mret = _OKCAPI.UpdateProductList(lstPrd, chkResetProducts.Checked);
                _Watch.Stop();
                AddProgLog("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                if (mret.HasError)
                {
                    AddProgLog("Hata kodu:", mret.ErrCode);
                    AddProgLog("Hata : ", mret.ErrDescription);
                }
                else
                {
                    AddProgLog("Ürün Adet ", lstPrd.Count);
                    AddProgLog(" ", "Ürünler Güncellendi");
                }

            }
            catch (OKCException ex)
            {
                AddProgLog("Hata Kodu", ex.ErrorCode);
                AddProgLog("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddProgLog("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }


        }
        private void dgProducts_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataTable dtPrd = dgProducts.DataSource as DataTable;
            if (dtPrd == null)
                return;
            if (e.RowIndex >= (dgProducts.Rows.Count - 1))
                return;
            DataGridViewRow drRor = dgProducts.Rows[e.RowIndex];


            string error = string.Empty;
            if (string.IsNullOrEmpty(drRor.Cells["colProductName"].Value + ""))
                error = "Ürün Adı Girilmeli";
            else if (string.IsNullOrEmpty(drRor.Cells["colPLU"].Value + "") && string.IsNullOrEmpty(drRor.Cells["colBarcode"].Value + ""))
                error = "Ürün PLU veya Barkod Girilmeli";
            else if (drRor.Cells["colPrdUnitTypeId"].Value == null)
                error = "Ürün Birimi Seçilmeli";
            else if (drRor.Cells["colPrdKISIM"].Value.Convert<int>() == 0)
                error = "Ürün KISIM seçilmeli";
            else if ((drRor.Cells["colPrdGroupCode"].Value + "").Length != 4)
                error = "Ürün Grubu Seçilmeli";
            else if (drRor.Cells["colPrdPrice"].Value.Convert<decimal>() <= 0)
                error = "Ürün Fiyatı '0' dan büyük olmalı";

            lstProgLog.Items.Clear();
            if (!string.IsNullOrEmpty(error))
            {
                e.Cancel = true;
                dgProducts.Rows[e.RowIndex].ErrorText = error;
                AddProgLog(string.Format("Ürün Satır {0} Hata", e.RowIndex + 1), error);
            }
            else
                dgProducts.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        private void cmdFillPrdTest_Click(object sender, EventArgs e)
        {
            _Watch.Restart();
            List<OKCProductDef> lstprd = new List<OKCProductDef>();
            Random rnd = new Random(DateTime.Now.Millisecond);
            int nPrdCount = rnd.Next(120, 1500);

            for (int i = 0; i < nPrdCount; i++)
            {
                lstprd.Add(new OKCProductDef()
                {
                    ProductName = string.Format("Ürün Adı {0:D4}", i + 1),
                    Barcode = string.Format("8690010{0:D4}", i + 1),
                    PLU = string.Format("1230{0:D4}", i + 1),
                    GroupCode = "0000",
                    KISIMNo = rnd.Next(1, 12),
                    UnitTypeId = rnd.Next(1, 13),
                    Price = Math.Round((decimal)(rnd.NextDouble() * 100), 2),
                });
            }
            FillPrdListGrid(lstprd);
            _Watch.Stop();
            lstProgLog.Items.Clear();
            AddProgLog("", "Rastgele Ürünler Üretildi");
            AddProgLog("", string.Format("Ürün Sayısı {0}", nPrdCount));
            AddProgLog("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);

        }















        #endregion Products

        ///KAFE OTOMASYON
        ///
        #region db/degisken
        public string masaName;
        public int ADİSYONID;
        public string sqlCon = "Provider=Microsoft.ACE.OLEDB.12.0;data Source=masaDurumu.accdb";
        OleDbConnection con;
        OleDbConnection con2;
        OleDbConnection con4;
        OleDbCommand cmd;
        DataSet ds;
        OleDbDataAdapter adapter;
        OleDbDataReader dr;
        OleDbDataReader dr2;
        OleDbDataReader dr3;
        MasaSiparisleri ms = new MasaSiparisleri();
        #endregion


        #region MasaProcess

        private void MasaProcess(int masaid)
        {
            
            int adısyonıd = 0;
            double Adetoplam = 0;
            con = new OleDbConnection(sqlCon);
            cmd = new OleDbCommand("Select * from MasaDurumu where ID=" + masaid, con);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string convert_durum = (dr["DURUM"].ToString());
                int durum = Convert.ToInt16(convert_durum);
                if (durum == 2)
                {
                    string convert_ADISYONID = (dr["ADISYONID"].ToString());
                    adısyonıd = Convert.ToInt16(convert_ADISYONID);
                    ms.label4.Text = convert_ADISYONID;
                    masaName = masaid.ToString();
                    ms.label1.Text = masaName;
                    con2 = new OleDbConnection(sqlCon);
                    cmd = new OleDbCommand("SELECT * from SiparisSepeti  WHERE ADISYONID=" + adısyonıd + "and IPTALID = 0", con2);
                    if (con2.State == ConnectionState.Closed)
                    {
                        con2.Open();
                    }
                    dr2 = cmd.ExecuteReader();
                    while (dr2.Read())
                    {
                        string _urunıd = (dr2["URUNID"]).ToString();
                        string _urunadı = (dr2["URUNADI"]).ToString();
                        string _urunfyt = (dr2["URUNFyt"]).ToString();
                        string _adısyonıd = (dr2["ADISYONID"]).ToString();
                        string _urunAdet = (dr2["URUNADET"]).ToString();
                        string _kdvoranı = ("%" + dr2["KDVOranı"].ToString());
                        string _ıptalID = (dr2["IPTALID"].ToString());
                        ListViewItem item = new ListViewItem(_urunıd);
                        item.SubItems.Add(dr2["URUNADI"].ToString());
                        item.SubItems.Add(_urunfyt.ToString());
                        item.SubItems.Add(_adısyonıd.ToString());
                        item.SubItems.Add(_urunAdet.ToString());
                        item.SubItems.Add(_kdvoranı.ToString());
                        item.SubItems.Add(_ıptalID.ToString());
                        ms.listView3.Items.Add(item);

                        if (Convert.ToInt16(_urunAdet) > 0)
                        {

                            Adetoplam = (Convert.ToDouble(_urunfyt) * Convert.ToDouble(_urunAdet) + Adetoplam);
                            ms.textBox2.Text = Adetoplam.ToString();
                            //--//
                            string kdvVv = _kdvoranı;
                            string SplikKdv = _kdvoranı;
                            char[] itemler = { '%' };
                            string[] oran2 = SplikKdv.Split(itemler);

                            kdvVv = oran2[1].ToString();
                            int ESASKDV = Convert.ToInt16(kdvVv);
                            decimal fiyat = Convert.ToDecimal(_urunfyt);
                            decimal kdv = Convert.ToDecimal(ESASKDV);

                            decimal kdvHaric = fiyat / (1 + (kdv / 100));

                            if (ms.textBox3.Text == "")
                            {
                                ms.textBox3.Text = "0";
                            }
                            decimal kdvToplam = fiyat - kdvHaric;
                            decimal genelToplam = (Convert.ToDecimal(ms.textBox3.Text) + kdvToplam);

                            string ConvertFormat = String.Format("{0:F2}", genelToplam);
                            string genelToplam2 = genelToplam.ToString();
                            genelToplam2 = ConvertFormat;
                            ms.textBox3.Text = genelToplam2.ToString();
                        }
                    }
                    con2.Close();
                    con2.Dispose();
                }
                else
                {
                    con = new OleDbConnection(sqlCon);
                    OleDbCommand cmd3 = new OleDbCommand("Select * from ADISYONID", con);
                    dr3 = null;
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    dr3 = cmd3.ExecuteReader();
                    while (dr3.Read())
                    {

                        string convert_ADISYONID = (dr3["ADISYONID"].ToString());
                        adısyonıd = Convert.ToInt16(convert_ADISYONID);
                        adısyonıd++;
                        convert_ADISYONID = adısyonıd.ToString();
                        ms.label4.Text = convert_ADISYONID;
                        masaName = masaid.ToString(); ;
                        ms.label1.Text = masaName;
                    }

                    dr3.Close();
                    con4 = new OleDbConnection(sqlCon);
                    OleDbCommand cmd4 = new OleDbCommand();
                    cmd4.Connection = con4;
                    if (con4.State == ConnectionState.Closed)
                    {
                        con4.Open();
                    }
                    cmd4.CommandText = "update ADISYONID set ADISYONID=" + adısyonıd + "";
                    cmd4.ExecuteNonQuery();
                    con4.Close();
                    con4.Dispose();
                }
            }
            ms.Show();
            this.Hide();
            dr.Close();
        }
        #endregion

        #region BUTONS
        private void button1_Click(object sender, EventArgs e)
        {
            MasaProcess(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MasaProcess(2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MasaProcess(3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MasaProcess(4);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MasaProcess(5);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MasaProcess(6);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MasaProcess(7);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MasaProcess(8);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MasaProcess(9);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            MasaProcess(10);
        }
        #endregion

        #region MasaDurumu

        public void MasaDurumu()
        {
            frmMain fr = new frmMain();
            con = new OleDbConnection(sqlCon);
            cmd = new OleDbCommand("Select DURUM,ID,ADISYONID from MasaDurumu", con);
            dr = null;
            string s_adisyonid = null;
            UniqueStatusResult uret = null;
            string uniqueidAd = null;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string masadurum = dr["DURUM"].ToString();
                if (masadurum == "2")
                {
                    s_adisyonid = (dr["ADISYONID"]).ToString();
                    con2 = new OleDbConnection(sqlCon);
                    cmd = new OleDbCommand("Select uID from uniqueid where adisyonid=" + s_adisyonid + "", con2);
                    dr2 = null;
                    if (con2.State == ConnectionState.Closed)
                    {
                        con2.Open();
                    }
                    dr2 = cmd.ExecuteReader();

                    if (dr2.Read())
                    {
                        uniqueidAd = (dr2["uID"]).ToString();

                    }
                    con2.Close();
                    con2.Dispose();
                    dr2.Close();
                    uret = _OKCAPI.GetUniqueIdStatus(Convert.ToInt32(uniqueidAd));

                    if (uret.UniqueStatus == UniqueIdStatus.Completed && dr["DURUM"].ToString() == "2")
                    {

                        con2 = new OleDbConnection(sqlCon);
                        cmd = new OleDbCommand("update Masadurumu set DURUM=1 where ADISYONID=" + s_adisyonid + "", con2);
                        dr2 = null;
                        if (con2.State == ConnectionState.Closed)
                        {
                            con2.Open();
                        }
                        dr2 = cmd.ExecuteReader();
                        masadurum = "1";
                        con2.Close();
                        con2.Dispose();
                        dr2.Close();

                    }
                }
                if (true)
                {
                    foreach (Control item in tabPage2.Controls)
                    {
                        if (item is Button)
                        {
                            if (item.Text == "Button" + dr["ID"].ToString() && masadurum == "1")
                            {
                                item.BackgroundImage = (System.Drawing.Image)(Properties.Resources.bosMasa1);
                                item.BackColor = Color.LightGreen;
                            }
                            else if (item.Text == "Button" + dr["ID"].ToString() && masadurum == "2")
                            {
                                item.BackgroundImage = (System.Drawing.Image)(Properties.Resources.doluMasa1);
                                item.BackColor = Color.Tomato;
                            }
                        }
                    }
                }
            }      

            button1.Text = "Masa 1";
            button2.Text = "Masa 2";
            button3.Text = "Masa 3";
            button4.Text = "Masa 4";
            button7.Text = "Masa 5";
            button8.Text = "Masa 6";
            button9.Text = "Masa 7";
            button10.Text = "Masa 8";
            button11.Text = "Masa 9";
            button12.Text = "Masa 10";
            con.Close();
            con.Dispose();
            dr.Close();
        }
        #endregion

        private void frmMain_Load(object sender, EventArgs e)
        {
            MasaDurumu();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void salesS1_Load(object sender, EventArgs e)
        {
            
        }

        private void cmdFirmInfo_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return;
            }
            try
            {
                string serialNo = "";
                _Watch.Restart();
                //EchoResult eret = _OKCAPI.Echo();
                FirmInfoResult firinf = _OKCAPI.GetFirmInfo();
                AddTestListItem(" ", " ----FIRM INFO--- ");
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);

                if (firinf.HasError)
                {
                    AddErrorList(firinf);
                }
                else
                {
                    DisplayFirmInfo((firinf));
                }
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }

        public bool IsNumeric(string text)
        {
            foreach (char chr in text)
            {
                if (!Char.IsNumber(chr)) return false;
            }
            return true;
        }

        private void btnHataKodu_Click(object sender, EventArgs e)
        {
            if (txtHataKodu.Text=="")
            {
                MessageBox.Show("Boş Değer Göndermeyiniz"); return;
            }
            bool idControl = IsNumeric(txtHataKodu.Text);
            if (idControl)
            {
                string hkResult = OKCAPI.ErrorDescription(Convert.ToInt16(txtHataKodu.Text)); // Hata Kodu Integer değer olmalıdır.
                MessageBox.Show(hkResult);
            }
            else
                MessageBox.Show("ID Numeric Olmalıdır");
        }

        private void btnAcquirSorgu_Click(object sender, EventArgs e)
        {
            lstTestResult.Items.Clear();
            _Watch.Restart();
            try
            {
                ArrayList sonuc = OKCAPI.BankAcquirerID();
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                AddTestListItem(" ", " ----Banka Kodları--- ");
                foreach (object obj in sonuc)
                {
                    AddTestListItem("", obj);

                }
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
            }
            finally
            {
                _Watch.Stop();
            }
        }
    }
}
