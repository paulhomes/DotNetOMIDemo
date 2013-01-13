﻿#region License
//
// DotNetOMIDemo: GetRepositoriesTask.cs
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
    /// An <see cref="OMITask"/> implementation to run the IOMI GetRepositories method and print
    /// the resulting XML response.
    /// </summary>
    public class GetRepositoriesTask : OMITask
    {

        /// <summary>
        /// Uses the SAS Metadata API GetRepositories method to get a list of metadata repositories
        /// being managed by the metadata server (in XML format).
        /// </summary>
        protected override void doTask()
        {
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

}
