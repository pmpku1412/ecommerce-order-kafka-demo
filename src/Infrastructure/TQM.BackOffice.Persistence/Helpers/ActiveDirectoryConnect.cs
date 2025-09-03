using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQM.Backoffice.Application.DTOs.Common;
using System.DirectoryServices.AccountManagement;

namespace TQM.BackOffice.Persistence.Helpers
{
    public class ActiveDirectoryConnect
    {
        public static string ADServer_Path_Config = "";
        public static string ADServer_Username_Config = "";
        public static string ADServer_Password_Config = "";

        public string ADServer_Path { get; set; }
        public string ADServer_Username { get; set; }
        public string ADServer_Password { get; set; }

        //public ActiveDirectoryConnect()
        //{ 
        //    this.ADServer_Path = "";
        //    this.ADServer_Username = "";
        //    this.ADServer_Password = "";
        //}


        public class ADUserInfo
        {
            public string ContactTitle { get; set; }
            public string ContactName { get; set; }
            public string ContactEmail { get; set; }
            public string ContactPhone { get; set; }
            public string ContactMobilePhone { get; set; }
            public byte[] ContactPicture { get; set; }
        }

        public ActiveDirectoryConnect(string _ADServer_Path, string _ADServer_Username, string _ADServer_Password)
        {
            this.ADServer_Path = _ADServer_Path;
            this.ADServer_Username = _ADServer_Username;
            this.ADServer_Password = _ADServer_Password;
        }

        public static List<string> getADList()
        {
            List<string> adList = new List<string>();
            adList.Add("LDAP://ldaphq.tqm");
            adList.Add("LDAP://tqmldap.apac");

            return adList;
        }



        public TemplateClass.ErrorTemplate ValidateUserAgainstAdList(List<string> ADServerList)
        {
            TemplateClass.ErrorTemplate isValid = new TemplateClass.ErrorTemplate();
            isValid.IsOK = true;
            isValid.Message = new List<string>();

            int ADCountLoop = 0;

            string WhichAdError = "";
            try
            {
                foreach (string ADServ in ADServerList)
                {
                    ADCountLoop += 1;

                    WhichAdError = ADServ;

                    //using (DirectoryEntry adsEntry = new DirectoryEntry(ADServ, this.ADServer_Username, this.ADServer_Password))
                    //{
                    //    using (DirectorySearcher adsSearcher = new DirectorySearcher(adsEntry))
                    //    {
                    //        adsSearcher.Filter = "(&(objectClass=user)(sAMAccountName=" + this.ADServer_Username + "))";
                    //        //adsSearcher.Filter = "(sAMAccountName=" + this.ADServer_Username + ")";

                    //        try
                    //        {
                    //            SearchResult adsSearchResult = adsSearcher.FindOne();
                    //            //bSucceeded = true;

                    //            //strAuthenticatedBy = "Active Directory";
                    //            //strError = "User has been authenticated by Active Directory.";

                    //            #region Get Property

                    //            //pull the collection of objects with this key name
                    //            ResultPropertyValueCollection valueCollection = adsSearchResult.Properties["description"];


                    //            #endregion


                    //            isValid.IsOK = true;
                    //            break;
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            // Failed to authenticate. Most likely it is caused by unknown user
                    //            // id or bad strPassword.

                    //            if (ex.Message.Contains("the server is not operational (" + ADServ + ")"))
                    //            {
                    //                isValid.IsOK = false;
                    //                isValid.Message.Add("ชื่อหรือพาสเวิร์ดไม่ถูกต้อง");
                    //            }
                    //            else
                    //            {
                    //                isValid.IsOK = false;
                    //                isValid.Message.Add("ชื่อหรือพาสเวิร์ดไม่ถูกต้อง");

                    //            }

                    //            //if (ADCountLoop == ADServerList.Count())
                    //            //{
                    //            //    MailControl.AdminErrorMail("Invalid UserID and Password", "Username : " + this.ADServer_Username.ToLower().Trim() + " Password : " + this.ADServer_Password.ToLower().Trim() + " " + ex.ToString(), "");
                    //            //}
                    //        }
                    //        finally
                    //        {
                    //            adsEntry.Close();
                    //        }
                    //    }
                    //}

                    #region PrincipalContext Method

                    string P_Domain = "";
                    string P_Container = "";

                    if (ADServ == "LDAP://ldaphq.tqm")
                    {
                        P_Domain = "LDAPHQ";
                        P_Container = "DC=ldaphq,DC=tqm";
                    }
                    else if (ADServ == "LDAP://tqmldap.apac")
                    {
                        P_Domain = "TQMLDAP";
                        P_Container = "DC=tqmldap,DC=apac";
                    }

                    PrincipalContext pc = new PrincipalContext(ContextType.Domain, P_Domain, P_Container);
                    bool isValidX = pc.ValidateCredentials(this.ADServer_Username, this.ADServer_Password);

                    if (isValidX)
                    {
                        isValid.IsOK = true;
                        break;
                    }
                    else
                    {
                        isValid.IsOK = false;
                        isValid.Message.Add("ชื่อหรือพาสเวิร์ดไม่ถูกต้อง");
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                isValid.IsOK = false;
                isValid.Message.Add("Ad Server Error : " + WhichAdError);
            }

            return isValid;
        }


        #region Get All User

        public TemplateClass.ErrorTemplate GetAllUser(List<ActiveDirectoryConnect> ADServerList)
        {
            TemplateClass.ErrorTemplate isValid = new TemplateClass.ErrorTemplate();
            isValid.IsOK = true;
            isValid.Message = new List<string>();

            string WhichAdError = "";
            try
            {
                foreach (ActiveDirectoryConnect ADServ in ADServerList)
                {
                    WhichAdError = ADServ.ADServer_Path;

                    SearchResultCollection sResults = null;

                    try
                    {
                        //modify this line to include your domain name
                        string path = ADServ.ADServer_Path; //"LDAP://EXTECH";
                        //init a directory entry

                        DirectoryEntry dEntry = new DirectoryEntry(path, ADServ.ADServer_Username, ADServ.ADServer_Password);

                        if (ADServ.ADServer_Username == "")
                        {
                            dEntry = new DirectoryEntry(path);
                        }

                        //init a directory searcher
                        DirectorySearcher dSearcher = new DirectorySearcher(dEntry);

                        //This line applies a filter to the search specifying a username to search for
                        //modify this line to specify a user name. if you want to search for all
                        //users who start with k - set SearchString to "k"

                        //dSearcher.Filter = "(&(objectClass=user))";

                        dSearcher.Filter = "(&(objectClass=user)(cn=Warot Yahom))";
                        //dSearcher.PropertiesToLoad.Add("description");


                        //perform search on active directory
                        sResults = dSearcher.FindAll();

                        //loop through results of search
                        foreach (SearchResult searchResult in sResults)
                        {
                            //if (searchResult.Properties["CN"][0].ToString() == "Adit")
                            if (true)
                            {
                                ////loop through the ad properties

                                //pull the collection of objects with this key name
                                //ResultPropertyValueCollection valueCollection = searchResult.Properties["manager"];

                                string testText = "";

                                foreach (System.Collections.DictionaryEntry propertyValue in searchResult.Properties)
                                {
                                    //loop through the values that have a specific name
                                    //an example of a property that would have multiple
                                    //collections for the same name would be memberof

                                    if (propertyValue.Key.ToString() == "description")
                                    {
                                        testText += "(" + propertyValue.Key + ") ";

                                        ResultPropertyValueCollection vc = ((ResultPropertyValueCollection)propertyValue.Value);
                                        if (vc.Count > 0)
                                            testText += " Property Value: ";

                                        int count = 0;
                                        foreach (var val in vc)
                                        {
                                            if (count == 0)
                                                testText += val.ToString();
                                            else
                                                testText += ";" + val.ToString();

                                            count++;
                                        }
                                        testText += "<br />";

                                    }
                                }

                                isValid.Message.Add(testText);
                            }
                        }
                    }
                    catch (InvalidOperationException iOe)
                    {
                        //
                    }
                    catch (NotSupportedException nSe)
                    {
                        //
                    }
                    finally
                    {

                        // dispose of objects used
                        if (sResults != null)
                            sResults.Dispose();

                    }
                }
            }
            catch (Exception ex)
            {
                isValid.IsOK = false;
                isValid.Message.Add("Ad Server Error : " + WhichAdError);
            }

            return isValid;
        }

        #endregion

    }
}
