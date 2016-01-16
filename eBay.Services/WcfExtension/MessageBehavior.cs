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
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using eBay.Services.Common;

namespace eBay.Services.WcfExtension
{
    /// <summary>
    /// Custom endpoint behavior
    /// </summary>
    class MessageBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members

        private ClientConfig clientConfig;
        private string serviceName;

        public MessageBehavior(ClientConfig config, string serviceName)
        {
            this.clientConfig = config;
            this.serviceName = serviceName;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Enable custom message inspector
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            MessageInspector inspector = new MessageInspector(this.clientConfig, serviceName);
            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            throw new Exception("Behavior not supported on the consumer side!");
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}
