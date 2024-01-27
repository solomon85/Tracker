using GPS_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GPS_Tracker
{
    public class ConnectionThread
    {
        ApplicationDataContext db = new ApplicationDataContext();
        public static Dictionary<int, RSocket> allSocket = new Dictionary<int, RSocket>();
        private static int rand = 0;
        public int id = 0;
        public string imei = "";
        public RSocket client; 
        ParserBase parser = null;
        public int Decode(List<byte> receiveBytes, string IMEI)
        {
            //Get codec ID and initialize appropriate parser
            var codecID = Convert.ToInt32(receiveBytes.Skip(8).Take(1).ToList()[0]);
            switch (codecID)
            {
                case 8:
                    parser = new FMXXXX_Parser(true);
                    break;
                case 7:
                    //parser = new GH3000Parser(trur);
                    break;
                default:
                    throw new Exception("Unsupported device type code: " + codecID);
            }
            parser.OnDataReceive += ShowDiagnosticInfo;
            int result = parser.DecodeAVL(receiveBytes, IMEI);
            parser.OnDataReceive -= ShowDiagnosticInfo;
            return result;
        }
        public void ShowDiagnosticInfo(string message)
        {
            Console.WriteLine(message);
        }
        public void SetNewId()
        {
            rand++;
            this.id = rand;
        }
        public string ByteArrayToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
        public void client_ReceiveData(RSocket sender, ReceivedData e)
        {
            if (e == null)
            {
                allSocket.Remove(id);
                client.Close();
                return;
            }
            try
            {
                if (e.ByteData.Length == 17)
                {
                    Console.WriteLine("Received Byte : " + ByteArrayToHexString(e.ByteData) + "  " + e.ByteData.Length);
                    byte[] byteData = new byte[e.ByteData.Length - 2];
                    Array.Copy(e.ByteData, 2, byteData, 0, e.ByteData.Length - 2);
                    imei = Encoding.ASCII.GetString(byteData);
                    Console.WriteLine("Received Data : " + imei);

                    var deviceId = Redis.GetCacheData<long>(imei);
                    if (deviceId == 0)
                    {
                        var blockedImei = Redis.GetCacheData("BlockedImei_" + imei);
                        if (String.IsNullOrEmpty(blockedImei))
                        {
                            var _imei = Convert.ToInt64(imei);
                            var device = db.Devices.FirstOrDefault(d => d.DeviceIMEI == _imei && d.DeviceValidDateTo > DateTime.Now);
                            if (device == null)
                            {
                                Redis.SetCacheData("BlockedImei_" + imei, DateTime.Now.AddMinutes(30).ToString());
                                allSocket.Remove(id);
                                client.Close();
                                return;
                            }
                            Redis.SetCacheData(imei, device.DeviceId.ToString());
                        }
                        else
                        {
                            allSocket.Remove(id);
                            client.Close();
                            return;
                        }
                    }
                    var res = new byte[] { 1 };
                    client.Send(res);
                    Console.WriteLine("Send Data : " + ByteArrayToHexString(res));

                }
                else
                {
                    Console.WriteLine("Received Byte : " + ByteArrayToHexString(e.ByteData) + "  " + e.ByteData.Length);
                    var data = new List<byte>();
                    data.AddRange(e.ByteData);
                    var responseCount = Decode(data, imei);


                    var res = BitConverter.GetBytes(responseCount);
                    Array.Reverse(res);
                    client.Send(res);

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
