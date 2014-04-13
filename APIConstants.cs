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

namespace Whitesource.Agent.Api
{

    /**
     * A class to hold constant values used in WhiteSource agents.
     * 
     * @author tom.shapira
     */
    static class APIConstants
    {

        public const String API_VERSION = "1.0";

        /* --- Parameters --- */

        public const String PARAM_TOKEN = "token";
        public const String PARAM_PRODUCT = "product";
        public const String PARAM_PRODUCT_VERSION = "productVersion";
        public const String PARAM_DIFF = "diff";
        public const String PARAM_DEPENDENCIES = "dependencies";
        public const String PARAM_AGENT = "agent";
        public const String PARAM_AGENT_VERSION = "agentVersion";
        public const String PARAM_REQUEST_TYPE = "type";
        public const String PARAM_TIME_STAMP = "timeStamp";

        /* --- Messages --- */

        public const String TOKEN_INVALID = "Invalid token";
        public const String TIME_STAMP_INVALID = "Invalid request time";
        public const String DIFF_INVALID = "Invalid diff";
        public const String UPDATE_SUCCESS = "update success";
        public const String JSON_ERROR = "Problem parsing json";

        /* --- Miscellaneous --- */

        public const int HASH_CODE_SEED = 133;
        public const int HASH_CODE_FACTOR = 23;

    }

}