
* [Index](0000-Index.md)
* [The APIs](6000-APIs.md)
* [NetworkRootApi](6100-NetworkRootApi.md)
* Next: [NetworkRootApi: APIs list](6102-NetworkRootApi-APIs.md)


NetworkRootApi: API keys and making requests
================================================


USAGE OF THIS API MAY CAUSE SECURITY RISKS  
READ AND *UNDERSTAND* the [Network ROOT API documentation](6100-NetworkRootApi.ms) before you compromise your systems.


SDKs
-----------

We can provide you base code for C#. If you write a SDK for our API, we would be glad to add a link in this list for others to use.

* C# API client is coming...
* There are example C# code in the `NetworkRootApiClient` class of the `Sparkle.ApiClients` project.


API keys
--------------

API keys are managed by-network. Open the main website of any network with an admin account. Navigate to /Dashboard/ApiKeys. Create a key.

You should create as many keys as necessary. Imagine a key is compromised, you want to disable it and create a new one. You will then replace the old key by the new one in some applications. It's best if few applications have to be changed.

**DO NOT EVER SAVE THE SECRET KEY IN CLIENT APPLICATIONS.**  
IT WOULD COMPROMISE THE SECRET KEY AND ANYONE WOULD BE ABLE TO MAKE ADMINISTRATIVE CALLS.  
AS A REMINDER, THIS API IS DESIGNED FOR SERVER-TO-SERVER CALLS WHERE THE SECRET KEY CAN BE KEPT SECRET. 


Make API calls
-------------------------

Here are the basic elements of the request.

    HTTP method HTTP query HTTP/1.1                                    // standard http line
    X-SparkleNetworksApi-NetworkName:       network name               // REQUIRED if NetworkDomainName is not set
    X-SparkleNetworksApi-NetworkDomainName: network domain name        // REQUIRED if NetworkName is not set
    X-SparkleNetworksApi-Key:               API Key (AK)               // REQUIRED
    X-SparkleNetworksApi-Identity:          Identity Key (IK)          // optional: authenticated calls
    X-SparkleNetworksApi-Time:              Time (format: yyyyMMdd'T'HHmmssffff'Z') // REQUIRED
    X-SparkleNetworksApi-Hash:              Hash                       // REQUIRED message signature
    X-SparkleNetworksApi-ClientRequestId:   ClientRequestId            // optional: logging correlation id
    Accept:          desired response content-type                     // REQUIRED
    Accept-Language: culture                                           // optional
    Content-Type:    application/json                                  // only when posting contents
    
    HTTP content


### Signing requests

You need to get an API key and the associated secret key.

To sign requests, you have to build a string from a few variables, hash it using SHA256 and send the hash in a header.

You should have received the following variables: *Application Key (AK)*, *Application Secret (AS)*,

When making user-authenticated calls, you will also have: *Identity Key (IK, aka user access token)*, *Identity Secret (IS)*. This API does not support it.

You must not reveal any secret value (*AS*, *IS*). If you are building a client app, ensure you hide the secret values as well as you can.

The Time variable is the current **UTC time** with the format `yyyyMMdd'T'HHmmssffff'Z'`. Exemple: 20160519T0633381785Z. If your device is not able to get the right time, you can call `/api/Util/Time` to get the server time.

Here is how you build the string (UTF 8) to be hashed (pre-hash) for the *Hash* variable.

    pre-hash = Application Key (AK) +    "\n"
             + Application Secret (AS) + "\n"
             + Identity Key (IK) +       "\n"   // might be empty
             + Identity Secret (IS) +    "\n"   // might be empty
             + UPPER(HTTP method) +      "\n"
             + HTTP query +              "\n"
             + HTTP content +            "\n"   // might be empty
             + Time                             // format: `yyyyMMdd'T'HHmmssffff'Z'`

Using SHA256, you can hash the `pre-hash` and use the HEX representation. Prefix the `hash` string value with the current protocol hint `$1$`.

    Hash = "$1$" + BytesToHex( SHA256( UTF8_StringToBytes( pre-hash ) ) )


### Request object & content types

POST methods often requires you to send a formatted message. You need to specify the message format using the `Content-Type` header. Supported values are `application/json`. You should not specify this header when you are not posting content.

    Content-Type: application/json; charset=utf-8

The default output format is JSON. You should always specify the desired output format using the `Accept` header.

    Accept: application/json


### Example

Here is an example for the api/Util/Ping method. Here are all our base variables.

    API Key (AK)         = "ak_123456789"
    API Secret (AS)      = "as_456789123"
    Identity Key (CK)    = "ik_852741963"
    Identity Secret (CS) = "is_789456132"
    HTTP method          = "GET"
    HTTP query           = "/api/Util/Ping"
    HTTP content         =  empty
    Time                 = "20150201T1444230000Z"

Let's compute the pre-hash and Hash. You may try this precise example to verify you do the hash right.

    pre-hash = "ak_123456789" + "\n"
             + "as_456789123" + "\n"
             + "ik_852741963" + "\n"
             + "is_789456132" + "\n"
             + "GET" + "\n"
             + "/api/Util/Ping" + "\n"  // don't forget the starting slash!
             + "" + "\n"                // note the empty string. A GET request does not have a body.
             + "20150201T1444230000Z"   // make sure your machine has a valid time.

    Hash = "$1$" + BytesToHex ( SHA256 ( UTF8_StringToBytes ( pre-hash ) ) )
    // $1$A240F863D8CA367C1724C3788560F489797E7E894B3A9F89192243C7E2CC2CA2
	
You need to find the recommended code in your framework that will replace the pseudo-methods. 

* `UTF8_StringToBytes`
  * C#: `Encoding.UTF8.GetBytes(string)` [[1](https://msdn.microsoft.com/en-us/library/ds4kkd55(v=vs.110).aspx)]
  * PHP: `unpack('H*', $preHash)` [[1](http://php.net/manual/en/function.unpack.php),[2](http://php.net/manual/en/function.pack.php)]
  * Java: `byte[] bytes = preHash.getBytes('UTF-8')` [[1](http://docs.oracle.com/javase/7/docs/api/java/lang/String.html#getBytes())]
* `SHA256`. 
  * C#: `Encoding.UTF8.GetBytes(string)` [[1](https://msdn.microsoft.com/en-us/library/ds4kkd55(v=vs.110).aspx)]
  * Java: `MessageDigest md = MessageDigest.getInstance("SHA-256"); md.update(bytes); byte[] hash = md.digest();` [[1](http://docs.oracle.com/javase/8/docs/api/java/security/MessageDigest.html)]
* `BytesToHex`
  * C#: `string hexHash = BitConverter.ToString(hash).Replace("-", "")` [[1](https://msdn.microsoft.com/en-us/library/3a733s97%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396)]
  * Java: `String hexHash = javax.xml.bind.DatatypeConverter.printHexBinary(hash);` [[1](http://docs.oracle.com/javase/6/docs/api/javax/xml/bind/DatatypeConverter.html)]

Note: some languages have built-in functions that will do these 3 steps using only 1 or 2 methods (JS, PHP).

Finally, here is the HTTP query

    GET /api/Util/Ping HTTP/1.1
    Accept: application/json
    X-SparkleNetworksApi-NetworkName: network name
    X-SparkleNetworksApi-Key: ak_123456789   
    X-SparkleNetworksApi-Identity: ik_852741963
    X-SparkleNetworksApi-Time: 20150201T1444230000Z
    X-SparkleNetworksApi-Hash: $1$A240F863D8CA367C1724C3788560F489797E7E894B3A9F89192243C7E2CC2CA2


Error messages
---------------------

If something is set wrong in your request the server will return errors messages.

Code 	|HTTP code 	|Resolution  
--------|-----------|----------------------
InvalidNetworkSpecification 	|401 	|Verify the specified header is set.  
MissingApplicationKey 	|401 	|Verify the specified header is set.  
MissingTime 	|401 	|Verify the specified header is set.  
MissingHash 	|401 	|Verify the specified header is set.  
UnknownApplicationKey 	|401 	|The application key is not valid.  
UnknownIdentityKey 	|401 	|The identity key is not valid.  
DisabledIdentityKey 	|401 	|The provided identity has expired or has been disabled. You should renew the identity with the user.  
InvalidTime 	|401 	|Verify the time is formated correctly and your machine is set to a valid time.  
InvalidHash 	|401 	|The hash differs from the server's computed hash.  
ApplicationKeyIsMissingPermission 	|401 	|You consumer key does not have the permission to make this call.  
Unsupported Media Type 	|415 	|Verify the Content-Type header is set.


General notes
---------------------

### Dates

Dates must be given in the ISO 8601 format. Preferably in the UTC (Z) time zone.

    C#:     DateTime value = DateTime.ParseExact("2016-10-04T12:23:34.1546Z", "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    C#:     https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Roundtrip
    PHP:    $d = new DateTime('2011-01-01T15:03:01.012345Z');
    PHP:    http://php.net/manual/en/class.datetime.php
    Note:   https://xkcd.com/1179/

### Endpoint paths

An endpoint is based on the following templates: 

* `/NetworkRootApi/{Service class}/{Action}`
* `/NetworkRootApi/{Service class}/{Action}/{id}`
* `/NetworkRootApi/{Service class}/{Action}?[Param1=Value]+`

Service class is a name that designated the kind of object you want to target. Some names: `Companies`, `CompanyCategories`, `Users`, `InformationNotes`, `Groups`, `TagCategories`, `TagDefinitions`... 

There is always an action name after the service class name. Here are some repeatable patterns.

* `GET /NetworkRootApi/{Service class}/ById/{id}` Fetch an item using its ID.
* `GET /NetworkRootApi/{Service class}/All` Returns all the items, reserved for small tables.
* `GET /NetworkRootApi/{Service class}/List?Offset=0&Count=100&KnownFilter=All` Returns a paginated list of items.
    * `Offset` is the pagination offset. Default is zero, increment using the Count parameter.
    * `Count` is the pagination size. The default size is optimized on a per-service basis. Avoid asking too many at a time.
    * `KnownFilter` is the name of a popular filter that can be used.
* `GET /NetworkRootApi/{Service class}/PrepareEdit` Returns the form data to create an item.
* `GET /NetworkRootApi/{Service class}/PrepareEdit/{id}` Returns the form data to edit an item.
* `POST /NetworkRootApi/{Service class}/Edit` Create or edit an item. See the Request-Result pattern.

### Request-Result pattern

Endpoints that allow changes in the database follow the Request-Result pattern.

There is usually a `Prepare` method to get the editable form. When editing an object, you receive the editable parts. When a precompletion list is required in the form, you receive the available values.

The actual method needs a request object as a body and a result object is returned within the usual response wrapper.

A typical request:

    {
      "Id": null,
      "Name": "New information note!",
      "Description": "Lorem ipsum dolor sit amet, ... lectus nam.",
      "ActingUserId": 6
    }

Note the `ActingUserId` that is often required for traceability.

Typical results:

    {
      "Data": {
        "Item": {
          "Id": 5,
          "Name": "New information note!",
          "Description": "Lorem ipsum dolor sit amet, ... lectus nam.",
          "NetworkId": 1,
          "UserId": 6,
          "IsDeleted": false
        },
        "Succeed": true,
        "Errors": []
      }
    }

    {
      "Data": {
        "Succeed": false,
        "Errors": [
          {
            "Code": "NoSuchActingUser",
            "DisplayMessage": "The acting user is not defined. Are you authenticated?",
            "Detail": "The user id 1045 does not match any user."
          }
        ],
        "Request": { }
      }
    }


### Errors

Validation errors (HTTP 404) are returned when the fields of a form do not pass validation. You can detect this kind of error with the property `ErrorCode = "InvalidMessage"`.  The `ModelState` property contains the validation errors.

    {
      "Data": null,
      "ErrorCode": "InvalidMessage",
      "ModelState": {
        "Description": [
          "This must contain less than 4000 characters"
        ]
      },
      "ErrorHelp": "Validation errors occured, check the ModelState property. "
    }

Domain errors (HTTP 400) are returned when the action cannot be performed because of a failing domain rule. The object in that `Data` property is a base domain result. The `Succeed` boolean allows you a quick check for success. The `Errors` collection will help you display error messages to the user. Note that the original `Request` object is given back.

    {
      "Data": {
        "Succeed": false,
        "Errors": [
          {
            "Code": "NoSuchActingUser",
            "DisplayMessage": "The acting user is not defined. Are you authenticated?",
            "Detail": "The user id 1045 does not match any user."
          }
        ],
        "Request": { }
      }
    }

Internal server errors (HTTP 500) look like that. The `Message` can be displayed to the user. The `ExceptionMessage`, `ExceptionType` and `StackTrace` properties are for developers.

    {
      "Message": "An error has occurred.",
      "ExceptionMessage": "Could not load file or assembly 'System.Web.Http.WebHost, Version=5.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference. (Exception from HRESULT: 0x80131040)",
      "ExceptionType": "System.IO.FileLoadException",
      "StackTrace": "   at Sparkle.WebBase.WebApi.GlobalApiExceptionFilterAttribute.OnException(HttpActionExecutedContext context)\r\n   at System.Web.Http.Filters.ExceptionFilterAttribute.OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken) .........."
    }





