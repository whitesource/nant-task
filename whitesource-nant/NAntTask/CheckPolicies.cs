using System;
using System.Collections.Generic;
using System.Text;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Util;

namespace Whitesource.NAnt.Tasks
{
    
    /**
     * Nested element in the White Source NAnt task, which checks for policies and generates a report.
     * 
     * @author tom.shapira
     */
    [ElementName("checkpolicies")]
    public class CheckPolicies : DataTypeBase
    {
        /**
	     * Output directory for White Source generated report file.
	     */
        private String _reportDir;

        [TaskAttribute("reportdir", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public String ReportDir
        {
            get { return _reportDir; }
            set { _reportDir = value; }
        }

        /**
         * Whether or not to fail the build if policy rejects a library.
         */
        private bool _failOnRejection;

        [TaskAttribute("failonrejection", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public bool FailOnRejection
        {
            get { return _failOnRejection; }
            set { _failOnRejection = value; }
        }

        /**
         * Updates organization inventory regardless of policy violations
         */
        private bool _forceUpdate;

        [TaskAttribute("forceupdate", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public bool ForceUpdate
        {
            get { return _forceUpdate; }
            set { _forceUpdate = value;  }
        }


        /* --- Constructors --- */

        public CheckPolicies()
        {
            _failOnRejection = true;
        }

    }
}
