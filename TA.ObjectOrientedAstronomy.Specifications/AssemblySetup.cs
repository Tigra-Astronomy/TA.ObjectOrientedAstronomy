// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: AssemblySetup.cs  Last modified: 2016-09-30@02:16 by Tim Long

using Machine.Specifications;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace TA.ObjectOrientedAstronomy.Specifications
    {
    public class AssemblySetup : IAssemblyContext
        {
        static Logger log;

        public static Logger Log
            {
            get
                {
                if (log == null)
                    ConfigureLogging();
                return log;
                }
            }

        public void OnAssemblyStart()
            {
            ConfigureLogging();
            log.Info("Logging configured");
            }

        public void OnAssemblyComplete()
            {
            log = null;
            }

        static void ConfigureLogging()
            {
            var config = new LoggingConfiguration();
            var traceTarget = new TraceTarget();
            config.AddTarget("Diagnostic", traceTarget);
            var traceRule = new LoggingRule("*", LogLevel.Debug, traceTarget);
            config.LoggingRules.Add(traceRule);
            LogManager.Configuration = config;
            log = LogManager.GetCurrentClassLogger();
            }
        }
    }