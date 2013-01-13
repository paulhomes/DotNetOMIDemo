#region License
//
// DotNetOMIDemo: DotNetOMIDemo.cs
//
// Author:
//   Paul Homes (paul.homes@gmail.com)
//
// Copyright (C) 2012 Paul Homes
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;

namespace DotNetOMIDemo
{

    /// <summary>
    /// Command line app to demo the use of the SAS® Open Metadata Interface (OMI) from the .NET platform.
    /// </summary>
    public class DotNetOMIDemo
    {
        /// <summary>
        /// The command line options for DotNetOMIDemo. 
        /// </summary>
        public class Options : CommandLineOptionsBase
        {
            /// <summary>
            /// The host name for the SAS metadata server connection.
            /// </summary>
            [Option("h", "host", DefaultValue = "localhost", HelpText = "SAS metadata server host name (default=localhost)")]
            public string Host { get; set; }

            /// <summary>
            /// The port number for the SAS metadata server connection.
            /// </summary>
            [Option("t", "port", DefaultValue = 8561, HelpText = "SAS metadata server port number (default=8561)")]
            public int Port { get; set; }

            /// <summary>
            /// The user id to be used for the SAS metadata server connection.
            /// </summary>
            [Option("u", "user", Required = true, HelpText = "SAS metadata server user id")]
            public string User { get; set; }

            /// <summary>
            /// The password to be used for the SAS metadata server connection.
            /// </summary>
            [Option("p", "password", Required = true, HelpText = "SAS metadata server password")]
            public string Password { get; set; }

            /// <summary>
            /// The authentication domain used for the SAS metadata server connection.
            /// </summary>
            [Option("d", "authdomain", DefaultValue = null, HelpText = "SAS metadata server authentication domain (default=blank)")]
            public string AuthDomain { get; set; }

            /// <summary>
            /// A switch to turn on/off verbose logging.
            /// </summary>
            [Option("v", "verbose", HelpText = "Enable verbose output")]
            public bool Verbose { get; set; }

            /// <summary>
            /// The name of the OMI task to be run.
            /// </summary>
            [Option("k", "task", DefaultValue = "GetRepositories", HelpText = "Metadata task name (default=GetRepositories)")]
            public string TaskName { get; set; }

            /// <summary>
            /// All of the remaining unparsed options (and may be used as input for some tasks).
            /// </summary>
            [ValueList(typeof(List<string>))]
            public IList<string> OtherOptions { get; set; }

            /// <summary>
            /// Prints a usage message.
            /// </summary>
            /// <returns></returns>
            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                    (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static int Main(string[] args)
        {
            int exitCode = 1; // Exit code defaults to failure.
            try
            {
                // Build a named list of known metadata tasks.
                Dictionary<string, OMITask> tasks = new Dictionary<string, OMITask>();
                tasks.Add("GetNamespaces", new GetNamespacesTask());
                tasks.Add("GetRepositories", new GetRepositoriesTask());
                tasks.Add("GetSubtypes", new GetSubtypesTask());
                tasks.Add("GetTypes", new GetTypesTask());
                tasks.Add("GetTypeProperties", new GetTypePropertiesTask());

                Options options = new Options();
                if (CommandLineParser.Default.ParseArguments(args, options))
                {
                    // Look for the required task by name.
                    if (!tasks.ContainsKey(options.TaskName))
                    {
                        throw new ArgumentOutOfRangeException("task", options.TaskName, "Unknown task requested.");
                    }
                    if (options.Verbose)
                    {
                        Console.WriteLine("Found required task '{0}'.", options.TaskName);
                    }
                    OMITask task = tasks[options.TaskName];
                    task.Options = options;
                    task.runTask();
                    exitCode = 0; // Task completed OK so exit code is 0.
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: DotNetOMIDemo failed: {0}", e);
            }
            return exitCode;
        }

    }

}
