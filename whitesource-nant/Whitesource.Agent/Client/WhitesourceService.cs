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
using System.Text;

using Whitesource.Agent.Client;
using Whitesource.Agent.Api.Dispatch;
using Whitesource.Agent.Api.Model;

namespace Whitesource.Agent.Client
{
    /**
     * A facade to the communication layer with the White Source service.
     *
     * @author tom.shapira
     */
    public class WhitesourceService
    {

        /* --- Members --- */

        private WssServiceClient client;

        private RequestFactory requestFactory;

        /* --- Constructors --- */

        public WhitesourceService()
            : this("generic", "1.0")
        {
        }

        public WhitesourceService(String agent, String agentVersion)
            : this(agent, agentVersion, null)
        {
        }

        public WhitesourceService(String agent, String agentVersion, String serviceUrl)
        {
            requestFactory = new RequestFactory(agent, agentVersion);

            String url = serviceUrl;
            if (url == null || url.Trim().Length == 0)
            {
                url = ClientConstants.DEFAULT_SERVICE_URL;
            }
            client = new WssServiceClientImpl(url);
        }

        public WhitesourceService(String agent, String agentVersion, String serviceUrl, String proxyHost, int proxyPort)
            : this(agent, agentVersion, serviceUrl)
        {
            client.SetProxy(proxyHost, proxyPort);
        }

        public WhitesourceService(String agent, String agentVersion, String serviceUrl, String proxyHost, int proxyPort, String proxyUsername, String proxyPassword)
            : this(agent, agentVersion, serviceUrl)
        {
            client.SetProxy(proxyHost, proxyPort, proxyUsername, proxyPassword);
        }

        /* --- Public methods --- */

        /**
         * The method update the White Source organization account with the given OSS information.
         *
         * @param orgToken     Organization token uniquely identifying the account at white source..
         * @param projectInfos OSS usage information to send to white source.
         * @return Result of updating white source.
         * @throws WssServiceException In case of errors while updating white source.
         */
        public UpdateInventoryResult Update(String orgToken, String product, String productVersion, List<AgentProjectInfo> projectInfos)
        {
            return client.UpdateInventory(
                    requestFactory.NewUpdateInventoryRequest(orgToken, product, productVersion, projectInfos));
        }

        /**
         * The method check the policies application of the given OSS information.
         *
         * @param orgToken     Orgnization token uniquely identifying the account at white source..
         * @param projectInfos OSS usage information to send to white source.
         * @return Potential result of applying the currently defined policies.
         * @throws WssServiceException In case of errors while checking the policies with white source.
         */
        public CheckPoliciesResult CheckPolicies(String orgToken, String product, String productVersion, List<AgentProjectInfo> projectInfos)
        {
            return client.CheckPolicies(
                    requestFactory.NewCheckPoliciesRequest(orgToken, product, productVersion, projectInfos));
        }

    }
}
