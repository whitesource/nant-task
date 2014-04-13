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
using System.Collections;
using System.Collections.Generic;

using Whitesource.Agent.Api.Dispatch;
using Whitesource.Agent.Api.Model;

namespace Whitesource.Agent.Api.Dispatch
{
    /**
     * Request to update organization inventory. 
     * 
     * @author tom.shapira
     */
    public class UpdateInventoryRequest : BaseRequest
    {

        /* --- Members --- */

        protected List<AgentProjectInfo> Projects { get; set; }

        /* --- Constructors --- */

        /**
         * Default constructor
         */
        public UpdateInventoryRequest()
            : base(RequestType.UPDATE)
        {
            Projects = new List<AgentProjectInfo>();
        }

        /**
         * Constructor
         *
         * @param projects Open Source usage statement to update White Source.
         */
        public UpdateInventoryRequest(List<AgentProjectInfo> projects)
            : this()
        {
            this.Projects = projects;
        }


        /**
         * Constructor
         * 
         * @param orgToken WhiteSource organization token.
         * @param projects Open Source usage statement to update White Source.
         */
        public UpdateInventoryRequest(String orgToken, List<AgentProjectInfo> projects)
            : this(projects)
        {
            this.OrgToken = orgToken;
        }
    }

}
