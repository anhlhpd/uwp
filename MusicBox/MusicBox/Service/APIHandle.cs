using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MusicBox.Entity;
using Newtonsoft.Json;
using Windows.Data.Json;
using Windows.Storage;

namespace MusicBox.Service
{
    class APIHandle
    {
        private static string API_REGISTER = "https://2-dot-backup-server-002.appspot.com/_api/v2/members";
        private static string API_LOGIN = "https://2-dot-backup-server-002.appspot.com/_api/v2/members/authentication";
        private static string API_SONG = "https://2-dot-backup-server-002.appspot.com/_api/v2/songs";
        private static string API_MINE = "https://2-dot-backup-server-002.appspot.com/_api/v2/songs/get-mine";

        public async static Task<HttpResponseMessage> Sign_Up(Member member)
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(member), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(API_REGISTER, content);
            return response.Result;
        }

        public async static Task<HttpResponseMessage> Sign_In(string email, string password)
        {
            Dictionary<String, String> memberLogin = new Dictionary<string, string>();
            memberLogin.Add("email", email);
            memberLogin.Add("password", password);
            HttpClient httpClient = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(memberLogin), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(API_LOGIN, content);
            return response.Result;
        }

        public async static Task<HttpResponseMessage> Create_Song(Song song)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync("token.txt");
            string json = await FileIO.ReadTextAsync(file);
            JsonValue jsonValue = JsonValue.Parse(json);
            string token = jsonValue.GetObject().GetNamedString("token");
            Debug.WriteLine(token);

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
            var content = new StringContent(JsonConvert.SerializeObject(song),  Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(API_SONG, content);
            return response.Result;
        }

        public async static Task<HttpResponseMessage> Get_List_Songs()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync("token.txt");
            string json = await FileIO.ReadTextAsync(file);
            JsonValue jsonValue = JsonValue.Parse(json);
            string token = jsonValue.GetObject().GetNamedString("token");

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
            var response = httpClient.GetAsync(API_SONG, 0);
            
            Debug.WriteLine(response.Result);
            return response.Result;
        }
    }
}
