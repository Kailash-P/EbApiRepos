using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IApiDal
    {
        string Get();

        List<tblLogin> ValidateLogin(string userName, string password);

        List<tblConsumer> LoadConsumer(int loginID = 0);

        List<vPaymentDetail> LoadPaymentDetails(int consumerID, int loginID);

        List<Profile> LoadProfileDetails(int loginID);

        int SaveProfile(Profile obj);

        bool QuickPay(int consumerNo);

        int GetBillAmount(int consumerNo);

        List<tblBillHistory> SearchPreviousBillStatements(string startDate, string endDate);

        List<Chart> LoadConsumptionChart(int consumerID);

        string ValidateProfile(Profile obj);

        bool AddBill(Bill obj);

        List<tblBillHistory> SearchConsumerBillStatements(int consumerID);

        bool DeleteConsumerBill(int consumerID, int ID);

        int GetLastInsertedLoginID();

        byte[] ConvertBase64StringToByteArray(string source);

        byte[] DownloadImportProfileTemplate();

        DataTable CreateEmptyDataTable();

        string SendConsumerSupportEmail(string from, string message);

        int SaveCustomerComplaint(int consumer_ID, string email, string concern);

        string GetSessionLoginName();

        string GetSessionEmail();

        List<vCustomerSupport> LoadCustomerSupportIssues(int isResolved = 0);

        bool ResolveCustomerSupportIssue(int ID, string resolvedMessage, string consumerName, string consumerEmail);

        List<SentimentAnalysis> AnalyseConsumerFeedback();

        string GetConsumerFeedBack();

        int SaveFeedBack(FeedBackModel model);
    }
}
