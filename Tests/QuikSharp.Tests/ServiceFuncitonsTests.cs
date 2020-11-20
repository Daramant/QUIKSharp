using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using QuikSharp.QuikFunctions.Services;

namespace QuikSharp.Tests
{
    [TestFixture]
    public class ServiceFunctionsTest : BaseTest
    {
        [SetUp]
        public void SetUp()
        {
            SetUpQuik();
        }

        [Test]
        public void IsConencted()
        {
            Console.WriteLine("IsConnected? : " + Quik.Functions.Service.IsConnectedAsync().Result);
        }

        [Test]
        public void GetWorkingFolder()
        {
            Console.WriteLine("GetWorkingFolder: " + Quik.Functions.Service.GetWorkingFolderAsync().Result);
        }

        [Test]
        public void GetScriptPath()
        {
            Console.WriteLine("GetScriptPath: " + Quik.Functions.Service.GetScriptPathAsync().Result);
        }

        [Test]
        public void GetInfoParam()
        {
            var values = Enum.GetValues(typeof(InfoParams)).Cast<InfoParams>().ToArray();
            foreach (var value in values)
            {
                Console.WriteLine(value
                    + " : " + Quik.Functions.Service.GetInfoParamAsync(value).Result);
            }
        }

        [Test]
        public void Message()
        {
            Console.WriteLine("This is a message: " +
                Quik.Functions.Service.MessageAsync("This is a message", NotificationType.Info).Result);
            Console.WriteLine("This is a warning: " +
                Quik.Functions.Service.MessageAsync("This is a warning", NotificationType.Warning).Result);
            Console.WriteLine("This is an error: " +
                Quik.Functions.Service.MessageAsync("This is an error", NotificationType.Error).Result);
        }

        [Test]
        public void PrintDbgStr()
        {
            Console.WriteLine("Debug: " +
                Quik.Functions.Service.PrintDbgStrAsync("This is debug info").Result);
        }

        [Test]
        public void AddLabel()
        {
            var res = Quik.Functions.Labels.AddLabelAsync(61000, "20170105", "100000", "1", "C:\\ClassesC\\Labels\\buy.bmp", "si", "BOTTOM", 0);
            Console.WriteLine("AddLabel: "
                    + String.Join(",", Convert.ToString(res.Result)));
        }

        [Test]
        public void DelLabel()
        {
            decimal tagId = 13;
            var res = Quik.Functions.Labels.DelLabelAsync("si", tagId);

            Console.WriteLine("delLabel: "
                    + String.Join(",", Convert.ToString(res.Result)));
        }

        [Test]
        public void DelAllLabels()
        {
            var res = Quik.Functions.Labels.DelAllLabelsAsync("si").Result;

            Console.WriteLine("delAllLabels: "
                  + String.Join(",", Convert.ToString(res)));
        }
    }
}
