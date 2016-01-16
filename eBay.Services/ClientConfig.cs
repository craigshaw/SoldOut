#region Copyright
//	Copyright (c) 2010 eBay, Inc.
//
//	This program is licensed under the terms of the eBay Common Development and 
//	Distribution License (CDDL) Version 1.0 (the "License") and any subsequent 
//	version thereof released by eBay.  The then-current version of the License 
//	can be found at https://www.codebase.ebay.com/Licenses.html and in the 
//	eBaySDKLicense file that is under the eBay SDK install directory.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eBay.Services
{
    /// <summary>
    /// Configurations for eBay SOA service client
    /// </summary>
    public class ClientConfig
    {
        #region Fields
        private string applicationId;
        private string serviceVersion;
        private string globalId;

        private string endPointAddress;
        private bool httpCompressionEnabled = true;
        private bool httpHeaderLoggingEnabled = true;
        private int httpTimeout = 60000;
        private bool soapMessageLoggingEnabled = true;
        #endregion


        #region Properties

        /// <summary>
        /// eBay developer account application ID (AppID),
        /// this is mandatory.
        /// </summary>
        public string ApplicationId
        {
            get { return applicationId; }
            set { applicationId = value; }
        }

        /// <summary>
        /// The service version your application want to use,
        /// If not set, the latest version will be used. 
        /// </summary>
        public string ServiceVersion
        {
            get { return serviceVersion; }
            set { serviceVersion = value; }
        }

        /// <summary>
        /// The unique identifier for a combination of site, language, and territory.
        /// For example, EBAY-US (the default) is the global ID that corresponds to the
        /// eBay US site. The Global ID you specify must correspond to an eBay site with
        /// site ID. Refer to <a href="http://developer.ebay.com/devzone/finding/Concepts/SiteIDToGlobalID.html">eBay Site ID to Global ID Mapping</a>. 
        /// In addition, <a href="http://developer.ebay.com/devzone/finding/CallRef/Enums/GlobalIdList.html">Global ID Values</a> contains a complete list of the eBay global IDs.
        /// 
        /// If not set, defaut to EBAY-US.
        /// </summary>
        public string GlobalId
        {
            get { return globalId; }
            set { globalId = value; }
        }

        /// <summary>
        /// The service endpoint(either production or sandbox) you request will be sent to,
        /// this is mandatory.
        /// </summary>
        public string EndPointAddress
        {
            get { return endPointAddress; }
            set { endPointAddress = value; }
        }

        /// <summary>
        /// Should http compression be enabled or not, ignored in current client implementation
        /// </summary>
        public bool HttpCompressionEnabled
        {
            get { return httpCompressionEnabled; }
            set { httpCompressionEnabled = value; }
        }

        /// <summary>
        /// Should http headers be logged or not,
        /// default to true
        /// </summary>
        public bool HttpHeaderLoggingEnabled
        {
            get { return httpHeaderLoggingEnabled; }
            set { httpHeaderLoggingEnabled = value; }
        }

        /// <summary>
        /// Http request timeout setting, unit Milliseconds.
        /// will take effect only if the timeout value > 0, otherwise, WCF framework default value will be used.
        /// </summary>
        public int HttpTimeout
        {
            get { return httpTimeout; }
            set { httpTimeout = value; }
        }

        /// <summary>
        /// Should soap message to blooged or not,
        /// default to true
        /// </summary>
        public bool SoapMessageLoggingEnabled
        {
            get { return soapMessageLoggingEnabled; }
            set { soapMessageLoggingEnabled = value; }
        }

        #endregion
    }
}
