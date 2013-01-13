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
using SAS.BI.AuthenticationService.ClientUserContext;
using SASObjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DotNetOMIDemo
{

    /// <summary>
    /// Command line app to demo the use of the SAS® Open Metadata Interface (OMI) from the .NET platform.
    /// </summary>
    class DotNetOMIDemo
    {
        /// <summary>
        /// The command line options for DotNetOMIDemo. 
        /// </summary>
        public class Options : CommandLineOptionsBase
        {
            [Option("h", "host", DefaultValue = "localhost", HelpText = "SAS metadata server host name (default=localhost)")]
            public string Host { get; set; }

            [Option("t", "port", DefaultValue = 8561, HelpText = "SAS metadata server port number (default=8561)")]
            public int Port { get; set; }

            [Option("u", "user", Required = true, HelpText = "SAS metadata server user id")]
            public string User { get; set; }

            [Option("p", "password", Required = true, HelpText = "SAS metadata server password")]
            public string Password { get; set; }

            [Option("d", "authdomain", DefaultValue = null, HelpText = "SAS metadata server authentication domain (default=blank)")]
            public string AuthDomain { get; set; }

            [Option("v", "verbose", HelpText = "Enable verbose output")]
            public bool Verbose { get; set; }

            [Option("k", "task", DefaultValue = "GetRepositories", HelpText = "Metadata task name (default=GetRepositories)")]
            public string TaskName { get; set; }

            // OtherOptions contains all of the remaining unparsed options (and may be used as input for some tasks).
            [ValueList(typeof(List<string>))]
            public IList<string> OtherOptions { get; set; }

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
                tasks.Add("GetTypes", new GetTypesTask());

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

        /// <summary>
        /// The base class for any DotNetOMIDemo metadata tasks. Handles metadata server connection processing.
        /// </summary>
        /// <remarks>
        /// Subclass this, overriding the doTask method, to implement your metadata procesing task.
        /// </remarks>
        public abstract class OMITask
        {
            private UserContext userContext;
            private SASOMI.IOMI iomi;

            /// <summary>
            /// The supplied command line options.
            /// </summary>
            public Options Options { get; set; }

            /// <summary>
            /// The user context used to obtain a metadata connection.
            /// </summary>
            public UserContext UserContext
            {
                get
                {
                    return userContext;
                }
                private set
                {
                    userContext = value;
                }
            }

            /// <summary>
            /// The metadata connection.
            /// </summary>
            public SASOMI.IOMI IOMI
            {
                get
                {
                    return iomi;
                }
                private set
                {
                    iomi = value;
                }
            }

            /// <summary>
            /// Called by DotNetOMIDemo to run the metadata task.
            /// </summary>
            /// <remarks>
            /// It connects to the metadata server using the supplied command line options and then,
            /// assuming the connection was successful, executes the <see cref="OMITask.doTask"/> method 
            /// to run the metadata processing task.
            /// Disconnects from the metadata server when done.
            /// </remarks>
            public void runTask()
            {
                validateOptions();

                connect();

                try
                {
                    if (iomi != null)
                    {
                        doTask();
                    }
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    // Log the task failure and rethrow (to be handled higher up).
                    Console.WriteLine("ERROR: failed whilst running metadata task.");
                    throw;
                }
                finally
                {
                    disconnect();
                }
            }

            /// <summary>
            /// Subclasses override this method to perform their metadata task, using the already established metadata connection.
            /// </summary>
            /// <remarks>
            /// This method will only be called after a succesful connection to the metadata server,
            /// so you don't need to worry about connecting/disconnecting.
            /// The metadata connection is available in the <see cref="OMITask.IOMI"/> property, which will always be 
            /// non-null for this method.
            /// </remarks>
            protected abstract void doTask();

            /// <summary>
            /// Subclasses can override this method to perform any additional validation of options prior to the metadata connection.
            /// </summary>
            /// <remarks>
            /// If a metadata task requires additional options and those options are missing or invalid this method
            /// should throw an exception.  If an exception is thrown no connection to the metadata server will
            /// be attempted.
            /// If no exception is thrown then it is assumes that all additional options are valid.
            /// </remarks>
            protected virtual void validateOptions()
            {
                // Do nothing by default (assumes no additional options are required).
            }

            private void connect()
            {
                // Validate 0 < port < 65536
                if (Options.Port < 1 || Options.Port > 65535)
                {
                    throw new ArgumentOutOfRangeException("port", Options.Port,
                        "SAS metadata server port number must be in the range 1...65535.");
                }

                // Get an object factory and make a connection to the SAS metadata server connection.
                if (Options.Verbose)
                {
                    Console.WriteLine("Attempting to connect to SAS metadata server {0}:{1} as user '{2}'.",
                        Options.Host, Options.Port, Options.User);
                }

                Login login;
                // TODO: support IWA with new option that leaves login as null.
                login = new Login(Options.User, Options.Password, Options.AuthDomain);
                try
                {
                    userContext = new UserContext();
                    userContext.SetOmrCredentials(login, Options.Host, Options.Port);
                    Repository repository = userContext.GetRepository();
                    iomi = (SASOMI.IOMI) repository.IOmi;
                    if (Options.Verbose)
                    {
                        Console.WriteLine("Successfully connected to SAS metadata server as '{0}' ({1})", 
                            userContext.GetUserName(), userContext.ResolvedIdentity);
                    }
                }
                catch (UserContextException)
                {
                    // Log the connection failure and rethrow (to be handled higher up).
                    Console.WriteLine("ERROR: Failed to connect to the SAS metadata server.");
                    throw;
                }
            }

            private void disconnect()
            {
                // Disconnect from the metadata server.
                iomi = null;
                if (userContext != null)
                {
                    if (Options.Verbose)
                    {
                        Console.WriteLine("Disconnecting from SAS metadata server.");
                    }
                    userContext.Dispose();
                    userContext = null;
                }
            }

        }

        /// <summary>
        /// An <see cref="OMITask"/> implementation to run the IOMI GetRepositories method and print the resulting XML response.
        /// </summary>
        public class GetRepositoriesTask : OMITask
        {
            protected override void doTask()
            {
                // Use the SAS Metadata API GetRepositories method to get a list of metadata repositories
                // being managed by the metadata server (in XML format).
                if (Options.Verbose)
                {
                    Console.WriteLine("Running IOMI GetRepositories method.");
                }
                int omiFlags = (int)SASOMI.CONSTANTS.OMI_ALL;
                string omiOptions = "";
                string repositoriesXml;
                int rc = IOMI.GetRepositories(out repositoriesXml, omiFlags, omiOptions);
                if (Options.Verbose)
                {
                    Console.WriteLine("Successfully run IOMI GetRepositories method. Return code={0}. XML response follows:", rc);
                }
                Console.WriteLine(FormatXml(repositoriesXml));
            }
        }

        /// <summary>
        /// An <see cref="OMITask"/> implementation to run the IOMI GetNamespaces method and print the resulting XML response.
        /// </summary>
        public class GetNamespacesTask : OMITask
        {
            protected override void doTask()
            {
                // Use the SAS Metadata API GetNamespaces method to get a list of
                // namespaces for the metadata server (in XML format).
                if (Options.Verbose)
                {
                    Console.WriteLine("Running IOMI GetNamespaces method.");
                }
                int omiFlags = 0;
                string omiOptions = "";
                string namespacesXml;
                int rc = IOMI.GetNamespaces(out namespacesXml, omiFlags, omiOptions);
                if (Options.Verbose)
                {
                    Console.WriteLine("Successfully run IOMI GetNamespaces method. Return code={0}. XML response follows:", rc);
                }
                Console.WriteLine(FormatXml(namespacesXml));
            }
        }

        /// <summary>
        /// An <see cref="OMITask"/> implementation to run the IOMI GetTypes method and print the resulting XML response.
        /// </summary>
        public class GetTypesTask : OMITask
        {
            private string ns;

            protected override void validateOptions()
            {
                // GetTypes requires a namespace which is a mandatory additional command line option.
                if (Options.OtherOptions.Count != 1)
                {
                    throw new ArgumentException("A namespace (SAS|REPOS) must be specified for the GetTypes metadata task.");
                }
                ns = Options.OtherOptions[0];
            }

            protected override void doTask()
            {
                // Use the SAS Metadata API GetTypes method to get a list of
                // namespaces for the metadata server (in XML format).
                if (Options.Verbose)
                {
                    Console.WriteLine("Running IOMI GetTypes method for namespace '{0}'.", ns);
                }
                int omiFlags = 0;
                string omiOptions = "";
                string typesXml;
                int rc = IOMI.GetTypes(out typesXml, ns, omiFlags, omiOptions);
                if (Options.Verbose)
                {
                    Console.WriteLine("Successfully run IOMI GetTypes method. Return code={0}. XML response follows:", rc);
                }
                Console.WriteLine(FormatXml(typesXml));
            }
        }

        /// <summary>
        /// Utility method to format compressed XML with indentation over multiple lines for better readability.
        /// </summary>
        /// <param name="xml">The unformatted XML.</param>
        /// <returns>A formatted version of the supplied unformatted XML</returns>
        static string FormatXml(string xml)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.OmitXmlDeclaration = true;
            XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            doc.Save(xmlWriter);
            return stringBuilder.ToString();
        }
    }
}
