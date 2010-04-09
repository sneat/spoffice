using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Spoffice.Website.Models.Output
{
    public class AccountInformationOutput
    {
        public bool Success
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string MyAccountHeading
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        [XmlIgnore]
        [JsonIgnore]
        public ModelStateDictionary Errors
        {
            get;
            set;
        }

        public List<ErrorResult> ErrorMessages
        {
            get
            {
                List<ErrorResult> msgs = new List<ErrorResult>();
                if (Errors != null)
                {
                    foreach (KeyValuePair<string, ModelState> state in Errors)
                    {
                        ErrorResult result = new ErrorResult();
                        result.Field = state.Key;
                        result.ErrorMessages = new List<string>();
                        foreach (ModelError error in state.Value.Errors)
                        {
                            result.ErrorMessages.Add(error.ErrorMessage);
                        }
                        if (result.ErrorMessages.Count > 0)
                        {
                            msgs.Add(result);
                        }
                    }
                }
                return msgs;
            }
            set { }
        }
    }
}
