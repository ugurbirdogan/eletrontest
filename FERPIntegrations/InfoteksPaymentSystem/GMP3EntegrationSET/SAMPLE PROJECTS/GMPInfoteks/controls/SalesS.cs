using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMP3Infoteks.OKC;
using GMP3Infoteks.Extention;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Management;

namespace GMPInfoteks.controls
{
    public partial class SalesS : UserControl
    {
        private OKCAPI _OKCAPI = null;
        private int _UniqueId;

        public SalesS()
        {
            InitializeComponent();

            _OKCAPI = OKCAPI.GetInstance();
            _UniqueId = 0;
            TotalPay = 0;
            TotalAmt = 0;
            CashPay = 0;
            FillDiscountType();
            FillDiscountMethod();
            FillInfTypes();
            FillSalesTYPE();

            cmbTCVK.SelectedIndex = 0;

        }



        private OKCAPI API
        {
            get
            {
                if (_OKCAPI == null)
                    _OKCAPI = OKCAPI.GetInstance();
                return _OKCAPI;
            }
        }
        private List<OKCKISIM> _KisimList;
        private List<OKCAppDef> _PayApps;
        private List<BasketItem> _BasketItems = new List<BasketItem>();
        private List<PayBasketItem> _PayItems = new List<PayBasketItem>();
        private BasketItem _SelectedItem;

        public decimal InfTotalAmt { private get; set; }
        public decimal GrosAmt { private get; set; }
        public decimal NetAmt { private get; set; }
        public decimal IncreaseAmtPrd { private get; set; }
        public decimal IncreaseAmt { private get; set; }
        public decimal DiscountAmtPrd { private get; set; }
        public decimal DiscountAmt { private get; set; }
        public decimal TotalAmt { private get; set; }
        public decimal TotalPay { get; set; }
        public decimal CashPay { get; set; }

        private List<OKCKISIM> KisimList
        {
            get
            {
                if (_KisimList == null || _KisimList.Count < 12)
                {
                    if (API.IsInited)
                    {
                        try
                        {
                            KISIMListResult depResult = API.GetKISIMList();
                            if (!depResult.HasError)
                                SetKISIMList(depResult.KISIMList);
                            else
                            {
                                //LOG
                            }
                        }
                        catch (Exception ex)
                        {
                            //LOG
                        }
                    }
                }
                return _KisimList;
            }
        }

        public void SetKISIMList(List<OKCKISIM> kisimlist)
        {
            _KisimList = kisimlist;
            if(_KisimList==null || _KisimList.Count!=12)
                return;
            Button[] cmdK = new Button[] { cmdK01, cmdK02, cmdK03, cmdK04, cmdK05, cmdK06, cmdK07, cmdK08 };
            for (int i = 0; i < 8; i++)
            {
                cmdK[i].Text = string.Format("{0} (%{1})", _KisimList[i].KISIMName, _KisimList[i].VatRate);
                
            }
        }
        internal void SetPayApps(List<OKCAppDef> payApps)
        {
            _PayApps = payApps;
            if (_PayApps != null)
            {
                FillPayAps();
            }
        }
        private void FocusInput()
        {
            txtInpt.Select();
        }


        private void cmdKisim_Click(object sender, EventArgs e)
        {
            if (!CheckAPIInit())
            {
                Log("Hata", "API Init Edilmeli");
                return ;
            }
            string snum = (sender as Button).Name.Replace("cmdK", "");
            int nKisimNo = 0;
            if (int.TryParse(snum, out nKisimNo))
            {
                if (KisimList == null)
                {
                    Log("Hata", "OKC den Kisim Bilgileri Alınmalı");
                    return;
                }
                OnKisim(nKisimNo);
            }
        }


        private void cmdN_Click(object sender, EventArgs e)
        {
            string snum = (sender as Button).Name.Substring(5, 3);
            int nnum = 0;
            if (int.TryParse(snum, out nnum))
            {
                ucDisplay1.AddNumberVal(nnum.ToString());
            }
            FocusInput();
        }

        private void txtInpt_KeyDown(object sender, KeyEventArgs e)
        {
            int nNumber = -1;
            Keys keyCode = e.KeyCode;
            int keyValue = e.KeyValue;
            while (true)
            {
                if (keyCode <= Keys.Return)
                {
                    if (keyCode != Keys.Back)
                    {
                        if (keyCode == Keys.Return)
                        {
                            OnEnterOpr();
                            //On Barcode Or PLU
                            break;
                        }
                    }
                    else
                    {
                        ucDisplay1.RemoveNumberVal();
                        break;
                    }
                }
                else
                {
                    switch (keyCode)
                    {
                        case Keys.Escape:
                            if (!ucDisplay1.IsNormalMode)
                                ucDisplay1.ChangePLUMode(controls.PLUMode.Normal);
                            else
                                ucDisplay1.ClearEnter();
                            break;
                        case Keys.X:
                        case Keys.Multiply:
                            ucDisplay1.DOOprX();
                            break;

                        case Keys.D0:
                        case Keys.D1:
                        case Keys.D2:
                        case Keys.D3:
                        case Keys.D4:
                        case Keys.D5:
                        case Keys.D6:
                        case Keys.D7:
                        case Keys.D8:
                        case Keys.D9:
                            nNumber = keyValue - 48;
                            ucDisplay1.AddNumberVal(nNumber.ToString());
                            break;
                        case Keys.NumPad0:
                        case Keys.NumPad1:
                        case Keys.NumPad2:
                        case Keys.NumPad3:
                        case Keys.NumPad4:
                        case Keys.NumPad5:
                        case Keys.NumPad6:
                        case Keys.NumPad7:
                        case Keys.NumPad8:
                        case Keys.NumPad9:
                            nNumber = keyValue - 96;
                            ucDisplay1.AddNumberVal(nNumber.ToString());
                            break;
                        case Keys.Separator:
                        case Keys.Decimal:
                        case Keys.Oemcomma:
                        case Keys.OemPeriod:
                            ucDisplay1.DotEnter();
                            break;
                    }
                }
                break;
            }
            this.txtInpt.Text = "";
        }
        private void m_btnNPoint_Click(object sender, EventArgs e)
        {
            ucDisplay1.DotEnter();
            FocusInput();
        }
        private void m_btnLineFeed_Click(object sender, EventArgs e)
        {
            ucDisplay1.DOOprX();
            FocusInput();
        }

        private void cmdNBack_Click(object sender, EventArgs e)
        {
            ucDisplay1.RemoveNumberVal();
            FocusInput();
        }
        private void cmdNCancel_Click(object sender, EventArgs e)
        {
            ucDisplay1.ClearEnter();
            FocusInput();
        }

        private void SalesS_MouseClick(object sender, MouseEventArgs e)
        {
            FocusInput();
        }

        private void SalesS_Enter(object sender, EventArgs e)
        {
            FocusInput();
        }

        private void cmdPLU_Click(object sender, EventArgs e)
        {
            if (controls.PLUMode.PLU == ucDisplay1.EnterMode)
            {
                ucDisplay1.ChangePLUMode(controls.PLUMode.Normal);
                Log("", "Normal Giriş Moduna Geçildi");
            }
            else
            {
                Log("", "PLU Moduna Geçildi");
                ucDisplay1.ChangePLUMode(controls.PLUMode.PLU);
            }
            FocusInput();
        }

        private void cmdBcod_Click(object sender, EventArgs e)
        {
            if (controls.PLUMode.BarCode == ucDisplay1.EnterMode)
            {
                ucDisplay1.ChangePLUMode(controls.PLUMode.Normal);
                Log("", "Normal Giriş Moduna Geçildi");
            }
            else
            {
                Log("", "Barkod Moduna Geçildi");
                ucDisplay1.ChangePLUMode(controls.PLUMode.BarCode);
            }
            FocusInput();
        }
        private void cmdNOk_Click(object sender, EventArgs e)
        {
            OnEnterOpr();
            FocusInput();
        }
        private void OnEnterOpr()
        {
            if (!ucDisplay1.IsNormalMode)
            {
                decimal price = 0;
                string sPluOrBarcode = "";
                ucDisplay1.GetPLUAndQty(ref price, ref sPluOrBarcode);
                if (price == 0)
                    price = 1;
                if (string.IsNullOrEmpty(sPluOrBarcode))
                    return;
                OKCBarcodeProduct prd = null;
                if (ucDisplay1.EnterMode == PLUMode.PLU)
                    prd = new OKCPLUProduct();
                else if (ucDisplay1.EnterMode == PLUMode.BarCode)
                    prd = new OKCBarcodeProduct();
                prd.Barcode = sPluOrBarcode;
                prd.Quantity = 1;
                prd.Price = price;
                AddBasketItem(prd);
            }
        }
        private void Log(string col1, string col2)
        {

            ListViewItem lvi = new ListViewItem(new string[] { col1, col2 });
            lvi.SubItems[0].BackColor = lstLog.BackColor;
            lvi.SubItems[1].BackColor = Color.FromArgb(255, 255, 192);
            lvi.UseItemStyleForSubItems = false;
            lstLog.Items.Add(lvi);
        }

        bool ControlSales()
        {
            if (_BasketItems.Count == 0)
            {
                Log("", "Ürün Ekleyiniz");
                return false;
            }

            SalesTYPE nSalesType = (SalesTYPE)cmbSaleType.SelectedValue.Convert<int>();
            if (nSalesType == SalesTYPE.Fatura)
            {
                if (string.IsNullOrEmpty(txtInvSeri.Text.Trim()))
                {
                    Log("", "Fatura Seri Girilmeli");
                    return false;
                }
            }
            if (nSalesType == SalesTYPE.Fatura
                || nSalesType == SalesTYPE.EFatura
                || nSalesType == SalesTYPE.EArsivFatura)
            {
                if (string.IsNullOrEmpty(txtInvNo.Text.Trim()))
                {
                    Log("", "Fatura No Girilmeli");
                    return false;
                }
                if (txtInvTCVK.Text.Trim().Length != txtInvTCVK.MaxLength)
                {
                    Log("", string.Format("{0} Haneli Müşteri TC/VK No Girilmeli",txtInvTCVK.MaxLength));
                    return false;
                }
            }
            

            return true;
        }
        public static int sellCount;
        private Stopwatch _Watch = new Stopwatch();
        void WriteLine(PrintPageEventArgs e, string s, Font font, Brush brush, float y)
        {
            int yazılanSatırCalib = 0;
            yazılanSatırCalib = s.Length;
            int convert = 0;
            float punt = 0;
            punt = font.Size;


            if (font.Name == "Courier New")
            {
                double milConvert = yazılanSatırCalib * 1.33;
                double hesap = (61 - milConvert) / 2;
                convert = Convert.ToInt16(hesap);
            }
            else
            {
                double milConvert2 = yazılanSatırCalib * 1.33;
                double hesap1 = (61 - milConvert2) / 2;
                convert = Convert.ToUInt16(hesap1);
            }
            e.Graphics.DrawString(s, font, brush, convert, y);
        }

        private void PrintVoucher(Object sender, PrintPageEventArgs e)
        {
            SolidBrush kalem = new SolidBrush(Color.Green);
            StringFormat strfrm = new StringFormat();
            float y = 30;
            Font fisBaslik = new Font("Courier New", 11);
            Font standart = new Font("DotumChe", 9);
            Font ucSatır = new Font("Courier New", 7);

            e.Graphics.PageUnit = GraphicsUnit.Millimeter;

            WriteLine(e, "    HOŞGELDİNİZ", standart, Brushes.Black, 2);
            e.Graphics.DrawString("AKYURT SÜPERMARKET GIDA A.Ş", new Font("Courier New", 8, FontStyle.Bold), Brushes.Black, 11, 6);
            WriteLine(e, "MELİH GÖKÇEK BULV. 1368. CADDE", standart, Brushes.Black, 9);
            WriteLine(e, "Numara : 113/2 Yenimahalle Ankara", standart, Brushes.Black, 12);
            //WriteLineYasin(e, "İNFOTEKS", new Font("Courier New", 7), Brushes.Black, 16);
            e.Graphics.DrawString("Tarih   : " + DateTime.Now.ToString("d"), standart, Brushes.Black, 1, 20);
            e.Graphics.DrawString("Saat    : " + DateTime.Now.ToString("t"), standart, Brushes.Black, 1, 24);
            //e.Graphics.DrawString("Fiş No : " + "0001" + "\n\r", standart, Brushes.Black, 45, 24);
            e.Graphics.DrawString("Fiş No : " + "1" + "\n\r", standart, Brushes.Black, 45, 24);
            y = Convert.ToInt16(y) + 3;

            for (int i = 0; i < sellCount; i++)
            {
                string urunadi = "ekmek";
                string kdvoranı = "%8";
                string urunfyt = "5";
                decimal urunFiyatDe = 3;
                string urunaDet = "1";
                string iptalıd = "0";
                string iptalAdet = "0";
                decimal toplamADet = 1;
                decimal urunFyt = toplamADet * Convert.ToDecimal(urunfyt);

                string ConvertFormat = String.Format("{0:F2}", urunFyt);
                string urunFiyat = Convert.ToString(urunFyt);
                urunFiyat = ConvertFormat;




                e.Graphics.DrawString(urunadi + Environment.NewLine, standart, Brushes.Black, 1, y);
                //e.Graphics.DrawString(toplamADet.ToString() + "x" + Environment.NewLine, standart, Brushes.Black, 25, y);
                e.Graphics.DrawString("%" + kdvoranı + Environment.NewLine, standart, Brushes.Black, 35, y);
                e.Graphics.DrawString("*" + urunFiyat, standart, Brushes.Black, 50, y);

                string ConvertFormat2 = String.Format("{0:F2}", urunFiyatDe);
                string urunFiyat2 = Convert.ToString(urunFiyatDe);
                urunFiyat2 = ConvertFormat2;


                if (toplamADet > 1)
                {
                    y = Convert.ToInt16(y) + 3;
                    e.Graphics.DrawString(toplamADet.ToString() + "   ADx   " + urunFiyat2 + " TL" + Environment.NewLine, standart, Brushes.Black, 1, y);

                }
                if (Convert.ToInt16(iptalAdet) > 0)
                {
                    y = Convert.ToInt16(y) + 3;

                    e.Graphics.DrawString("İPTAL" + Environment.NewLine, standart, Brushes.Black, 3, y);
                    y = Convert.ToInt16(y) + 3;
                    e.Graphics.DrawString(urunadi + Environment.NewLine, standart, Brushes.Black, 1, y);
                    //e.Graphics.DrawString(iptalAdet + "x" + Environment.NewLine, standart, Brushes.Black, 25, y);
                    e.Graphics.DrawString("%" + kdvoranı + Environment.NewLine, standart, Brushes.Black, 35, y);
                    decimal _urunfyt = Convert.ToDecimal(urunfyt) * Convert.ToDecimal(iptalAdet);
                    //string urunfiyati = _urunfyt.ToString();

                    ConvertFormat = String.Format("{0:F2}", _urunfyt);
                    string _urunFiyat2 = Convert.ToString(_urunfyt);
                    _urunFiyat2 = ConvertFormat;


                    e.Graphics.DrawString("*-" + _urunFiyat2, standart, Brushes.Black, 50, y);
                    if (Convert.ToInt16(iptalAdet) > 1)
                    {
                        y = Convert.ToInt16(y) + 3;
                        e.Graphics.DrawString(iptalAdet + "   ADx     " + _urunFiyat2 + " TL" + Environment.NewLine, standart, Brushes.Black, 1, y);

                    }
                }
                y = Convert.ToInt16(y) + 3;
            }
            int d;
            d = Convert.ToInt16(y) + 3;
            e.Graphics.DrawString("-------------------------------------------------------------------------------------------------------------" + Environment.NewLine, standart, Brushes.Black, 0, d);
            d = d + 4;
            e.Graphics.DrawString("TOPLAM KDV   :" + Environment.NewLine, new Font("DotumChe", 9, FontStyle.Bold), Brushes.Black, 1, d);
            e.Graphics.DrawString("*" + "!'^" + Environment.NewLine, new Font("DotumChe", 9, FontStyle.Bold), Brushes.Black, 57, d);
            d = d + 4;
            e.Graphics.DrawString("TOPLAM TUTAR :", new Font("DotumChe", 9, FontStyle.Bold), Brushes.Black, 1, d);
            e.Graphics.DrawString("*" + "!'^" + Environment.NewLine, new Font("DotumChe", 9, FontStyle.Bold), Brushes.Black, 57, d);

            d = d + 4;
            e.Graphics.DrawString("-------------------------------------------------------------------------------------------------------------" + Environment.NewLine, standart, Brushes.Black, 0, d);
            d = d + 5;
            e.Graphics.DrawString("İNFOTEKS ŞUBE" + Environment.NewLine, ucSatır, Brushes.Black, 1, d);
            d = d + 3;
            e.Graphics.DrawString("İSTANBUL " + Environment.NewLine, ucSatır, Brushes.Black, 1, d);
            d = d + 3;
            e.Graphics.DrawString("MERSİS NO :0000-0000-0000-0000" + Environment.NewLine, ucSatır, Brushes.Black, 1, d);
            d = d + 3;
            e.Graphics.DrawString("Web : www.infoteks.com.tr" + Environment.NewLine, ucSatır, Brushes.Black, 1, d);
            d = d + 3;
            e.Graphics.DrawString("mail = yazarkasa@infoteks.com.tr" + Environment.NewLine, ucSatır, Brushes.Black, 1, d);
            d = d + 3;
            e.Graphics.DrawString("-------------------------------------------------------------------------------------------------------------" + Environment.NewLine, standart, Brushes.Black, 0, d);
            d = d + 3;
            WriteLine(e, "BİZİ TERCİH ETTİĞİNİZ İÇİN " + Environment.NewLine, ucSatır, Brushes.Black, d);
            d = d + 3;
            WriteLine(e, "TEŞEKKÜR EDERİZ", ucSatır, Brushes.Black, d);
            d = d + 7;
            //e.Graphics.DrawString("Z NO  :" + "0001" + Environment.NewLine, standart, Brushes.Black, 1, d);
            //e.Graphics.DrawString("EKÜ NO :" + "0001" + Environment.NewLine, standart, Brushes.Black, 45, d);
            e.Graphics.DrawString("Z NO  :" + "5" + Environment.NewLine, standart, Brushes.Black, 1, d);
            e.Graphics.DrawString("EKÜ NO :" + "1" + Environment.NewLine, standart, Brushes.Black, 45, d);
            d = d + 5;
            WriteLine(e, _OKCAPI.CurrentOKC.SerialNo, standart, Brushes.Black, d);
            d = d + 5;
            WriteLine(e, "İşlem Tekil No = " + _UniqueId.ToString(), standart, Brushes.Black, d);
        }

        private void cmdSales_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            if (!ControlSales())
                return;
            string inValidMsg = "";
            OKCSalesInfo sales = new OKCSalesInfo();
            sales.SalesType = (SalesTYPE)cmbSaleType.SelectedValue.Convert<int>();
            foreach (var item in _BasketItems)
            {
                inValidMsg = item.OKCPrd.IsValid;
                if (!string.IsNullOrEmpty(inValidMsg))
                {
                    Log("Hata", inValidMsg);
                    return;
                }
                sales.AddProduct(item.OKCPrd);
            }
            if(sales.SalesType==SalesTYPE.Fatura
                || sales.SalesType==SalesTYPE.EFatura
                || sales.SalesType==SalesTYPE.EArsivFatura)
            {
                sales.InvoiceInfo = new OKCInvoiceInfo();
                sales.InvoiceInfo.InvoiceDate = dtInvDate.Value.Date;
                if (sales.SalesType == SalesTYPE.Fatura)
                    sales.InvoiceInfo.InvoiceNo = txtInvSeri.Text + txtInvNo.Text;
                else
                    sales.InvoiceInfo.InvoiceNo = txtInvNo.Text;
                sales.InvoiceInfo.IsIrsaliye = chkInvIrsaliye.Checked;
                if(cmbTCVK.SelectedIndex==0)
                    sales.InvoiceInfo.TCNo=txtInvTCVK.Text;
                else
                    sales.InvoiceInfo.VKNo = txtInvTCVK.Text;
                
            }

            sales.SlipMessages.TopMsg1 = "HOŞGELDİNİZ<n>Satış Test1";
            sales.SlipMessages.DownMsg1 = "TEŞEKKÜRLER";

            if (_PayApps != null && _PayApps.Count > 0)
            {
                foreach (var item in _PayItems)
                {
                    sales.BankPayments.Add(item.Pay); 
                }
            }
            if (CashPay > 0)
            {
                sales.ForeignPayments.Add(new OKCForeignPayments() { LocalAmt = CashPay, CurNo = 949, CurCode = "TRY" });
            }

            sales.PayWithCCard = chkIsCC.Checked;

            sales.TotalAmount = TotalAmt;
            sales.UniqueId = _UniqueId;

            try
            {
                PrintDocument Voucher = new PrintDocument();
                // Ayrık İşlem izni 
                sales.SeperateSlip = 0;
                string aaa = null;
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");


                foreach (ManagementObject printer in searcher.Get())
                {
                    aaa = printer["Name"].ToString().ToLower();
                    if (aaa.Contains(@"XP-80") || aaa.Contains(@"xp-80"))
                    {

                        if (printer["WorkOffline"].ToString().ToLower().Equals
                          ("true"))
                        {
                            // printer is offline by user
                            sales.SeperateSlip = 0;
                            break;
                        }
                        else
                        {
                            // printer is online
                            sales.SeperateSlip = 1;
                            break;
                        }
                    }
                }
                int b;
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(WaitForm1));

                _Watch.Restart();
                SalesResult sret = API.Sales(sales);
                _Watch.Stop();
                //System.Threading.Thread.Sleep(700); // nyb 20.07.18 Peş peşe durmaksızın gönderilen satışlar OKC tarafında gecikmelere sebep olabiliyor. Buda mesaj dönüş sürelerini uzatabiliyor.
                if (sret.SeperatedProc == 0)
                {
                    sellCount = sales.Products.Count;
                    Voucher.PrintPage += new PrintPageEventHandler(PrintVoucher);
                    Voucher.Print();
                }
                Log("Toplam Süre(ms)", _Watch.Elapsed.ToString());
                if (!sret.HasError) //satış tamamlandı veya yarım kaldı(yarım kalan satış okc de bir şekilde tamamlanır,
                {
                    _UniqueId = 0;
                    Log("Z NO     ", sret.ZNo.ToString());
                    Log("EKÜ NO   ", sret.EkuNo.ToString());
                    Log("Fiş No   ", sret.ReceiptNo.ToString());
                    Log(" ", "------Ödeme Bilgileri------");
                    if (sret.CashAmt > 0)
                        Log("Nakit   ", sret.CashAmt.ToString("N2"));
                    if (sret.CCardAmt > 0)
                        Log("Kredi Kartı ", sret.CCardAmt.ToString("N2"));
                    if (sret.CCardPayments != null && sret.CCardPayments.Count > 0)
                    {
                        foreach (CCardPayment item in sret.CCardPayments)
                        {
                            Log("Banka Id ", item.BankAppId.ToString());
                            Log("Tutar ", item.Amount.ToString("N2"));
                            Log("--------", "");
                        }
                    }
                    ClearSales();
                }
                else
                {
                    AddErrorLog(sret);
                    if (sret.ErrCode == 479)    //INVALIDUNIQUEID
                    {

                        // OKCAPI.GetUniqueIdStatus
                        // Satış tamamlandı ise satışı kapat.
                    }
                    
                }
            }
            catch (OKCException ex)
            {
                Log("Hata Kodu", ex.ErrorCode.ToString());
                Log("Hata", ex.Message);
            }
            catch (Exception ex)
            {
                Log("Hata", ex.Message);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
            }

        }
        private void cmbTCVK_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTCVK.SelectedIndex == 0)
                txtInvTCVK.MaxLength = 11;
            else if (cmbTCVK.SelectedIndex == 1)
                txtInvTCVK.MaxLength = 10;
        }

        private void cmbSaleType_SelectedValueChanged(object sender, EventArgs e)
        {
            SalesTYPE nSalesType = (SalesTYPE)cmbSaleType.SelectedValue.Convert<int>();
            switch (nSalesType)
            {
                case SalesTYPE.Fis:
                    pnlInv.Visible = false;

                    break;
                case SalesTYPE.Fatura:
                    pnlInv.Visible = true;
                    lblFatSeri.Text = "Fatura Seri-Sıra";
                    txtInvNo.Visible = true;
                    txtInvSeri.Visible = true;
                   
                    txtInvNo.MaxLength = 10;
                    txtInvNo.Location = new Point(160, 26);
                    txtInvNo.Size = new Size(84, 20);
                    break;
                case SalesTYPE.EFatura:
                    pnlInv.Visible = true;
                    lblFatSeri.Text = "E-Fatura No";
                    
                    txtInvNo.Visible = true;
                    txtInvSeri.Visible = false;

                    txtInvNo.MaxLength = 16;
                    txtInvNo.Location = new Point(112, 26);
                    txtInvNo.Size = new Size(125, 20);
                    break;

                case SalesTYPE.EArsivFatura:
                    pnlInv.Visible = true;
                    lblFatSeri.Text = "E-Arsiv Fatura No";
                   
                    txtInvNo.Visible = true;
                    txtInvSeri.Visible = false;

                    txtInvNo.MaxLength = 16;
                    txtInvNo.Location = new Point(112, 26);
                    txtInvNo.Size = new Size(125, 20);


                    break;
            }

        }
        private bool ControlDiscountAdd(ref decimal price)
        {
            price = 0;
            if (_BasketItems.Count == 0)
            {
                Log("", "Sepette Ürün yokken İndirim/Artırım eklenemez");
                return false;
            }

            if (string.IsNullOrEmpty(txtDiscName.Text.Trim()))
            {
                Log("", "Indirirm/Artırım Adı Girilmeli");
                return false;
            }
            decimal qty = 0;
            ucDisplay1.GetPriceAndQty(ref qty, ref price);
            if (price == 0)
            {
                Log("", "Indirirm/Artırım Tutarı Girilmeli");
                return false;
            }


            bool isIncrease = rdIncrease.Checked;
            DiscountType dType = (DiscountType)cmbDiscType.SelectedValue.Convert<int>();
            DiscountMethod dMethod = (DiscountMethod)cmbDiscMethod.SelectedValue.Convert<int>();

            if (dMethod == DiscountMethod.Percent && price >= 100)
            {
                Log("", "Indirirm/Artırım Oranı 100 ve Üzeri olamaz");
                return false;
            }

            if (dType == DiscountType.Total
                && dMethod == DiscountMethod.Const
                && price >= TotalAmt)
            {
                Log("", "Indirirm/Artırım Tutarı Toplam tutardan küçük olmalı");
                return false;
            }
            BasketItem lastProduct = null;
            if (dType == DiscountType.Product)
            {
                int i = _BasketItems.Count;
                OProductType prdType;
                while (i > 0)
                {
                    i--;
                    prdType = _BasketItems[i].OKCPrd.ProductType;
                    if (prdType == OProductType.InfProduct)
                    {
                        Log("", "Matrahsız Ürün Kalemine İndirim/Artırım uygulanamaz");
                        return false;
                    }
                    if ((prdType == OProductType.Discount || prdType == OProductType.Increase)
                        && (_BasketItems[i].OKCPrd as OKCDiscount).DiscountType == DiscountType.Total)
                    {
                        Log("", "Toplam İndirimden Sonra Ürün indirimi uygulanmaz"); //Üründen sonra toplam indirim varsa ürün indirimi uygulanmaz, yeni ürün eklenip ona ürün indirimi uygulanabilir
                        return false;
                    }
                    else if (_BasketItems[i].OKCPrd.ProductType <= OProductType.BarcodeProduct)
                    {
                        lastProduct = _BasketItems[i]; //indirim uygulanacak en son ürün
                        break;
                    }
                }
                if (lastProduct == null)
                {
                    Log("", "indirim yapılacak ürün bulunamadı");
                    return false;
                }
                if (dMethod == DiscountMethod.Const
                    && price >= lastProduct.NetAmount)
                {
                    Log("", "Ürün Tutarına eşit veya büyük indirim uygulanmaz");
                    return false;
                }
            }
            return true;
        }
        private void cmdAddDiscount_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            decimal price = 0;
            if (!ControlDiscountAdd(ref price))
            {
                FocusInput();
                return;
            }

            bool isIncrease = rdIncrease.Checked;
            DiscountType dType = (DiscountType)cmbDiscType.SelectedValue.Convert<int>();
            DiscountMethod dMethod = (DiscountMethod)cmbDiscMethod.SelectedValue.Convert<int>();
            

            OKCDiscount disc=null;
            if(isIncrease)
                disc=new OKCIncrease();
            else
                disc=new OKCDiscount();
            disc.DiscountType=dType;
            disc.DiscountMethod=dMethod;
            disc.Discount=price;
            disc.Quantity=1;
            disc.ProductName=txtDiscName.Text.Trim();
            AddBasketItem(disc);
            txtDiscName.Text = "";
            cmbDiscType.SelectedIndex = 0;
            cmbDiscMethod.SelectedIndex = 0;
            FocusInput();

        }

        private void cmdAddInfPrd_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();

            if (string.IsNullOrEmpty(txtInfName.Text.Trim()))
            {
                Log("", "Matrahsız Ürün Adı Girilmeli");
                return;
            }
            if (string.IsNullOrEmpty(txtInfName.Text.Trim()))
            {
                Log("", "Matrahsız Ürün Adı Girilmeli");
                return;
            }
            decimal qty = 0, price = 0;
            ucDisplay1.GetPriceAndQty(ref qty, ref price);
            if (price == 0)
            {
                Log("", "Matrahsız Ürün Birim Fiyatı Girilmeli");
                return;
            }
            if (qty == 0)
                qty = 1;
            int infType = cmbItemInfType.SelectedValue.Convert<int>();
            OKCInfProduct inf = new OKCInfProduct();
            inf.ProductName = txtInfName.Text.Trim();
            inf.Price = price;
            inf.Quantity = (double)qty;
            inf.InfType = infType;
            AddBasketItem(inf);
            txtInfName.Text = "";
            cmbItemInfType.SelectedIndex = 0;

        }
        private void cmdRemovePrd_Click(object sender, EventArgs e)
        {
            if (_SelectedItem != null)
            {
                
                if (_SelectedItem.OKCPrd.ProductType == OProductType.Increase
                    || _SelectedItem.OKCPrd.ProductType == OProductType.Discount
                    || _SelectedItem.OKCPrd.ProductType == OProductType.InfProduct)
                {
                    _BasketItems.Remove(_SelectedItem);
                    _SelectedItem = null;
                }
                else if (_SelectedItem.OKCPrd.ProductType <= OProductType.BarcodeProduct)
                {
                    int nIx = _BasketItems.IndexOf(_SelectedItem);
                    nIx++;
                    while (nIx<_BasketItems.Count)
                    {
                        if (_BasketItems[nIx].OKCPrd.ProductType == OProductType.Discount
                            || _BasketItems[nIx].OKCPrd.ProductType == OProductType.Increase)
                        {
                            if ((_BasketItems[nIx].OKCPrd as OKCDiscount).DiscountType == DiscountType.Product)
                                _BasketItems.RemoveAt(nIx);
                        }
                        else
                            break;
                    }
                    _BasketItems.Remove(_SelectedItem);
                    _SelectedItem = null;
                }
                ClearPay();
                UpdateScreen();
            }
            FocusInput();
        }
        private void lstProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelectedItem = null;
            ListViewItem lvi = null;
            if (lstProducts.SelectedItems.Count > 0)
            {
                lvi=lstProducts.SelectedItems[0];
                
            }
            lblSelPrd.Text = "";
            if (lvi != null)
            {
                _SelectedItem = lvi.Tag as BasketItem;
                lblSelPrd.Text=string.Format("{0}  {1} {2} {3}", _SelectedItem.GetLVStrings());
            }
            FocusInput();
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tpPay)
            {
                if (_PayApps == null)
                {
                    lstLog.Items.Clear();
                    try
                    {
                         AppListResult apResult = API.GetPayAppList();
                         if (apResult.HasError)
                         {
                             AddErrorLog(apResult);
                         }
                         else
                         {
                             SetPayApps(apResult.AppList);
                             Log("", "Ödeme uygulamaları yüklendi");
                         }
                    }
                    catch (OKCException oex)
                    {
                        Log("Hata Kodu", oex.ErrorCode.ToString());
                        Log("Hata", oex.Message);
                    }
                    catch (Exception ex)
                    {
                        Log("Hata", ex.Message);
                    }
                }
            }
            FocusInput();

        }
        private void cmbItemInfType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtInfName.Text = cmbItemInfType.Text;
        }
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            FocusInput();
        }
        private void lstProducts_MouseClick(object sender, MouseEventArgs e)
        {
            FocusInput();
        }

        private void cmdAddCash_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            decimal qty = 0, price = 0;
            ucDisplay1.GetPriceAndQty(ref qty, ref price);
            if (price == 0)
            {
                Log("", "Tutar Girilmeli");
                return;
            }
            if ((price + TotalPay) > TotalAmt)
            {
                Log("", "Ödeme Tutarı Toplam Tutardan büyük olmamalı");
                return;
            }
            CashPay += price;
            TotalPay += price;
            lblPay.Text = string.Format("ÖDENEN : {0:N3} TL", TotalPay);
            ucDisplay1.ClearEnter();
            FocusInput();
        }
        
        private void cmdAddCC_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            decimal qty=0,price=0;
            ucDisplay1.GetPriceAndQty(ref qty, ref price);
            if (price == 0)
            {
                Log("", "Tutar Girilmeli");
                return;
            }
            if ((price + TotalPay) > TotalAmt)
            {
                Log("", "Ödeme Tutarı Toplam Tutardan büyük olmamalı");
                return;
            }
            if (cmbPayApps.SelectedItem == null || cmbPayApps.SelectedValue == null)
            {
                Log("", "Ödeme uygulaması Seçilmeli");
            }
            else
            {
                int nAppId=cmbPayApps.SelectedValue.Convert<int>();
                PayBasketItem pbask = null;
                foreach (var item in _PayItems)
                {
                    if (item.Pay.AppId == nAppId)
                    {
                        pbask = item;
                        break;
                    }
                }
                if (pbask == null)
                {
                    OKCBankPayments bp = new OKCBankPayments();
                    bp.Amount = price;
                    bp.AppId = nAppId;
                    pbask = new PayBasketItem(bp, cmbPayApps.Text);
                    _PayItems.Add(pbask);
                }
                else
                {
                    pbask.Pay.Amount += price;
                }
                RefreshPayList();
                TotalPay += price;
                lblPay.Text = string.Format("ÖDENEN : {0:N2}", TotalPay);
                ucDisplay1.ClearEnter();
            }
            FocusInput();
        }
        
        private void chkIsCC_CheckedChanged(object sender, EventArgs e)
        {
            lstPayments.Items.Clear();
        }
        private void cmdClearPay_Click(object sender, EventArgs e)
        {
            ClearPay();
        }

        private void rdIncrease_CheckedChanged(object sender, EventArgs e)
        {
            string sDisc="İndirim";
            string sIncr="Artırım";
            string sDiscountName =sDisc ;
            if (rdIncrease.Checked)
                sDiscountName = sIncr;


            DataTable dtDisc = cmbDiscType.DataSource as DataTable;
            foreach (DataRow item in dtDisc.Rows)
            {
                item["DiscountTypeName"]=item["DiscountTypeName"].ToString().Replace(sIncr,sDiscountName).Replace(sDisc,sDiscountName);
            }

            DataTable dtDiscM = cmbDiscMethod.DataSource as DataTable;
            foreach (DataRow item in dtDiscM.Rows)
            {
                item["DiscountMethodName"] = item["DiscountMethodName"].ToString().Replace(sIncr, sDiscountName).Replace(sDisc, sDiscountName);
            }

            //cmbDiscType
            //cmbDiscMethod
        }
        

        #region Combobox Fill
        private void FillDiscountType()
        {

            DataTable dtDisc = new DataTable();
            dtDisc.Columns.Add("DiscountTypeId", typeof(int));
            dtDisc.Columns.Add("DiscountTypeName", typeof(string));

            DataRow drDisc = dtDisc.NewRow();
            drDisc["DiscountTypeId"] = (int)DiscountType.Product;
            drDisc["DiscountTypeName"] = "Ürün İndirimi";
            dtDisc.Rows.Add(drDisc);

            drDisc = dtDisc.NewRow();
            drDisc["DiscountTypeId"] = (int)DiscountType.Total;
            drDisc["DiscountTypeName"] = "Toplam İndirim";
            dtDisc.Rows.Add(drDisc);

            cmbDiscType.DataSource = dtDisc;
            cmbDiscType.ValueMember = "DiscountTypeId";
            cmbDiscType.DisplayMember = "DiscountTypeName";
            cmbDiscType.SelectedIndex = 0;

        }

        private void FillDiscountMethod()
        {

            DataTable dtDisc = new DataTable();
            dtDisc.Columns.Add("DiscountMethodId", typeof(int));
            dtDisc.Columns.Add("DiscountMethodName", typeof(string));

            DataRow drDisc = dtDisc.NewRow();
            drDisc["DiscountMethodId"] = (int)DiscountMethod.Percent;
            drDisc["DiscountMethodName"] = "% İndirim";
            dtDisc.Rows.Add(drDisc);

            drDisc = dtDisc.NewRow();
            drDisc["DiscountMethodId"] = (int)DiscountType.Total;
            drDisc["DiscountMethodName"] = "Sabit İndirim";
            dtDisc.Rows.Add(drDisc);

            cmbDiscMethod.DataSource = dtDisc;
            cmbDiscMethod.ValueMember = "DiscountMethodId";
            cmbDiscMethod.DisplayMember = "DiscountMethodName";
            cmbDiscMethod.SelectedIndex = 0;


        }
        private void FillInfTypes()
        {
            InfReceiptType[] infTypes = new InfReceiptType[] { InfReceiptType.InvoiceCollect, InfReceiptType.Ilac_Hastane_Katilim, InfReceiptType.Ikram_Zayi_Promosyon };
            string[] infNames = new string[] { "Fatura Komisyon", "İlaç/Eczane Katılım Payı", "İKram/Zayi/Promosyon" };
            DataTable dtInf = new DataTable();
            dtInf.Columns.Add("InfId", typeof(int));
            dtInf.Columns.Add("InfName", typeof(string));
            for (int i = 0; i < infTypes.Length; i++)
            {
                DataRow drInf = dtInf.NewRow();
                drInf["InfId"] = (int)infTypes[i];
                drInf["InfName"] = infNames[i];
                dtInf.Rows.Add(drInf);
            }
            cmbItemInfType.ValueMember = "InfId";
            cmbItemInfType.DisplayMember = "InfName";
            cmbItemInfType.DataSource = dtInf;            
            cmbItemInfType.SelectedIndex = 0;
        }
        private void FillSalesTYPE()
        {
            SalesTYPE[] salesTypes = (SalesTYPE[])Enum.GetValues(typeof(SalesTYPE));
            string[] salesNames = Enum.GetNames(typeof(SalesTYPE));
            DataTable dtSType = new DataTable();
            dtSType.Columns.Add("STypeId", typeof(int));
            dtSType.Columns.Add("STypeName", typeof(string));
            for (int i = 0; i < 4; i++)
            {
                DataRow drSType = dtSType.NewRow();
                drSType["STypeId"] = (int)salesTypes[i];
                drSType["STypeName"] = salesNames[i];
                dtSType.Rows.Add(drSType);
            }
            
            cmbSaleType.ValueMember = "STypeId";
            cmbSaleType.DisplayMember = "STypeName";
            cmbSaleType.DataSource = dtSType;
            cmbSaleType.SelectedIndex = 0;
        }

        private void FillPayAps()
        {
            DataTable dtPayAps = new DataTable();
            dtPayAps.Columns.Add("AppId", typeof(int));
            dtPayAps.Columns.Add("AppName", typeof(string));

            foreach (var item in _PayApps)
            {
                DataRow drPay = dtPayAps.NewRow();
                drPay["AppId"] = item.AppId;
                drPay["AppName"] = item.AppName;
                dtPayAps.Rows.Add(drPay);
            }
            
            cmbPayApps.DataSource = dtPayAps;
            cmbPayApps.ValueMember = "AppId";
            cmbPayApps.DisplayMember = "AppName";
            cmbPayApps.SelectedIndex = 0;
        }

        #endregion

        #region Opr
        private void OnKisim(int nKisimNo)
        {
            if (nKisimNo < 1 && nKisimNo > 12)
            {
                Log("Hata", string.Format("Geçersiz Kısım No", nKisimNo));
                return;
            }
            decimal qty = 0, price = 0;
            ucDisplay1.GetPriceAndQty(ref qty, ref price);

            if (price > 0)
            {
                if (qty == 0)
                    qty = 1;
                OKCKISIM kisim = KisimList[nKisimNo-1];
                OKCProduct prd = new OKCProduct();
                prd.ProductName = kisim.KISIMName;
                prd.Quantity = (double)qty;
                prd.Price = price;

                prd.UnitTypeId = kisim.UnitTypeId;
                prd.VatRate = kisim.VatRate;

                AddBasketItem(prd);

            }
            else
            {
                Log("Hata", "Tutar Girilmeli");
            }
        }
        private void AddErrorLog(MsgResult mret)
        {
            if (mret.LocalError != LocalErrors.ER_SUCCESS)
                Log("Hata", mret.LocalError.ToString());

            else
            {

                Log("Cevap Kodu ", mret.RespCode);
                Log("Hata Kodu  ", mret.ErrCode.ToString());
                Log("Hata       ", mret.ErrDescription);
            }
        }
        private bool CheckAPIInit()
        {
            if (API.IsInited)
                return true;

            try
            {
                API.Init("", "");
            }
            catch (Exception ex)
            {
                
            }
            return API.IsInited;
        }
        private bool GetUniqueId()
        {
            lstLog.Items.Clear();
            if (!CheckAPIInit())
            {
                Log("Hata", "API Init Edilmeli");
                return false;
            }
            UniqueResult uret = null;
            bool result = false;
            Log(" ", " ----UNIQUEID Alınıyor--- ");

            try
            {
                uret = API.GetUniqueId();
                if (uret.HasError)
                {
                    AddErrorLog(uret);
                }
                else
                {
                    _UniqueId = uret.UniqueId;
                    result = true;
                    Log("UniqueId ", _UniqueId.ToString());
                    Log(" ", " --------------------------- ");
                }
            }
            catch (OKCException oex)
            {
                Log("Hata Kodu:" + oex.ErrorCode.ToString(), oex.Message);
            }
            catch (Exception ex)
            {
                Log("", ex.Message);
            }
            finally
            {

            }
            return result;

        }
        private void AddBasketItem(OKCProductBase prd)
        {
            if (_UniqueId == 0 && _BasketItems.Count == 0)
            {
                if (!GetUniqueId())
                    return;
            }
            BasketItem item = new BasketItem(prd);
            _BasketItems.Add(item);
            UpdateScreen();
        }
        private void UpdateScreen()
        {
            CalculateTotal();
            RefreshList();
            lblTotal.Text = string.Format("TOPLAM : {0:N3} TL", TotalAmt);
            ucDisplay1.ClearEnter();            
            FocusInput();
        }
        private void ClearSales()
        {
            _BasketItems.Clear();
            ClearPay();
            lstProducts.Items.Clear();
            _UniqueId = 0;
            _SelectedItem = null;
            cmbSaleType.SelectedIndex = 0;
            txtInvNo.Text = txtInvSeri.Text = txtInvTCVK.Text = "";
            UpdateScreen();
            FocusInput();
        }
        private void RefreshList()
        {
            lstProducts.Items.Clear();
            foreach (BasketItem item in _BasketItems)
            {

                ListViewItem lvi = new ListViewItem(item.GetLVStrings());
                lvi.Tag = item;
                lstProducts.Items.Add(lvi);
            }
            _SelectedItem = null;
            lblSelPrd.Text = "";
        }
        private void CalculateTotal()
        {
            InfTotalAmt = 0;
            GrosAmt = 0;
            NetAmt = 0;
            IncreaseAmtPrd = 0;
            IncreaseAmt = 0;
            DiscountAmtPrd = 0;
            DiscountAmt = 0;
            TotalAmt = 0;

            BasketItem oldPrd = null;
            for (int i = 0; i < _BasketItems.Count; i++)
            {
                BasketItem item = _BasketItems[i];
                OKCProductBase prd = item.OKCPrd;

                if (prd.ProductType == OProductType.InfProduct)
                {
                    InfTotalAmt += prd.TotalPrice();
                }
                if (prd.ProductType == OProductType.NormalProduct
                    || prd.ProductType == OProductType.PLUProduct
                    || prd.ProductType == OProductType.BarcodeProduct)
                {

                    GrosAmt += Math.Round(prd.TotalPrice(), 2, MidpointRounding.AwayFromZero);
                    oldPrd = item;
                }
                else if (prd.ProductType == OProductType.Discount
                    || prd.ProductType == OProductType.Increase)
                {
                    OKCDiscount disc = prd as OKCDiscount;
                    decimal discount = 0;
                    if (disc.DiscountType == DiscountType.Product)
                    {
                        if (oldPrd == null)
                        {
                            //Hata 
                        }
                        if (disc.DiscountMethod == DiscountMethod.Percent)
                            discount = Math.Round( oldPrd.OKCPrd.TotalPrice() * disc.Discount / 100.0M,2,  MidpointRounding.AwayFromZero);
                        else
                            discount = disc.Discount;
                    }
                    else
                    {
                        if (disc.DiscountMethod == DiscountMethod.Percent)
                            discount = Math.Round(NetAmt * disc.Discount / 1000.0M, 3,  MidpointRounding.AwayFromZero); // discount = Math.Round(NetAmt * disc.Discount / 100.0M, 2);
                        else
                            discount = disc.Discount;
                    }

                    if (prd.ProductType == OProductType.Increase)
                    {
                        if (disc.DiscountType == DiscountType.Product)
                        {
                            IncreaseAmtPrd += discount;
                            if (oldPrd != null)
                                oldPrd.PrdIncrease += discount;
                        }
                        else
                            IncreaseAmt += discount;
                    }
                    else
                    {

                        if (disc.DiscountType == DiscountType.Product)
                        {
                            DiscountAmtPrd += discount;
                            if (oldPrd != null)
                                oldPrd.PrdDiscount += discount;
                        }
                        else
                            DiscountAmt += discount;//Genel İndirim
                    }

                }
                NetAmt = GrosAmt + IncreaseAmt + IncreaseAmtPrd - DiscountAmt - DiscountAmtPrd;
            }
            NetAmt = GrosAmt + IncreaseAmt + IncreaseAmtPrd - DiscountAmt - DiscountAmtPrd;
            TotalAmt = NetAmt + InfTotalAmt;
        }
        private void ClearPay()
        {
            TotalPay = 0;
            CashPay = 0;
            _PayItems.Clear();
            lblPay.Text = string.Format("ÖDENEN : {0:N3} TL", TotalPay);
            lstPayments.Items.Clear();
            FocusInput();
        }
        private void RefreshPayList()
        {
            lstPayments.Items.Clear();

            foreach (var item in _PayItems)
            {
                ListViewItem lvi = new ListViewItem(item.GetLVStr());
                lvi.Tag = item;
                lstPayments.Items.Add(lvi);
            }
        }
        #endregion Opr

     






        
    }
}
