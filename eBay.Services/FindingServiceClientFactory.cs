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
using System.ServiceModel;
using eBay.Services.Finding;
using eBay.Services.Common;
using Slf;

namespace eBay.Services
{
    /// <summary>
    /// Factory class to get eBay Finding Service client proxy
    /// </summary>
    public class FindingServiceClientFactory
    {
        /// <summary>
        /// A simple interface to get eBay Finding Service client proxy
        /// </summary>
        /// <param name="config">Client configuration</param>
        /// <returns>eBay Finding Service client proxy of type FindingServicePortTypeClient</returns>
        public static FindingServicePortTypeClient getServiceClient(ClientConfig config)
        {
            return (FindingServicePortTypeClient)ClientFactory.GetSerivceClient<FindingServicePortType>(config, typeof(FindingServicePortTypeClient), ServiceConstants.FINDING_SERVICE_NAME);
        }
    }
}
