using QuikSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace AutoConnector
{
    public static class AutoLogin
    {
        public static bool Enter(string login, string pass)
        {
            var hLoginWnd = Win32Functions.FindWindow("#32770", "Идентификация пользователя");

            if (hLoginWnd == IntPtr.Zero)
            {
                hLoginWnd = Win32Functions.FindWindow("#32770", "User identification");
            }

            if (hLoginWnd == IntPtr.Zero)
            {
                return false;
            }

            if (hLoginWnd != IntPtr.Zero)
            {
                var nBtnOk = WindowTools.FindWindowByIndex(hLoginWnd, 1, "Button");
                var hLogin = WindowTools.FindWindowByIndex(hLoginWnd, 1, "Edit");
                var nPassw = WindowTools.FindWindowByIndex(hLoginWnd, 2, "Edit");

                WindowTools.SetWindowText(hLogin, login);
                WindowTools.SetWindowText(nPassw, pass);
                WindowTools.Click(nBtnOk);
            }

            return true;
        }
    }
}
