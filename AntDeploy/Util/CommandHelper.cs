﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Utilities;

namespace AntDeploy.Util
{

    /// <summary>
    /// 执行command命令的Helper
    /// </summary>
    public class CommandHelper
    {
        

        public static bool RunMsbuild(string path,Action<string> logAction,Action<string> errLogAction)
        {
            var msBuild = GetMsBuildPath();
            if (string.IsNullOrEmpty(msBuild))
            {
                return false;
            }
            return RunDotnetExternalExe(string.Empty,msBuild+"\\MsBuild.exe",
                path + " /t:Rebuild /p:Configuration=Release",
                logAction,errLogAction);
        }


        public static string GetMsBuildPath()
        {
            var getmS = ToolLocationHelper.GetPathToBuildTools(ToolLocationHelper.CurrentToolsVersion);
            return getmS;


            //try
            //{
               
                

              
           
            //    //var parms = new BuildParameters
            //    //{
            //    //    DetailedSummary = true,
                    
            //    //};

            //    //var projectInstance = new ProjectInstance(path);
            //    //projectInstance.SetProperty("Configuration", "Release");
            //    //projectInstance.SetProperty("Platform", "Any CPU");
            //    //var request = new BuildRequestData(projectInstance,  new string[] { "Rebuild" });
                
            //    //parms.Loggers = new List<Microsoft.Build.Framework.ILogger>
            //    //{
            //    //    new ConsoleLogger(LoggerVerbosity.Normal,
            //    //        message => { log(message); }, color => { }, () => { })
            //    //    {
            //    //        ShowSummary = true
            //    //    }
            //    //};

            //    //var result = BuildManager.DefaultBuildManager.Build(parms, request);
            //    //if (result.OverallResult == BuildResultCode.Success)
            //    //{
            //    //    return string.Empty;
            //    //}

            //    return getmS;
            //}
            //catch (Exception e)
            //{
            //    return e.Message;
            //}
        }


        /// <summary>
        /// 执行dotnet Command命令
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="arguments"></param>
        /// <param name="logAction"></param>
        /// <param name="errLogAction"></param>
        /// <returns></returns>
        public static bool RunDotnetExternalExe(string projectPath,string fileName,string arguments,Action<string> logAction,Action<string> errLogAction)
        {
            try
            {
                if (string.IsNullOrEmpty(arguments))
                {
                    throw new ArgumentException(nameof(arguments));
                }

                //if (!arguments.StartsWith(" "))
                //{
                //    arguments = " " + arguments;
                //}

                var process = new Process();

                process.StartInfo.WorkingDirectory = projectPath;
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Verb = "runas";
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;



                process.Start();


                process.OutputDataReceived += (sender, args) =>
                {
                    if(!string.IsNullOrWhiteSpace(args.Data))logAction(args.Data);
                };
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, data) =>
                {
                    if (!string.IsNullOrWhiteSpace(data.Data)) errLogAction(data.Data);
                };
                process.BeginErrorReadLine();

                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                errLogAction(ex.Message);
                return false;
            }
        }

        
    }
}