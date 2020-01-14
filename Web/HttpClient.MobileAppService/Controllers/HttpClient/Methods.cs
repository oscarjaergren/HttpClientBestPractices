//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Threading.Tasks;

//using Newtonsoft.Json.Linq;

//namespace HttpClient.MobileAppService.Controllers.HttpClient
//{
//    public class Methods
//    {

//        [WebMethod]
//        public string GetAllFormsByID(string sessionID)
//        {
//            JObject json = new JObject();
//            UserDetails user = DBUserHelper.GetUserBySessionID(new Guid(sessionID));
//            if (user == null)
//            {
//                json["Error"] = "Not logged in";
//                return json.ToString(Newtonsoft.Json.Formatting.None);
//            }
//            Client client = Client.GetClient(user.ClientID);
//            JObject formIDs = JObject.Parse(GetFormLibrary(sessionID));
//            JArray libraryIDs = JArray.FromObject(formIDs["LibraryIDs"]);
//            JArray priorityTaskIDs = JArray.FromObject(formIDs["PriorityTaskIDs"]);
//            JArray formArray = new JArray();
//            List<string> formList = new List<string>();
//            foreach (int id in libraryIDs)
//            {
//                formArray.Add(GetFormByID(sessionID, id));
//            }
//            foreach (int id in priorityTaskIDs)
//            {
//                formArray.Add(GetFormByID(sessionID, id));
//            }
//            JObject formObject = JObject.FromObject(formArray);
//            return formObject.ToString(Newtonsoft.Json.Formatting.None);
//        }

//        [WebMethod]
//        public string AndroidGetFormByID(string sessionID, int formID)
//        {
//            JObject json = new JObject();
//            UserDetails user = DBUserHelper.GetUserBySessionID(new Guid(sessionID));
//            if (user == null)
//            {
//                json["Error"] = "Not logged in";
//                return json.ToString(Newtonsoft.Json.Formatting.None);
//            }
//            Client client = Client.GetClient(user.ClientID);

//            var formTemplateRecord = SqlInsertUpdate.SelectQuery("SELECT JSON, CreatedDate FROM FormTemplates WHERE ID=@ID AND clientID=@clientID", "FormsConnectionString", new List<SqlParameter> { new SqlParameter("@ID", formID), new SqlParameter("@clientID", client.ID) }).GetFirstRow();
//            var formJson = formTemplateRecord["JSON"].ToString();

//            if (formJson == null)
//            {
//                json["Error"] = "No such form";
//                return json.ToString(Newtonsoft.Json.Formatting.None);
//            }

//            json = JObject.Parse(formJson);
//            json["formID"] = formID;
//            try
//            {
//                json["created"] = Convert.ToDateTime(formTemplateRecord["CreatedDate"]).ToString("dd/MM/yyyy");
//            }
//            catch (Exception e)
//            {
//            }
//            MemoryStream convertedFormData = new MemoryStream();
//            try
//            {
//                using (MemoryStream ms = new MemoryStream(json.ToString(Newtonsoft.Json.Formatting.None).ToByteArray()))
//                {
//                    ms.Seek(0, SeekOrigin.Begin);
//                    using (ZipFile zipedForm = new ZipFile())
//                    {
//                        zipedForm.AddEntry(json["title"].ToString() + "_" + json["formID"].ToString(), ms);
//                        zipedForm.Save(convertedFormData);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return ex.Message.ToString();
//            }

//            return Convert.ToBase64String(convertedFormData.ToArray());
//        }

//        [WebMethod]
//        public string GetFormByID(string sessionID, int formID)
//        {
//            JObject json = new JObject();
//            UserDetails user = DBUserHelper.GetUserBySessionID(new Guid(sessionID));
//            if (user == null)
//            {
//                json["Error"] = "Not logged in";
//                return json.ToString(Newtonsoft.Json.Formatting.None);
//            }
//            Client client = Client.GetClient(user.ClientID);

//            var formTemplateRecord = SqlInsertUpdate.SelectQuery("SELECT JSON, CreatedDate FROM FormTemplates WHERE ID=@ID AND clientID=@clientID", "FormsConnectionString", new List<SqlParameter> { new SqlParameter("@ID", formID), new SqlParameter("@clientID", client.ID) }).GetFirstRow();
//            var formJson = formTemplateRecord["JSON"].ToString();

//            if (formJson == null)
//            {
//                json["Error"] = "No such form";
//                return json.ToString(Newtonsoft.Json.Formatting.None);
//            }

//            json = JObject.Parse(formJson);
//            json["formID"] = formID;
//            try
//            {
//                json["created"] = Convert.ToDateTime(formTemplateRecord["CreatedDate"]).ToString("dd/MM/yyyy");
//            }
//            catch (Exception e)
//            {
//            }

//            return json.ToString(Newtonsoft.Json.Formatting.None);
//        }
//    }
//}
