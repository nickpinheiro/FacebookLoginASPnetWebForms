using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Collections;
using System.Configuration;
using FacebookLoginASPnetWebForms.Models;

namespace FacebookLoginASPnetWebForms.account
{
    public partial class user : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the Facebook code from the querystring
            if (Request.QueryString["code"] != "")
            {
                var obj = GetFacebookUserData(Request.QueryString["code"]);

                ListView1.DataSource = obj;
                ListView1.DataBind();
            }
        }
        
        protected string CreateTokenProof(string token, string secret)
        {
            
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            
            // Convert the app secret to byte and declare SHA256 hash
            byte[] secretByte = encoding.GetBytes(secret);
            HMACSHA256 hmacsha256 = new HMACSHA256(secretByte);

            // Convert the access token to byte and compute it to hash
            byte[] tokenBytes = encoding.GetBytes(token);
            byte[] hashmessage = hmacsha256.ComputeHash(tokenBytes);


            // Convert the hash to string
            string tokenProof = "";
            for (int i = 0; i < hashmessage.Length; i++)
            {
                tokenProof += hashmessage[i].ToString("X2"); // hex format
            }

            // Return the token proof in lowercase which is acceptable by Facebook Graph API
            return tokenProof.ToLower();
            
        }
        
        protected List<Facebook.User> GetFacebookUserData(string code)
        {
            // Exchange the code for an access token
            Uri targetUri = new Uri("https://graph.facebook.com/oauth/access_token?client_id=" + ConfigurationManager.AppSettings["FacebookAppId"] + "&client_secret=" + ConfigurationManager.AppSettings["FacebookAppSecret"] + "&redirect_uri=http://" + Request.ServerVariables["SERVER_NAME"] + ":" + Request.ServerVariables["SERVER_PORT"] + "/account/user.aspx&code=" + code);
            HttpWebRequest at = (HttpWebRequest)HttpWebRequest.Create(targetUri);

            System.IO.StreamReader str = new System.IO.StreamReader(at.GetResponse().GetResponseStream());
            string token = str.ReadToEnd().ToString().Replace("access_token=", "");

            // Split the access token and expiration from the single string
            string[] combined = token.Split('\"');
            string accessToken = combined[3];

            // Exchange the code for an extended access token
            Uri eatTargetUri = new Uri("https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id=" + ConfigurationManager.AppSettings["FacebookAppId"] + "&client_secret=" + ConfigurationManager.AppSettings["FacebookAppSecret"] + "&fb_exchange_token=" + accessToken);
            HttpWebRequest eat = (HttpWebRequest)HttpWebRequest.Create(eatTargetUri);

            StreamReader eatStr = new StreamReader(eat.GetResponse().GetResponseStream());
            string eatToken = eatStr.ReadToEnd().ToString().Replace("access_token=", "");

            // Split the access token and expiration from the single string
            string[] eatWords = eatToken.Split('\"');
            string extendedAccessToken = eatWords[3];
            
            // Create Token Proof
            string appsecretProof = CreateTokenProof(accessToken, ConfigurationManager.AppSettings["FacebookAppSecret"]);

            // Request the Facebook user information
            Uri targetUserUri = new Uri("https://graph.facebook.com/me?fields=first_name,last_name,gender,locale,link&access_token=" + accessToken + "&appsecret_proof=" + appsecretProof);
            HttpWebRequest user = (HttpWebRequest)HttpWebRequest.Create(targetUserUri);

            // Read the returned JSON object response
            StreamReader userInfo = new StreamReader(user.GetResponse().GetResponseStream());
            string jsonResponse = string.Empty;
            jsonResponse = userInfo.ReadToEnd();

            // Deserialize and convert the JSON object to the Facebook.User object type
            JavaScriptSerializer sr = new JavaScriptSerializer();
            string jsondata = jsonResponse;
            Facebook.User converted = sr.Deserialize<Facebook.User>(jsondata);

            // Write the user data to a List
            List<Facebook.User> currentUser = new List<Facebook.User>();
            currentUser.Add(converted);

            // Return the current Facebook user
            return currentUser;
        }
    }
}
