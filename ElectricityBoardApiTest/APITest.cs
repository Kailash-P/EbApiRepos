using DAL;
using ElectricityBoardApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBoardApiTest
{
    [TestClass]
    public class ApiTest : IDisposable
    {

         #region Member Variables

        bool disposed = false;
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        readonly LoginApiController api = null;

        #endregion Member Variables


        public ApiTest()
        {
            api = new LoginApiController();
        }

        [TestMethod]
        public void ValidateLoginTest1()
        {
            var list = api.ValidateLogin("admin", "admin");

            if(list == null || list.Count == 0)
            {
                Assert.Fail("Invalid UserName & Password");
            }
        }

        [TestMethod]
        public void ValidateLoginTest2()
        {
            var list = api.ValidateLogin("Kailash", "admin");

            if (list == null || list.Count == 0)
            {
                Assert.Fail("Invalid UserName & Password");
            }
        }

        [TestMethod]
        public void LoadConsumerTest1()
        {
            var list = api.LoadConsumer();

            if (list == null || list.Count == 0)
            {
                Assert.Fail("Invalid Consumer List.");
            }
        }

        [TestMethod]
        public void LoadConsumerTest2()
        {
            var list = api.LoadConsumer(123123123);

            if (list == null || list.Count == 0)
            {
                Assert.Fail("No Records Found.");
            }
        }

        [TestMethod]
        public void LoadConsumptionChartTest1()
        {
            var list = api.LoadConsumptionChart(123123123);

            if (list == null || list.Count == 0)
            {
                Assert.Fail("No records found.");
            }
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
                api.Dispose();
            }

            disposed = true;
        }

        ~ApiTest()
        {
            Dispose(false);
        }
    }
}
