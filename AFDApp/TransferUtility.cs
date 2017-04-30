using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace AFDApp
{
    public static class TransferUtility
    {
        public static List<CustomerData> LoadStreamForProcessing(Stream stream, string fileName, string yourName)
        {
            string[] items;
            CustomerData customerData;
            var itemsToProcess = new List<CustomerData>();
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                int value;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    customerData = new CustomerData();
                    items = line.Split(new char[] { ',' });
                    if (items.Length > 0)
                    {
                        customerData.customer = items[0];
                    }
                    if (items.Length > 1)
                    {
                        if(int.TryParse(items[1], out value))
                        {
                            customerData.value = value;
                        }
                        else
                        {
                            customerData.uploaderror = string.Format("Invalid value {0}", items[1]);
                        }
                    }
                    customerData.id = Guid.NewGuid();
                    customerData.property = yourName;
                    customerData.action = "order created";
                    customerData.file = fileName;
                    customerData.uploadstatus = 1;  // to be processed
                    itemsToProcess.Add(customerData);
                }
            }
            return itemsToProcess;
        }
        public static void SendDataToEvil(List<CustomerData> itemsToProcess, Guid requestId, Action<Dictionary<CustomerData, EvilReturn>, Guid> finishedAction)
        {
            var processingResults = new Dictionary<CustomerData, EvilReturn>();
            foreach (var customerData in itemsToProcess)
            {
                SendDataToEvil(processingResults, customerData, itemsToProcess.Count, requestId, finishedAction);
            }
        }
        public async static void SendDataToEvil(Dictionary<CustomerData, EvilReturn> results, CustomerData customerData, int totalBeingProcessed, Guid requestId, Action<Dictionary<CustomerData, EvilReturn>, Guid> finishedAction)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://evilapi-env.ap-southeast-2.elasticbeanstalk.com/upload");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = new JavaScriptSerializer().Serialize(new
                {
                    property = customerData.property,
                    customer = customerData.customer,
                    action = customerData.action,
                    value = customerData.value,
                    file = customerData.file
                });
                streamWriter.Write(json);
            }

            EvilReturn result;
            try
            {
                var httpResponse = await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var resultText = streamReader.ReadToEnd();
                    result = (new JavaScriptSerializer()).Deserialize<EvilReturn>(resultText);
                }
            }
            catch (Exception ex)
            {
                result = new EvilReturn() { added = "false", errors = new string[] { ex.Message } };
            }
            var checkResult = await CheckData(customerData, result.hash);
            customerData.checkResult = checkResult.Status;
            customerData.checkerror = checkResult.Message;
            results.Add(customerData, result);
            if (results.Count == totalBeingProcessed)
            {
                if (finishedAction != null)
                {
                    finishedAction(results, requestId);
                }
            }
        }
        public class CheckResult
        {
            public byte Status;
            public string Message;
        }
        public async static Task<CheckResult> CheckData(CustomerData customerData, string hash)
        {
            if(string.IsNullOrWhiteSpace(hash))
            {
                return new CheckResult() { Status = 2, Message = "Empty hash" };
            }

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://evilapi-env.ap-southeast-2.elasticbeanstalk.com/check?hash=" + hash);
                httpWebRequest.Method = "GET";

                var serializer = (new JavaScriptSerializer());
                var httpResponse = await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var resultText = streamReader.ReadToEnd();
                    if(resultText.IndexOf("\"errors\":", StringComparison.CurrentCultureIgnoreCase) == -1)
                    {
                        var goodResult = serializer.Deserialize<CustomerData>(resultText);
                        if(customerData.property == goodResult.property &&
                            customerData.customer == goodResult.customer &&
                            customerData.action == goodResult.action &&
                            customerData.file == goodResult.file &&
                            customerData.value == goodResult.value)
                        {
                            return new CheckResult() { Status = 1 };
                            // has been run and matched
                        }
                        else
                        {
                            return new CheckResult() { Status = 3, Message = "Check did not match" };
                            //  has been run but did not match
                        }
                    }
                    else
                    {
                        var evilResult = serializer.Deserialize<EvilReturn>(resultText);
                        return new CheckResult() { Status = 4, Message = evilResult.errors[0] };
                        //  Invalid check
                    }
                }
            }
            catch (Exception ex)
            {
                //results.Add(customerData, new EvilReturn() { added = "false", errors = new string[] { ex.Message } });
                //  Exception when running the check.
                return new CheckResult() { Status = 5, Message = ex.Message };
            }
        }
        public static void SaveItemsToDb(List<CustomerData> items)
        {
            var dbContext = new AFDDataContext();
            foreach (var item in items)
            {
                dbContext.Customers.Add(item);
            }
            dbContext.SaveChanges();
        }
        public static void TransferFinished(Dictionary<CustomerData, EvilReturn> results, Guid requestId)
        {
            var dbContext = new AFDDataContext();
            var failedItems = new Dictionary<CustomerData, EvilReturn>();
            var builder = new StringBuilder();
            int goodCount = 0;
            foreach (var result in results)
            {
                var parms = new SqlParameter[] {
                        new SqlParameter("@status", System.Data.SqlDbType.TinyInt),
                        new SqlParameter("@hash", System.Data.SqlDbType.NVarChar),
                        new SqlParameter("@checkResult", System.Data.SqlDbType.TinyInt),
                        new SqlParameter("@checkerror", System.Data.SqlDbType.NVarChar),
                        new SqlParameter("@id", System.Data.SqlDbType.UniqueIdentifier)
                };
                if (result.Value.added.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    goodCount++;
                    //result.Key.hash = result.Value.hash;
                    //result.Key.uploadstatus = 0;
                    parms[0].Value = 3;
                    parms[1].Value = result.Value.hash;
                    parms[2].Value = result.Key.checkResult;
                    parms[3].SqlValue = ((result.Key.checkerror == null) ? DBNull.Value : (object)result.Key.checkerror);
                    parms[4].Value = result.Key.id;
                    dbContext.Database.ExecuteSqlCommand(
                        "Update CustomerDatas " + 
                        "Set uploadstatus = @status, hash = @hash, checkResult = @checkResult, checkerror = @checkerror " +
                        "Where id = @id",
                        parms.ToArray());
                }
                else
                {
                    //result.Key.hash = null;
                    //result.Key.uploadstatus = 2;    //  failed
                    parms[0].Value = 2;
                    parms[1].Value = "";
                    parms[2].Value = result.Key.checkResult;
                    parms[3].SqlValue = result.Key.checkerror;
                    parms[4].Value = result.Key.id;
                    dbContext.Database.ExecuteSqlCommand(
                        "Update CustomerDatas " +
                        "Set uploadstatus = @status, hash = @hash, checkResult = @checkResult, checkerror = @checkerror " +
                        "Where id = @id",
                        parms);
                    failedItems.Add(result.Key, result.Value);
                }
                //dbContext.Customers.Add(result.Key);
            }
            //dbContext.SaveChanges();
            builder.Append(string.Format("{0} orders successfully updated<br/>", goodCount));
            if(failedItems.Count != 0)
            {
                foreach (var item in failedItems)
                {
                    builder.Append(string.Format("Customer {0} was not updated. Error {1}<br/>", item.Key.customer, ((item.Value.errors.Length != 0) ? item.Value.errors[0] : "")));
                }
            }
            ThreadResult.Add(requestId.ToString(), builder.ToString()); // you  can add your result in second parameter.
        }
    }
    public class EvilReturn
    {
        public string added;
        public string hash;
        public string[] errors;
        public override string ToString()
        {
            return "added: " + this.added + ((errors.Length == 0) ? "" : " error: " + errors[0]);
        }
    }
}