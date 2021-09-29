using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Diagnostics;

using GMP3Infoteks.OKC;
using GMP3Infoteks.Utils;
using GMP3Infoteks.Extention;
using System.Drawing.Printing;
using System.Management;

namespace GMPInfoteks
{
    
    public partial class MasaSiparisleri : Form
    {
        private OKCAPI _OKCAPI = null;
        private int _UniqueId;
        private Stopwatch _Watch = new Stopwatch();
        PrintDocument Voucher;
        Font fisBaslik = new Font("Arial Black", 9);
        Font standart = new Font("Calibri (Gövde)", 6);
        SalesResult sret = null;

        public MasaSiparisleri()
        {
            InitializeComponent();
            CreateAPIInstance();
            
            

        }


        #region DataBase
        public string urunıD;
        public string sqlCon = "Provider=Microsoft.ACE.OLEDB.12.0;data Source=masaDurumu.accdb";
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader dr;
        int sayac = 0;
        #endregion

        private void MasaSiparisleri_Load(object sender, EventArgs e)
        {
            b1.Click += new EventHandler(adet);
            b2.Click += new EventHandler(adet);
            b3.Click += new EventHandler(adet);
            b4.Click += new EventHandler(adet);
            b5.Click += new EventHandler(adet);
            b6.Click += new EventHandler(adet);
            b7.Click += new EventHandler(adet);
            b8.Click += new EventHandler(adet);
            b9.Click += new EventHandler(adet);
            b0.Click += new EventHandler(adet);
        }

        private void adet(Object gonder,EventArgs e)
        {
            Button btn = gonder as Button;
            switch (btn.Name)
            {
                case "b1":
                    textBox1.Text += (1).ToString();
                    break;
                case "b2":
                    textBox1.Text += (2).ToString();
                    break;
                case "b3":
                    textBox1.Text += (3).ToString();
                    break;
                case "b4":
                    textBox1.Text += (4).ToString();
                    break;
                case "b5":
                    textBox1.Text += (5).ToString();
                    break;
                case "b6":
                    textBox1.Text += (6).ToString();
                    break;
                case "b7":
                    textBox1.Text += (7).ToString();
                    break;
                case "b8":
                    textBox1.Text += (8).ToString();
                    break;
                case "b9":
                    textBox1.Text += (9).ToString();
                    break;
                case "b0":
                    textBox1.Text += (0).ToString();
                    break;
                default:
                    try
                    {
                        MessageBox.Show("Tanımlanamayan Hata");
                    }
                    catch (Exception ex)
                    {
                       string deneme = ex.Message;
                    }
                    break;
            }
        }

        void urunGetir(int grupID)
        {
            try
            {
                listView1.Items.Clear();
                con = new OleDbConnection(sqlCon);
                cmd = new OleDbCommand("SELECT * FROM URUN WHERE URUNGRPID = " + grupID, con);
                //dr = null;
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ListViewItem item = new ListViewItem(dr["ID"].ToString());
                    item.SubItems.Add(dr["URUNFyt"].ToString());
                    item.SubItems.Add(dr["URUNADI"].ToString());
                    item.SubItems.Add(dr["URUNGRPID"].ToString());
                    item.SubItems.Add("%" + dr["KDVOranı"].ToString());
                    listView1.Items.Add(item);
                }
                con.Close();
                con.Dispose();
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void RefreshList(int adisyonID)
        {
            listView3.Items.Clear();
            listView3.Update();
            listView3.Refresh();
            decimal Adetoplam = 0;

            OleDbDataReader dr2;
            OleDbConnection con2 = new OleDbConnection(sqlCon);
            cmd = new OleDbCommand("SELECT * from SiparisSepeti  WHERE ADISYONID=" + adisyonID + "and IPTALID = 0", con2);
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
                listView3.Items.Add(item);

                if (Convert.ToInt16(_urunAdet) > 0)
                {

                    Adetoplam = (Convert.ToDecimal(_urunfyt) * Convert.ToDecimal(_urunAdet) + Adetoplam);
                    string eneme3 = String.Format("{0:F2}", Adetoplam);
                    textBox2.Text = eneme3;
                }
            }
            con2.Close();
            con2.Dispose();
            dr2.Close();
        }

        #region UrunGrupButon
        private void button1_Click(object sender, EventArgs e)
        {
            urunGetir(40);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            urunGetir(42);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            urunGetir(43);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            urunGetir(41);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            urunGetir(44);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            urunGetir(45);
        }
        private void button16_Click(object sender, EventArgs e)
        {
            textBox1.Text = "1";
        }
        #endregion

        #region Listview ve İçerikleri
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            // Eğer eklenen ilk ürünse ökc den unique id al ve data base e adisyon id ile eşleşecek şekilde kaydet
            if (listView3.Items.Count == 0)
            {
                if (createUniqueId(Convert.ToInt16(label4.Text)) != 0)
                {
                    // Unique id alınamadı işlem başarısız hata logu bas
                    return;
                }          
            }
            try
            {
                string adisyonID = label4.Text;

                if (textBox1.Text == "")
                {
                    textBox1.Text = "1";
                }
                if (textBox2.Text == "")
                {
                    textBox2.Text = "0";
                }
                sayac = listView3.Items.Count;
                listView3.Items.Add(listView1.SelectedItems[0].SubItems[0].Text); // urunıd
                listView3.Items[sayac].SubItems.Add(listView1.SelectedItems[0].SubItems[2].Text); //urunadı
                listView3.Items[sayac].SubItems.Add(listView1.SelectedItems[0].SubItems[1].Text);//ürün fiyatı
                listView3.Items[sayac].SubItems.Add(label4.Text); // adısyon ıd
                listView3.Items[sayac].SubItems.Add(textBox1.Text); // urun adet
                listView3.Items[sayac].SubItems.Add(listView1.SelectedItems[0].SubItems[4].Text); // urun KDV Oranı
                decimal toplam = Convert.ToDecimal(listView1.SelectedItems[0].SubItems[1].Text) * Convert.ToDecimal(textBox1.Text);
                //textBox2.Text = (Convert.ToDecimal(textBox2.Text) + toplam).ToString();

                decimal deneme2 = (Convert.ToDecimal(textBox2.Text) + toplam);
                string eneme3 = String.Format("{0:F2}", deneme2);

                textBox2.Text = eneme3;

                if (textBox3.Text == "")
                {
                    textBox3.Text = "0";
                }

                string kdvVv = listView1.SelectedItems[0].SubItems[4].Text;
                string SplikKdv = listView1.SelectedItems[0].SubItems[4].Text;
                char[] itemler = { '%' };
                string[] oran2 = SplikKdv.Split(itemler);

                kdvVv = oran2[1].ToString();
                int ESASKDV = Convert.ToInt16(kdvVv);
                decimal fiyat = Convert.ToDecimal(listView1.SelectedItems[0].SubItems[1].Text);
                decimal kdv = Convert.ToDecimal(ESASKDV);

                decimal kdvHaric = fiyat / (1 + (kdv / 100));


                decimal kdvToplam = fiyat - kdvHaric;
                decimal genelToplam = (Convert.ToDecimal(textBox3.Text) + kdvToplam);

                string ConvertFormat = String.Format("{0:F2}", genelToplam);
                string genelToplam2 = genelToplam.ToString();
                genelToplam2 = ConvertFormat;
                textBox3.Text = genelToplam2.ToString();

                //KDV Oranı % ifadesinden ayrılıyor.(Split)
                string _KDVORANI = listView3.Items[sayac].SubItems[5].Text;

                string SplitKDV = listView3.Items[sayac].SubItems[5].Text;
                char[] yüzde = { '%' };
                string[] oran = SplitKDV.Split(yüzde);
                _KDVORANI = oran[1].ToString();
                int KDVORANI = Convert.ToInt16(_KDVORANI);

                //string Fiyat = listView3.Items[sayac].SubItems[2].Text; 
                //listviEw3 = 0  urunıd
                //listviEw3 = 1  urunadı
                //4  = ÜrünAdet
                ///
                /// MEVCUT SİPARİŞ SEPETİNDE AYNI ÜRÜNDEN VARMI Kontrolü ? if{
                /// varsa yeni ürün eklemicez, sadece adet güncelliyeceğiz.
                OleDbCommand cmd5 = new OleDbCommand();
                OleDbConnection con5 = new OleDbConnection(sqlCon);
                OleDbDataReader dr5;
                con5.Open();
                cmd5.Connection = con5;
                cmd5 = new OleDbCommand("SELECT * from SiparisSepeti  WHERE ADISYONID=" + label4.Text + "and IPTALID = 0" + "and URUNID=" + listView3.Items[sayac].SubItems[0].Text, con5);
                dr5 = cmd5.ExecuteReader();
                //ListViewItem item = new ListViewItem(dr["ID"].ToString());

                if (dr5.Read())
                {
                    OleDbConnection con = new OleDbConnection(sqlCon);
                    con.Open();
                    cmd.Connection = con;

                    string _urunAdet = (dr5["URUNADET"]).ToString();
                    string _satısıd = (dr5["SATISID"]).ToString();
                    int urunAdet = Convert.ToInt16(_urunAdet);
                    int satisid = Convert.ToInt16(_satısıd);
                    int mevcutToplam = Convert.ToInt16(textBox1.Text);
                    int toplam2 = urunAdet + mevcutToplam;
                    cmd.CommandText = "update SiparisSepeti set URUNADET='" + toplam2.ToString() + "'where SATISID=" + _satısıd + "";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    dr.Close();

                    int AdisyonIDConvert = Convert.ToInt16(label4.Text);
                    RefreshList(AdisyonIDConvert);
                }
                else
                {
                    OleDbConnection con = new OleDbConnection(sqlCon);
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = "insert into SiparisSepeti(URUNFyt,Tarih,URUNID,URUNADI,URUNADET,IPTALID,MASAID,ADISYONID,KDVOranı,IPTALADET  ) values ('" + listView3.Items[sayac].SubItems[2].Text + "','" + DateTime.Now + "','" + listView3.Items[sayac].SubItems[0].Text + "','" +
                    listView3.Items[sayac].SubItems[1].Text + "','" + listView3.Items[sayac].SubItems[4].Text + "','" + 0 + "','" + label1.Text + "','" + label4.Text + "','" + _KDVORANI + "','"+0+ "')";
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                }


                ////////////////////


                if (listView3.Items.Count > 0)  //Masadan çıkmadan ürün durumu kontrol ediliyor eğer listede ürün var ise, DB de masanın durumunu (2) Meşgule çekiyorum.
                {
                    OleDbConnection con3 = new OleDbConnection(sqlCon);
                    con3.Open();
                    cmd.Connection = con3;
                    cmd.CommandText = "update MasaDurumu set DURUM='" + 2 + "',ADISYONID='" + label4.Text + "'where ID=" + label1.Text + "";
                    cmd.ExecuteNonQuery();
                    con3.Close();
                    con3.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            textBox1.Text = "";
        }

        private void listView3_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                OleDbCommand cmd2 = new OleDbCommand();
                OleDbCommand cmd3 = new OleDbCommand();
                OleDbCommand cmd4 = new OleDbCommand();

                string _URUNID = listView3.SelectedItems[0].SubItems[0].Text;
                int URUNID = Convert.ToInt16(_URUNID);

                string _urunfyt = listView3.SelectedItems[0].SubItems[2].Text; // ürünler değişkene atılıp hesaplama yapılacak.
                decimal urunfyt = Convert.ToDecimal(_urunfyt);
                string _urunAdt = listView3.SelectedItems[0].SubItems[4].Text; // ürün adet.
                decimal urunAdt = Convert.ToDecimal(_urunAdt);
                string _urunKDV = listView3.SelectedItems[0].SubItems[5].Text; // ürün adet.

                decimal adetHesap = urunAdt - 1;
                con = new OleDbConnection(sqlCon);
                con.Open();
                cmd2.Connection = con;
                cmd2.CommandText = "update SiparisSepeti set IPTALID='" + 1 + "',URUNADET='" + adetHesap + "'where URUNID= " + URUNID + "and ADISYONID=" + label4.Text + "";
                cmd2.ExecuteNonQuery();

                //listView3.Items.RemoveAt(listView3.SelectedItems[0].Index);
                //decimal toplam = Convert.ToDecimal(textBox2.Text) - urunfyt;
                //textBox2.Text = toplam.ToString();

                decimal toplam = Convert.ToDecimal(textBox2.Text) - urunfyt;
                string ConvertFormat = String.Format("{0:F2}", toplam);
                textBox2.Text = ConvertFormat;

                string adetHesap2 = Convert.ToString(adetHesap);
                int sorgu = Convert.ToInt16(listView3.SelectedItems[0].SubItems[4].Text);
                if (sorgu > 0)
                {
                    listView3.SelectedItems[0].SubItems[4].Text = adetHesap.ToString();


                    OleDbDataReader dr2;
                    OleDbConnection con2 = new OleDbConnection(sqlCon);
                    con2.Open();
                    cmd3 = new OleDbCommand("SELECT * from SiparisSepeti  WHERE URUNID=" + URUNID + "and ADISYONID=" + label4.Text, con2);
                    dr2 = cmd3.ExecuteReader();
                    if (dr2.Read())
                    {
                        string ıptalAdet = (dr2["IPTALADET"]).ToString();
                        if (ıptalAdet == "")
                        {
                            ıptalAdet = "0";
                        }
                        int adet = Convert.ToInt16(ıptalAdet);
                        int hesap = adet + 1;

                        cmd4.Connection = con2;
                        cmd4.CommandText = "update SiparisSepeti set IPTALADET='" + hesap + "'where URUNID= " + URUNID + "and ADISYONID=" + label4.Text + "";
                        cmd4.ExecuteNonQuery();

                    }

                    if (adetHesap2 == "0")
                    {
                        listView3.Items.RemoveAt(listView3.SelectedItems[0].Index);
                    }
                    con2.Close();
                    con2.Dispose();
                }
                con.Close();
                con.Dispose();
                
                if (listView3.Items.Count == 0)
                {
                    // Masayıiptal et
                    CancelMasa();
                    OleDbConnection con3 = new OleDbConnection(sqlCon);
                    con3.Open();
                    cmd3.Connection = con3;
                    cmd3.CommandText = "update MasaDurumu set DURUM='" + 1 + "'where ADISYONID= " + label4.Text + "";
                    cmd3.ExecuteNonQuery();
                }
                
                string kdvVv = _urunKDV;
                string SplikKdv = _urunKDV;
                char[] itemler = { '%' };
                string[] oran2 = SplikKdv.Split(itemler);
                kdvVv = oran2[1].ToString();
                int ESASKDV = Convert.ToInt16(kdvVv);
                urunfyt = Convert.ToDecimal(_urunfyt);
                decimal kdv = Convert.ToDecimal(ESASKDV);
                decimal kdvToplam = urunfyt * (kdv / 100);
                decimal genelToplam = (Convert.ToDecimal(textBox3.Text) - kdvToplam);
                textBox3.Text = genelToplam.ToString();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void button7_Click(object sender, EventArgs e)
        {
            // Data baseden unique id yi oku
            int adisyonid = Convert.ToInt16(label4.Text);
            getUniqueIdfromDB(adisyonid);
            //getUniqueId();
            //Nakit İslem
            // Satış nesnesini oluştur
            OKCSalesInfo salesInfo = new OKCSalesInfo();
            //Satış tipi Fiş
            salesInfo.SalesType = SalesTYPE.Fis;
            //salesInfo.SalesType = SalesTYPE.FisIptal;
            // Fiş Üst ve Alt Başlıklar
            salesInfo.SlipMessages.TopMsg1 = "                   HOŞGELDİNİZ<n>                     Satış Test1";
            salesInfo.SlipMessages.DownMsg1 = "                   TEŞEKKÜRLER";
            salesInfo.SlipMessages.DownMsg2 = "İşlem Tekil No = " + _UniqueId.ToString();
            decimal TotalCost = 0;
            // Adisyona ait ürünleri databaseden getir
            OleDbDataReader dr2;
            OleDbConnection con2 = new OleDbConnection(sqlCon);
            cmd = new OleDbCommand("SELECT * from SiparisSepeti  WHERE ADISYONID=" + adisyonid + "", con2);
            if (con2.State == ConnectionState.Closed)
            {
                con2.Open();
            }
            dr2 = cmd.ExecuteReader();
            // her bir ürün için salesInfo satış bilgisi nesnesine ürün ekle ve toplam tutarı hesapla
            while (dr2.Read())
            {
                string _urunıd = (dr2["URUNID"]).ToString();
                string _urunadı = (dr2["URUNADI"]).ToString();
                string _urunfyt = (dr2["URUNFyt"]).ToString();
                string _adısyonıd = (dr2["ADISYONID"]).ToString();
                string _urunAdet = (dr2["URUNADET"]).ToString();
                string _kdvorani = (dr2["KDVOranı"].ToString());
                string _kdvoranipercent = ("%" + _kdvorani);
                string _ıptalID = (dr2["IPTALID"].ToString());
                if (Convert.ToInt16(_ıptalID) == 0)
                {
                    salesInfo.AddProduct(new OKCProduct() { Price = Convert.ToDecimal(_urunfyt), ProductName = _urunadı, Quantity = Convert.ToInt16(_urunAdet), VatRate = Convert.ToInt16(_kdvorani) });
                    TotalCost = (Convert.ToDecimal(_urunfyt) * Convert.ToDecimal(_urunAdet) + TotalCost);
                }
                else
                    salesInfo.AddProduct(new OKCCancelProduct() { Price = Convert.ToDecimal(_urunfyt), ProductName = _urunadı, Quantity = Convert.ToInt16(_urunAdet), VatRate = Convert.ToInt16(_kdvorani) });
                //textBox2.Text = TotalCost.ToString();
            }

            con2.Close();
            con2.Dispose();
            dr2.Close();

            //salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Karışık Kek", Quantity = 1, VatRate = 18 });
            //salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1.5, ProductName = "Kola 1 LT.", Quantity = 1.5, VatRate = 8 });
            //salesInfo.AddProduct(new OKCProduct() { Price = (decimal)4.99, ProductName = "Sucuklu Pide", Quantity = 1, VatRate = 1 });

           /* int nTestPrdCnt = 0;
            for (int i = 0; i < nTestPrdCnt; i++)
            {
                salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Ürünüm Pide " + i.ToString(), Quantity = 1, VatRate = 1 });
            }
            */
            // Kur bilgisi ekle
            salesInfo.ForeignPayments.Add(new OKCForeignPayments() { LocalAmt = (decimal)0, CurNo = 949, CurCode = "TRY" });
            // Nakit Kredi kartı seçimi nakit seçildi
            salesInfo.PayWithCCard = false;
            // satış nesnesine toplam tutarı  ekle
            salesInfo.TotalAmount = TotalCost;
            // satışa ait unique id yi ekle
            salesInfo.UniqueId = _UniqueId;
            Voucher = new PrintDocument();
            // Ayrık İşlem izni 
            salesInfo.SeperateSlip = 0;
            string aaa = null ;
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
                        salesInfo.SeperateSlip = 0;
                        break;
                    }
                    else
                    {
                        // printer is online
                        salesInfo.SeperateSlip = 1;
                        break;
                    }
                }
            }

            //if(Voucher.PrinterSettings.PrinterName.Contains("XP-80"))
            //{
            
            //    if(Voucher.PrinterSettings.IsValid)
            //        salesInfo.SeperateSlip = 1;
            //}
            //foreach (string yazıcı in PrinterSettings.InstalledPrinters)
            //{
            //    // tanımlı yazıcıları listeler
            //    //comboBox2.Items.Add(yazıcı);
            //    //aaa = yazıcı;
            //    if (yazıcı.Contains("XP-80"))
            //    { 
            //        //driver yüklü
            //        // Online mı kontrol et
            //        Voucher.PrinterSettings.IsValid
            //    }
            //}
            // Masayı kapat
            //listView3.Items.Clear();
            //OleDbCommand cmd3 = new OleDbCommand();
            //OleDbConnection con3 = new OleDbConnection(sqlCon);
            //con3.Open();
            //cmd3.Connection = con3;
            //cmd3.CommandText = "update MasaDurumu set DURUM='" + 1 + "'where ADISYONID= " + label4.Text + "";
            //cmd3.ExecuteNonQuery();
            //con3.Close();
            //con3.Dispose();
            //frmMain fr = new frmMain();
            //this.Hide();
            //fr.Show();
            //////
            _Watch.Restart();
            try
            {
                // Satışı yap
                sret = _OKCAPI.Sales(salesInfo);
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                // Satış başarılımı ?
                if (sret.HasError)
                {
                    // Satış yapılamadı hata kodunu bas
                    AddErrorList(sret);
                }
                else
                {

                    if (sret.SeperatedProc == 1)
                    {
                        
                        Voucher.PrintPage += new PrintPageEventHandler(PrintVoucher);
                        Voucher.Print();
                    }

                    // Satış başarılı bilgileri yaz
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
                    AddTestListItem("Ayrık İşlem   ", sret.SeperatedProc);

                    
                    _UniqueId = 0;
                    AddTestListItem(" ", "---------------------------");
                }
            }
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
                return;
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
                return;
            }
            try
            {
                // Masayı kapat
                OleDbCommand cmd3 = new OleDbCommand();
                OleDbConnection con3 = new OleDbConnection(sqlCon);
                con3.Open();
                cmd3.Connection = con3;
                cmd3.CommandText = "update MasaDurumu set DURUM='" + 1 + "'where ADISYONID= " + label4.Text + "";
                cmd3.ExecuteNonQuery();
                con3.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                OleDbCommand cmd4 = new OleDbCommand();
                OleDbConnection con4 = new OleDbConnection(sqlCon);
                con4.Open();
                cmd4.Connection = con4;
                cmd4.CommandText = "update uniqueid set salesType='" + 0 + "'where adisyonid= " + label4.Text + "";
                cmd4.ExecuteNonQuery();
                con4.Close();
                con4.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            frmMain fr = new frmMain();
            this.Hide();
            fr.Show();
        
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
            e.Graphics.DrawString("Fiş No : " + sret.ReceiptNo.ToString() + "\n\r", standart, Brushes.Black, 45, 24);
            y = Convert.ToInt16(y) + 3;


            //WriteLine(e, "HOŞGELDİNİZ", standart, Brushes.Black, 2);
            //e.Graphics.DrawString("AKYURT SÜPERMARKET GIDA A.Ş", new Font("Courier New", 8, FontStyle.Bold), Brushes.Black, 11, 6);
            //WriteLine(e, "AKYURT SÜPERMARKET GIDA A.Ş", fisBaslik, Brushes.Black, 5);
            //WriteLine(e, "MELİH GÖKÇEK BULV. 1368. CADDE", standart, Brushes.Black, 9);
            //WriteLine(e, "Numara : 113/2 Yenimahalle Ankara", standart, Brushes.Black, 12);
            //WriteLine(e, "İNFOTEKS", standart, Brushes.Black, 16);
            //e.Graphics.DrawString("Tarih   : " + DateTime.Now.ToString("d"), standart, Brushes.Black, 5, 20);
            //e.Graphics.DrawString("Saat    : " + DateTime.Now.ToString("t"), standart, Brushes.Black, 5, 24);
            //e.Graphics.DrawString("Fiş No : " + "0001" + "\n\r", standart, Brushes.Black, 5, 28);
            con = new OleDbConnection(sqlCon);
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "Select * FROM SiparisSepeti WHERE ADISYONID =" + label4.Text + "";
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string urunadi = (dr["URUNADI"].ToString());
                string kdvoranı = (dr["KDVOranı"].ToString());
                string urunfyt = (dr["URUNFyt"].ToString());
                decimal urunFiyatDe = Convert.ToDecimal(urunfyt);
                string urunaDet = (dr["URUNADET"].ToString());
                string iptalıd = (dr["IPTALID"].ToString());
                string iptalAdet = (dr["IPTALADET"].ToString());
                decimal toplamADet = Convert.ToDecimal(iptalAdet) + Convert.ToDecimal(urunaDet);
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
            e.Graphics.DrawString("*" + textBox3.Text + Environment.NewLine, new Font("DotumChe", 9, FontStyle.Bold), Brushes.Black, 57, d);
            d = d + 4;
            e.Graphics.DrawString("TOPLAM TUTAR :", new Font("DotumChe", 9, FontStyle.Bold), Brushes.Black, 1, d);
            e.Graphics.DrawString("*" + textBox2.Text + Environment.NewLine, new Font("DotumChe", 9, FontStyle.Bold), Brushes.Black, 57, d);

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
            e.Graphics.DrawString("Z NO  :" + sret.ZNo.ToString() + Environment.NewLine, standart, Brushes.Black, 1, d);
            e.Graphics.DrawString("EKÜ NO :" + sret.EkuNo.ToString() + Environment.NewLine, standart, Brushes.Black, 45, d);
            d = d + 5;
            WriteLine(e, _OKCAPI.CurrentOKC.SerialNo, standart, Brushes.Black, d);
            d = d + 5;
            WriteLine(e, "İşlem Tekil No = " + _UniqueId.ToString(), standart, Brushes.Black, d);
        }


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

        private void button10_Click(object sender, EventArgs e)
        {
            //Masa İptal
            CancelMasa();

           
        }

        private void CancelMasa()
        {
            int adisyonid = Convert.ToInt16(label4.Text);
            getUniqueIdfromDB(adisyonid);
            //getUniqueId();
            //Nakit İslem
            // Satış nesnesini oluştur
            OKCSalesInfo salesInfo = new OKCSalesInfo();
            //Satış tipi Fiş
            salesInfo.SalesType = SalesTYPE.FisIptal;
            //salesInfo.SalesType = SalesTYPE.FisIptal;
            // Fiş Üst ve Alt Başlıklar
            salesInfo.SlipMessages.TopMsg1 = "                   HOŞGELDİNİZ<n>                     Satış Test1";
            salesInfo.SlipMessages.DownMsg1 = "                   TEŞEKKÜRLER";
            salesInfo.SlipMessages.DownMsg2 = "İşlem Tekil No = " + _UniqueId.ToString();
            decimal TotalCost = 0;
            // Adisyona ait ürünleri databaseden getir
            OleDbDataReader dr2;
            OleDbConnection con2 = new OleDbConnection(sqlCon);
            cmd = new OleDbCommand("SELECT * from SiparisSepeti  WHERE ADISYONID=" + adisyonid + "", con2);
            if (con2.State == ConnectionState.Closed)
            {
                con2.Open();
            }
            dr2 = cmd.ExecuteReader();
            // her bir ürün için salesInfo satış bilgisi nesnesine ürün ekle ve toplam tutarı hesapla
            while (dr2.Read())
            {
                string _urunıd = (dr2["URUNID"]).ToString();
                string _urunadı = (dr2["URUNADI"]).ToString();
                string _urunfyt = (dr2["URUNFyt"]).ToString();
                string _adısyonıd = (dr2["ADISYONID"]).ToString();
                string _urunAdet = (dr2["URUNADET"]).ToString();
                string _kdvorani = (dr2["KDVOranı"].ToString());
                string _kdvoranipercent = ("%" + _kdvorani);
                string _ıptalID = (dr2["IPTALID"].ToString());
                if (Convert.ToInt16(_ıptalID) == 0)
                {
                    salesInfo.AddProduct(new OKCProduct() { Price = Convert.ToDecimal(_urunfyt), ProductName = _urunadı, Quantity = Convert.ToInt16(_urunAdet), VatRate = Convert.ToInt16(_kdvorani) });
                    TotalCost = (Convert.ToDecimal(_urunfyt) * Convert.ToDecimal(_urunAdet) + TotalCost);
                }
                else
                    salesInfo.AddProduct(new OKCCancelProduct() { Price = Convert.ToDecimal(_urunfyt), ProductName = _urunadı, Quantity = Convert.ToInt16(_urunAdet), VatRate = Convert.ToInt16(_kdvorani) });
               // salesInfo.AddProduct(new OKCProduct() { Price = Convert.ToDecimal(_urunfyt), ProductName = _urunadı, Quantity = Convert.ToInt16(_urunAdet), VatRate = Convert.ToInt16(_kdvorani) });

                //TotalCost = (Convert.ToDecimal(_urunfyt) * Convert.ToDecimal(_urunAdet) + TotalCost);
                //textBox2.Text = TotalCost.ToString();
            }

            con2.Close();
            con2.Dispose();
            dr2.Close();

            //salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Karışık Kek", Quantity = 1, VatRate = 18 });
            //salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1.5, ProductName = "Kola 1 LT.", Quantity = 1.5, VatRate = 8 });
            //salesInfo.AddProduct(new OKCProduct() { Price = (decimal)4.99, ProductName = "Sucuklu Pide", Quantity = 1, VatRate = 1 });

            /* int nTestPrdCnt = 0;
             for (int i = 0; i < nTestPrdCnt; i++)
             {
                 salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Ürünüm Pide " + i.ToString(), Quantity = 1, VatRate = 1 });
             }
             */
            // Kur bilgisi ekle
            salesInfo.ForeignPayments.Add(new OKCForeignPayments() { LocalAmt = (decimal)0, CurNo = 949, CurCode = "TRY" });
            // Nakit Kredi kartı seçimi nakit seçildi
            salesInfo.PayWithCCard = false;
            // satış nesnesine toplam tutarı  ekle
            salesInfo.TotalAmount = TotalCost;
            // satışa ait unique id yi ekle
            salesInfo.UniqueId = _UniqueId;
            //// Masayı kapat
            //listView3.Items.Clear();
            //OleDbCommand cmd3 = new OleDbCommand();
            //OleDbConnection con3 = new OleDbConnection(sqlCon);
            //con3.Open();
            //cmd3.Connection = con3;
            //cmd3.CommandText = "update MasaDurumu set DURUM='" + 1 + "'where ADISYONID= " + label4.Text + "";
            //cmd3.ExecuteNonQuery();
            //con3.Close();
            //con3.Dispose();
            //frmMain fr = new frmMain();
            //this.Hide();
            //fr.Show();
            //////
            _Watch.Restart();
            try
            {
                // Satışı yap
                SalesResult sret = _OKCAPI.Sales(salesInfo);
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                // Satış başarılımı ?
                if (sret.HasError)
                {
                    // Satış yapılamadı hata kodunu bas
                    AddErrorList(sret);
                }
                else
                {
                    // Satış başarılı bilgileri yaz
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
            catch (OKCException ex)
            {
                AddTestListItem("Hata Kodu", ex.ErrorCode);
                AddTestListItem("Hata", ex.Message);
                return;
            }
            catch (Exception ex)
            {
                AddTestListItem("Hata", ex.Message);
                return;
            }

            try
            {
                OleDbCommand cmd4 = new OleDbCommand();
                OleDbConnection con4 = new OleDbConnection(sqlCon);
                con4.Open();
                for (int i = 0; i < listView3.Items.Count; i++)
                {
                    cmd4.Connection = con4;
                    cmd4.CommandText = "update SiparisSepeti set IPTALID='" + 1 + "'where ADISYONID= " + label4.Text + "";
                    cmd4.ExecuteNonQuery();
                }
                cmd4.Connection = con4;
                cmd4.CommandText = cmd4.CommandText = "update MasaDurumu set DURUM='" + 1 + "'where ADISYONID= " + label4.Text + "";
                cmd4.ExecuteNonQuery();
                con4.Close();
                con4.Dispose();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            try
            {
                OleDbCommand cmd4 = new OleDbCommand();
                OleDbConnection con4 = new OleDbConnection(sqlCon);
                con4.Open();
                cmd4.Connection = con4;
                cmd4.CommandText = "update uniqueid set salesType='" + 80 + "'where adisyonid= " + label4.Text + "";
                cmd4.ExecuteNonQuery();
                con4.Close();
                con4.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            frmMain fr = new frmMain();
            this.Hide();
            fr.Show();
        
        
        
        }
        private void button8_Click(object sender, EventArgs e)
        {
            //Banka uygulama seçimi ÖKC den yapılsın diye aşağıdaki değerlere 0 atılıyor
            int nBankAppId1 = 0;
            decimal dBankAppAmount1 = 0;
            int nBankAppId2 = 0;
            decimal dBankAppAmount2 = 0;
            // Data baseden unique id yi oku
            int adisyonid = Convert.ToInt16(label4.Text);
            getUniqueIdfromDB(adisyonid);
            //Kredi Kartı ile Sonlandır
            listView2.Items.Clear();
            // API Init Kontrol
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
            // Satış Nesnesi oluştur
            OKCSalesInfo sales = new OKCSalesInfo();
            // Satıl Tipi Fiş
            sales.SalesType = SalesTYPE.Fis;
            // Satış Detay nesnesinioluştur
            OKCSalesInfo salesInfo = new OKCSalesInfo();
            salesInfo.SlipMessages.TopMsg1 = "HOŞGELDİNİZ<n>Satış Test1";
            salesInfo.SlipMessages.DownMsg1 = "TEŞEKKÜRLER";
            salesInfo.SlipMessages.DownMsg2 = "İşlem Tekil No = " + _UniqueId.ToString();


            decimal TotalCost = 0;
            // Adisyona ait ürünleri databaseden getir
            OleDbDataReader dr2;
            OleDbConnection con2 = new OleDbConnection(sqlCon);
            cmd = new OleDbCommand("SELECT * from SiparisSepeti  WHERE ADISYONID=" + adisyonid + "", con2);
            if (con2.State == ConnectionState.Closed)
            {
                con2.Open();
            }
            dr2 = cmd.ExecuteReader();
            // her bir ürün için salesInfo satış bilgisi nesnesine ürün ekle ve toplam tutarı hesapla
            while (dr2.Read())
            {
                string _urunıd = (dr2["URUNID"]).ToString();
                string _urunadı = (dr2["URUNADI"]).ToString();
                string _urunfyt = (dr2["URUNFyt"]).ToString();
                string _adısyonıd = (dr2["ADISYONID"]).ToString();
                string _urunAdet = (dr2["URUNADET"]).ToString();
                string _kdvorani = (dr2["KDVOranı"].ToString());
                string _kdvoranipercent = ("%" + _kdvorani);
                string _ıptalID = (dr2["IPTALID"].ToString());
                if (Convert.ToInt16(_ıptalID) == 0)
                {
                    salesInfo.AddProduct(new OKCProduct() { Price = Convert.ToDecimal(_urunfyt), ProductName = _urunadı, Quantity = Convert.ToInt16(_urunAdet), VatRate = Convert.ToInt16(_kdvorani) });
                    TotalCost = (Convert.ToDecimal(_urunfyt) * Convert.ToDecimal(_urunAdet) + TotalCost);
                }
                else
                    salesInfo.AddProduct(new OKCCancelProduct() { Price = Convert.ToDecimal(_urunfyt), ProductName = _urunadı, Quantity = Convert.ToInt16(_urunAdet), VatRate = Convert.ToInt16(_kdvorani) });
                //salesInfo.AddProduct(new OKCProduct() { Price = Convert.ToDecimal(_urunfyt), ProductName = _urunadı, Quantity = Convert.ToInt16(_urunAdet), VatRate = Convert.ToInt16(_kdvorani) });

                //TotalCost = (Convert.ToDecimal(_urunfyt) * Convert.ToDecimal(_urunAdet) + TotalCost);
                //textBox2.Text = TotalCost.ToString();
            }

            con2.Close();
            con2.Dispose();
            dr2.Close();


            //salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1, ProductName = "Karışık Kek", Quantity = 1, VatRate = 18 });
            //salesInfo.AddProduct(new OKCProduct() { Price = (decimal)1.5, ProductName = "Kola 1 LT.", Quantity = 1.5, VatRate = 8 });
            //salesInfo.AddProduct(new OKCProduct() { Price = (decimal)4.99, ProductName = "Sucuklu Pide", Quantity = 1, VatRate = 1 });

            salesInfo.ForeignPayments.Add(new OKCForeignPayments() { LocalAmt = (decimal)0, CurNo = 949, CurCode = "TRY" });
            if (nBankAppId1 > 0 && dBankAppAmount1 > 0)
                salesInfo.BankPayments.Add(new OKCBankPayments() { Amount = dBankAppAmount1, AppId = nBankAppId1 });            //nBankAppId1 Ödeme Uygulamasından dBankAppAmount1 miktarında çekilsin
            if (nBankAppId2 > 0 && dBankAppAmount2 > 0)
                salesInfo.BankPayments.Add(new OKCBankPayments() { Amount = dBankAppAmount2, AppId = nBankAppId2 });            //nBankAppId2 Ödeme Uygulamasından dBankAppAmount2 miktarında çekilsin

            // Kredi Kartı ile ödensin
            salesInfo.PayWithCCard = true;      //Kredi Kartlı Ödeme Yapılsın
            //Toplam Tutar
            salesInfo.TotalAmount = TotalCost;
            // İşlem unique ID
            salesInfo.UniqueId = _UniqueId;

            // Satışı yap
            SalesResult sret = _OKCAPI.Sales(salesInfo);

            if (sret.HasError)
            {
                AddErrorList(sret);
                return;
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
            try
            {
                OleDbCommand cmd3 = new OleDbCommand();
                OleDbConnection con3 = new OleDbConnection(sqlCon);
                con3.Open();
                cmd3.Connection = con3;
                cmd3.CommandText = "update MasaDurumu set DURUM='" + 1 + "'where ADISYONID= " + label4.Text + "";
                cmd3.ExecuteNonQuery();
                con3.Close();
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                OleDbCommand cmd4 = new OleDbCommand();
                OleDbConnection con4 = new OleDbConnection(sqlCon);
                con4.Open();
                cmd4.Connection = con4;
                cmd4.CommandText = "update uniqueid set salesType='" + 0 + "'where adisyonid= " + label4.Text + "";
                cmd4.ExecuteNonQuery();
                con4.Close();
                con4.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            frmMain fr = new frmMain();
            this.Hide();
            fr.Show();
        }
        #endregion

        private void MasaSiparisleri_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void MasaSiparisleri_FormClosed(object sender, FormClosedEventArgs e)
        {

            frmMain form = new frmMain();
            form.Show();
            this.Hide();
            
        }

        private void MasaSiparisleri_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (listView3.Items.Count > 0)
            {
                frmMain fr = new frmMain();
                fr.MasaDurumu();
                if (listView3.Items.Count > 0)
                {
                    BackgroundImage = (System.Drawing.Image)(Properties.Resources.doluMasa);
                }
                else
                {
                    BackgroundImage = (System.Drawing.Image)(Properties.Resources.bosMasa);


                }
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
            lvi.SubItems[0].BackColor = listView2.BackColor;
            lvi.SubItems[1].BackColor = Color.FromArgb(255, 255, 192);
            lvi.UseItemStyleForSubItems = false;
            listView2.Items.Add(lvi);
        }
        private void AddProgLog(string col1, object col2)
        {
            string scol2 = "";
            if (col2.GetType() == typeof(bool))
                scol2 = ((bool)col2).ToTrString();
            else
                scol2 = col2.ToString();


            ListViewItem lvi = new ListViewItem(new string[] { col1, scol2 });
            lvi.SubItems[0].BackColor = listView2.BackColor;
            lvi.SubItems[1].BackColor = Color.FromArgb(255, 255, 192);
            lvi.UseItemStyleForSubItems = false;
            listView2.Items.Add(lvi);
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

        private void CreateAPIInstance()
        {

            OKCAPI.CreateInstnce("infoteks", "410PC");
            _OKCAPI = OKCAPI.GetInstance();
            InitOKCAPI();
        }
        private void InitOKCAPI()
        {
            listView2.Items.Clear();
            AddTestListItem("", "--------OKC API INIT----------");
            _Watch.Restart();
            try
            {

                _OKCAPI.Init("", "");                        //OKC seri no belli seri noya göre portu bul
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

        private int createUniqueId(int adisyonid)
        {
            UniqueResult uret = null;
            listView2.Items.Clear();
            // OKC API init edilmişmi kontrol et
            if (!CheckAPIInit())
            {
                AddTestListItem(" ", " ----OKC API Init Edilmeli--- ");
                return 1;
            }
            AddTestListItem(" ", " ----OKC UNIQUEID AL--- ");
            _Watch.Restart();
            try
            {
                // ÖKC den unique id al
                uret = _OKCAPI.GetUniqueId(adisyonid);
                _Watch.Stop();
                AddTestListItem("Toplam Süre(ms)", _Watch.ElapsedMilliseconds);
                // Başarılı mı ?
                if (uret.HasError)
                {
                    // alınamadı hata mesajı bas
                    AddErrorList(uret);
                    
                }
                else
                {
                    // Alındı unique id yi kaydet
                    _UniqueId = uret.UniqueId;
                    AddTestListItem("UniqueId ", _UniqueId.ToString());
                    AddTestListItem(" ", " --------------------------- ");
                }
                if (uret.OKCStatus != null)
                    DisplayOKCStatus(uret.OKCStatus);



            }
            catch (OKCException oex)
            {
                AddTestListItem("Hata Kodu:" + oex.ErrorCode.ToString(), oex.Message);
                return 1;
            }
            catch (Exception ex)
            {
                AddTestListItem("", ex.Message);
                return 1;
            }
            finally
            {
                _Watch.Stop();
            }

            // Başarılı alındı database kaydet
            OleDbDataReader dr2;
            OleDbConnection con2 = new OleDbConnection(sqlCon);
            
            cmd = new OleDbCommand("insert into uniqueid(adisyonid,uID)values(" + adisyonid.ToString() + "," + _UniqueId.ToString() + ")", con2);
            if (con2.State == ConnectionState.Closed)
            {
                con2.Open();
            }
            dr2 = cmd.ExecuteReader();

            con2.Close();
            con2.Dispose();
            dr2.Close();
            return 0;
        }

        private int getUniqueIdfromDB(int adisyonid)
        {
            // Database den adisyona ait unique id yi al
            // Başarılı alındı database kaydet
            OleDbDataReader dr2;
            OleDbConnection con2 = new OleDbConnection(sqlCon);

            cmd = new OleDbCommand("select * from uniqueid where adisyonid="+adisyonid.ToString() +"", con2);
            if (con2.State == ConnectionState.Closed)
            {
                con2.Open();
            }
            dr2 = cmd.ExecuteReader();
            while (dr2.Read())
            {
                _UniqueId = Convert.ToInt32(dr2["uID"]);
                
            }       
            con2.Close();
            con2.Dispose();
            dr2.Close();
            return 0;
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

        private void hsp1_Click(object sender, EventArgs e)
        {

        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
