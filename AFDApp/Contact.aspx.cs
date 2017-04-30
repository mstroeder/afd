using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AFDApp
{
    public partial class Contact : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindFileName();
            }
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            this.BindGridView(this.cmbFileName.SelectedValue, this.cmbErrorsFilter.SelectedValue);
        }
        private void bindFileName()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AFDConnection"].ConnectionString);

            SqlCommand cmd = new SqlCommand("Select Distinct [file] From CustomerDatas", con);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbFileName.DataTextField = "file";
            cmbFileName.DataValueField = "file";

            cmbFileName.DataSource = dt;
            cmbFileName.DataBind();
            cmbFileName.Items.Insert(0, new ListItem("All", "All"));
        }
        protected void GetDataButton_Click(object sender, EventArgs e)
        {
            this.BindGridView(this.cmbFileName.SelectedValue, this.cmbErrorsFilter.SelectedValue);
        }
        private void BindGridView(string fileName, string filterOption)
        {
            var dbContext = new AFDDataContext();
            var where = new StringBuilder();
            var parms = new List<SqlParameter>();
            if(!fileName.Equals("All", StringComparison.CurrentCultureIgnoreCase))
            {
                where.Append("[file] = @file");
                parms.Add(new SqlParameter("@file", fileName));
            }
            if(filterOption.Equals("errorsonly", StringComparison.CurrentCultureIgnoreCase))
            {
                if(where.Length != 0)
                {
                    where.Append(" And ");
                }
                where.Append("checkResult > 1");
            }
            if(where.Length != 0)
            {
                where.Insert(0, "Where ");
            }
            this.GridView1.DataSource = dbContext.Customers.SqlQuery("Select * From CustomerDatas " + where.ToString(), parms.ToArray()).ToList();
            this.GridView1.DataBind();
        }
    }
}