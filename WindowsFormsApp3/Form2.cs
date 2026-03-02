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
    public partial class Form2 : Form
    {
        public Form2(PaySlipDTO data)
        {
            InitializeComponent();
            lblCompanyNameResult.Text = data.Company;
            lblEmployeeCodeResult.Text = data.EmployeeCode;
            lblEmployeeNameResult.Text = data.EmployeeName;
            lblDepartmentResult.Text = data.Department;
            lblCutOffResult.Text = data.CutOff;
            lblPayPeriodResult.Text = data.PayPeriod;
            lblBasicHours.Text = data.BasicPayHours;
            lblBasicTaxable.Text = data.BasicPay;
            lblOvertimeHours.Text = data.OvertimeHours;
            lblOvertimeTaxable.Text = data.OvertimePay;
            lblHonorariumHours.Text = data.HonorariumHours;
            lblHonorariumTaxable.Text = data.HonorariumPay;
            txtEarnings.Text = data.GrossIncome;
            lblWithholdAmount.Text = data.WithholdingTax;
            lblSSSAmount.Text = data.SSSCont;
            lblPhilHealthAmount.Text = data.PhilHealthCont;
            lblHDMFAmount.Text = data.PagIBIGCont;
            lblWISPAmount.Text = "750";
            txtDeductions.Text = data.Deductions;
            lblGrossEarningsAmount.Text = data.GrossIncome;
            lblDeductionsAmount.Text = data.Deductions;
            lblNetPayAmount.Text = data.NetIncome;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
