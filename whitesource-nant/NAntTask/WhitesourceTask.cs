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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using Whitesource.Agent;
using Whitesource.Agent.Client;
using Whitesource.Agent.Report;
using Whitesource.Agent.Api;
using Whitesource.Agent.Api.Model;
using Whitesource.Agent.Api.Dispatch;

using Consts = Whitesource.NAnt.Constants.Constants;

namespace Whitesource.NAnt.Tasks
{
    /**
     * Sends an inventory update request to White Source.
     * 
     * @author tom.shapira
     */
    [TaskName("whitesource-task")]
    public class WhiteSourceTask : Task
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

        [BuildElement("fileset", Required = false)]
        public virtual FileSet FileSet
        {
            get { return _fileset; }
            set { _fileset = value; }
        }

        /**
	     * Url to send requests (debug or on-premise).
	     */
        private String _wssUrl;

        [TaskAttribute("wssurl", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public String WssUrl
        {
            get { return _wssUrl; }
            set { _wssUrl = value; }
        }

        /**
         * Name or unique identifier of the product to update.
         */
        private String _product;

        [TaskAttribute("product", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public String Product
        {
            get { return _product; }
            set { _product = value; }
        }

        private String _productVersion;

        /**
         * Version of the product to update.
         */
        [TaskAttribute("productversion", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public String ProductVersion
        {
            get { return _productVersion; }
            set { _productVersion = value; }
        }

        /**
         * The network proxy to use to access the Internet resource.
         */
        private Proxy _proxy;

        [BuildElement("proxy", Required = false)]
        public Proxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        /**
         * Check policies configuration.
         */
        private CheckPolicies _checkPolicies;

        [BuildElement("checkpolicies", Required = false)]
        public CheckPolicies CheckPolicies
        {
            get { return _checkPolicies; }
            set { _checkPolicies = value; }
        }

        /**
	     * Module token to match White Source project.
	     */
        private String _projectToken;

        [TaskAttribute("projecttoken", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public String ProjectToken
        {
            get { return _projectToken; }
            set { _projectToken = value; }
        }

        private bool _debug;

        [TaskAttribute("debug", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }

        /* --- Members --- */

        private WhitesourceService service;

        private List<AgentProjectInfo> projects;

        /* --- Task Methods --- */

        // Override the ExecuteTask method.
        protected override void ExecuteTask()
        {
            CreateService();
            projects = new List<AgentProjectInfo>();
            ScanFiles();

            // determine product name and version
            String productName;
            if (String.IsNullOrEmpty(Product))
            {
                productName = Project.ProjectName;
            }
            else
            {
                productName = Product;
            }

            String productVersion = "";
            if (String.IsNullOrEmpty(ProductVersion))
            {
                productVersion = ProductVersion;
            }

            // send check policies request
            if (CheckPolicies != null)
            {
                Log(Level.Info, "Checking policies");
                CheckPoliciesResult checkPoliciesResult = service.CheckPolicies(ApiKey, productName, productVersion, projects);
                HandlePoliciesResult(checkPoliciesResult);
            }

            // send update request
            Log(Level.Info, "Updating White Source");
            UpdateInventoryResult updateInventoryResult = service.Update(ApiKey, productName, productVersion, projects);
            if (updateInventoryResult != null)
            {
                LogUpdateResult(updateInventoryResult);
            }
        }

        /* --- Private methods --- */

        private void CreateService()
        {
            Log(Level.Debug, "Service Url is " + WssUrl);

            if (Proxy == null)
            {
                service = new WhitesourceService(Consts.AGENT_TYPE, Consts.AGENT_VERSION, WssUrl);
            }
            else
            {
                Credential credentials = Proxy.Credentials;
                if (credentials == null)
                {
                    service = new WhitesourceService(Consts.AGENT_TYPE, Consts.AGENT_VERSION, WssUrl, Proxy.Host, Proxy.Port);
                }
                else
                {
                    service = new WhitesourceService(Consts.AGENT_TYPE, Consts.AGENT_VERSION, WssUrl, 
                        Proxy.Host, Proxy.Port, credentials.UserName, credentials.Password);
                }
            }
            service.SetDebug(Debug || Level.Debug == Project.Threshold);
        }

        private void ScanFiles()
        {
            // If no includes were specified, add all files and subdirectories
            // from the fileset's base directory to the fileset.
            if ((FileSet.Includes.Count == 0) && (FileSet.AsIs.Count == 0))
            {
                FileSet.Includes.Add("**/*.dll");

                // Make sure to rescan the fileset
                FileSet.Scan();
            }

            AgentProjectInfo projectInfo = new AgentProjectInfo();
            if (String.IsNullOrEmpty(ProjectToken))
            {
                projectInfo.Coordinates = new Coordinates(null, Project.ProjectName, null);
            }
            else
            {
                projectInfo.ProjectToken = ProjectToken;
            }

            // scan files and calculate SHA-1 values
            List<DependencyInfo> dependencies = projectInfo.Dependencies;
            foreach (String pathname in FileSet.FileNames)
            {
                FileInfo srcInfo = new FileInfo(pathname);
                if (srcInfo.Exists)
                {
                    String sha1 = ChecksumUtils.GetSha1Hash(pathname);
                    String filename = srcInfo.Name;
                    Log(Level.Debug, "SHA-1 for " + filename + " is: " + sha1);

                    DependencyInfo dependency = new DependencyInfo();
                    dependency.Sha1 = sha1;
                    dependency.ArtifactId = filename;
                    dependency.SystemPath = pathname;
                    dependencies.Add(dependency);
                }
            }
            Log(Level.Info, "Found " + dependencies.Count + " direct dependencies");
            projects.Add(projectInfo);
        }

        private void HandlePoliciesResult(CheckPoliciesResult result)
        {
            // generate report
            try
            {
                Log(Level.Info, "Creating policies report");
                PolicyCheckReport report = new PolicyCheckReport(result);
                report.Generate(CheckPolicies.ReportDir, false);
            }
            catch (IOException e)
            {
                error(e);
            }

            // handle rejections if any
            if (result.HasRejections())
            {
                String rejectionsErrorMessage = "Some dependencies do not conform with open source policies, see report for details.";
                if (CheckPolicies.FailOnRejection)
                {
                    throw new BuildException(rejectionsErrorMessage);
                }
                else
                {
                    Log(Level.Warning, rejectionsErrorMessage);
                }
            }
            else
            {
                Log(Level.Info, "All dependencies conform with open source policies");
            }
        }

        private void LogUpdateResult(UpdateInventoryResult result)
        {
            Log(Level.Info, "White Source update results:");
            Log(Level.Info, "White Source organization: " + result.Organization);

            // newly created projects
            List<String> createdProjects = result.CreatedProjects;
            if (createdProjects.Count == 0)
            {
                Log(Level.Info, "No new projects found");
            }
            else
            {
                Log(Level.Info, createdProjects.Count + " Newly created projects:");
                foreach (String projectName in createdProjects)
                {
                    Log(Level.Info, projectName);
                }
            }

            // updated projects
            List<String> updatedProjects = result.UpdatedProjects;
            if (updatedProjects.Count == 0)
            {
                Log(Level.Info, "No projects were updated");
            }
            else
            {
                Log(Level.Info, updatedProjects.Count + " existing projects were updated:");
                foreach (String projectName in updatedProjects)
                {
                    Log(Level.Info, projectName);
                }
            }
        }

        private void error(String errorMsg)
        {
            if (FailOnError)
            {
                throw new BuildException(errorMsg);
            }
            else
            {
                Log(Level.Error, errorMsg);
            }
        }

        private void error(Exception ex)
        {
            if (FailOnError)
            {
                throw new BuildException(ex.Message, ex);
            }
            else
            {
                Log(Level.Error, ex.Message);
            }
        }

    }
}
