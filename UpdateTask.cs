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
using NAnt.Core;
using Whitesource.Agent.Api.Model;

namespace Whitesource.NAnt.Tasks
{
    /**
     * Sends an inventory update request to White Source.
     * 
     * @author tom.shapira
     */
    [TaskName("updatetask")]
    public class UpdateTask : Task
    {

        /* --- Task Attributes --- */

        /**
         * Unique identifier of the organization with White Source.
         */
        private String _apikey;

        [TaskAttribute("apikey", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public String ApiKey
        {
            get { return _apikey; }
            set { _apikey = value; }
        }

        /**
         * The file set to scan and send to White Source.
         */
        private FileSet _fileset = new FileSet();

        [BuildElement("fileset", Required = true)]
        public virtual FileSet FileSet
        {
            get { return _fileset; }
            set { _fileset = value; }
        }

        /**
         * Whether or not to stop the build when encountering an error.
         */
        private bool _failOnError;

        [TaskAttribute("failOnError", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public String FailOnError
        {
            get { return _failOnError; }
            set { _failOnError = value; }
        }

        /**
	     * Url to send requests (debug or on-premise).
	     */
        private String _wssUrl;

        [TaskAttribute("wssUrl", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public String WssUrl
        {
            get { return _wssUrl; }
            set { _wssUrl = value; }
        }

        /**
         * Check policies configuration.
         */
        private bool _checkPolicies;

        [TaskAttribute("checkPolicies", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public bool CheckPolicies
        {
            get { return _checkPolicies; }
            set { _checkPolicies = value; }
        }

        /* --- Task Methods --- */

        public void RunUpdate(Project project)
        {
            // If no includes were specified, add all files and subdirectories
            // from the fileset's base directory to the fileset.
            if ((FileSet.Includes.Count == 0) && (FileSet.AsIs.Count == 0))
            {
                // Set default values in case none found
                FileSet.Includes.Add("**/*.dll");

                // Rescan the fileset after adding default values
                FileSet.Scan();
            }

            Log(Level.Info, "Collecting OSS usage information");

            AgentProjectInfo projectInfo = new AgentProjectInfo();
            projectInfo.SetCoordinates(new Coordinates(null, project.ProjectName, null));

            // scan files and calculate SHA-1 values
            Collection<DependencyInfo> dependencies = projectInfo.GetDependencies();
            foreach (string pathname in FileSet.FileNames)
            {
                FileInfo srcInfo = new FileInfo(pathname);
                if (srcInfo.Exists)
                {
                    string sha1 = GetSha1Hash(pathname);
                    string filename = srcInfo.Name;
                    Log(Level.Debug, "SHA-1 for " + filename + " is: " + sha1);

                    DependencyInfo dependency = new DependencyInfo();
                    dependency.SetSha1(sha1);
                    dependency.SetArtifactId(filename);
                    dependency.SetSystemPath(pathname);
                }
            }
            Log(Level.Info, "Found " + dependencies.Count + " direct dependencies");
        }

        /**
         * Calculate the given file SHA-1
         */
        private String GetSha1Hash(String filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                SHA1 sha = new SHA1Managed();
                byte[] hash = sha.ComputeHash(fs);
                StringBuilder builder = new StringBuilder(2 * hash.Length);
                foreach (byte b in hash)
                {
                    builder.AppendFormat("{0:X2}", b);
                }
                return builder.ToString();
            }
        }

        private void DebugAgentProjectInfos(Collection<AgentProjectInfo> projectInfos)
        {
            Log(Level.Debug, "|----------------- dumping projectInfos -----------------|");
            Log(Level.Debug, "Total number of projects : " + projectInfos.size());

            foreach (AgentProjectInfo projectInfo in projectInfos)
            {
                Log(Level.Debug, "Project coordinates: " + projectInfo.getCoordinates());
                Log(Level.Debug, "Project parent coordinates: " + projectInfo.getParentCoordinates());
                Log(Level.Debug, "Project project token: " + projectInfo.getProjectToken());

                Collection<DependencyInfo> dependencies = projectInfo.getDependencies();
                Log(Level.Debug, "total # of dependencies: " + dependencies.size());
                foreach (DependencyInfo info in dependencies)
                {
                    Log(Level.Debug, info + " SHA-1: " + info.getSha1());
                }
            }
            Log(Level.Debug, "|-------------------- dump finished --------------------|");
        }

        // Override the ExecuteTask method.
        protected override void ExecuteTask()
        {
            RunUpdate(Project);
            Log(Level.Info, "Your ApiKey: " + _apikey);
        }
    }
}
