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
using System.Text;
using System.Runtime.Serialization;

using Whitesource.Agent.Api;

namespace Whitesource.Agent.Api.Dispatch
{
    [DataContract]
    public class ResultEnvelope
    {

        /* --- Static members --- */

        public const int STATUS_SUCCESS = 1;
        public const int STATUS_BAD_REQUEST = 2;
        public const int STATUS_SERVER_ERROR = 3;

        public const String MESSAGE_OK = "ok";
        public const String MESSAGE_ILLEGAL_ARGUMENTS = "Illegal arguments";
        public const String MESSAGE_SERVER_ERROR = "Server error";

        /* --- Members --- */

        private String envelopeVersion = APIConstants.API_VERSION;

        /** Status code of the operation. */
        [DataMember(Name = "status")]
        public int Status { get; set; }

        /** Human readable message. */
        [DataMember(Name = "message")]
        public String Message { get; set; }

        /** Data associated with the result */
        [DataMember(Name = "data")]
        public String Data { get; set; }

        /* --- Constructors --- */

        /**
	     * Default constructor
	     */
        public ResultEnvelope()
        {
        }

        /**
	     * Constructor
	     * 
	     * @param status
	     * @param message
	     * @param data
	     */
        public ResultEnvelope(int status, String message, String data)
        {
            this.Status = status;
            this.Message = message;
            this.Data = data;
        }

        /* --- Overridden methods --- */

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("ResultEnvelope@").Append(GetHashCode().ToString("X4")).Append("[")
            .Append("\n").Append("envelopeVersion=").Append(envelopeVersion).Append(",")
            .Append("\n").Append("status=").Append(Status).Append(",")
            .Append("\n").Append("message=").Append(Message).Append(",")
            .Append("\n").Append("data=").Append(Data)
            .Append("\n]");

            return sb.ToString();
        }
    }
}
