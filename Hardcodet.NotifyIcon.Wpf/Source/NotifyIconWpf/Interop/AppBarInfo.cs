﻿// Some interop code taken from Mike Marshall's AnyForm

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Hardcodet.Wpf.TaskbarNotification.Interop
{
    /// <summary>
    /// 
    /// </summary>
    public class AppBarInfo
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("shell32.dll")]
        private static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA data);

        [DllImport("user32.dll")]
        private static extern int SystemParametersInfo(uint uiAction, uint uiParam,
            IntPtr pvParam, uint fWinIni);


        private const int ABE_BOTTOM = 3;
        private const int ABE_LEFT = 0;
        private const int ABE_RIGHT = 2;
        private const int ABE_TOP = 1;

        private const int ABM_GETTASKBARPOS = 0x00000005;

        // SystemParametersInfo constants
        private const uint SPI_GETWORKAREA = 0x0030;

        private APPBARDATA m_data;

        /// <summary>
        /// 
        /// </summary>
        public ScreenEdge Edge
        {
            get { return (ScreenEdge) m_data.uEdge; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Rectangle WorkArea
        {
            get { return GetRectangle(m_data.rc); }
        }

        private Rectangle GetRectangle(RECT rc)
        {
            return new Rectangle(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
        }     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strClassName"></param>
        /// <param name="strWindowName"></param>
        public void GetPosition(string strClassName, string strWindowName)
        {
            m_data = new APPBARDATA();
            m_data.cbSize = (uint) Marshal.SizeOf(m_data.GetType());

            IntPtr hWnd = FindWindow(strClassName, strWindowName);

            if (hWnd != IntPtr.Zero)
            {
                uint uResult = SHAppBarMessage(ABM_GETTASKBARPOS, ref m_data);

                if (uResult != 1)
                {
                    throw new Exception("Failed to communicate with the given AppBar");
                }
            }
            else
            {
                throw new Exception("Failed to find an AppBar that matched the given criteria");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void GetSystemTaskBarPosition()
        {
            GetPosition("Shell_TrayWnd", null);
        }


        /// <summary>
        /// 
        /// </summary>
        public enum ScreenEdge
        {
            /// <summary>
            /// 
            /// </summary>
            Undefined = -1,
            /// <summary>
            /// 
            /// </summary>
            Left = ABE_LEFT,
            /// <summary>
            /// 
            /// </summary>
            Top = ABE_TOP,
            /// <summary>
            /// 
            /// </summary>
            Right = ABE_RIGHT,
            /// <summary>
            /// 
            /// </summary>
            Bottom = ABE_BOTTOM
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}