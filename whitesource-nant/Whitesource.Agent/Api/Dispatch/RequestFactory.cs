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

using Whitesource.Agent.Api.Model;

namespace Whitesource.Agent.Api.Dispatch
{
    /**
     * Factory for constructing requests.
     *
     * @author tom.shapira
     */
    public class RequestFactory
    {

        /* --- Members --- */

        private String agent;

        private String agentVersion;

        /* --- Constructors --- */

        /**
         * Constructor
         *
         * @param agent Agent type identifier.
         * @param agentVersion Agent version.
         */
        public RequestFactory(String agent, String agentVersion)
        {
            this.agent = agent;
            this.agentVersion = agentVersion;
        }

        /* --- Public methods --- */

        /**
         * Create new Inventory Update request.
         *
         * @param orgToken WhiteSource organization token.
         * @param projects Projects status statement to update.
         * @return Newly created request to update organization inventory.
         */
        public UpdateInventoryRequest NewUpdateInventoryRequest(String orgToken, List<AgentProjectInfo> projects)
        {
            return NewUpdateInventoryRequest(orgToken, null, null, projects);
        }

        /**
         * Create new Inventory Update request.
         *
         * @param orgToken WhiteSource organization token.
         * @param projects Projects status statement to update.
         * @param product Name or WhiteSource service token of the product to update.
         * @param productVersion Version of the product to update.
         * @return Newly created request to update organization inventory.
         */
        public UpdateInventoryRequest NewUpdateInventoryRequest(String orgToken, String product, String productVersion, List<AgentProjectInfo> projects)
        {
            return (UpdateInventoryRequest)PrepareRequest(new UpdateInventoryRequest(projects), orgToken, product, productVersion);
        }

        /**
         * Create new Check policies request.
         *
         * @param orgToken WhiteSource organization token.
         * @param projects Projects status statement to check.
         * @return Newly created request to check policies application.
         */
        public CheckPoliciesRequest NewCheckPoliciesRequest(String orgToken, List<AgentProjectInfo> projects)
        {
            return NewCheckPoliciesRequest(orgToken, projects);
        }

        /**
         * Create new Check policies request.
         *
         * @param orgToken WhiteSource organization token.
         * @param projects Projects status statement to check.
         * @param product Name or WhiteSource service token of the product whose policies to check.
         * @param productVersion Version of the product whose policies to check.
         * @return Newly created request to check policies application.
         */
        public CheckPoliciesRequest NewCheckPoliciesRequest(String orgToken, String product, String productVersion, List<AgentProjectInfo> projects)
        {
            return (CheckPoliciesRequest)PrepareRequest(new CheckPoliciesRequest(projects), orgToken, product, productVersion);
        }

        /* --- Protected methods --- */

        protected BaseRequest<R> PrepareRequest<R>(BaseRequest<R> request, String orgToken, String product, String productVersion)
        {
            request.Agent = agent;
            request.AgentVersion = agentVersion;
            request.OrgToken = orgToken;
            request.Product = product;
            request.ProductVersion = productVersion;

            return request;
        }

    }
}
