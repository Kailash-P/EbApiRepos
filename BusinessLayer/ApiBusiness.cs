using Common;
using DAL;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;

namespace BusinessLayer
{
    public class ApiBusiness : IDisposable
    {
        #region Member Variables

        bool disposed = false;
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        #endregion Member Variables

        #region Member Functions

        public List<tblLogin> ValidateLogin(string userName, string password)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.ValidateLogin(userName, password);
            }
        }

        public bool ValidateConsumerNo(int consumerNo, int loginID)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.ValidateConsumerNo(consumerNo, loginID);
            }
        }

        public List<tblConsumer> LoadConsumer(int loginID = 0)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.LoadConsumer(loginID);
            }
        }

        public List<vPaymentDetail> LoadPaymentDetails(int consumerID, int loginID)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.LoadPaymentDetails(consumerID, loginID);
            }
        }

        public List<Profile> LoadProfileDetails(int loginID)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.LoadProfileDetails(loginID);
            }
        }

        public int SaveProfile(Profile obj)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.SaveProfile(obj);
            }
        }

        public bool QuickPay(int consumerNo)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.QuickPay(consumerNo);
            }
        }

        public int GetBillAmount(int consumerNo)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.GetBillAmount(consumerNo);
            }
        }

        public List<tblBillHistory> SearchPreviousBillStatements(string startDate, string endDate)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.SearchPreviousBillStatements(startDate, endDate);
            }
        }


        public List<Chart> LoadConsumptionChart(int consumerID)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.LoadConsumptionChart(consumerID);
            }
        }

        public string ValidateProfile(Profile obj)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.ValidateProfile(obj);
            }
        }

        public bool AddBill(Bill obj)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.AddBill(obj);
            }
        }

        public List<tblBillHistory> SearchConsumerBillStatements(int consumerID)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.SearchConsumerBillStatements(consumerID);
            }
        }

        public bool DeleteConsumerBill(int consumerID, int ID)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.DeleteConsumerBill(consumerID, ID);
            }
        }


        public int GetLastInsertedLoginID()
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.GetLastInsertedLoginID();
            }
        }


        public byte[] ConvertBase64StringToByteArray(string source)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.ConvertBase64StringToByteArray(source);
            }
        }


        public byte[] DownloadImportProfileTemplate()
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.DownloadImportProfileTemplate();
            }
        }

        public DataTable CreateEmptyDataTable()
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.CreateEmptyDataTable();
            }
        }

        public string SendConsumerSupportEmail(string from, string message)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.SendConsumerSupportEmail(from, message);
            }
        }

        public int SaveCustomerComplaint(int consumer_ID, string email, string concern)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.SaveCustomerComplaint(consumer_ID, email, concern);
            }
        }

        public string GetSessionLoginName()
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.GetSessionLoginName();
            }
        }

        public string GetSessionEmail()
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.GetSessionEmail();
            }
        }

        public List<vCustomerSupport> LoadCustomerSupportIssues(int isResolved = 0)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.LoadCustomerSupportIssues(isResolved);
            }
        }

        public bool ResolveCustomerSupportIssue(int ID, string resolvedMessage, string consumerName, string consumerEmail)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.ResolveCustomerSupportIssue(ID, resolvedMessage, consumerName, consumerEmail);
            }
        }

        public int SaveFeedBack(FeedBackModel model)
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.SaveFeedBack(model);
            }
        }

        public List<SentimentAnalysis> AnalyseConsumerFeedback()
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.AnalyseConsumerFeedback();
            }
        }

        public string GetConsumerFeedBack()
        {
            using (ApiDal apiDal = new ApiDal())
            {
                return apiDal.GetConsumerFeedBack();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
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
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }

        ~ApiBusiness()
        {
            Dispose(false);
        }

        #endregion Member Functions
    }
}
