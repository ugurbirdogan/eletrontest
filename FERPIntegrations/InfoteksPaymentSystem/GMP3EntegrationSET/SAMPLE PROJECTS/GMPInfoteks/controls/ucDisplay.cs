using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GMPInfoteks.controls
{
    public enum PLUMode
    {
        Normal = 0,
        PLU = 1,
        BarCode = 2
    }

    public partial class ucDisplay : UserControl
    {
        public ucDisplay()
        {
            InitializeComponent();
            HighEnterVal = "";
            LowEnterVal = "";
            cmdPLU.Image = Properties.Resources.PLUBMP;
            ChangePLUMode(PLUMode.Normal);
        }
        
        public bool IsNormalMode
        {
            get
            {
                return EnterMode == PLUMode.Normal;
            }
        }
        public PLUMode EnterMode { get; set; }
        public decimal PrdOprCnt { get; set; }
        public bool IsDotEnter { get; set; }
        private string HighEnterVal { get; set; }
        private string LowEnterVal { get; set; }



        public decimal GetEnteredPrice()
        {

            int nGighVal = 0;
            int.TryParse(HighEnterVal, out nGighVal);
            int nLowVal = 0;
            if (IsDotEnter)
            {
                int.TryParse(LowEnterVal, out nLowVal);
                if (LowEnterVal.Length == 1 && nLowVal < 10)
                    nLowVal *= 100; //10
            }
            decimal dRetVal = 0;
            if (LowEnterVal.Length == 2)
                dRetVal = nGighVal + (decimal)nLowVal / 100.0M;
            else
                dRetVal = nGighVal + (decimal)nLowVal / 1000.0M; //100.0M
            return dRetVal;
        }

        public string GetEnteredText()
        {
            string sret = HighEnterVal + "";
            if (IsDotEnter)
            {
                if (sret.Length == 0)
                    sret += "0";
                sret += ",";
                sret += LowEnterVal;
            }
            return sret;
        }
        public void GetPriceAndQty(ref decimal quantity, ref decimal price)
        {
            price = quantity = 0;
            decimal fPrice = GetEnteredPrice();
            if (fPrice <= 0 && PrdOprCnt <= 0)
            {
                return;
            }
            else if (fPrice <= 0 && PrdOprCnt > 0)
            {
                price = PrdOprCnt; //ilk girilen tutar 2. girlen qty
                /*quantity=1;*/
            }
            else if (fPrice > 0 && PrdOprCnt > 0)
            {
                price = fPrice;
                quantity = PrdOprCnt;
            }
            else if (fPrice > 0 && PrdOprCnt <= 0)
            {
                price = fPrice;
                //quantity=1;
            }
        }
        public void GetPLUAndQty(ref decimal quantity, ref string plu)
        {
            quantity = PrdOprCnt;
            plu = HighEnterVal;
        }
        public void AddNumberVal(string number)
        {
            if (IsDotEnter)
            {
                if (number == "0" && LowEnterVal == "0")
                    return;
                if (LowEnterVal.Length < 3) //<2
                    LowEnterVal += number;
            }
            else
            {
                if (number == "0" && HighEnterVal.Length == 0)
                    return;
                int maxLen = 6;
                if (!IsNormalMode)
                    maxLen = 13;
                if (HighEnterVal.Length < maxLen)
                {
                    HighEnterVal += number;
                }
            }
            UpdateDisplay();
        }
        public bool RemoveNumberVal()
        {
            if (IsDotEnter)
            {
                if (LowEnterVal.Length > 0)
                {
                    LowEnterVal = LowEnterVal.Substring(0, LowEnterVal.Length - 1);
                }
                else
                {
                    IsDotEnter = false;
                }
            }
            else
            {
                if (HighEnterVal.Length > 0)
                {
                    HighEnterVal = HighEnterVal.Substring(0, HighEnterVal.Length - 1);
                    UpdateDisplay();
                }
                else if (PrdOprCnt > 0)
                {//Önceki Değere Geç
                    HighEnterVal = PrdOprCnt.ToString();
                    int nLowVal = (int)Math.Round((PrdOprCnt - ((int)PrdOprCnt)) * 100, 2);
                    LowEnterVal = nLowVal.ToString();
                    if (nLowVal > 0)
                        IsDotEnter = true;

                    PrdOprCnt = 0;
                }
                else
                {

                    return false; //
                }
            }
            UpdateDisplay();
            return true;
        }
        public bool ChangePLUMode(PLUMode mode)
        {
            if (EnterMode == mode)
                return false;
            EnterMode=mode;
            
            ArrangeDisplay();
            ClearEnter();
            return true;
        }
        public void DotEnter()
        {
            if (!IsDotEnter)
            {
                if (IsNormalMode || (!IsNormalMode && PrdOprCnt == 0)) //PLU modda Miktar girildi ise Dot Ekleme
                {
                    IsDotEnter = true;
                    LowEnterVal = "";

                    UpdateDisplay();
                }
            }
        }
        public void ClearEnter()
        {
            PrdOprCnt = 0;
            LowEnterVal = HighEnterVal = "";

            IsDotEnter = false;
            SetDisplayText("");
        }
        public void SetTotalAmt(string sTotal)
        {
        }
        public void SetDisplayText(string text)
        {
            if (text.Length == 0 && IsNormalMode)
                text = "0,00";
            else
                text.Replace('.', ',');
            txtDisplay.Text = text;
            if (text.Length == 0)
                return;
            int maxLen = 8;
            if (!IsNormalMode)
                maxLen = 6;
            //if (text.Length <= maxLen)
            //    txtDisplay.Font= new Font();
            //else
            //    txtDisplay.Font = new Font();
        }
        public void UpdateDisplay()
        {
            string text = "";
            if (IsNormalMode)
            {
                decimal dValue = GetEnteredPrice();
                if (!IsDotEnter && (int)dValue > 0)
                    text = ((int)dValue).ToString();
                else if (!IsDotEnter)
                    text = "";
                else
                    text = string.Format("{0:N3}", dValue);
                
            }
            else
            {
                text = HighEnterVal;
            }
            
            if (PrdOprCnt > 0)
            {
                if (text.Length > 0)
                    text = string.Format("{0:N2}{1}{2} {3}", PrdOprCnt, "X", text, !IsNormalMode ? "" : "TL");
                else
                    text=string.Format("{0:N2}{1}", PrdOprCnt, "X");
            }
            SetDisplayText(text);
        }
        public void DOOprX()
        {
            PrdOprCnt = GetEnteredPrice();
            PrdOprCnt = Math.Max(0, Math.Min(PrdOprCnt, 999999));

            LowEnterVal = HighEnterVal = "";
            IsDotEnter = false;

            if (PrdOprCnt == 0)
            {
                MessageBox.Show("Çarpma işlemi yapmak için tutar alanına katsayı girmelisiniz.", "UYARI");
                return;
            }

            if (PrdOprCnt > 0) 	//old 1.25X ne 1.1 x
            {
                UpdateDisplay();
                return;
            }
        }
        public void ArrangeDisplay()
        {
            if (!IsNormalMode)
                cmdPLU.Visible = true;
            else
                cmdPLU.Visible = false;
            if (EnterMode == PLUMode.BarCode)
                cmdPLU.Image = Properties.Resources.bkodBMP;
            else if (EnterMode == PLUMode.PLU)
                cmdPLU.Image = Properties.Resources.PLUBMP;
        }

        private void cmdPLU_Click(object sender, EventArgs e)
        {
            ChangePLUMode(PLUMode.Normal);
            
        }

    }
}
