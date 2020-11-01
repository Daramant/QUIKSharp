using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoConnector
{
    public static class WindowTools
    {
        private const int WM_CHAR = 0x0102;
        private const uint WM_SETTEXT = 0x000c;
        private const int BM_CLICK = 0x00F5;

        /// <summary>
        /// Найти дискриптор окна по индексу.
        /// </summary>
        /// <param name="hWndParent"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IntPtr FindWindowByIndex(IntPtr hWndParent, int index, string type)
        {
            if (index == 0)
                return hWndParent;

            int ct = 0;
            var result = IntPtr.Zero;

            do
            {
                result = Win32Functions.FindWindowEx(hWndParent, result, type, null);

                if (result != IntPtr.Zero)
                {
                    ++ct;
                }
            }
            while (ct < index && result != IntPtr.Zero);

            return result;
        }

        /// <summary>
        /// Отправить текст в окно с данными пользователя.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="text"></param>
        public static void SetWindowText(IntPtr hWnd, string text)
        {
            try
            {
                Win32Functions.SetFocus(hWnd);
                Win32Functions.SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, null);

                foreach (var c in text)
                {
                    Thread.Sleep(50);
                    var val = new IntPtr((Int32)c);
                    Win32Functions.PostMessage(hWnd, WM_CHAR, val, new IntPtr(0));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public static void Click(IntPtr nBtn)
        {
            Win32Functions.SetFocus(nBtn);
            Win32Functions.PostMessage(nBtn, BM_CLICK, new IntPtr(0), new IntPtr(0));
        }
    }
}
