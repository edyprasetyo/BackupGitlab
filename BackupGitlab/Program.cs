using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;

namespace BackupGitlab
{
    class Program
    {
        const string gitlabToken = "YOUR_GITLAB_TOKEN";
        const string folderBackup = "\\\\10.200.0.65\\Data\\MIS\\Backup\\GitRepositories";
        static void Main(string[] args)
        {
            var oListRoot = GetGitlabRepo();
            bool exists = Directory.Exists(folderBackup);
            if (!exists)
            {
                Directory.CreateDirectory(folderBackup);
            }
            else
            {
                Directory.Delete(folderBackup, true);
            }

            foreach (var o in oListRoot)
            {
                Console.Clear();
                Console.WriteLine(o.name);
                //git clonse using git clone https://oauth2:<access_token>@gitlab.com/<user>/<repository>.git
                var url = o.path_with_namespace + ".git";
                var folder = folderBackup + "\\" + o.name;
                var command = "git clone https://oauth2:" + gitlabToken + "@gitlab.com/" + url + " " + folder;
                Console.WriteLine(command);
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/C " + command,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                process.WaitForExit();
            }
            Environment.Exit(0);
            return;
        }

        public static List<Root> GetGitlabRepo()
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string url = "https://gitlab.com/api/v4/projects?per_page=1000&visibility=private";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Private-Token", gitlabToken);
                var response = client.GetAsync(url).Result;
                var responseJSON = response.Content.ReadAsStringAsync().Result;
                var oRoot = JsonConvert.DeserializeObject<List<Root>>(responseJSON);
                oRoot.Sort((x, y) => string.Compare(x.name, y.name));
                return oRoot;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public class Namespace
        {
            public int id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public string kind { get; set; }
            public string full_path { get; set; }
            public object parent_id { get; set; }
            public string avatar_url { get; set; }
            public string web_url { get; set; }
        }

        public class Root
        {
            public int id { get; set; }
            public string description { get; set; }
            public string name { get; set; }
            public string name_with_namespace { get; set; }
            public string path { get; set; }
            public string path_with_namespace { get; set; }
            public DateTime created_at { get; set; }
            public string default_branch { get; set; }
            public List<object> tag_list { get; set; }
            public List<object> topics { get; set; }
            public string ssh_url_to_repo { get; set; }
            public string http_url_to_repo { get; set; }
            public string web_url { get; set; }
            public string readme_url { get; set; }
            public object avatar_url { get; set; }
            public int forks_count { get; set; }
            public int star_count { get; set; }
            public DateTime last_activity_at { get; set; }
            public Namespace @namespace { get; set; }
        }

    }
}
