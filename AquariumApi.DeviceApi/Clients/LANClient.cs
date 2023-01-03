using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace AquariumApi.DeviceApi.Clients
{
    public class LANClient
    {
        public List<KeyValuePair<string, int>> _addresses { get; private set; }

        // Addresse that the ping will reach
        public string addressToBroadcast = "255.255.255.255";

        public int timeout = 1000;

        public LANClient()
        {
            _addresses = new List<KeyValuePair<string, int>>();
        }

        public void SendPing()
        {
            _addresses.Clear();

            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

            // 128 TTL and DontFragment
            PingOptions pingOption = new PingOptions(128, true);

            // Once the ping has reached his target (or has timed out), call this function
            ping.PingCompleted += ping_PingCompleted;

            byte[] buffer = Encoding.ASCII.GetBytes("ping");

            // Do not block the main thread
            ping.SendAsync(addressToBroadcast, timeout, buffer, pingOption, addressToBroadcast);
        }


        private void ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string address = (string)e.UserState;

            // For debug purpose
            _addresses.Add(new KeyValuePair<string, int>("127.0.0.1", 26000));

            if (e.Cancelled)
            {
                Console.WriteLine("Ping Canceled!");
            }

            if (e.Error != null)
            {
                Console.WriteLine(e.Error);
            }

            displayReply(e.Reply);
        }

        private void displayReply(PingReply reply)
        {
            if (reply != null)
            {
                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine("Pong from " + reply.Address);
                }
            }
            else
            {
                Console.WriteLine("No reply");
            }
        }
    }
}
