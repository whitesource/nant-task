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

using Whitesource.Agent;
using Whitesource.Agent.Api;
using Whitesource.Agent.Client;
using Whitesource.Agent.Api.Model;
using Whitesource.Agent.Api.Dispatch;
using System.Runtime.Serialization.Json;

namespace Whitesource.Agent.Client
{
    /**
     * Default Implementation of the interface using Apache's HttpClient.
     * 
     * @author tom.shapira
     */
    public class WssServiceClientImpl : WssServiceClient
    {

        /* --- Members --- */

        protected String serviceUrl;

        protected String proxyHost;

        protected int proxyPort;

        protected String proxyUsername;

        protected String proxyPassword;

        private bool debug;

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public WssServiceClientImpl()
            : this(ClientConstants.DEFAULT_SERVICE_URL)
        {
        }

        /**
	     * Constructor
	     * 
	     * @param serviceUrl WhiteSource service URL to use.
	     */
        public WssServiceClientImpl(String serviceUrl)
        {
            if (String.IsNullOrEmpty(serviceUrl))
            {
                this.serviceUrl = ClientConstants.DEFAULT_SERVICE_URL;
            }
            else
            {
                this.serviceUrl = serviceUrl;
            }
            debug = false;
        }

        /* --- Overridden methods --- */

        UpdateInventoryResult WssServiceClient.UpdateInventory(UpdateInventoryRequest request)
        {
            return Service(request);
        }

        CheckPoliciesResult WssServiceClient.CheckPolicies(CheckPoliciesRequest request)
        {
            return Service(request);
        }

        void WssServiceClient.SetProxy(String proxyHost, int proxyPort)
        {
            this.proxyHost = proxyHost;
            this.proxyPort = proxyPort;
        }

        void WssServiceClient.SetProxy(String proxyHost, int proxyPort, String proxyUsername, String proxyPassword)
        {
            this.proxyHost = proxyHost;
            this.proxyPort = proxyPort;
            this.proxyUsername = proxyUsername;
            this.proxyPassword = proxyPassword;
        }

        void WssServiceClient.SetDebug(bool debug)
        {
            this.debug = debug;
        }

        /* --- Private methods --- */

        /**
         * The method calls the White Source service.
         */
        private R Service<R>(BaseRequest<R> request)
        {
            try
            {
                HttpWebRequest httpRequest = WebRequest.Create(serviceUrl) as HttpWebRequest;
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Accept = "application/json";

                SetProxy(httpRequest);

                // add post data to request
                StringBuilder postString = new StringBuilder();
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_REQUEST_TYPE, System.Web.HttpUtility.UrlEncode(request.Type.ToString()));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_AGENT, System.Web.HttpUtility.UrlEncode(request.Agent));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_AGENT_VERSION, System.Web.HttpUtility.UrlEncode(request.AgentVersion));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_TOKEN, System.Web.HttpUtility.UrlEncode(request.OrgToken));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_PRODUCT, System.Web.HttpUtility.UrlEncode(request.Product));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_PRODUCT_VERSION, System.Web.HttpUtility.UrlEncode(request.ProductVersion));
                postString.Append("&");
                postString.AppendFormat("{0}={1}", APIConstants.PARAM_TIME_STAMP, System.Web.HttpUtility.UrlEncode(request.TimeStamp.ToString()));

                // Serialize projects to JSON
                switch (request.Type)
                {
                    case RequestType.UPDATE:
                        UpdateInventoryRequest updateRequest = (UpdateInventoryRequest)Convert.ChangeType(request, typeof(UpdateInventoryRequest));
                        AppendProjects(updateRequest.Projects, postString);
                        break;
                    case RequestType.CHECK_POLICIES:
                        CheckPoliciesRequest checkPoliciesRequest = (CheckPoliciesRequest)Convert.ChangeType(request, typeof(CheckPoliciesRequest));
                        AppendProjects(checkPoliciesRequest.Projects, postString);
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported request type.");
                }

                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] postBytes = ascii.GetBytes(postString.ToString());
                httpRequest.ContentLength = postBytes.Length;

                Stream postStream = httpRequest.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();

                using (HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                    }

                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        String responseString = reader.ReadToEnd();
                        Debug("response: " + responseString);

                        // convert response JSON to ResultEnvelope
                        ResultEnvelope resultEnvelope;
                        MemoryStream responseMS = new MemoryStream(Encoding.Unicode.GetBytes(responseString));
                        System.Runtime.Serialization.Json.DataContractJsonSerializer responseSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ResultEnvelope));
                        resultEnvelope = responseSerializer.ReadObject(responseMS) as ResultEnvelope;
                        responseMS.Close();

                        String data = resultEnvelope.Data;
                        Debug("Result data is: " + data);

                        // convert data JSON to result according to type
                        R result;
                        MemoryStream dataMS = new MemoryStream(Encoding.Unicode.GetBytes(data));
                        switch (request.Type)
                        {
                            case RequestType.UPDATE:
                                System.Runtime.Serialization.Json.DataContractJsonSerializer updateSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(UpdateInventoryResult));
                                result = (R)Convert.ChangeType(updateSerializer.ReadObject(dataMS) as UpdateInventoryResult, typeof(R));
                                dataMS.Close();
                                break;
                            case RequestType.CHECK_POLICIES:
                                // enable Dictionary support
                                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                                settings.UseSimpleDictionaryFormat = true;

                                System.Runtime.Serialization.Json.DataContractJsonSerializer checkPoliciesSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(CheckPoliciesResult), settings);
                                result = (R)Convert.ChangeType(checkPoliciesSerializer.ReadObject(dataMS) as CheckPoliciesResult, typeof(R));
                                dataMS.Close();
                                break;
                            default:
                                dataMS.Close();
                                throw new InvalidOperationException("Unsupported request type.");
                        }
                        dataMS.Close();

                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                Debug(e.Message);
                return default(R);
            }
        }

        /**
         * Set proxy configurations
         */
        private void SetProxy(HttpWebRequest request)
        {
            if (String.IsNullOrEmpty(proxyHost))
            {
                Debug("Not using proxy");
            }
            else
            {
                //Log(Level.Debug, "Using proxy: " + host + ":" + port);
                if (String.IsNullOrEmpty(proxyUsername) && String.IsNullOrEmpty(proxyPassword))
                {
                    request.Proxy = new WebProxy(proxyHost, proxyPort);
                }
                else
                {
                    Uri proxyURI = new Uri(String.Format("{0}:{1}", proxyHost, proxyPort));
                    ICredentials credentials = new NetworkCredential(proxyUsername, proxyPassword);
                    request.Proxy = new WebProxy(proxyURI, true, null, credentials);
                }
            }
        }

        private void AppendProjects(List<AgentProjectInfo> projects, StringBuilder postString)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<AgentProjectInfo>));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, projects);
            String json = Encoding.Default.GetString(ms.ToArray());
            postString.Append("&");
            postString.AppendFormat("{0}={1}", APIConstants.PARAM_DIFF, System.Web.HttpUtility.UrlEncode(json));

            Debug("AgentProjectInfo JSON: " + json);
            DebugAgentProjectInfos(projects);
        }

        private void DebugAgentProjectInfos(List<AgentProjectInfo> projectInfos)
        {
            Debug("|----------------- dumping projectInfos -----------------|");
            Debug("Total number of projects : " + projectInfos.Count);

            foreach (AgentProjectInfo projectInfo in projectInfos)
            {
                Debug("Project coordinates: " + projectInfo.Coordinates);
                Debug("Project parent coordinates: " + projectInfo.ParentCoordinates);
                Debug("Project project token: " + projectInfo.ProjectToken);

                List<DependencyInfo> dependencies = projectInfo.Dependencies;
                Debug("total # of dependencies: " + dependencies.Count);
                foreach (DependencyInfo info in dependencies)
                {
                    Debug("" + info + " SHA-1: " + info.Sha1);
                }
            }
            Debug("|-------------------- dump finished --------------------|");
        }

        private void Debug(String msg)
        {
            if (debug)
            {
                Console.WriteLine("[whitesource-task] " + msg);
            }
        }
    }
}
