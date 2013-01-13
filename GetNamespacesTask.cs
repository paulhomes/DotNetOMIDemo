#region License
//
// DotNetOMIDemo: GetNamespacesTask.cs
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

namespace DotNetOMIDemo
{
    
    /// <summary>
    /// An <see cref="OMITask"/> implementation to run the IOMI GetNamespaces method and print the
    /// resulting XML response.
    /// </summary>
    public class GetNamespacesTask : OMITask
    {

        /// <summary>
        /// Uses the SAS Metadata API GetNamespaces method to get a list of namespaces for the
        /// metadata server (in XML format).
        /// </summary>
        protected override void doTask()
        {
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

}
