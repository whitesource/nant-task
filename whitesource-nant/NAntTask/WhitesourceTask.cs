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
using Whitesource.Agent.Api;
using Whitesource.Agent.Client;
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
        private bool _checkPolicies;

        [TaskAttribute("checkPolicies", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public bool CheckPolicies
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

        /* --- Task Methods --- */

        // Override the ExecuteTask method.
        protected override void ExecuteTask()
        {
            String wssUrl = WssUrl;
            if (String.IsNullOrEmpty(WssUrl))
            {
                wssUrl = ClientConstants.DEFAULT_SERVICE_URL;
            }
            RunUpdate(Project, ApiKey, wssUrl, ProjectToken);
        }

        public void RunUpdate(Project project, String apiKey, String wssUrl, String projectToken)
        {
            // If no includes were specified, add all files and subdirectories
            // from the fileset's base directory to the fileset.
            if ((FileSet.Includes.Count == 0) && (FileSet.AsIs.Count == 0))
            {
                FileSet.Includes.Add("**/*.dll");

                // Make sure to rescan the fileset after adding "**/*"
                FileSet.Scan();
            }

            AgentProjectInfo projectInfo = new AgentProjectInfo();
            if (String.IsNullOrEmpty(projectToken))
            {
                projectInfo.Coordinates = new Coordinates(null, project.ProjectName, null);
            }
            else
            {
                projectInfo.ProjectToken = projectToken;
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

            // send request
            UpdateInventoryResult updateInventoryResult = UpdateInventory(wssUrl, apiKey, project.ProjectName, projectInfo);
            if (updateInventoryResult != null)
            {
                LogUpdateResult(updateInventoryResult);
            }
        }

        /* --- Private methods --- */

        /**
         * The method calls the White Source service for inventory update.
         */
        private UpdateInventoryResult UpdateInventory(String serviceUrl, String apiKey, String productName, AgentProjectInfo projectInfo)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(serviceUrl) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "application/json";

                SetProxy(request);

                // add post data to request
                StringBuilder postString = new StringBuilder();
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_REQUEST_TYPE, System.Web.HttpUtility.UrlEncode("UPDATE"));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_AGENT, System.Web.HttpUtility.UrlEncode(Consts.AGENT_TYPE));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_AGENT_VERSION, System.Web.HttpUtility.UrlEncode(Consts.AGENT_VERSION));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_TOKEN, System.Web.HttpUtility.UrlEncode(apiKey));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_PRODUCT, System.Web.HttpUtility.UrlEncode(productName));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_PRODUCT_VERSION, System.Web.HttpUtility.UrlEncode(""));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_TIME_STAMP, System.Web.HttpUtility.UrlEncode(DateTime.Now.ToFileTime().ToString()));
                postString.Append("&");

                // Serialize to JSON
                List<AgentProjectInfo> projects = new List<AgentProjectInfo>();
                projects.Add(projectInfo);
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<AgentProjectInfo>));
                MemoryStream ms = new MemoryStream();
                serializer.WriteObject(ms, projects);
                String json = Encoding.Default.GetString(ms.ToArray());
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_DIFF, System.Web.HttpUtility.UrlEncode(json));
                Log(Level.Debug, "AgentProjectInfo JSON: " + json);
                DebugAgentProjectInfos(projects);

                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] postBytes = ascii.GetBytes(postString.ToString());
                request.ContentLength = postBytes.Length;

                Stream postStream = request.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                    }

                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        String responseString = reader.ReadToEnd();
                        Log(Level.Debug, "response: " + responseString);

                        // convert response JSON to ResultEnvelope
                        ResultEnvelope resultEnvelope;
                        MemoryStream responseMS = new MemoryStream(Encoding.Unicode.GetBytes(responseString));
                        System.Runtime.Serialization.Json.DataContractJsonSerializer responseSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ResultEnvelope));
                        resultEnvelope = responseSerializer.ReadObject(responseMS) as ResultEnvelope;
                        responseMS.Close();

                        String data = resultEnvelope.Data;
                        Log(Level.Debug, "Result data is: " + data);

                        // convert data JSON to UpdateInventoryResult
                        UpdateInventoryResult updateInventoryResult;
                        MemoryStream dataMS = new MemoryStream(Encoding.Unicode.GetBytes(data));
                        System.Runtime.Serialization.Json.DataContractJsonSerializer updateSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(UpdateInventoryResult));
                        updateInventoryResult = updateSerializer.ReadObject(dataMS) as UpdateInventoryResult;
                        dataMS.Close();

                        return updateInventoryResult;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private void SetProxy(HttpWebRequest request)
        {
            if (Proxy == null)
            {
                Log(Level.Debug, "Not using proxy");
            }
            else
            {
                String host = Proxy.Host;
                int port = Proxy.Port;
                Log(Level.Debug, "Using proxy: " + host + ":" + port);

                // check credentials
                Credential credential = Proxy.Credentials;
                if (credential == null)
                {
                    request.Proxy = new WebProxy(host, port);
                }
                else
                {
                    Uri proxyURI = new Uri(String.Format("{0}:{1}", host, port));
                    String userName = credential.UserName;
                    String password = credential.Password;
                    if (String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(password))
                    {
                        request.Proxy = new WebProxy(host, port);
                    }
                    else
                    {
                        ICredentials credentials = new NetworkCredential(userName, password);
                        request.Proxy = new WebProxy(proxyURI, true, null, credentials);
                    }
                }
            }
        }

        private void DebugAgentProjectInfos(List<AgentProjectInfo> projectInfos)
        {
            Log(Level.Debug, "|----------------- dumping projectInfos -----------------|");
            Log(Level.Debug, "Total number of projects : " + projectInfos.Count);

            foreach (AgentProjectInfo projectInfo in projectInfos)
            {
                Log(Level.Debug, "Project coordinates: " + projectInfo.Coordinates);
                Log(Level.Debug, "Project parent coordinates: " + projectInfo.ParentCoordinates);
                Log(Level.Debug, "Project project token: " + projectInfo.ProjectToken);

                List<DependencyInfo> dependencies = projectInfo.Dependencies;
                Log(Level.Debug, "total # of dependencies: " + dependencies.Count);
                foreach (DependencyInfo info in dependencies)
                {
                    Log(Level.Debug, info + " SHA-1: " + info.Sha1);
                }
            }
            Log(Level.Debug, "|-------------------- dump finished --------------------|");
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

    }
}
