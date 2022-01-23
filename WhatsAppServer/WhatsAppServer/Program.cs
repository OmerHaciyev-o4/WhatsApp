using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WhatsAppServer.Models;

namespace WhatsAppServer
{
    class Program
    {
        public static TcpListener server { get; set; }
        private static readonly List<TcpClient> clientSockets = new List<TcpClient>();
        private static readonly List<string> clientConnectTimeGetInfo = new List<string>();
        private static readonly List<User> users = new List<User>();
        private static List<string> _usersPaths = new List<string>();
        private static List<BinaryWriter> writers = new List<BinaryWriter>();


        static void Main(string[] args)
        {
            Console.Title = "Server";
            SetupUsers();
            SetupServer();
            Console.WriteLine("User waiting...");
            Console.ReadLine();
            SaveAllInfo();
        }


        private static void SaveAllInfo()
        {
            for (int i = 0; i < users.Count; i++)
                SaveUser(users[i]);
        }

        private static void SaveUser(User user)
        {
            string path = Directory.GetCurrentDirectory() + $"\\Data\\{user.ID}";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "\\userInfo.log";
            if (!File.Exists(path))
                File.Create(path);

            var info = JsonConvert.SerializeObject(user);
            File.WriteAllText(path, info);
            File.AppendAllText("users.log", $"{path}");
        }

        private static void SetupUsers()
        {
            string path = Directory.GetCurrentDirectory();

            if (Directory.Exists(path + "\\Data"))
            {
                if (File.Exists(path + "\\Data\\users.log"))
                {
                    var usersPaths = File.ReadAllLines(path + "\\Data\\users.log");
                    if (usersPaths != null && usersPaths.Length > 0)
                    {
                        for (int i = 0; i < usersPaths.Length; i++)
                        {
                            _usersPaths.Add(usersPaths[i]);
                            GetUserData(usersPaths[i]);
                        }
                    }
                    else
                        return;
                }
                else
                    File.Create(path + "\\Data\\users.log");
            }
            else
            {
                Directory.CreateDirectory(path + "\\Data");
                File.Create(path + "\\Data\\users.log");
            }
        }

        private static void GetUserData(string user)
        {
            if (!string.IsNullOrEmpty(user))
            {
                var infos = File.ReadAllLines(user);
                string info = string.Empty;
                if (infos.Length > 0 || infos[0] != "")
                {
                    users.Add(new User()
                    {
                        ID = Convert.ToInt64(infos[0]),
                        ProfileImagePath = infos[1],
                        ProfileImageByte = File.ReadAllBytes(infos[1]),
                        Name = infos[2],
                        About = infos[3],
                        PhoneNumber = infos[4]
                    });
                }
            }
        }

        private static void SetupServer()
        {
            //string IP = File.ReadAllText("LocalAddress.txt");

            server = new TcpListener(new IPEndPoint(IPAddress.Any, 27001));
            server.Start();
            Console.WriteLine($"Server started. Server connect parameters: IP:{IPAddress.Any}, PORT: {27001}");

            Task.Run(() =>
            {
                while (true)
                {
                    var client = server.AcceptTcpClient();
                    Console.WriteLine($"Client:{client.Client.RemoteEndPoint} connected, waiting for request...");
                    clientSockets.Add(client);

                    Task.Run(() =>
                    {
                        BinaryReader reader = new BinaryReader(client.GetStream());
                        BinaryWriter writer = new BinaryWriter(client.GetStream());
                        writers.Add(writer);

                        var firstData = reader.ReadString();
                        if (!firstData.Contains("PhoneNumber") || !firstData.Contains("NewUser"))
                            clientConnectTimeGetInfo.Add(client.Client.RemoteEndPoint.ToString() + "|" + firstData);


                        while (true)
                        {
                            try
                            {
                                clientWork(reader.ReadString(), writer, client);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine($"Client forcefully disconnected: {client.Client.RemoteEndPoint}");

                                for (int i = 0; i < clientConnectTimeGetInfo.Count; i++)
                                {
                                    if (clientConnectTimeGetInfo[i].Contains(client.Client.RemoteEndPoint.ToString()))
                                    {
                                        clientConnectTimeGetInfo.RemoveAt(i);
                                        break;
                                    }
                                }
                                clientSockets.Remove(client);
                                writers.Remove(writer);
                                client.Close();
                                return;
                            }
                        }
                    });
                }
            });
        }

        private static void clientWork(string info, BinaryWriter writer, TcpClient client)
        {
            User user = null;
            string[] infosPhoneOrSMS = null;

            try
            {
                infosPhoneOrSMS = info.Split('|');
                user = JsonConvert.DeserializeObject<User>(info);
            }
            catch (Exception) { }

            if (infosPhoneOrSMS[0] == "close")
            {
                for (int i = 0; i < clientSockets.Count; i++)
                {
                    if (clientSockets[i].Client.RemoteEndPoint.ToString() == clientConnectTimeGetInfo[i].Split('|')[0])
                    {
                        clientSockets.RemoveAt(i);
                        clientConnectTimeGetInfo.RemoveAt(i);
                        break;
                    }
                }
                client.Close();
            }
            else if (infosPhoneOrSMS.Length > 1 && infosPhoneOrSMS[0] == "PhoneNumber")
            {
                bool state = false;
                foreach (var us in users)
                {
                    if (infosPhoneOrSMS[1] == us.PhoneNumber)
                    {
                        state = true;
                        writer.Write("Error");
                        break;
                    }
                }
                if (state == false)
                    writer.Write("Correct");
            }
            else if (user != null)
            {
                Random random = new Random();
                int id = random.Next(0, 10000);
                while (true)
                {
                    bool state = false;
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (id == users[i].ID)
                        {
                            state = true;
                            break;
                        }
                    }
                    if (!state)
                        break;

                    id = random.Next(0, 10000);
                }

                writer.Write(id.ToString());
                user.ID = id;
                users.Add(user);

                var currentPath = Directory.GetCurrentDirectory() + $"\\Data\\{id}";
                Directory.CreateDirectory(currentPath);
                var imagePath = currentPath + $"\\{user.ProfileImagePath}";
                currentPath += "\\info.log";
                var infos = new List<string>();
                infos.Add(user.ID.ToString());
                infos.Add(imagePath);
                infos.Add(user.Name);
                infos.Add(user.About);
                infos.Add(user.PhoneNumber);
                File.WriteAllLines(currentPath, infos.ToArray());
                File.WriteAllBytes(imagePath, user.ProfileImageByte);

                if (new FileInfo(Directory.GetCurrentDirectory() + "\\Data\\users.log").Length == 0)
                    File.WriteAllText(Directory.GetCurrentDirectory() + "\\Data\\users.log", currentPath + '\n');
                else
                    File.AppendAllText(Directory.GetCurrentDirectory() + "\\Data\\users.log", currentPath + '\n');
            }
            else if (infosPhoneOrSMS.Length > 1 && (infosPhoneOrSMS[0] == "voice" || infosPhoneOrSMS[0] == "image" || infosPhoneOrSMS[0] == "file" || infosPhoneOrSMS[0] == "sms"))
            {
                #region Code Bihand

                //string customPath = string.Empty;
                //string friendPath = string.Empty;
                //bool state = false;
                //bool state1 = false;

                //for (int i = 0; i < _usersPaths.Count; i++)
                //{
                //    var tempData = _usersPaths[i].Split('\\');
                //    for (int j = 0; j < tempData.Length; j++)
                //    {
                //        if (tempData[j] == "Data" && chatData.CustomID == Convert.ToInt64(tempData[j + 1]))
                //        {
                //            customPath = _usersPaths[i];
                //            state = true;
                //            break;
                //        }
                //        else if (tempData[j] == "Data" && chatData.FriendID == Convert.ToInt64(tempData[j + 1]))
                //        {
                //            friendPath = _usersPaths[i];
                //            state1 = true;
                //            break;
                //        }
                //    }
                //    if (state && state1)
                //        break;
                //}

                //var infos = File.ReadAllLines(File.ReadAllLines(customPath).Last());
                //state = false;
                //for (int i = 0; i < infos.Length; i++)
                //{
                //    var tempInfo = infos[i].Split('\\');
                //    for (int j = 0; j < tempInfo.Length; j++)
                //    {
                //        if (tempInfo[j] == "Friends" && Convert.ToInt64(tempInfo[j + 1]) == chatData.FriendID)
                //        {
                //            customPath = infos[i];
                //            state = true;
                //            break;
                //        }
                //    }
                //    if (state)
                //        break;
                //}

                //infos = File.ReadAllLines(File.ReadAllLines(friendPath).Last());
                //state1 = false;
                //for (int i = 0; i < infos.Length; i++)
                //{
                //    var tempInfo = infos[i].Split('\\');
                //    for (int j = 0; j < tempInfo.Length; j++)
                //    {
                //        if (tempInfo[j] == "Friends" && Convert.ToInt64(tempInfo[j + 1]) == chatData.CustomID)
                //        {
                //            friendPath = infos[i];
                //            state1 = true;
                //            break;
                //        }
                //    }
                //    if (state1)
                //        break;
                //}

                //infos = File.ReadAllLines(customPath);
                //var countStrArr = infos[infos.Length - 1].Split('|');
                //var soundCount = Convert.ToInt64(countStrArr[1]);
                //soundCount++;
                //infos[infos.Length - 1] = $"sound|{soundCount}";
                //File.WriteAllLines(customPath, infos);
                //infos = File.ReadAllLines(friendPath);
                //infos[infos.Length - 1] = $"sound|{soundCount}";
                //File.WriteAllLines(friendPath, infos);

                //customPath = customPath.Remove(customPath.IndexOf("info.log"));
                //friendPath = friendPath.Remove(friendPath.IndexOf("info.log"));

                //var portState = false;
                //for (int i = 0; i < clientConnectTimeGetInfo.Count; i++)
                //{
                //    var infosPath = clientConnectTimeGetInfo[i].Split(':');
                //    if (Convert.ToInt64(infosPath[2]) == chatData.CustomID)
                //        portState = true;
                //}

                //if (!portState)
                //    clientConnectTimeGetInfo.Add(client.RemoteEndPoint.ToString() + ":" + chatData.CustomID);
                #endregion

                string fileType = infosPhoneOrSMS[0];
                var chatData = JsonConvert.DeserializeObject<ChatDataSend>(infosPhoneOrSMS[1]);

                string customPath = $"{Directory.GetCurrentDirectory()}\\Data\\{chatData.CustomID}\\Friends\\{chatData.FriendID}\\";
                string friendPath = $"{Directory.GetCurrentDirectory()}\\Data\\{chatData.FriendID}\\Friends\\{chatData.CustomID}\\";

                var time = DateTime.Now;

                if (infosPhoneOrSMS[0] == "sms")
                {
                    File.AppendAllText(customPath + "chat.log", $"me: {Encoding.ASCII.GetString(chatData.DataBytes)}| |{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                    File.AppendAllText(friendPath + "chat.log", $"you: {Encoding.ASCII.GetString(chatData.DataBytes)}| |{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                }
                else if (infosPhoneOrSMS[0] == "voice")
                {
                    File.WriteAllBytes(customPath + $"{chatData.FileName}", chatData.DataBytes);
                    File.WriteAllBytes(friendPath + $"{chatData.FileName}", chatData.DataBytes);

                    File.AppendAllText(customPath + "chat.log", $"me: voice|{customPath}{chatData.FileName}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                    File.AppendAllText(friendPath + "chat.log", $"you: voice|{friendPath}{chatData.FileName}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                }
                else if (infosPhoneOrSMS[0] == "image")
                {
                    File.WriteAllBytes(customPath + chatData.FileName, chatData.DataBytes);
                    File.WriteAllBytes(friendPath + chatData.FileName, chatData.DataBytes);

                    File.AppendAllText(customPath + "chat.log", $"me: image|{customPath}{chatData.FileName}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                    File.AppendAllText(friendPath + "chat.log", $"you: image|{customPath}{chatData.FileName}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                }
                else if (infosPhoneOrSMS[0] == "file")
                {
                    File.WriteAllBytes(customPath + chatData.FileName, chatData.DataBytes);
                    File.WriteAllBytes(friendPath + chatData.FileName, chatData.DataBytes);

                    File.AppendAllText(customPath + "chat.log", $"me: file|{customPath}{chatData.FileName}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                    File.AppendAllText(friendPath + "chat.log", $"you: file|{customPath}{chatData.FileName}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                }

                try
                {
                    var ip = string.Empty;
                    TcpClient friendClient = null;
                    for (int i = 0; i < clientConnectTimeGetInfo.Count; i++)
                    {
                        var temp = clientConnectTimeGetInfo[i].Split('|');
                        if (Convert.ToInt64(temp[1]) == chatData.FriendID && temp.Length == 3 && temp[2] == "App")
                        {
                            ip = temp[0];
                            break;
                        }
                    }

                    for (int i = 0; i < writers.Count; i++)
                    {
                        if (clientSockets[i].Client.RemoteEndPoint.ToString() == ip)
                        {
                            writers[i].Write(info);
                            //friendClient = clientSockets[i];

                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Client not connecting");
                }
            }
            else if (infosPhoneOrSMS.Length > 1 && infosPhoneOrSMS[0] == "GetUser")
            {
                var data = JsonConvert.DeserializeObject<ChatDataSend>(infosPhoneOrSMS[1]);
                var phoneNumber = Encoding.ASCII.GetString(data.DataBytes);

                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].PhoneNumber == phoneNumber)
                    {
                        writer.Write(JsonConvert.SerializeObject(users[i]));

                        for (int j = 0; j < users.Count; j++)
                        {
                            if (users[j].ID == data.CustomID)
                            {
                                user = users[j];
                                break;
                            }
                        }
                        TcpClient client1 = null;
                        for (int j = 0; j < clientConnectTimeGetInfo.Count; j++)
                        {
                            if (clientConnectTimeGetInfo[j].Contains(users[i].ID.ToString()) && clientConnectTimeGetInfo[j].Contains("App"))
                            {
                                client1 = clientSockets[j];
                                break;
                            }
                        }
                        if (client1 != null)
                        {
                            writer = new BinaryWriter(client1.GetStream());
                            writer.Write("NewFriend|" + JsonConvert.SerializeObject(user));
                        }


                        var customPath = Directory.GetCurrentDirectory() + $"\\Data\\{data.CustomID}\\";
                        var friendPath = Directory.GetCurrentDirectory() + $"\\Data\\{users[i].ID}\\";

                        //Create required file
                        customPath += "Friends";
                        friendPath += "Friends";

                        if (!Directory.Exists(customPath))
                        {
                            Directory.CreateDirectory(customPath);
                            File.WriteAllText(customPath + "\\usersInfo.log", "");
                        }

                        if (!Directory.Exists(friendPath))
                        {
                            Directory.CreateDirectory(friendPath);
                            File.WriteAllText(friendPath + "\\usersInfo.log", "");
                        }

                        customPath += $"\\{users[i].ID}";
                        friendPath += $"\\{data.CustomID}";
                        Directory.CreateDirectory(customPath);
                        Directory.CreateDirectory(friendPath);

                        var infos = new List<string>();
                        infos.Add(users[i].ID.ToString());
                        infos.Add(users[i].ProfileImagePath);
                        infos.Add(users[i].Name);
                        infos.Add(users[i].About);
                        infos.Add(users[i].PhoneNumber);
                        File.WriteAllLines(customPath + $"\\info.log", infos.ToArray());
                        File.WriteAllText(customPath + $"\\chat.log", "");



                        infos = new List<string>();
                        infos.Add(user.ID.ToString());
                        infos.Add(user.ProfileImagePath);
                        infos.Add(user.Name);
                        infos.Add(user.About);
                        infos.Add(user.PhoneNumber);
                        File.WriteAllLines(friendPath + $"\\info.log", infos.ToArray());
                        File.WriteAllText(friendPath + $"\\chat.log", "");



                        break;
                    }
                }
            }
        }
    }
}