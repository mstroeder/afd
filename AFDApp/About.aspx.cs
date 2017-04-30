using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AFDApp
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string requestId = Request.QueryString["RequestId"];

            if (!string.IsNullOrEmpty(requestId))
            {
                // if we found requestid means thread is processed
                if (ThreadResult.Contains(requestId))
                {
                    // get value
                    string result = (string)ThreadResult.Get(requestId);

                    // show message
                    lblResult.Text = result;

                    lblProcessing.Visible = false;
                    lblResult.Visible = true;
                    // Remove value from HashTable
                    ThreadResult.Remove(requestId);

                }
                else
                {
                    // keep refreshing progress window
                    Response.AddHeader("refresh", "3");
                }
            }
        }
    }
}