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
        string grantToken = "";
        string accessToken = "";
        public HomeController()
        {
            grantToken = "1000.9819e810f851cba17fcca50123f296e5.605ea4c56c943ad144e94a95e2600ffc";
            accessToken = "1000.f2db348b9840a057a34e58cd9787840d.408467376830d47b446900d8ec052307";
        }
        //ZohoSign.templates.ALL
        public async Task<IActionResult> SelfSign()
        {

            string postContent = "scope=ZohoSign.documents.all";
            //postContent = postContent + "&authtoken=Your AUTHTOKEN";//Give your authtoken
            // string grantToken = "1000.634e7dd2131e4a0d0d8750c2df83944d.e8679a883f93bbaeb69743fa965d5ad7";
            string accessToken = "1000.26c384a6cc2a15c74f001cb505a5dbc2.2b8a873076fd5dc6d5038cdd6f279320";

            GenerateAccessNRefreshTokenaa();

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

                            var jsonContent = "data={'requests':{'actions':[{'action_id':'" + actionId + "','verify_recipient':false,'action_type':'SIGN','private_notes':'Test document','signing_order':0,'recipient_email':'" + recipient_email + "','recipient_name':'" + recipient_name + "','fields':{'image_fields':[{'action_id': '" + actionId + "','field_type_name':'Signature','field_category':'image','field_label':'Signature','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Signature','y_coord':3,'abs_width':382,'description_tooltip':'TEst Tooltip','x_coord':42,'abs_height':80},{'action_id': '" + actionId + "','field_type_name':'Initial','field_category':'image','field_label':'Initial','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Initial', 'y_coord':80, 'abs_width':280, 'description_tooltip':'', 'x_coord':71, 'abs_height':50 } ] }, 'recipient_phonenumber':'9921799551', 'recipient_countrycode':'91' } ], 'self_sign':true, 'request_name':'Leave a note test', 'expiration_days':10, 'is_sequential':true, 'notes':'Test Document' }}";
                            requestSendSign.Headers.TryAddWithoutValidation("Authorization", "Zoho-oauthtoken " + accessToken);
                            requestSendSign.Content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                            HttpResponseMessage responseSendSign = httpClientSendSign.SendAsync(requestSendSign).Result;
                            var resultSendSign = responseSendSign.Content.ReadAsStringAsync().Result;
                        }
                    }
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




        public async Task<IActionResult> Index()
        {


            //postContent = postContent + "&authtoken=Your AUTHTOKEN";//Give your authtoken
            //string grantToken = "1000.522HB3WI52Q679002L4O6H06FMHUCH";

           // GenerateAccessNRefreshTokenaa();
           // var resultTemplateDetails = await GetTemplateDetails();
            return View();

            #region GetFieldTypes

            // GenerateAccessNRefreshTokenaa();
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

            //Create New Document for Signing and Send for Signature
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
                            "'recipient_email': 'ashutosh.kumar@sharpitech.in'," +
                            "'recipient_phonenumber': '9921799551'," +
                            "'recipient_countrycode': ''," +
                            "'action_type': 'SIGN'," +
                            "'private_notes': 'Please get back to us for further queries'," +
                            "'signing_order': 1," +
                            "'verify_recipient': false," +
                            "'verification_type': 'EMAIL'," +
                            "'verification_code': ''" +
                            "}," +
                            "{" +
                            "'recipient_name': 'Ravindra Patil'," +
                            "'recipient_email': 'ravindra.patil@ayanworks.com'," +
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
                    string actionId1 = jsonData.requests.actions[1].action_id;

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
                            string jsonContent = JsonSendForSign(jsonData.requests.actions, recipient_email, recipient_name, documentId, requestTypeId);
                            // var jsonContent111 = "data={'requests':{'actions':[{'action_id':'" + actionId + "','verify_recipient':false,'action_type':'SIGN','private_notes':'Test document','signing_order':0,'recipient_email':'" + recipient_email + "','recipient_name':'" + recipient_name + "','fields':{'image_fields':[{'action_id': '" + actionId + "','field_type_name':'Signature','field_category':'image','field_label':'Signature','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Signature','y_coord':3,'abs_width':382,'description_tooltip':'TEst Tooltip','x_coord':42,'abs_height':80},{'action_id': '" + actionId + "','field_type_name':'Initial','field_category':'image','field_label':'Initial','is_mandatory':true,'page_no':0,'document_id':'" + documentId + "','field_name':'Initial', 'y_coord':80, 'abs_width':280, 'description_tooltip':'', 'x_coord':71, 'abs_height':50 } ] }, 'recipient_phonenumber':'9921799551', 'recipient_countrycode':'91' } ],'request_type_id' : '" + requestTypeId + "',  'request_name':'Leave a note test', 'expiration_days':10, 'is_sequential':true, 'notes':'Test Document' }}";

                            requestSendSign.Headers.TryAddWithoutValidation("Authorization", "Zoho-oauthtoken " + accessToken);
                            requestSendSign.Content = new StringContent(jsonContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                            HttpResponseMessage responseSendSign = httpClientSendSign.SendAsync(requestSendSign).Result;
                            var resultSendSign = responseSendSign.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
                #endregion
            }



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


        private string JsonSendForSign(List<ActionsModel> actions, string recipient_email, string recipient_name, string documentId, string requestTypeId)
        {

            return "data={" +
                 "'requests':{" +
                    "'actions':[" +
                       "{  " +
                         "'action_id':'" + actions[0].action_id + "'," +
                         "'verify_recipient':false," +
                         "'action_type':'SIGN'," +
                         "'private_notes':'Test document'," +
                         "'signing_order':1," +
                         "'recipient_email':'ashok.kasar@ayanworks.com'," +
                         "'recipient_name':'" + recipient_name + "'," +
                         "'fields':{  " +
                             "'image_fields':[" +
                                    "{  " +
                                     " 'action_id':'" + actions[0].action_id + "'," +
                                     " 'field_type_name':'Signature', " +
                                     " 'field_category':'image'," +
                                     " 'field_label':'Signature'," +
                                     " 'is_mandatory':true," +
                                     " 'page_no':0," +
                                     " 'document_id':'" + documentId + "'," +
                                     " 'field_name':'Signature'," +
                                     " 'y_coord':3," +
                                     " 'abs_width':382," +
                                     " 'description_tooltip':'TEst Tooltip'," +
                                     " 'x_coord':42," +
                                     " 'abs_height':80" +
                                    " }," +
                                      "{  " +
                                    " 'action_id':'" + actions[0].action_id + "'," +
                                    " 'field_type_name':'Initial'," +
                                    " 'field_category':'image'," +
                                    " 'field_label':'Initial'," +
                                    " 'is_mandatory':true," +
                                    " 'page_no':0," +
                                    " 'document_id':'" + documentId + "'," +
                                    " 'field_name':'Initial'," +
                                    " 'y_coord':80," +
                                    " 'abs_width':280," +
                                    " 'description_tooltip':''," +
                                    " 'x_coord':71," +
                                    " 'abs_height':50" +
                                    "}" +
                                "]" +
                                "}," +
                            "'recipient_phonenumber':'9921799551'," +
                            "'recipient_countrycode':'91'" +
                       "}," +

                     //*******************

                     "{  " +
                         "'action_id':'" + actions[1].action_id + "'," +
                         "'verify_recipient':false," +
                         "'action_type':'SIGN'," +
                         "'private_notes':'Test document'," +
                         "'signing_order':0," +
                         "'recipient_email':'ravindra.patil@ayanworks.com'," +
                         "'recipient_name':'" + recipient_name + "'," +
                         "'fields':{  " +
                             "'image_fields':[" +
                                    "{  " +
                                     " 'action_id':'" + actions[1].action_id + "'," +
                                     " 'field_type_name':'Signature', " +
                                     " 'field_category':'image'," +
                                     " 'field_label':'Signature'," +
                                     " 'is_mandatory':true," +
                                     " 'page_no':0," +
                                     " 'document_id':'" + documentId + "'," +
                                     " 'field_name':'Signature'," +
                                     " 'y_coord':3," +
                                     " 'abs_width':382," +
                                     " 'description_tooltip':'TEst Tooltip'," +
                                     " 'x_coord':42," +
                                     " 'abs_height':80" +
                                    " }," +
                                      "{  " +
                                    " 'action_id':'" + actions[1].action_id + "'," +
                                    " 'field_type_name':'Initial'," +
                                    " 'field_category':'image'," +
                                    " 'field_label':'Initial'," +
                                    " 'is_mandatory':true," +
                                    " 'page_no':0," +
                                    " 'document_id':'" + documentId + "'," +
                                    " 'field_name':'Initial'," +
                                    " 'y_coord':80," +
                                    " 'abs_width':280," +
                                    " 'description_tooltip':''," +
                                    " 'x_coord':71," +
                                    " 'abs_height':50" +
                                    "}" +
                                "]" +
                                "}," +
                            "'recipient_phonenumber':'9921799551'," +
                            "'recipient_countrycode':'91'" +
                       "}" +
                    //*******************

                    "]," +
                  "'request_type_id':'" + requestTypeId + "'," +
                  "'request_name':'Leave a note test'," +
                  "'expiration_days':10," +
                  "'is_sequential':true," +
                  "'notes':'Test Document'" +
                 "}" +
                 "}";


            #region json generator classs implemenation 
            //RequestsJson requests = new RequestsJson
            //{
            //    request_type_id = "'" + requestTypeId + "'",
            //    request_name = "Leave a Note Test",
            //    expiration_days = "10",
            //    is_sequential = "true",
            //    notes = "Test Document",
            //    actions = new ActionsJsonArray
            //    {
            //        action_id = actionId,
            //        verify_recipient = "false",
            //        action_type = "SIGN",
            //        private_notes = "Test Document",
            //        signing_order = "0",
            //        recipient_name = "Ashutosh Kumar",
            //        recipient_phonenumber = "9921799551",
            //        recipient_email = "ashok.kasar@ayanworks.com",
            //        recipient_countrycode = "91",
            //        Fields = new FieldsJson
            //        {
            //            image_fields = new imagefieldsJsonArray
            //            {
            //                action_id = actionId.ToString(),
            //                field_type_name = "Signature",
            //                field_category = "image",
            //                field_label = "Signature",
            //                is_mandatory = "true",
            //                page_no = "0",
            //                document_id = documentId.ToString(),
            //                field_name = "Signature",
            //                y_coord = "3",
            //                abs_width = "382",
            //                description_tooltip = "TEst Tooltip",
            //                x_coord = "42",
            //                abs_height = "80",
            //            }
            //        }
            //    }
            //};
            //var testresult = JsonConvert.SerializeObject(requests);
            #endregion


        }

        public async Task<ActionResult> SendTemplate()
        {
           await GetTemplateDetails();
            TempData["Success"] = "Send Successfully!";
            return View("Index"); ;
        }

        private async Task<IActionResult> GetTemplateDetails()
        {
            TemplateDetails jsonData = new TemplateDetails();
            using (var httpClient = new HttpClient())
            {
                #region Get Template Details 


                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://sign.zoho.com/api/v1/templates/77354000000063011"))
                {
                    request.Headers.Add("Authorization", "Zoho-oauthtoken " + accessToken);

                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    var result = response.Content.ReadAsStringAsync().Result.ToString();

                    jsonData = JsonConvert.DeserializeObject<TemplateDetails>(result);
                    string actionId = jsonData.templates.actions[0].action_id;
                }


                #endregion
                #region Send Document template

                TemplateDetails templateDetails = new TemplateDetails();
                templateDetails.templates = new Templates();
                templateDetails.templates.actions = new List<Actions>();
                templateDetails.templates.actions.Add(new Actions { action_id = jsonData.templates.actions[0].action_id, action_type = "SIGN", private_notes = "", recipient_email = "ashok.kasar@ayanworks.com", recipient_name = "Ashok Kasasr", role = "Agent", verification_type = "EMAIL", verify_recipient = "false" });
                //templateDetails.templates.actions.Add(new Actions { action_id = jsonData.templates.actions[0].action_id, action_type = "SIGN", private_notes = "", recipient_email = "ASHUTOSH.KUMAR@sharpitech.in", recipient_name = "Ashutosh Kumar", role = "Agent", verification_type = "EMAIL", verify_recipient = "false" });
                var json = JsonConvert.SerializeObject(templateDetails);


                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://sign.zoho.com/api/v1/templates/77354000000063011/createdocument"))
                {
                    request.Headers.Add("Authorization", "Zoho-oauthtoken " + accessToken);

                    request.Content = new StringContent("data=" + json, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                    var result = response.Content.ReadAsStringAsync().Result.ToString();
                }


                #endregion
            }



            return null;
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
            accessToken = test.access_token;
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




// ------------- JSON ARRAY CREATE


public class RequestsJson
{
    public string request_type_id { get; set; }
    public string request_name { get; set; }
    public string expiration_days { get; set; }
    public string is_sequential { get; set; }
    public string notes { get; set; }
    public ActionsJsonArray actions;
}


public class ActionsJsonArray
{
    public string action_id { get; set; }
    public string verify_recipient { get; set; }
    public string action_type { get; set; }
    public string private_notes { get; set; }
    public string signing_order { get; set; }
    public string recipient_email { get; set; }
    public string recipient_name { get; set; }
    public string recipient_phonenumber { get; set; }
    public string recipient_countrycode { get; set; }
    public string MyProperty { get; set; }
    public FieldsJson Fields;

}

public class FieldsJson
{
    public imagefieldsJsonArray image_fields;
}

public class imagefieldsJsonArray
{
    public string action_id { get; set; }
    public string field_type_name { get; set; }
    public string field_category { get; set; }
    public string field_label { get; set; }
    public string is_mandatory { get; set; }
    public string page_no { get; set; }
    public string document_id { get; set; }
    public string field_name { get; set; }
    public string abs_width { get; set; }
    public string y_coord { get; set; }
    public string description_tooltip { get; set; }
    public string x_coord { get; set; }
    public string abs_height { get; set; }
}


#region FOR TEMPLATE 


public class TemplateDetails
{
    public Templates templates { get; set; }
}
public class Templates
{
    public List<Actions> actions { get; set; }
    public string notes { get; set; }
}

public class Actions
{
    public string action_id { get; set; }
    public string action_type { get; set; }
    public string recipient_email { get; set; }
    public string recipient_name { get; set; }
    public string role { get; set; }
    public string private_notes { get; set; }
    public string verify_recipient { get; set; }
    public string verification_type { get; set; }

}



#endregion


