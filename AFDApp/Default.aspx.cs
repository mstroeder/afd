using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AFDApp
{
    public partial class _Default : Page
    {
        protected Guid requestId;
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (FileUploadControl1.HasFile)
            {
                try
                {
                    string filename = Path.GetFileName(FileUploadControl1.FileName);
                    var items = TransferUtility.LoadStreamForProcessing(FileUploadControl1.FileContent, FileUploadControl1.FileName, "Michael Stroeder");
                    TransferUtility.SaveItemsToDb(items);

                    requestId = Guid.NewGuid();

                    ParameterizedThreadStart parameterizedThreadStart = new ParameterizedThreadStart(LongRunningTask);
                    Thread thread = new Thread(parameterizedThreadStart);
                    thread.Start(new TaskData() { RequestID = requestId, Items = items });

                    Response.Redirect("About.aspx?RequestId=" + requestId);
                    //StatusLabel.Text = "Upload status: File uploaded!";
                }
                catch (Exception ex)
                {
                    StatusLabel.Text = "Upload status: The file could not be uploaded. The following error occured: " + ex.Message;
                }
            }
        }
        private class TaskData
        {
            public Guid RequestID;
            public List<CustomerData> Items;
        }
        private void LongRunningTask(object data)
        {
            var taskData = (TaskData)data;
            TransferUtility.SendDataToEvil(taskData.Items, taskData.RequestID, TransferUtility.TransferFinished);

            // Add ThreadResult -- when this
            // line executes it  means task has been
            // completed
            //ThreadResult.Add(requestId.ToString(), "Item Processed Successfully."); // you  can add your result in second parameter.
        }
    }
}