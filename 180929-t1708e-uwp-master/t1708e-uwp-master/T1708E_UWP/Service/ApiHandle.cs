using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using T1708E_UWP.Entity;

namespace T1708E_UWP.Service
{
    class ApiHandle
    {
        private static string API_URL = "http://1-dot-backup-server-002.appspot.com/member/register";
        public static bool Sign_Up(Member member) {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync(API_URL, content).Result;
            Debug.WriteLine(result);
            return true;
        }
    }
}
