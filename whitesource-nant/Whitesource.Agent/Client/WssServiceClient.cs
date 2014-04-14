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

using Whitesource.Agent.Api.Dispatch;

namespace Whitesource.Agent.Client
{
    /**
     * The interface describes the functionality to be exposed by a client implementation to the White Source agent service.
     *
     * @author tom.shapira
     */
    public interface WssServiceClient
    {
        /**
         * The method calls the White Source service for inventory update.
         *
         * @param request Inventory update request.
         * @return Inventory update result.
         * @throws WssServiceException In case an error occurred during the call to White Source server.
         */
        UpdateInventoryResult UpdateInventory(UpdateInventoryRequest request);

        /**
         * The method call the White Source service for checking policies application.
         * @param request Check Policies request.
         * @return Check Policies result.
         * @throws WssServiceException In case an error occurred during the call to White Source server.
         */
        CheckPoliciesResult CheckPolicies(CheckPoliciesRequest request);

        void SetProxy(String proxyHost, int proxyPort);

        void SetProxy(String proxyHost, int proxyPort, String proxyUsername, String proxyPassword);
    }
}
