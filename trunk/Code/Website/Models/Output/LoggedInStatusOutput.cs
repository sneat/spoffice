using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Collections;

namespace Spoffice.Website.Models.Output
{
    public class LoggedInStatusOutput
    {
        public bool LoggedIn { 
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

        public List<string> Favourites
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
    public class ErrorResult
    {
        public ErrorResult()
        {
        }
        [XmlElement("Message")]
        [JsonProperty("Message")]
        public List<string> ErrorMessages { get;set;}

        [XmlAttribute]
        public string Field { get; set; }
    }
}
