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

namespace eBay.Services.Common
{
    /// <summary>
    /// Readonly constants specific to eBay SOA Services
    /// </summary>
    public abstract class ServiceConstants
    {
	    /**
	     * Prefix used by all SOA headers.
	     */
	    public static readonly string SYS_PREFIX = "X-EBAY-SOA-";

	    /**
	     * Message protocol (SOAP, etc.).
	     */
	    public static readonly string MESSAGE_PROTOCOL = SYS_PREFIX + "MESSAGE-PROTOCOL";

	    /**
	     * Service operation name.
	     */
	    public static readonly string SERVICE_OPERATION_NAME = SYS_PREFIX + "OPERATION-NAME";

	    /**
	     * Service qname.
	     */
	    public static readonly string SERVICE_NAME = SYS_PREFIX + "SERVICE-NAME";

	    /**
	     * Global ID for this request/response.
	     */
	    public static readonly string GLOBAL_ID = SYS_PREFIX + "GLOBAL-ID";

	    /**
	     * Service version in which client (in requests) or server (in responses) is operating.
	     */
	    public static readonly string VERSION = SYS_PREFIX + "SERVICE-VERSION";
    	
	    /**
	     * Security related SOA headers
	     */
	    public static readonly string AUTH_APPNAME = SYS_PREFIX + "SECURITY-APPNAME";
    	
	    /**
	     * SOA name for SOAP 1.1 protocol processor.
	     */
	    public static readonly string MSG_PROTOCOL_SOAP_11 = "SOAP11";

	    /**
	     * SOA name for SOAP 1.2 protocol processor.
	     */
	    public static readonly string MSG_PROTOCOL_SOAP_12 = "SOAP12";

       /**
	    * Service kit name for tracking purpose
	    */
	   public static readonly string SERVICE_KIT_NAME = "eBayServiceKit(DotNet)";
	
	   /**
	    * User agent http header value for tracking purpose
	    */
       public static readonly string USER_AGENT_VALUE = SERVICE_KIT_NAME;

       public static readonly string HEADER_USER_AGENT = "User-Agent";


       /**
        * Names of supported services, for tracking
        */
       public static readonly string FINDING_SERVICE_NAME = "FindingAPI";
    }
}
