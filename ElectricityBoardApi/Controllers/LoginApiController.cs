using BusinessLayer;
using Common;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ElectricityBoardApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginApiController : ApiController
    {
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
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.ValidateLogin(userName, password);
            }
        }

        /// <summary>
        /// Validate Consumer No
        /// </summary>
        /// <param name="consumerNo"></param>
        /// <returns></returns>
        [HttpPost]
        public bool ValidateConsumerNo([FromUri] int consumerNo, [FromUri] int loginID)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.ValidateConsumerNo(consumerNo, loginID);
            }
        }

        [HttpGet]
        public List<tblConsumer> LoadConsumer([FromUri]int loginID = 0)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.LoadConsumer(loginID);
            }
        }

        public List<vPaymentDetail> LoadPaymentDetails([FromUri]int consumerID, [FromUri] int loginID)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.LoadPaymentDetails(consumerID, loginID);
            }
        }

        [HttpPost]
        public List<Profile> LoadProfileDetails([FromUri] int loginID)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.LoadProfileDetails(loginID);
            }
        }

        [HttpPost]
        public int SaveProfile([FromBody] Profile obj)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.SaveProfile(obj);
            }
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
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.QuickPay(consumerNo);
            }
        }

        /// <summary>
        /// Get Bill Amount
        /// </summary>
        /// <param name="consumerNo"></param>
        /// <returns></returns>
        [HttpPost]
        public int GetBillAmount([FromUri] int consumerNo)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.GetBillAmount(consumerNo);
            }
        }

        #endregion Consumer Functionality Region        

        #region Admin Functionality Region

        [HttpPost]

        public List<tblBillHistory> SearchPreviousBillStatements([FromUri]string startDate, [FromUri]string endDate)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.SearchPreviousBillStatements(startDate, endDate);
            }
        }

        /// <summary>
        /// Load Consumption Chart
        /// </summary>
        /// <param name="consumerID"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Chart> LoadConsumptionChart(int consumerID)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.LoadConsumptionChart(consumerID);
            }
        }

        /// <summary>
        /// Validate Profile
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public string ValidateProfile([FromBody] Profile obj)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.ValidateProfile(obj);
            }
        }

        /// <summary>
        /// Add Bill for consumers
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool AddBill([FromBody]Bill obj)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.AddBill(obj);
            }
        }

        /// <summary>
        /// Search Consumer Bill Statements
        /// </summary>
        /// <param name="consumerID"></param>
        /// <returns></returns>
        [HttpPost]
        public List<tblBillHistory> SearchConsumerBillStatements([FromUri]int consumerID)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.SearchConsumerBillStatements(consumerID);
            }
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
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.DeleteConsumerBill(consumerID, ID);
            }
        }

        /// <summary>
        /// Get Last Inserted Login ID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public int GetLastInsertedLoginID()
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.GetLastInsertedLoginID();
            }
        }

        /// <summary>
        /// Convert Base 64 String to Byte []
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public byte[] ConvertBase64StringToByteArray(string source)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.ConvertBase64StringToByteArray(source);
            }
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
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.DownloadImportProfileTemplate();
            }
        }

        /// <summary>
        /// Create Empty Data Table
        /// </summary>
        /// <returns></returns>
        public DataTable CreateEmptyDataTable()
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.CreateEmptyDataTable();
            }
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
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.SendConsumerSupportEmail(from, message);
            }
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
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.SaveCustomerComplaint(consumer_ID, email, concern);
            }
        }

        /// <summary>
        /// Get Sesion Login Name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetSessionLoginName()
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.GetSessionLoginName();
            }
        }

        /// <summary>
        /// Get Session Email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetSessionEmail()
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.GetSessionEmail();
            }
        }

        /// <summary>
        /// Load Customer Support Issues
        /// </summary>
        /// <param name="isResolved"></param>
        /// <returns></returns>
        [HttpGet]
        public List<vCustomerSupport> LoadCustomerSupportIssues([FromUri]int isResolved = 0)
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.LoadCustomerSupportIssues(isResolved);
            }
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
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.ResolveCustomerSupportIssue(ID, resolvedMessage, consumerName, consumerEmail);
            }
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
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.SaveFeedBack(model);
            }
        }

        /// <summary>
        /// Analyse Consumer Feedback
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<SentimentAnalysis> AnalyseConsumerFeedback()
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.AnalyseConsumerFeedback();
            }
        }

        /// <summary>
        /// Get Consumer Feedback
        /// </summary>
        /// <returns></returns>
        public string GetConsumerFeedBack()
        {
            using (ApiBusiness apiBusiness = new ApiBusiness())
            {
                return apiBusiness.GetConsumerFeedBack();
            }
        }

        #endregion Feedback Region

        #endregion Member Functions
    }
}
