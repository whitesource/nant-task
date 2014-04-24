/**
 * Copyright (C) 2012 White Source Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.IO.Compression;

using Whitesource.Agent.Api.Dispatch;

using whitesource_nant.Properties;

namespace Whitesource.Agent.Report
{
    /**
     * A report generator for policy check results.
     *
     * @author tom.shapira
     */
    public class PolicyCheckReport
    {

        /* --- Static members --- */

        private const String TEMPLATE_FOLDER = "Templates/";
        private const String TEMPLATE_FILE = "policy-check.vm";
        private const String CSS_FILE = "wss.css";

        /* --- Members --- */

        public CheckPoliciesResult Result { get; set; }

        public String BuildName { get; set; }

        public String BuildNumber { get; set; }

        /* --- Constructors --- */

        /**
         * Constructor
         *
         * @param result
         */
        public PolicyCheckReport(CheckPoliciesResult result)
        {
            this.Result = result;
        }

        /**
         * Constructor
         *
         * @param result
         * @param buildName
         * @param buildNumber
         */
        public PolicyCheckReport(CheckPoliciesResult result, String buildName, String buildNumber)
            : this(result)
        {
            {
                this.BuildName = buildName;
                this.BuildNumber = buildNumber;
            }

        }

        /* --- Public methods --- */

        /**
         * The method generates the policy check report
         *
         * @param outputDir Directory where report files will be created.
         * @param pack      <code>True</code> to create a zip file from the resulting directory.
         */
        public void Generate(String outputDir, bool pack)
        {
            if (Result == null)
            {
                throw new InvalidOperationException("Check policies result is null");
            }

            // prepare working directory
            if (!Directory.Exists(outputDir))
            {
                throw new IOException("Output directory doesn't exist: " + outputDir);
            }
            DirectoryInfo workDir = Directory.CreateDirectory(outputDir).CreateSubdirectory("whitesource");

            // create actual report
            StreamWriter writer = new StreamWriter(Path.Combine(workDir.FullName, "index.html"));
            new ReportGenerator().GenerateReport(Result, BuildName, BuildNumber, writer);

            // copy resources
            CopyReportResources(workDir.FullName);

            // package report into a zip archive
            if (pack)
            {
                ZipFile.CreateFromDirectory(workDir.FullName, Path.Combine(outputDir, "whitesource.zip"));
                Directory.Delete(workDir.FullName, true);
            }
        }

        /* --- Protected methods --- */

        /**
         * Copy required resources for the report.
         *
         * @param workDir Report work directory.
         *
         * @throws IOException
         */
        protected void CopyReportResources(String workDir)
        {
            File.WriteAllText(Path.Combine(workDir, CSS_FILE), Resources.wss);
        }

    }

}