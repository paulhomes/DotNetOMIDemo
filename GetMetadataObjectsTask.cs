#region License
//
// DotNetOMIDemo: GetMetadataObjectsTask.cs
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
    /// An <see cref="OMITask"/> implementation to run the IOMI GetMetadataObjects method and print
    /// the resulting XML response.
    /// </summary>
    public class GetMetadataObjectsTask : OMITask
    {
        private string ns;
        private string modelType;

        /// <summary>
        /// Performs additional validation for the GetMetadataObjects task which requires additional
        /// mandatory command line options to specify:
        /// * namespace
        /// * model type name
        /// </summary>
        protected override void validateOptions()
        {
            if (Options.OtherOptions.Count != 2)
            {
                throw new ArgumentException("A namespace (SAS|REPOS) and model type name must be specified for the GetMetadataObjects metadata task.");
            }
            ns = Options.OtherOptions[0];
            modelType = Options.OtherOptions[1];
        }

        /// <summary>
        /// Uses the SAS Metadata API GetMetadataObjects method to get a list of metadata objects
        /// (attributes and associations) for the specified  model type in the specified namespace
        /// (in XML format).
        /// </summary>
        protected override void doTask()
        {
            if (Options.Verbose)
            {
                Console.WriteLine("Running IOMI GetMetadataObjects method for namespace '{0}' and model type '{1}'.", ns, modelType);
            }

            int omiFlags = 0;
            // Flags for GetMetadataObjects
            omiFlags += 16384; // No SASOMI constant for OMI_DEPENDENCY_USED_BY (include all private/project repos)
            omiFlags += 8192; // No SASOMI constant for OMI_DEPENDENCY_USES (include all public/foundation&custom repos)
            omiFlags += (int)SASOMI.CONSTANTS.OMI_GET_METADATA;
            omiFlags += (int)SASOMI.CONSTANTS.OMI_INCLUDE_SUBTYPES;
            //omiFlags += (int)SASOMI.CONSTANTS.OMI_MATCH_CASE;
            //omiFlags += (int)SASOMI.CONSTANTS.OMI_XMLSELECT;
            // Flags for GetMetadata
            omiFlags += (int)SASOMI.CONSTANTS.OMI_ALL;
            omiFlags += (int)SASOMI.CONSTANTS.OMI_ALL_SIMPLE;
            //omiFlags += 2; // No SASOMI constant for OMI_FULL_OBJECT;
            omiFlags += (int)SASOMI.CONSTANTS.OMI_SUCCINCT;

            string omiOptions = "";
            string objectsXml;
            string foundationId = getFoundationRepositoryId();
            int rc = IOMI.GetMetadataObjects(foundationId, modelType, out objectsXml, ns, omiFlags, omiOptions);
            if (Options.Verbose)
            {
                Console.WriteLine("Successfully run IOMI GetMetadataObjects method. Return code={0}. XML response follows:", rc);
            }
            Console.WriteLine(FormatXml(objectsXml));
        }

    }

}
