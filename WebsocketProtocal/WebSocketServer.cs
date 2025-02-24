using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Services.Description;
using WebsocketProtocal;
using WebsocketProtocal.Models;

public class WebSocketServer
{
    private readonly HttpListener _httpListener;
    private readonly ConcurrentDictionary<WebSocket, string> _clients = new ConcurrentDictionary<WebSocket, string>();

    private List<string> strClients = new List<string>();
    Dictionary<string, string> lstDevice = new Dictionary<string, string>();
    private readonly ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
    private Timer processingTimer;
    private WebSocket firtWebsocket;
    private List<tb_Device> lstDevices = new List<tb_Device>();
    private bool isfirstConnect = false;
    public WebSocketServer(string uriPrefix)
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(uriPrefix);
    }

    public async Task Start()
    {
        _httpListener.Start();
        System.Diagnostics.Debug.WriteLine("WebSocket server started...");

        while (true)
        {
            var context = await _httpListener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                string clientEndpoint = context.Request.RemoteEndPoint.ToString();
                _clients.TryAdd(webSocketContext.WebSocket, clientEndpoint);

                string message = "";
               
                HandleWebSocket(webSocketContext.WebSocket, clientEndpoint);
            }
        }
    }

    private async Task sendFirtClient(WebSocket webSocket,string message)
    { 
        var msgBuffer = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(new ArraySegment<byte>(msgBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    private async Task SendWelcomeMessage(WebSocket webSocket)
    {
        var welcomeMessage = "Hello! Welcome to the WebSocket server!";
        var msgBuffer = Encoding.UTF8.GetBytes(welcomeMessage);
        await webSocket.SendAsync(new ArraySegment<byte>(msgBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    bool readData = true;
    private async void HandleWebSocket(WebSocket webSocket,string clientEndpoint)
    {
        if (firtWebsocket == null)
        {
            firtWebsocket = webSocket;
            //Trường hợp kết nối lần đầu
            if (isfirstConnect == false)
                isfirstConnect = true;
            else
            {
                //trường hợp reconnect
                if (isfirstConnect == true)
                {
                    foreach(var item in lstDevices)
                    {
                        var getsplit = item.DeviceName.Split(',');
                        await sendFirtClient(firtWebsocket, getsplit[1] + " has connected");
                    }
                    
                }
            }
                

        }
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var getname = lstDevice.FirstOrDefault(m => m.Value == clientEndpoint).Key;
                    _clients.TryRemove(webSocket, out _);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed bxy the client", CancellationToken.None);
                    if (webSocket != firtWebsocket)
                    {
                        lstDevice.Remove(getname);
                        tb_Device tb_Device = lstDevices.Where(m => m.DeviceName.Contains(getname)).FirstOrDefault();
                        if (tb_Device != null)
                            lstDevices.Remove(tb_Device);
                        await firtWebsocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(getname + " has disconnected")), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                        firtWebsocket = null;
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    System.Diagnostics.Debug.WriteLine("Received from client: " + message);
                    if (message.Contains("The device"))
                    {
                         StartProcessingTimer();
                        var getsplit = message.Split(',');
                        if (!lstDevice.ContainsKey(getsplit[1]))
                        {
                            lstDevice.Add(getsplit[1], clientEndpoint);
                        }

                        //add device vào lstDevices mục đích để clientManager load lại
                        tb_Device tb_Device = new tb_Device();
                        tb_Device.DeviceName = message;
                        lstDevices.Add(tb_Device);
                        await sendFirtClient(firtWebsocket, getsplit[1] + " has connected");
                    }
                    else
                    {
                        if (readData == true)
                        {
                            messageQueue.Enqueue(message);

                        }
                    }
                   
                    //if (readData == true)
                    //{
                    //    if (message.Contains("The device"))
                    //    {
                    //        var getsplit = message.Split(',');
                    //        if (!lstDevice.ContainsKey(getsplit[1]))
                    //        {
                    //            lstDevice.Add(getsplit[1], clientEndpoint);
                    //        }
                    //        await sendFirtClient(_clients.Keys.FirstOrDefault(), getsplit[1] + " has connected");
                    //    }
                    //    foreach (var client in _clients.Keys)
                    //    {
                    //        try
                    //        {
                    //            if (client.State == WebSocketState.Open && client == webSocket)
                    //            {
                    //                if (!message.Contains("The device"))
                    //                    await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("[Callback from server : ] " + message)), WebSocketMessageType.Text, true, CancellationToken.None);
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            System.Diagnostics.Debug.WriteLine($"Error sending to client: {ex.Message}");
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in WebSocket communication: {ex.Message}");
                break;
            }
        }
    }
    private async void ProcessMessages2(object state)
    {
        readData = true;
        processingTimer = new Timer(ProcessMessages2, null, 10000, Timeout.Infinite); // Xử lý sau 10 giây
    }
    private async void ProcessMessages(object state)
    {
        var messagesToProcess = new List<string>();

        // Lấy các tin nhắn từ hàng đợi
        while (messageQueue.TryDequeue(out var message))
        {
            messagesToProcess.Add(message);
        }
        if (messagesToProcess.Count > 0)
        {
            List<Tag> lstTag = new List<Tag>();
            // Xử lý các tin nhắn
            foreach (var msg in messagesToProcess)
            {

                try
                {
                    Tag tag = JsonConvert.DeserializeObject<Tag>(msg);
                    lstTag.Add(tag);
                }
                catch(Exception ex)
                {

                }
            }
            var highestPointsByType = lstTag
         .GroupBy(a => a.TagID) // Nhóm theo TagID
         .Select(g => g.OrderByDescending(a => a.Timestamp).FirstOrDefault()) // Lấy đối tượng có Timestamp mới nhất
         .ToList(); // Chuyển đổi kết quả về danh sách

            // Chuyển đổi danh sách sang JSON

            string jsonResult = JsonConvert.SerializeObject(highestPointsByType, Formatting.Indented);
            NotifyClients(jsonResult);
            processingTimer = new Timer(ProcessMessages, null, 3000, Timeout.Infinite); // Xử lý sau 10 giây
        }
    }
    // Khởi động timer
    private void StartProcessingTimer()
    {
        if (processingTimer == null)
        {
            processingTimer = new Timer(ProcessMessages, null,3000, Timeout.Infinite); // Xử lý sau 10 giây
        }
    }
    private async Task NotifyClients(string notification)
    {
        var notificationBuffer = Encoding.UTF8.GetBytes(notification);
        foreach (var client in _clients.Keys)
        {
            if (client.State == WebSocketState.Open && client == firtWebsocket)
            {
                await client.SendAsync(new ArraySegment<byte>(notificationBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
    public string[] GetClientList()
    {
        return _clients.Values.ToArray();
    }
    public class Tag
    {
        public string TagID { get; set; }
        public int Timestamp { get; set; }
        public List<Anchor> Anchors { get; set; }
    }

    public class Anchor
    {
        public string Id { get; set; }
        public float Distance { get; set; }
    }

}