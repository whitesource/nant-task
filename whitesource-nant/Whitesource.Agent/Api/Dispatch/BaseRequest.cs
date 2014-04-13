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

namespace Whitesource.Agent.Api.Dispatch
{

    /**
     * Base, abstract, implementation of the interface.
     * 
     * @author tom.shapira
     */
    public abstract class BaseRequest
    {

        /* --- Members --- */

        protected RequestType type { get; set; }

        protected String agent { get; set; }

        protected String agentVersion { get; set; }

        protected String orgToken { get; set; }

        protected String product { get; set; }

        protected String productVersion { get; set; }

        protected long timeStamp { get; set; }

        /* --- Constructors --- */

        /**
	     * Constructor
	     * 
	     * @param type Request operation type.
	     */
        public BaseRequest(RequestType type)
            : this(type, null, null)
        {
        }

        /**
	     * Constructor
	     * 
	     * @param type Request operation type.
	     * @param agent Agent type identifier.
	     */
        public BaseRequest(RequestType type, String agent)
            : this(type, agent, null)
        {
        }

        /**
	     * Constructor
	     * 
	     * @param type Request operation type.
	     * @param agent Agent type identifier.
	     * @param agentVersion Agent version.
	     */
        public BaseRequest(RequestType type, String agent, String agentVersion)
        {
            this.type = type;
            this.agent = agent;
            this.agentVersion = agentVersion;

            // get current time in ms
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            long timeStamp = (long)ts.TotalMilliseconds;
        }
    }
}