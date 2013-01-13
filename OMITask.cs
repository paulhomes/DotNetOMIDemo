#region License
//
// DotNetOMIDemo: OMITask.cs
//
// Author:
//   Paul Homes (paul.homes@gmail.com)
//
// Copyright (C) 2013 Paul Homes
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

using System;
using System.Text;
using System.Xml;
using SAS.BI.AuthenticationService.ClientUserContext;
using SASObjectManager;

namespace DotNetOMIDemo
{

    /// <summary>
    /// The base class for any DotNetOMIDemo metadata tasks. Handles metadata server connection
    /// processing.
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
        public DotNetOMIDemo.Options Options { get; set; }

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
        /// assuming the connection was successful, executes the <see cref="OMITask.doTask"/>
        /// method to run the metadata processing task.
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
        /// Subclasses override this method to perform their metadata task, using the already
        /// established metadata connection.
        /// </summary>
        /// <remarks>
        /// This method will only be called after a succesful connection to the metadata server,
        /// so you don't need to worry about connecting/disconnecting.
        /// The metadata connection is available in the <see cref="OMITask.IOMI"/> property,
        /// which will always be  non-null for this method.
        /// </remarks>
        protected abstract void doTask();

        /// <summary>
        /// Subclasses can override this method to perform any additional validation of options
        /// prior to the metadata connection.
        /// </summary>
        /// <remarks>
        /// If a metadata task requires additional options and those options are missing or
        /// invalid this method should throw an exception.  If an exception is thrown no connection
        /// to the metadata server will be attempted.
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
                iomi = (SASOMI.IOMI)repository.IOmi;
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

        /// <summary>
        /// Utility method to format compressed XML with indentation over multiple lines for
        /// better readability.
        /// </summary>
        /// <param name="xml">The unformatted XML.</param>
        /// <returns>A formatted version of the supplied unformatted XML</returns>
        public static string FormatXml(string xml)
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
