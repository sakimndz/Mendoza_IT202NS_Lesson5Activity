using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtNetIncome.Text))
            {
                MessageBox.Show("Invalid Income Input.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            };

            PaySlipDTO result = new PaySlipDTO

            {
                Company = "Lyceum of the Philippines University Cavite",
                EmployeeCode = txtEmployeeNum.Text,
                EmployeeName = txtSurname.Text + txtFirstName.Text + txtMiddleName.Text,
                Department = txtDepartment.Text,
                CutOff = dateTimePicker1.Value.ToString(),
                PayPeriod = dateTimePicker1.Value.ToString(),
                BasicPayHours = txtBasicNumHours.Text,
                BasicPay = txtBasicIncome.Text,
                OvertimeHours = txtOtherNumHours.Text,
                OvertimePay = txtOtherIncomePay.Text,
                HonorariumHours = txtHonorNumHours.Text,
                HonorariumPay = txtHonorariumPay.Text,
                WithholdingTax = txtTax.Text,
                PhilHealthCont = txtPhilHealthCont.Text,
                PagIBIGCont = txtPagibigCont.Text,
                SSSCont = txtSSSCont.Text,
                GrossIncome = txtGrossIncome.Text,
                Deductions = txtTotalDeduc.Text,
                NetIncome = txtNetIncome.Text,
            };

            Form2 secondForm = new Form2(result);
            secondForm.ShowDialog();
        }

        private void btnNetIncome_Click(object sender, EventArgs e)
        {
            CalculateNetIncome();
        }

        private void btnGrossIncome_Click(object sender, EventArgs e)
        {
            CalculateGrossIncome();
            if (decimal.TryParse(txtGrossIncome.Text, out decimal grossIncome))
            {
                var deductions = PayrollService.CalculateAll(grossIncome);

                txtSSSCont.Text = deductions.SSS.ToString("N2");
                txtPhilHealthCont.Text = deductions.PhilHealth.ToString("N2");
                txtPagibigCont.Text = deductions.PagIBIG.ToString("N2");
                txtTax.Text = deductions.WithholdingTax.ToString("N2");
            }
        }

        private void ValidateInputs(string tag)
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox txt && tag.Equals(txt.Tag))
                {
                    if (string.IsNullOrEmpty(txt.Text))
                    {
                        throw new ArgumentException("One or more fields are empty. Please fill them in.");
                    }
                }
            }
        }
        private void CalculateGrossIncome()
        {
            try
            {
                ValidateInputs("GrossIncomeInput");

                double basicRate = Convert.ToDouble(txtBasicRate.Text);
                double basicNumHrs = Convert.ToDouble(txtBasicNumHours.Text);
                double basicPay = basicRate * basicNumHrs;
                txtBasicIncome.Text = basicPay.ToString("N2");

                double honorRate = Convert.ToDouble(txtHonorRate.Text);
                double honorNumHrs = Convert.ToDouble(txtHonorNumHours.Text);
                double honorPay = honorRate * honorNumHrs;
                txtHonorariumPay.Text = honorPay.ToString("N2");

                double otherRate = Convert.ToDouble(txtOtherRate.Text);
                double otherNumHrs = Convert.ToDouble(txtOtherNumHours.Text);
                double otherPay = otherRate * otherNumHrs;
                txtOtherIncomePay.Text = otherPay.ToString("N2");

                double grossIncome = basicPay + honorPay + otherPay;
                txtGrossIncome.Text = grossIncome.ToString("N2");

            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid Income Input.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }

        private void CalculateNetIncome()
        {
            try
            {
                ValidateInputs("NetIncomeInput");

                decimal totalDeductions = 0;
                foreach (Control c in this.Controls)
                {
                    if (c is TextBox txt && "NetIncomeInput".Equals(txt.Tag))
                    {
                        totalDeductions += Convert.ToDecimal(txt.Text);
                    }
                }

                decimal grossIncome = Convert.ToDecimal(txtGrossIncome.Text);
                decimal netIncome = grossIncome - totalDeductions;
                txtNetIncome.Text = netIncome.ToString("N2");
                txtTotalDeduc.Text = totalDeductions.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class DeductionResult
    {
        public decimal SSS { get; set; }
        public decimal PhilHealth { get; set; }
        public decimal PagIBIG { get; set; }
        public decimal WithholdingTax { get; set; }
    }

    public class PaySlipDTO
    {
        public string Company { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string CutOff { get; set; }
        public string PayPeriod { get; set; }
        public string BasicPayHours { get; set; }
        public string BasicPay { get; set; }
        public string OvertimeHours { get; set; }
        public string OvertimePay { get; set; }
        public string HonorariumHours { get; set; }
        public string HonorariumPay { get; set; }
        public string WithholdingTax { get; set; }
        public string SSSCont {  get; set; }
        public string PhilHealthCont { get; set; }
        public string PagIBIGCont { get; set; }
        public string GrossIncome {  get; set; }
        public string Deductions { get; set; }
        public string NetIncome { get; set; }

    }

    public static class PayrollService
    {
        private static readonly List<(decimal Min, decimal Max, decimal MSC)> SSSTable = new List<(decimal, decimal, decimal)>
        {
            (0, 5249.99m, 5000m),
            (5250, 5749.99m, 5500m),
            (5750, 6249.99m, 6000m),
            (6250, 6749.99m, 6500m),
            (6750, 7249.99m, 7000m),
            (7250, 7749.99m, 7500m),
            (7750, 8249.99m, 8000m),
            (8250, 8749.99m, 8500m),
            (8750, 9249.99m, 9000m),
            (9250, 9749.99m, 9500m),
            (9750, 10249.99m, 10000m),
            (10250, 10749.99m, 10500m),
            (10750, 11249.99m, 11000m),
            (11250, 11749.99m, 11500m),
            (11750, 12249.99m, 12000m),
            (12250, 12749.99m, 12500m),
            (12750, 13249.99m, 13000m),
            (13250, 13749.99m, 13500m),
            (13750, 14249.99m, 14000m),
            (14250, 14749.99m, 14500m),
            (14750, 15249.99m, 15000m),
            (15250, 15749.99m, 15500m),
            (15750, 16249.99m, 16000m),
            (16250, 16749.99m, 16500m),
            (16750, 17249.99m, 17000m),
            (17250, 17749.99m, 17500m),
            (17750, 18249.99m, 18000m),
            (18250, 18749.99m, 18500m),
            (18750, 19249.99m, 19000m),
            (19250, 19749.99m, 19500m),
            (19750, 20249.99m, 20000m),
            (20250, 20749.99m, 20500m),
            (20750, 21249.99m, 21000m),
            (21250, 21749.99m, 21500m),
            (21750, 22249.99m, 22000m),
            (22250, 22749.99m, 22500m),
            (22750, 23249.99m, 23000m),
            (23250, 23749.99m, 23500m),
            (23750, 24249.99m, 24000m),
            (24250, 24749.99m, 24500m),
            (24750, 25249.99m, 25000m),
            (25250, 25749.99m, 25500m),
            (25750, 26249.99m, 26000m),
            (26250, 26749.99m, 26500m),
            (26750, 27249.99m, 27000m),
            (27250, 27749.99m, 27500m),
            (27750, 28249.99m, 28000m),
            (28250, 28749.99m, 28500m),
            (28750, 29249.99m, 29000m),
            (29250, 29749.99m, 29500m),
            (29750, 30249.99m, 30000m),
            (30250, 30749.99m, 30500m),
            (30750, 31249.99m, 31000m),
            (31250, 31749.99m, 31500m),
            (31750, 32249.99m, 32000m),
            (32250, 32749.99m, 32500m),
            (32750, 33249.99m, 33000m),
            (33250, 33749.99m, 33500m),
            (33750, 34249.99m, 34000m),
            (34250, 34749.99m, 34500m),
            (34750, decimal.MaxValue, 35000m)
        };

        public static DeductionResult CalculateAll(decimal grossIncome)
        {
            var result = new DeductionResult();

            //SSS
            var sssBracket = SSSTable.FirstOrDefault(x => grossIncome >= x.Min && grossIncome <= x.Max);
            decimal msc = sssBracket.MSC > 0 ? sssBracket.MSC : 35000;
            result.SSS = msc * 0.05m;

            //PhilHealth
            decimal phBase;
            if (grossIncome <= 10000m) phBase = 10000m;
            else if (grossIncome >= 100000m) phBase = 100000m;
            else phBase = grossIncome;

            result.PhilHealth = phBase * 0.025m;

            //Pagibig
            result.PagIBIG = 200m;

            //Income Tax
            decimal taxableIncome = grossIncome - (result.SSS + result.PhilHealth + result.PagIBIG);
            result.WithholdingTax = CalculateAnnualTax(taxableIncome);

            return result;
        }

        private static decimal CalculateAnnualTax(decimal taxable)
        {
            if (taxable <= 20833) return 0;
            if (taxable <= 33333) return (taxable - 20833) * 0.15m;
            if (taxable <= 66667) return 1875 + (taxable - 33333) * 0.20m;
            if (taxable <= 166667) return 8541.80m + (taxable - 66667) * 0.25m;
            if (taxable <= 666667) return 33541.80m + (taxable - 166667) * 0.30m;
            return 183541.80m + (taxable - 666667) * 0.35m;
        }
    }
}
