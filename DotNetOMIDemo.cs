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
using SASObjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DotNetOMIDemo
{
    class DotNetOMIDemo
    {
        class Options : CommandLineOptionsBase
        {
            [Option("h", "host", DefaultValue = "localhost", HelpText = "SAS metadata server host name (default=localhost)")]
            public string Host { get; set; }

            [Option("t", "port", DefaultValue = 8561, HelpText = "SAS metadata server port number (default=8561)")]
            public int Port { get; set; }

            [Option("u", "user", Required = true, HelpText = "SAS metadata server user id")]
            public string User { get; set; }

            [Option("p", "password", Required = true, HelpText = "SAS metadata server password")]
            public string Password { get; set; }

            [Option("v", "verbose", HelpText = "Enable verbose output")]
            public bool Verbose { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                    (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLineParser.Default.ParseArguments(args, options))
            {
                // Construct configuration for the SAS metadata server connection.
                ServerDef serverDef = new ServerDef();
                // NOTE: sasoman.chm > ServerDef > ClassIdentifier Property says 
                // OMR is 2887E7D7-4780-11D4-879F-00C04F38F0DB but that doesn't 
                // seem to work. A manually generated XML config
                // (C:\Users\userid\AppData\Roaming\SAS\MetadataServer\oms_serverinfo2.xml)
                // using C:\Program Files\SAS\SharedFiles\Integration Technologies\ITConfig2.exe
                // has ClassIdentifier="0217E202-B560-11DB-AD91-001083FF6836".  This does work.
                serverDef.ClassIdentifier = "0217E202-B560-11DB-AD91-001083FF6836";
                // NOTE: sasoman.chm > ServerDef > ProgID Property says it's an alternative to
                // ClassIdentifier.
                // serverDef.ProgID = "SASOMI.OMI.1.0";
                serverDef.Protocol = SASObjectManager.Protocols.ProtocolBridge;
                serverDef.BridgeEncryptionLevel = SASObjectManager.EncryptionLevels.EncryptUserAndPassword;
                //serverDef.BridgeEncryptionAlgorithm = "SASProprietary";
                serverDef.MachineDNSName = options.Host;
                // TODO: validate 0 < port < 65536
                serverDef.Port = options.Port;

                // Get an object factory and make a connection to the SAS metadata server connection.
                if (options.Verbose)
                {
                    Console.WriteLine("Attempting to connect to SAS metadata server {0}:{1} as user '{2}'",
                        options.Host, options.Port, options.User);
                }
                ObjectFactoryMulti2 objectFactory = new SASObjectManager.ObjectFactoryMulti2();
                SASOMI.IOMI iomi = (SASOMI.IOMI)objectFactory.CreateOMRConnection(serverDef, options.User, options.Password);
                if (options.Verbose)
                {
                    Console.WriteLine("Successfully connected to SAS metadata server.");
                }

                // Use the SAS Metadata API GetRepositories method to get a list of metadata repositories
                // being managed by the metadata server (in XML format).
                if (options.Verbose)
                {
                    Console.WriteLine("Running IOMI GetRepositories method.");
                }
                int omiFlags = (int)SASOMI.CONSTANTS.OMI_ALL;
                string omiOptions = "";
                string repositoriesXml;
                int rc = iomi.GetRepositories(out repositoriesXml, omiFlags, omiOptions);
                if (options.Verbose)
                {
                    Console.WriteLine("Successfully run IOMI GetRepositories method. Return code={0}. XML response follows:", rc);
                }
                Console.WriteLine(FormatXml(repositoriesXml));

                // Disconnect from the metadata server.
                // TODO: what is the correct way of disconnecting the metadata connection in .NET?
                // There appear to be no API close, dispose, disconnect methods.
                // Marshal.ReleaseComObject appears to do the trick but I'm not sure it's the best way.
                if (options.Verbose)
                {
                    Console.WriteLine("Disconnecting from SAS metadata server.");
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(iomi);
                iomi = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objectFactory);
                objectFactory = null;
            }
        }

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
