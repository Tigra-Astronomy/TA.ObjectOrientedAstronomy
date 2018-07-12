// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: Program.cs  Last modified: 2016-10-04@00:58 by Tim Long

using System;
using System.Windows.Forms;

namespace FitsViewer
    {
    internal static class Program
        {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
            {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimpleFitsViewer());
            }
        }
    }