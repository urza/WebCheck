Simple ConsoleApp in .net 5 that checks URLs in regular interval and optionally reports failures to file or email. It can also search response for some expected string.

Success is considered HTTP 2xx reposne with expected string in the reponse (if defined).
<br/>Failure is considered all other HTTP responses like 404, 500 etc. Or any exception from request. Or if the expected string is defined but not found.

This app has no dependecies other than .net 5 framework. Tested on Ubuntu Server 20.04 and Windows 10.
 
 ### Minimum setup:
 just run the app with urls.txt file that is in the same folder as the binary and contains urls to be checked (one per line)
 
 ## More options:
 *urls.txt*
 Each lines can contain either simply just the ULR OR "URL EXPECTED_STRING" where EXPECTED_STRING is string that must be found in the response.
 See example in https://github.com/urza/WebCheck/blob/main/WebCheck/urls.txt
 
 *webcheck.config*
 You can create webcheck.config and place it in the same directory as the binary file.
 There you can configure more options such as smtp server, internval length etc.
 Example: https://github.com/urza/WebCheck/blob/main/WebCheck/webcheck.config
