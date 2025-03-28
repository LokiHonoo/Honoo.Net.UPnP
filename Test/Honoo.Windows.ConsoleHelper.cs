/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System;
using System.Runtime.InteropServices;

namespace Honoo.Windows
{
    /// <summary>
    /// Windows 控制台控制。
    /// </summary>
    internal static class ConsoleHelper
    {
        #region 快速编辑模式

        #region Native

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr consoleHandle, out uint mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int consoleHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr consoleHandle, uint mode);

        #endregion Native

        private const uint ENABLE_INSERT_MODE = 0x0020;
        private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        private const int STD_INPUT_HANDLE = -0x000A;

        /// <summary>
        /// 关闭控制台快速编辑模式，防止阻塞现象。
        /// </summary>
        /// <returns></returns>
        internal static bool DisableQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            if (GetConsoleMode(hStdin, out uint mode))
            {
                mode &= ~ENABLE_INSERT_MODE;
                mode &= ~ENABLE_QUICK_EDIT_MODE;
                return SetConsoleMode(hStdin, mode);
            }
            return false;
        }

        /// <summary>
        /// 开启控制台快速编辑模式。
        /// </summary>
        /// <returns></returns>
        internal static bool EnableQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            if (GetConsoleMode(hStdin, out uint mode))
            {
                mode ^= ~ENABLE_INSERT_MODE;
                mode ^= ~ENABLE_QUICK_EDIT_MODE;
                return SetConsoleMode(hStdin, mode);
            }
            return false;
        }

        #endregion 快速编辑模式

        #region 控制信号

        #region Native

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(CtrlHandlerRoutine routine, bool add);

        #endregion Native

        /// <summary>
        /// 处理接受到的控制信号的委托类型。
        /// </summary>
        /// <param name="ctrlType">控制信号的类型。</param>
        /// <returns></returns>
        internal delegate bool CtrlHandlerRoutine(int ctrlType);

        /// <summary>
        /// 设置控制信号的处理方法。
        /// </summary>
        /// <param name="routine">处理方法。方法包含一个参数，表示控制信号的类型。</param>
        /// <returns></returns>
        internal static bool AddCtrlHandler(CtrlHandlerRoutine routine)
        {
            return SetConsoleCtrlHandler(routine, true);
        }

        /// <summary>
        /// 移除控制信号的处理方法。
        /// </summary>
        /// <param name="routine">处理方法。方法包含一个参数，表示控制信号的类型。</param>
        /// <returns></returns>
        internal static bool RemoveCtrlHandler(CtrlHandlerRoutine routine)
        {
            return SetConsoleCtrlHandler(routine, false);
        }

        #endregion 控制信号
    }
}