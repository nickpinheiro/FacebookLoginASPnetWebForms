﻿using System;
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

        public class AccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }
    }
}