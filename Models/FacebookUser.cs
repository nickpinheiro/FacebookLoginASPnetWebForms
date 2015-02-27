using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookLoginASPnetWebForms.Models
{
    public class Facebook
    {
        public class User
        {
            public string id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string link { get; set; }
            public string username { get; set; }
            public string gender { get; set; }
            public string locale { get; set; }
        }
    }
}