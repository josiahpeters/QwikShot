using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace QwikShot.WinApp.Sharing
{
    public class Imgur : IWebImageShare
    {
        // client id: 5beae783508d4ba
        string clientId = "5beae783508d4ba";

        // client secret: 943582015ffe7ca0fa86ba23ed164fe846b131e9
        string secret = "943582015ffe7ca0fa86ba23ed164fe846b131e9";

        //https://api.imgur.com/oauth2#response_type

        public string GetOathUrl()
        {
            return String.Format("https://api.imgur.com/oauth2/authorize?client_id={0}&response_type=pin&state=derp", clientId);
        }

        public void AuthorizePin(string pin)
        {
            pin = "3e0b40647d";
            var authUrl = "https://api.imgur.com/oauth2/token";

            var data = new NameValueCollection();

            data.Add("client_id", clientId);
            data.Add("client_secret", secret);
            data.Add("grant_type", "pin");
            data.Add("pin", pin);

            string response = HttpHelper.PostToUrl(authUrl, data);

            //var deserialized = DynamicJson.Deserialize(json);

            //File.WriteAllText("pin.txt", response);
        }

        public void GetAuthToken(string accessToken)
        {
            var authUrl = "https://api.imgur.com/oauth2/token";

            var data = new NameValueCollection();

            data.Add("client_id", clientId);
            data.Add("client_secret", secret);
            data.Add("grant_type", "authorization_code");
            data.Add("code", accessToken);

            string response = HttpHelper.PostToUrl(authUrl, data);

            //File.WriteAllText("token.txt", response);
        }

        public string UploadImage(Bitmap image)
        {
            var url = "https://api.imgur.com/3/upload";

            var data = new NameValueCollection();

            //data.Add("type", "base64");
            //data.Add("grant_type", "authorization_code");
            //data.Add("name", "derp");
            //data.Add("title", "derp");
            //data.Add("description", "derp");

            using (var stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);

                stream.Seek(0, SeekOrigin.Begin);

                byte[] buffer = stream.ToArray();

                string response = HttpHelper.UploadFilesToRemoteUrl(url, clientId, "image.png", buffer, data);

                //File.WriteAllText("upload.txt", response);

                return response;
            }
        }
    }
}
