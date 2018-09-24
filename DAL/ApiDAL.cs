using Common;
using Microsoft.Win32.SafeHandles;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace DAL
{
    public class ApiDal : IApiDal, IDisposable
    {
        #region Member Variables

        readonly ElectricityBoardDatabaseEntities db = new ElectricityBoardDatabaseEntities();
        bool disposed = false;
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        readonly string sentimentAnalysisUri = "https://brazilsouth.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";

        #endregion Member Variables

        #region Member Functions

        #region General Methods

        [HttpGet]
        public string Get()
        {
            return "Web Api is running.";
        }

        /// <summary>
        /// Validate Login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public List<tblLogin> ValidateLogin([FromUri]string userName, [FromUri]string password)
        {
            if (userName != string.Empty && password != string.Empty)
            {
                var list = db.tblLogins.Where(x => x.UserName.ToLower() == userName.Trim().ToLower() && x.Password == password).Distinct().ToList();
                if (list != null && list.Count > 0)
                {
                    Session.Login_ID = list[0].ID;
                    Session.LoginName = list[0].UserName;
                    Session.LoginEmail = list[0].EmailId;
                    return list;
                }
            }
            return new List<tblLogin>();
        }

        /// <summary>
        /// Validate Consumer No
        /// </summary>
        /// <param name="consumerNo"></param>
        /// <returns></returns>
        [HttpPost]
        public bool ValidateConsumerNo([FromUri] int consumerNo, [FromUri] int loginID)
        {
            if (consumerNo > 0)
            {
                var consumerObj = db.tblConsumers.Where(x => x.ConsumerNo == consumerNo && x.Login_ID == loginID).ToList();

                if (consumerObj != null && consumerObj.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        [HttpGet]
        public List<tblConsumer> LoadConsumer([FromUri]int loginID = 0)
        {
            List<tblConsumer> list = null;
            if (loginID > 0)
            {
                list = db.tblConsumers.Where(x => x.Login_ID == loginID).ToList();
            }
            else
            {
                list = db.tblConsumers.ToList();
            }
            return list;
        }

        public List<vPaymentDetail> LoadPaymentDetails([FromUri]int consumerID, [FromUri] int loginID)
        {
            var list = new List<vPaymentDetail>();

            if (consumerID > 0 && loginID > 0)
            {
                list = db.vPaymentDetails.Where(x => x.Consumer_ID == consumerID && x.Login_ID == loginID).ToList();
            }

            return list;
        }

        [HttpPost]
        public List<Profile> LoadProfileDetails([FromUri] int loginID)
        {
            var list = new List<tblLogin>();

            if (loginID > 0)
            {
                list = db.tblLogins.Where(x => x.ID == loginID).ToList();
            }

            return list.Select(s => new Profile
            {
                ID = s.ID,
                UserName = s.UserName,
                Address = s.Address,
                Email = s.EmailId,
                City = s.City,
                State = s.State,
                ZipCode = s.ZipCode,
                ProfilePicture = Convert.ToBase64String(s.ProfilePicture, 0, s.ProfilePicture.Length)
            }).ToList();
        }

        [HttpPost]
        public int SaveProfile([FromBody] Profile obj)
        {
            if (obj != null)
            {
                if (obj.ID > 0)
                {
                    var loginObj = db.tblLogins.Where(x => x.ID == obj.ID).ToList();
                    if (loginObj != null && loginObj.Count > 0)
                    {
                        loginObj[0].Password = obj.NewPassword;
                        loginObj[0].EmailId = !string.IsNullOrEmpty(obj.Email) ? obj.Email : string.Empty;
                        loginObj[0].Address = !string.IsNullOrEmpty(obj.Address) ? obj.Address : string.Empty;
                        loginObj[0].City = !string.IsNullOrEmpty(obj.City) ? obj.City : string.Empty;
                        loginObj[0].ZipCode = obj.ZipCode;
                        loginObj[0].ProfilePicture = ConvertBase64StringToByteArray(obj.ProfilePicture);
                        loginObj[0].State = obj.State;

                        db.Entry(loginObj[0]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    tblLogin loginObj = new tblLogin()
                    {
                        UserName = obj.UserName,
                        Password = obj.NewPassword,
                        EmailId = !string.IsNullOrEmpty(obj.Email) ? obj.Email : string.Empty,
                        Address = !string.IsNullOrEmpty(obj.Address) ? obj.Address : string.Empty,
                        City = !string.IsNullOrEmpty(obj.City) ? obj.City : string.Empty,
                        State = obj.State,
                        ZipCode = obj.ZipCode,
                        IsAdmin = obj.IsAdmin,
                        ProfilePicture = ConvertBase64StringToByteArray(obj.ProfilePicture)
                    };
                    db.tblLogins.Add(loginObj);
                    int loginID = db.SaveChanges();
                    if (loginID > 0)
                    {
                        tblConsumer consumerObj = new tblConsumer()
                        {
                            ConsumerNo = obj.ConsumerNo,
                            ConsumerName = obj.UserName,
                            RegionCode = obj.RegionCode,
                            Login_ID = loginObj.ID
                        };
                        db.tblConsumers.Add(consumerObj);
                        db.SaveChanges();
                    }

                    return loginObj.ID;
                }
            }

            return obj.ID;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                db.Dispose();
            }

            disposed = true;
        }

        ~ApiDal()
        {
            Dispose(false);
        }

        

        #endregion General Methods

        #region Consumer Functionality Region

        /// <summary>
        /// Quick Pay
        /// </summary>
        /// <param name="consumerNo"></param>
        /// <returns></returns>
        [HttpPost]
        public bool QuickPay([FromUri] int consumerNo)
        {
            if (consumerNo > 0)
            {
                var consumerObj = db.tblConsumers.Where(x => x.ConsumerNo == consumerNo).ToList();

                if (consumerObj != null && consumerObj.Count > 0)
                {
                    int consumerID = consumerObj[0].ID;
                    var billObj = db.tblBillHistories.Where(x => x.Consumer_ID == consumerID && !x.Paid).ToList();

                    if (billObj != null && billObj.Count > 0)
                    {
                        billObj[0].Paid = true;
                        billObj[0].BillPaidDate = DateTime.Now;

                        db.Entry(billObj[0]).State = EntityState.Modified;
                        db.SaveChanges();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Get Bill Amount
        /// </summary>
        /// <param name="consumerNo"></param>
        /// <returns></returns>
        [HttpPost]
        public int GetBillAmount([FromUri] int consumerNo)
        {
            if (consumerNo > 0)
            {
                var consumerObj = db.tblConsumers.Where(x => x.ConsumerNo == consumerNo).ToList();

                if (consumerObj != null && consumerObj.Count > 0)
                {
                    int consumerID = consumerObj[0].ID;
                    var billObj = db.tblBillHistories.Where(x => x.Consumer_ID == consumerID && !x.Paid).ToList();

                    if (billObj != null && billObj.Count > 0)
                    {
                        return billObj[0].BillAmount;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            return 0;
        }

        #endregion Consumer Functionality Region        

        #region Admin Functionality Region

        [HttpPost]

        public List<tblBillHistory> SearchPreviousBillStatements([FromUri]string startDate, [FromUri]string endDate)
        {
            var sDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var eDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<tblBillHistory> list = null;

            string keyname = "BillHistory_" + sDate + eDate + Session.Login_ID;
            if (RedisCacheHelper.keyExistsInCache(keyname))
                list = RedisCacheHelper.GetCacheData<tblBillHistory>(keyname);
            else
            {
                list = db.tblBillHistories.Where(x => x.BillDate >= sDate && x.BillDate <= eDate).ToList();
                RedisCacheHelper.addItemCache<tblBillHistory>(keyname, list);
            }

            return list != null && list.Count > 0 ? list : new List<tblBillHistory>();
        }

        /// <summary>
        /// Load Consumption Chart
        /// </summary>
        /// <param name="consumerID"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Chart> LoadConsumptionChart(int consumerID)
        {
            var list = new List<Chart>();

            if (consumerID > 0)
            {
                var billHistoryList = db.vUnitsConsumptions.Where(x => x.Consumer_ID == consumerID).ToList();

                if (billHistoryList != null && billHistoryList.Count > 0)
                {
                    foreach (var item in billHistoryList)
                    {
                        var obj = new Chart()
                        {
                            y = item.UnitsConsumed ?? 0,
                            label = item.Year
                        };
                        list.Add(obj);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Validate Profile
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public string ValidateProfile([FromBody] Profile obj)
        {
            var errorMessage = string.Empty;
            if (obj != null)
            {
                if (obj.UserName != string.Empty)
                {
                    var loginList = db.tblLogins.Where(x => x.UserName == obj.UserName.Trim()).ToList();
                    if (loginList != null && loginList.Count > 0)
                    {
                        errorMessage = "Login already exists in the database.";
                    }
                }

                if (obj.ConsumerNo > 0)
                {
                    var consumerList = db.tblConsumers.Where(x => x.ConsumerNo == obj.ConsumerNo).ToList();
                    if (consumerList != null && consumerList.Count > 0)
                    {
                        errorMessage = "Consumer No already exists in the database.";
                    }
                }
            }

            return errorMessage;
        }

        /// <summary>
        /// Add Bill for consumers
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool AddBill([FromBody]Bill obj)
        {
            tblBillHistory billHistoryObj = new tblBillHistory();
            if (obj != null)
            {
                var billDate = DateTime.ParseExact(obj.BillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                billHistoryObj.BillDate = billDate;
                billHistoryObj.UnitsConsumed = obj.Units;
                billHistoryObj.BillAmount = obj.Amount;
                billHistoryObj.Consumer_ID = obj.ConsumerID;
                billHistoryObj.BillPaidDate = Convert.ToDateTime("1900/01/01");
                billHistoryObj.Paid = false;

                db.tblBillHistories.Add(billHistoryObj);

                db.SaveChanges();

                RedisCacheHelper.ClearAllCacheData("BillHistory_");
            }

            return billHistoryObj.ID > 0;
        }

        /// <summary>
        /// Search Consumer Bill Statements
        /// </summary>
        /// <param name="consumerID"></param>
        /// <returns></returns>
        [HttpPost]
        public List<tblBillHistory> SearchConsumerBillStatements([FromUri]int consumerID)
        {
            var list = db.tblBillHistories.Where(x => x.Consumer_ID == consumerID).ToList();

            return list != null && list.Count > 0 ? list : new List<tblBillHistory>();
        }

        /// <summary>
        /// Delete Consumer Bill
        /// </summary>
        /// <param name="consumerID"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        public bool DeleteConsumerBill([FromUri]int consumerID, [FromUri]int ID)
        {
            var result = db.Database.ExecuteSqlCommand("DELETE FROM tblBillHistory WHERE Consumer_ID = {0} AND ID = {1}", new object[] { consumerID, ID });

            return result > 0;
        }

        /// <summary>
        /// Get Last Inserted Login ID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public int GetLastInsertedLoginID()
        {
            var list = db.tblLogins.OrderByDescending(o => o.ID).ToList();

            return list != null && list.Count > 0 ? list[0].ID : 0;
        }

        /// <summary>
        /// Convert Base 64 String to Byte []
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public byte[] ConvertBase64StringToByteArray(string source)
        {
            byte[] data = null;
            try
            {
                string base64 = source.Substring(source.IndexOf(',') + 1);
                base64 = base64.Trim('\0');
                data = Convert.FromBase64String(base64);
            }
            catch
            {
                data = new byte[] { };
            }
            return data;
        }

        #endregion Admin Functionality Region

        #region Bulk Import Region

        /// <summary>
        /// Download Import Profile Template
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public byte[] DownloadImportProfileTemplate()
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("SearchReport");
                ws.Protection.IsProtected = false;
                DataTable dt = CreateEmptyDataTable();
                ws.Cells["A1"].LoadFromDataTable(dt, true);
                //prepare the range for the column headers
                string cellRange = "A1:" + Convert.ToChar('A' + dt.Columns.Count - 1) + 1;
                //Format the header for columns
                using (ExcelRange rng = ws.Cells[cellRange])
                {
                    rng.Style.WrapText = false;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                
                //Format the rows
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    ExcelColumn col = ws.Column(i);
                    col.Style.Numberformat.Format = "@";
                }
                Byte[] fileBytes = pck.GetAsByteArray();
                return fileBytes;
            }
        }

        /// <summary>
        /// Create Empty Data Table
        /// </summary>
        /// <returns></returns>
        public DataTable CreateEmptyDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "UserName", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "Password", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "ConsumerNo", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "RegionCode", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "Address", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "Email", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "City", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "State", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "ZipCode", ReadOnly = false });
            dt.Columns.Add(new DataColumn() { ColumnName = "IsAdmin", ReadOnly = false });

            return dt;
        }

        #endregion Bulk Import Region

        #region Consumer Support Region

        /// <summary>
        /// Send Consumer Support Email
        /// </summary>
        /// <param name="from"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public string SendConsumerSupportEmail([FromUri]string from, [FromUri]string message)
        {
            var result = string.Empty;
            var consumerObj = LoadConsumer(Session.Login_ID)[0];

            try
            {
                if (consumerObj != null)
                {
                    var complaintID = SaveCustomerComplaint(consumerObj.ID, from, message);
                    if (complaintID > 0)
                    {
                        EmailParameters param = new EmailParameters()
                        {
                            ID = complaintID,
                            fromEmailAddress = from,
                            toEmailAddress = "mr.p.kailash@gmail.com",
                            consumerName = consumerObj.ConsumerName,
                            resolvedMessage = message
                        };
                        SendEmail.TriggerMail(param);

                        result = string.Empty;
                    }
                }
            }
            catch
            {
                result = "Error";
            }


            return result;
        }

        /// <summary>
        /// Save Customer Complaint
        /// </summary>
        /// <param name="consumer_ID"></param>
        /// <param name="email"></param>
        /// <param name="concern"></param>
        /// <returns></returns>
        public int SaveCustomerComplaint(int consumer_ID, string email, string concern)
        {
            try
            {
                tblCustomerSupport customerSupportObj = new tblCustomerSupport();
                if (consumer_ID > 0 && email != string.Empty && concern != string.Empty)
                {
                    customerSupportObj.Consumer_ID = consumer_ID;
                    customerSupportObj.Email = email;
                    customerSupportObj.Concern = concern;
                    customerSupportObj.ResolvedMessage = string.Empty;
                    customerSupportObj.IsResolved = false;
                    customerSupportObj.RaisedDate = DateTime.Now;
                    customerSupportObj.ResolvedDate = new DateTime(1900, 01, 01);
                    customerSupportObj.Severity = 0;

                    db.tblCustomerSupports.Add(customerSupportObj);

                    db.SaveChanges();
                }
                return customerSupportObj.ID > 0 ? customerSupportObj.ID : 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Get Sesion Login Name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetSessionLoginName()
        {
            return Session.LoginName;
        }

        /// <summary>
        /// Get Session Email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetSessionEmail()
        {
            return Session.LoginEmail;
        }

        /// <summary>
        /// Load Customer Support Issues
        /// </summary>
        /// <param name="isResolved"></param>
        /// <returns></returns>
        [HttpGet]
        public List<vCustomerSupport> LoadCustomerSupportIssues([FromUri]int isResolved = 0)
        {
            return db.vCustomerSupports.Where(x => x.IsResolved == isResolved > 0).ToList();
        }

        /// <summary>
        /// Resolve Customer Support Issue
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="resolvedMessage"></param>
        /// <param name="consumerName"></param>
        /// <param name="consumerEmail"></param>
        /// <returns></returns>
        [HttpPost]
        public bool ResolveCustomerSupportIssue([FromUri] int ID, [FromUri] string resolvedMessage, [FromUri] string consumerName, [FromUri] string consumerEmail)
        {
            try
            {
                if (ID > 0 && resolvedMessage != string.Empty)
                {
                    tblCustomerSupport customerSupportObj = db.tblCustomerSupports.Where(x => x.ID == ID).FirstOrDefault();
                    {
                        customerSupportObj.ResolvedMessage = resolvedMessage;
                        customerSupportObj.IsResolved = true;
                        customerSupportObj.ResolvedDate = DateTime.Now;
                        customerSupportObj.Severity = 0;

                        db.Entry(customerSupportObj).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    EmailParameters param = new EmailParameters()
                    {
                        ID = ID,
                        fromEmailAddress = "mr.p.kailash@gmail.com",
                        toEmailAddress = consumerEmail,
                        consumerName = consumerName,
                        resolvedMessage = resolvedMessage
                    };
                    SendEmail.TriggerMail(param);

                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        #endregion Consumer Support Region

        #region FeedBack Region

        /// <summary>
        /// Save Feedback
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public int SaveFeedBack(FeedBackModel model)
        {
            tblFeedback feedBackObj = new tblFeedback();
            if (model != null)
            {
                feedBackObj.Headline = model.FeedBackHeadline;
                feedBackObj.Feedback = model.FeedBack;
                feedBackObj.ConsumerProfilePicture = ConvertBase64StringToByteArray(model.ProfilePicture);
                feedBackObj.Consumer_ID = db.tblConsumers.Where(x => x.Login_ID == Session.Login_ID).FirstOrDefault().ID;
                feedBackObj.ConsumerName = model.UserName;

                db.tblFeedbacks.Add(feedBackObj);
                db.SaveChanges();
            }
            return feedBackObj.ID > 0 ? feedBackObj.ID : 0;
        }

        /// <summary>
        /// Analyse Consumer Feedback
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<SentimentAnalysis> AnalyseConsumerFeedback()
        {
            var list = new List<SentimentAnalysis>();
            var client = new RestClient(sentimentAnalysisUri);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "d6b94fb3-8520-48cf-bff8-d983397eb7d9");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Ocp-Apim-Subscription-Key", "72c321385eac4027b570a73b39c90361");
            var param = GetConsumerFeedBack();
            if (param != string.Empty)
            {
                var res = "{\r\n        \"documents\":  " + param + "  \r\n}";
                request.AddParameter("undefined", res, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var jsonSerialiser = new JavaScriptSerializer();
                var result = jsonSerialiser.Deserialize<SentimentResponse>(response.Content).documents.ToList();

                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        var feedBackObj = db.tblFeedbacks.Where(x => x.ID == item.id).FirstOrDefault();
                        if (feedBackObj != null)
                        {
                            var consumerObj = db.tblConsumers.Where(x => x.ID == feedBackObj.Consumer_ID).FirstOrDefault();
                            if (consumerObj != null)
                            {
                                SentimentAnalysis analysisObj = new SentimentAnalysis()
                                {
                                    ID = item.id,
                                    ConsumerName = consumerObj.ConsumerName,
                                    FeedBack = feedBackObj.Feedback,
                                    Sentiment = item.score,
                                    ConsumerProfilePicture = Convert.ToBase64String(feedBackObj.ConsumerProfilePicture, 0, feedBackObj.ConsumerProfilePicture.Length)
                                };
                                list.Add(analysisObj);
                            }
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Get Consumer Feedback
        /// </summary>
        /// <returns></returns>
        public string GetConsumerFeedBack()
        {
            var list = db.tblFeedbacks.ToList();
            var tempList = list.Select(s => new FeedBack
            {
                id = s.ID,
                languate = "en",
                text = s.Feedback
            }).ToList();

            var jsonSerialiser = new JavaScriptSerializer();
            return jsonSerialiser.Serialize(tempList);
        }

        #endregion Feedback Region

        #endregion Member Functions
    }
}
