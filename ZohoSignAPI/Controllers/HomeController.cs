using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZCRMSDK.CRM.Library.Setup.RestClient;
using ZCRMSDK.OAuth.Client;
using ZCRMSDK.OAuth.Contract;
using ZohoSignAPI.Controllers;
using ZohoSignAPI.Models;

namespace ZohoSignAPI.Controllers
{

    public class HomeController : Controller
    {
        public async Task<IActionResult> SelfSign()
        {
            string postContent = "scope=ZohoSign.documents.all";
            //postContent = postContent + "&authtoken=Your AUTHTOKEN";//Give your authtoken
            string grantToken = "1000.7a003ea13ecb9810c2965cd54a70d946.25cae430b8c7c1c5f3e86ff8b9ec7ca6";
            string accessToken = "1000.26c384a6cc2a15c74f001cb505a5dbc2.2b8a873076fd5dc6d5038cdd6f279320";




            #region Send Document for Signature
            #region GetFieldTypes

            using (var httpClient = new HttpClient())
            {
                using (var requestCreateDocument = new HttpRequestMessage(new HttpMethod("GET"), "https://sign.zoho.com/api/v1/fieldtypes"))
                {

                    requestCreateDocument.Headers.Add("Authorization", "Zoho-oauthtoken " + accessToken);

                    HttpResponseMessage responseresult = httpClient.SendAsync(requestCreateDocument).Result;
                    var result = responseresult.Content.ReadAsStringAsync().Result.ToString();
                    JObject o = JObject.Parse(result);
                    var jsonData = JsonConvert.DeserializeObject<Fields>(result);
                    foreach (var item in jsonData.field_types)
                    {





                    }
                }
            }
            #endregion

            using (var httpClient = new HttpClient())
            {
                CreateDocumentModel jsonData = null;

                using (var requestCreateDocument = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/requests"))
                {

                    //Create New Document 
                    requestCreateDocument.Headers.Add("Authorization", "Zoho-oauthtoken " + accessToken);
                    var multipartContent = new MultipartFormDataContent();

                    multipartContent.Add(new ByteArrayContent(System.IO.File.ReadAllBytes("VOC1.png")), "file", Path.GetFileName("VOC1.png"));
                    multipartContent.Add(new StringContent("{'requests': {'request_name':'NDA ','self_sign':true}}"), "data");
                    requestCreateDocument.Content = multipartContent;
                    HttpResponseMessage responseresult = httpClient.SendAsync(requestCreateDocument).Result;
                    var result = responseresult.Content.ReadAsStringAsync().Result;
                    jsonData = JsonConvert.DeserializeObject<CreateDocumentModel>(result);


                }
                //Self Sign Document
                if (jsonData != null && jsonData.status == "success")
                {
                    string requestId = jsonData.requests.request_id;
                    string documentId = jsonData.requests.document_ids[0].document_id;
                    string actionId = jsonData.requests.actions[0].action_id;
                    string recipient_email = jsonData.requests.actions[0].recipient_email;
                    string recipient_name = jsonData.requests.actions[0].recipient_name;
                    string requestTypeId = jsonData.requests.request_type_id;

                    using (var httpClientSendSign = new HttpClient())
                    {
                        using (var requestSendSign = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/requests/" + requestId + "/submit"))
                        {
                            //var jsonContent = "data={'requests':{'actions':[{'action_id':'" + actionId + "','verify_recipient':false,'action_type':'SIGN','private_notes':'Test document','signing_order':0,'recipient_email':'" + recipient_email + "','recipient_name':'" + recipient_name + "','fields':{'image_fields':[{'action_id': '" + actionId + "','field_type_name':'Signature','field_category':'image','field_label':'Signature','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Signature','y_coord':3,'abs_width':22,'description_tooltip':'','x_coord':42,'abs_height':2},{'action_id': '" + actionId + "','field_type_name':'Initial','field_category':'image','field_label':'Initial','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Initial', 'y_coord':3, 'abs_width':18, 'description_tooltip':'', 'x_coord':71, 'abs_height':5 } ] }, 'recipient_phonenumber':'9921799551', 'recipient_countrycode':'91' } ], 'self_sign':true, 'request_name':'Leave a note test', 'expiration_days':10, 'is_sequential':true, 'notes':'Test Document' }}";
                            var jsonContent = "data={'requests':{'actions':[{'action_id':'" + actionId + "','verify_recipient':false,'action_type':'SIGN','private_notes':'Test document','signing_order':0,'recipient_email':'" + recipient_email + "','recipient_name':'" + recipient_name + "','fields':{'image_fields':[{'action_id': '" + actionId + "','field_type_name':'Signature','field_category':'image','field_label':'Signature','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Signature','y_coord':3,'abs_width':382,'description_tooltip':'TEst Tooltip','x_coord':42,'abs_height':80},{'action_id': '" + actionId + "','field_type_name':'Initial','field_category':'image','field_label':'Initial','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Initial', 'y_coord':80, 'abs_width':280, 'description_tooltip':'', 'x_coord':71, 'abs_height':50 } ] }, 'recipient_phonenumber':'9921799551', 'recipient_countrycode':'91' } ], 'self_sign':true, 'request_name':'Leave a note test', 'expiration_days':10, 'is_sequential':true, 'notes':'Test Document' }}";
                            //var jsonContent =                                "data={'requests':{'actions':[{'verify_recipient':false,'action_type':'SIGN','private_notes':'Test document','signing_order':0,'recipient_email':'" + recipient_email + "','recipient_name':'" + recipient_name + "','fields':{'image_fields':[{'action_id': '" + actionId + "',  'field_type_id':'77354000000000141','field_type_name':'Signature','field_category':'image','field_label':'Signature','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Signature','y_coord':3,'abs_width':22,'description_tooltip':'','x_coord':42,'abs_height':2},{'action_id': '" + actionId + "','field_type_id':'77354000000000143','field_type_name':'Initial','field_category':'image','field_label':'Initial','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Initial', 'y_coord':3, 'abs_width':18, 'description_tooltip':'', 'x_coord':71, 'abs_height':5 } ] }, 'recipient_phonenumber':'9921799551', 'recipient_countrycode':'91' } ], 'self_sign':true, 'request_name':'Leave a note test', 'expiration_days':10, 'is_sequential':true, 'notes':'Test Document' }}";
                            requestSendSign.Headers.TryAddWithoutValidation("Authorization", "Zoho-oauthtoken " + accessToken);
                            requestSendSign.Content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                            HttpResponseMessage responseSendSign = httpClientSendSign.SendAsync(requestSendSign).Result;
                            var resultSendSign = responseSendSign.Content.ReadAsStringAsync().Result;
                        }
                    }

                    #region Complete Sign Process 


                    //using (var httpClientSendSign = new HttpClient())
                    //{
                    //    using (var requestSendSign = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/requests/" + requestId + "/sign"))
                    //    {
                    //        var jsonContent = "data={'requests':{'actions':[{'action_id':'" + actionId + "','verify_recipient':false,'action_type':'SIGN','private_notes':'Test document','signing_order':0,'recipient_email':'" + recipient_email + "','recipient_name':'" + recipient_name + "','fields':{'image_fields':[{'action_id': '" + actionId + "','field_type_name':'Signature','field_category':'image','field_label':'Signature','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Signature','y_coord':3,'abs_width':382,'description_tooltip':'TEst Tooltip','x_coord':42,'abs_height':80},{'action_id': '" + actionId + "','field_type_name':'Initial','field_category':'image','field_label':'Initial','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Initial', 'y_coord':80, 'abs_width':280, 'description_tooltip':'', 'x_coord':71, 'abs_height':50 } ] }, 'recipient_phonenumber':'9921799551', 'recipient_countrycode':'91' } ], 'request_name':'Leave a note test', 'expiration_days':10, 'is_sequential':true, 'notes':'Test Document' }}";
                    //        requestSendSign.Headers.TryAddWithoutValidation("Authorization", "Zoho-oauthtoken " + accessToken);
                    //        requestSendSign.Content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                    //        HttpResponseMessage responseSendSign = httpClientSendSign.SendAsync(requestSendSign).Result;
                    //        var resultSendSign = responseSendSign.Content.ReadAsStringAsync().Result;
                    //    }
                    //}
                    #endregion



                    // SEND document for signature
                    recipient_email = "ashok.kasar@ayanworks.com";
                    recipient_name = "ashok kasar";
                    using (var httpClientSendSign = new HttpClient())
                    {
                        using (var requestSendSign = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/requests/" + requestId))
                        {

                            var jsonContent = "data={'requests':{'actions':[{'action_id':'" + actionId + "','verify_recipient':false,'action_type':'SIGN','private_notes':'Test document','signing_order':0,'recipient_email':'" + recipient_email + "','recipient_name':'" + recipient_name + "','fields':{'image_fields':[{'action_id': '" + actionId + "','field_type_name':'Signature','field_category':'image','field_label':'Signature','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Signature','y_coord':3,'abs_width':382,'description_tooltip':'TEst Tooltip','x_coord':42,'abs_height':80},{'action_id': '" + actionId + "','field_type_name':'Initial','field_category':'image','field_label':'Initial','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Initial', 'y_coord':80, 'abs_width':280, 'description_tooltip':'', 'x_coord':71, 'abs_height':50 } ] }, 'recipient_phonenumber':'9921799551', 'recipient_countrycode':'91' } ],'request_type_id' : '" + requestTypeId + "',  'request_name':'Leave a note test', 'expiration_days':10, 'is_sequential':true, 'notes':'Test Document' }}";

                            requestSendSign.Headers.TryAddWithoutValidation("Authorization", "Zoho-oauthtoken " + accessToken);
                            requestSendSign.Content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                            HttpResponseMessage responseSendSign = httpClientSendSign.SendAsync(requestSendSign).Result;
                            var resultSendSign = responseSendSign.Content.ReadAsStringAsync().Result;
                        }
                    }


                }

            }

            #endregion

            //Refresh Token "1000.b967ca4b8f33c8686f3d111f4b720b84.324087ac6af5bd434a94cbb313964899"

            #region Call to Create Document API

            using (var httpClient = new HttpClient())
            {
                using (var requestCreateDocument = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/requests"))
                {

                    requestCreateDocument.Headers.Add("Authorization", "Zoho-oauthtoken " + "1000.8087fe29ecbb87c059646081a20ed9f3.c15376b169c0038352600c6ecd52c731");
                    var multipartContent = new MultipartFormDataContent();

                    multipartContent.Add(new ByteArrayContent(System.IO.File.ReadAllBytes("VOC1.png")), "file", Path.GetFileName("VOC1.png"));
                    // working multipartContent.Add(new StringContent("{\"requests\": {\"request_name\":\"NDA \",\"self_sign\":true}}"), "data");
                    multipartContent.Add(new StringContent("{'requests': {'request_name':'NDA ','self_sign':true}}"), "data");
                    //         multipartContent.Add(new StringContent("{\"requests\": {\"request_name\":\"NDA \",\"self_sign\":true }"), "data");
                    requestCreateDocument.Content = multipartContent;

                    //var responseResult = await httpClient.SendAsync(requestCreateDocument);
                    HttpResponseMessage responseresult = httpClient.SendAsync(requestCreateDocument).Result;
                    var result = responseresult.Content.ReadAsStringAsync().Result;
                    var jsonData = JsonConvert.DeserializeObject(result);
                }
            }

            #endregion

            #region Generate Grant Token

            //WebRequest requestGenGrantToken = WebRequest.Create("https://accounts.zoho.com/oauth/v2/auth?scope=ZohoSign.documents.all&client_id=1000.522HB3WI52Q679002L4O6H06FMHUCH&state=testing&response_type=code&redirect_uri=https://www.zohoapis.com/crm/v2/&access_type=offline");
            //requestGenGrantToken.Method = "GET";
            //WebResponse responseGenGrantToken = requestGenGrantToken.GetResponse();
            //var responseTextGenGrantToken = new StreamReader(responseGenGrantToken.GetResponseStream()).ReadToEnd();
            ////var resultGenGrantToken = JsonConvert.DeserializeObject(responseTextGenGrantToken);

            #endregion



            return View();

            #region Generate Access Token and Refresh Token 


            WebRequest request = WebRequest.Create("https://accounts.zoho.com/oauth/v2/token?code=" + grantToken + "&redirect_uri=https://www.zohoapis.com/crm/v2/&client_id=1000.522HB3WI52Q679002L4O6H06FMHUCH&client_secret=854d4cf847fb7827b307af55dd45f92a278dd68b96&grant_type=authorization_code");
            // Ashok WebRequest request = WebRequest.Create("https://accounts.zoho.com/oauth/v2/token?code=" + grantToken + "&redirect_uri=https://www.zohoapis.com/crm/v2/&client_id=1000.PD0WTBCRLN7670000W1CWEXMD102NH&client_secret=33ac8f02f9202ff9d949ee08d10b4bcb30e12776f8&grant_type=authorization_code");
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(postContent);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            var responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var test = JsonConvert.DeserializeObject<ResonseModel>(responseText);
            var refreshToken = test.refresh_token;
            var JaccessToken = test.access_token;
            //dataStream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream);
            //string responseFromServer = reader.ReadToEnd();
            //reader.Close();
            //dataStream.Close();
            response.Close();


            #endregion


            #region c# sdk code
            // ZCRMRestClient zCRMRestClient = new ZCRMRestClient();
            //var config = new Dictionary<string, string>()
            //{
            //    { "client_id","1000.522HB3WI52Q679002L4O6H06FMHUCH"},
            //    { "client_secret","854d4cf847fb7827b307af55dd45f92a278dd68b96"},
            //    { "redirect_uri","https://www.zohoapis.com/crm/v2/"},
            //    { "access_type","offline"},
            //    { "persistence_handler_class","ZCRMSDK.OAuth.ClientApp.ZohoOAuthFilePersistence, ZCRMSDK"},
            //    //{ "oauth_tokens_file_path",@"E:\Ashok\ZOHOSIGN\ZohoSignAPI\ZohoSignAPI\Tokens"},
            //    { "oauth_tokens_file_path",@"\\DESKTOP-NG6ES8V\Tokens\Tokens.txt"},
            //    //{ "mysql_username","root"},
            //    //{ "mysql_password","root"},
            //    //{ "mysql_database","C:\\Users\\AyanWorks\\TESTDB"},
            //    //{ "mysql_server","127.0.0.1"},
            //    //{ "mysql_port","3306"},
            //    { "apiBaseUrl","https://www.zohoapis.com/" },
            //    { "photoUrl","https://profile.zoho.com/api/v1/user/self/photo"},
            //    //{ "apiBaseUrl","https//www.zohoapis.com"},
            //    { "apiVersion","v2"},
            //    //{ "logFilePath",@"E:\Ashok\ZOHOSIGN\ZohoSignAPI\ZohoSignAPI\Log" },
            //    { "logFilePath",@"\\DESKTOP-NG6ES8V\Log" },
            //    { "timeout","99000"},
            //    { "minLogLevel","WARNING"},
            //    { "domainSuffix","com"},
            //    { "currentUserEmail","ashutosh.kumar@sharpitech.in"}
            //};
            //ZCRMRestClient.Initialize(config);
            //ZohoOAuthClient client = ZohoOAuthClient.GetInstance(); 
            //  ZohoOAuthTokens tokens = client.GenerateAccessToken(grantToken);


            //ZCRMRestClient.SetCurrentUser("ashutosh.kumar@sharpitech.in");
            //ZCRMRestClient.Initialize();
            // ZohoOAuthClient client = ZohoOAuthClient.GetInstance();
            ////string grantToken = grantToken;
            //ZohoOAuthTokens tokens = client.GenerateAccessToken(grantToken);
            //string accessToken1 = tokens.AccessToken;
            //string refreshToken1 = tokens.RefreshToken;

            // return responseFromServer;
            #endregion

            #region Commented code 
            //using// (var httpClient = new HttpClient())
            //{
            //    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/requests"))
            //    {
            //        request.Headers.TryAddWithoutValidation("Authorization", tokens.AccessToken);

            //        var multipartContent = new MultipartFormDataContent();
            //        multipartContent.Add(new StringContent("E:AshokImagesVOC1.PNG"), "file");
            //        multipartContent.Add(new StringContent("{\n  \"requests\": {\n    \"request_type_id\": \"2000000322040\",\n    \"request_name\": \"NDA \",\n    \"actions\": [\n      {\n        \"recipient_name\": \"Ashok Kasar\",\n        \"recipient_email\": \"ashokdkasar@gmail.com\",\n        \"recipient_phonenumber\": \"9921799551\",\n        \"recipient_countrycode\": \"\",\n        \"action_type\": \"SIGN\",\n        \"private_notes\": \"Please get back to us for further queries\",\n        \"signing_order\": 0,\n        \"verify_recipient\": true,\n        \"verification_type\": \"EMAIL\",\n        \"verification_code\": \"\"\n      }\n    ],\n    \"expiration_days\": 1,\n    \"is_sequential\": true,\n    \"email_reminders\": true,\n    \"reminder_period\": 8,\n    \"folder_id\": \"2000000489161\"\n  }\n}"), "data");
            //        request.Content = multipartContent;

            //        var response1 = await httpClient.SendAsync(request);
            //    }
            //}

            //replace <value>
            // byte[] dataStream = Encoding.UTF8.GetBytes(data);

            //WebRequest request1 = WebRequest.Create("https://sign.zoho.com/api/v1/requests");
            //request1.Headers.Add("Authorization:"+ tokens.AccessToken);
            #endregion
        }




        public async Task<IActionResult> Index()
        {

            SelfSign();
            return null;
            //postContent = postContent + "&authtoken=Your AUTHTOKEN";//Give your authtoken
           
            string accessToken = "1000.26c384a6cc2a15c74f001cb505a5dbc2.2b8a873076fd5dc6d5038cdd6f279320";
           // GenerateAccessNRefreshTokenaa();
            #region GetFieldTypes

            using (var httpClient = new HttpClient())
            {
                using (var requestCreateDocument = new HttpRequestMessage(new HttpMethod("GET"), "https://sign.zoho.com/api/v1/fieldtypes"))
                {

                    requestCreateDocument.Headers.Add("Authorization", "Zoho-oauthtoken " + accessToken);

                    HttpResponseMessage responseresult = httpClient.SendAsync(requestCreateDocument).Result;
                    var result = responseresult.Content.ReadAsStringAsync().Result.ToString();
                    JObject o = JObject.Parse(result);
                    var jsonData = JsonConvert.DeserializeObject<Fields>(result);
                    //foreach (var item in jsonData.field_types)
                    //{
                    //}
                }
            }
            #endregion

            //Create New Document for Signing
            using (var httpClient = new HttpClient())
            {
                CreateDocumentModel jsonData = null;

                using (var requestCreateDocument = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/requests"))
                {
                   
                #region Create New Document for Signing
                    requestCreateDocument.Headers.Add("Authorization", "Zoho-oauthtoken " + accessToken);
                    var multipartContent = new MultipartFormDataContent();
                    var jsonContent = "{'requests': {" +
                        "'request_name': 'NDA '," +
                        "'actions': [" +
                            "{" +
                            "'recipient_name': 'Ashok Kasar'," +
                            "'recipient_email': 'ashok.kasar@ayanworks.com'," +
                            "'recipient_phonenumber': '9921799551'," +
                            "'recipient_countrycode': ''," +
                            "'action_type': 'SIGN'," +
                            "'private_notes': 'Please get back to us for further queries'," +
                            "'signing_order': 0," +
                            "'verify_recipient': false," +
                            "'verification_type': 'EMAIL'," +
                            "'verification_code': ''" +
                            "}" +
                        "]," +
                        "'expiration_days': 10," +
                        "'is_sequential': true," +
                        "'email_reminders': true," +
                        "'reminder_period': 8 " +
                        "}" +
                    "}";


                    multipartContent.Add(new ByteArrayContent(System.IO.File.ReadAllBytes("VOC1.png")), "file", Path.GetFileName("VOC1.png"));
                    multipartContent.Add(new StringContent(jsonContent), "data");
                    requestCreateDocument.Content = multipartContent;
                    HttpResponseMessage responseresult = httpClient.SendAsync(requestCreateDocument).Result;
                    var result = responseresult.Content.ReadAsStringAsync().Result;
                    jsonData = JsonConvert.DeserializeObject<CreateDocumentModel>(result);
                }

                #endregion

                #region Send Document for Signing 
                //Send for Sign Document
                if (jsonData != null && jsonData.status == "success")
                {
                    string requestId = jsonData.requests.request_id;
                    string documentId = jsonData.requests.document_ids[0].document_id;
                    string actionId = jsonData.requests.actions[0].action_id;
                    string recipient_email = jsonData.requests.actions[0].recipient_email;
                    string recipient_name = jsonData.requests.actions[0].recipient_name;
                    string requestTypeId = jsonData.requests.request_type_id;

                    // SEND document for signature
                    recipient_email = "ashok.kasar@ayanworks.com";
                    recipient_name = "Ashutosh Kumar";
                    using (var httpClientSendSign = new HttpClient())
                    {
                        using (var requestSendSign = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/requests/" + requestId + "/submit"))
                        {

                            var jsonContent = "data={'requests':{'actions':[{'action_id':'" + actionId + "','verify_recipient':false,'action_type':'SIGN','private_notes':'Test document','signing_order':0,'recipient_email':'" + recipient_email + "','recipient_name':'" + recipient_name + "','fields':{'image_fields':[{'action_id': '" + actionId + "','field_type_name':'Signature','field_category':'image','field_label':'Signature','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Signature','y_coord':3,'abs_width':382,'description_tooltip':'TEst Tooltip','x_coord':42,'abs_height':80},{'action_id': '" + actionId + "','field_type_name':'Initial','field_category':'image','field_label':'Initial','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Initial', 'y_coord':80, 'abs_width':280, 'description_tooltip':'', 'x_coord':71, 'abs_height':50 } ] }, 'recipient_phonenumber':'9921799551', 'recipient_countrycode':'91' } ],'request_type_id' : '" + requestTypeId + "',  'request_name':'Leave a note test', 'expiration_days':10, 'is_sequential':true, 'notes':'Test Document' }}";

                            requestSendSign.Headers.TryAddWithoutValidation("Authorization", "Zoho-oauthtoken " + accessToken);
                            requestSendSign.Content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                            HttpResponseMessage responseSendSign = httpClientSendSign.SendAsync(requestSendSign).Result;
                            var resultSendSign = responseSendSign.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }

            #endregion

            //Refresh Token "1000.b967ca4b8f33c8686f3d111f4b720b84.324087ac6af5bd434a94cbb313964899"

            #region Call to Create Document API

            using (var httpClient = new HttpClient())
            {
                using (var requestCreateDocument = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/requests"))
                {

                    requestCreateDocument.Headers.Add("Authorization", "Zoho-oauthtoken " + "1000.8087fe29ecbb87c059646081a20ed9f3.c15376b169c0038352600c6ecd52c731");
                    var multipartContent = new MultipartFormDataContent();

                    multipartContent.Add(new ByteArrayContent(System.IO.File.ReadAllBytes("VOC1.png")), "file", Path.GetFileName("VOC1.png"));
                    // working multipartContent.Add(new StringContent("{\"requests\": {\"request_name\":\"NDA \",\"self_sign\":true}}"), "data");
                    multipartContent.Add(new StringContent("{'requests': {'request_name':'NDA ','self_sign':true}}"), "data");
                    //         multipartContent.Add(new StringContent("{\"requests\": {\"request_name\":\"NDA \",\"self_sign\":true }"), "data");
                    requestCreateDocument.Content = multipartContent;

                    //var responseResult = await httpClient.SendAsync(requestCreateDocument);
                    HttpResponseMessage responseresult = httpClient.SendAsync(requestCreateDocument).Result;
                    var result = responseresult.Content.ReadAsStringAsync().Result;
                    var jsonData = JsonConvert.DeserializeObject(result);
                }
            }

            #endregion

            #region Generate Grant Token

            //WebRequest requestGenGrantToken = WebRequest.Create("https://accounts.zoho.com/oauth/v2/auth?scope=ZohoSign.documents.all&client_id=1000.522HB3WI52Q679002L4O6H06FMHUCH&state=testing&response_type=code&redirect_uri=https://www.zohoapis.com/crm/v2/&access_type=offline");
            //requestGenGrantToken.Method = "GET";
            //WebResponse responseGenGrantToken = requestGenGrantToken.GetResponse();
            //var responseTextGenGrantToken = new StreamReader(responseGenGrantToken.GetResponseStream()).ReadToEnd();
            ////var resultGenGrantToken = JsonConvert.DeserializeObject(responseTextGenGrantToken);

            #endregion



            return View();

            





        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public void GenerateAccessNRefreshTokenaa()
        {
            #region Generate Access Token and Refresh Token 
            string postContent = "scope=ZohoSign.documents.all";
            string grantToken = "1000.e98c39633b702b825b7987cd557c4ea7.64ecdd3fa2606d745ed927553947556e";

            WebRequest request = WebRequest.Create("https://accounts.zoho.com/oauth/v2/token?code=" + grantToken + "&redirect_uri=https://www.zohoapis.com/crm/v2/&client_id=1000.522HB3WI52Q679002L4O6H06FMHUCH&client_secret=854d4cf847fb7827b307af55dd45f92a278dd68b96&grant_type=authorization_code");
            // Ashok WebRequest request = WebRequest.Create("https://accounts.zoho.com/oauth/v2/token?code=" + grantToken + "&redirect_uri=https://www.zohoapis.com/crm/v2/&client_id=1000.PD0WTBCRLN7670000W1CWEXMD102NH&client_secret=33ac8f02f9202ff9d949ee08d10b4bcb30e12776f8&grant_type=authorization_code");
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(postContent);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            var responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var test = JsonConvert.DeserializeObject<ResonseModel>(responseText);
            var refreshToken = test.refresh_token;
            var JaccessToken = test.access_token;
            //dataStream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream);
            //string responseFromServer = reader.ReadToEnd();
            //reader.Close();
            //dataStream.Close();
            response.Close();


            #endregion
        }

    }

    public class ResonseModel
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}



public class FieldTypesDetails
{
    public string field_type_id { get; set; }
    public string field_category { get; set; }
    public string is_mandatory { get; set; }
    public string field_type_name { get; set; }
}
public class FieldTypes
{
    //public string field_types { get; set; }
    //public FieldTypesDetails FieldTypesDetails { get; set; }
    public string field_type_id { get; set; }
    public string field_category { get; set; }
    public string is_mandatory { get; set; }
    public string field_type_name { get; set; }

}
public class Fields
{
    public List<FieldTypes> field_types { get; set; }

}


public class CreateDocumentModel
{
    public string code { get; set; }
    public RequestsModel requests { get; set; }
    public string message { get; set; }
    public string status { get; set; }


}

public class RequestsModel
{
    public string request_id { get; set; }
    public string request_type_id { get; set; }
    public bool self_sign { get; set; }
    public List<DocumentIdsModel> document_ids { get; set; }
    public List<ActionsModel> actions { get; set; }

}

public class DocumentIdsModel
{
    public string image_string { get; set; }
    public string document_name { get; set; }
    public int document_size { get; set; }
    public int document_order { get; set; }
    public int total_pages { get; set; }
    public string document_id { get; set; }
    public List<pagesModel> pages { get; set; }
    // public FieldsModel Fields { get; set; }
}

public class pagesModel
{
    public string image_string { get; set; }
    public int page { get; set; }
    public bool is_thumbnail { get; set; }
}


public class ActionsModel
{
    public string action_id { get; set; }
    public string recipient_email { get; set; }
    public string recipient_name { get; set; }
    public string request_type_id { get; set; }
    public string notes { get; set; }
}


public class FieldsModel
{
    public string recipient_name { get; set; }

}
