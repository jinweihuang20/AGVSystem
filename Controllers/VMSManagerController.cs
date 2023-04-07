﻿using AGVSystem.VMS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AGVSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VMSManagerController : ControllerBase
    {

        [HttpGet("AGVOnline")]
        public async Task<IActionResult> AGVOnline(string agv_name)
        {
            (bool success, string message) result = await VMSManager.AGVOnline(agv_name);
            return Ok(new { Success = result.success, Message = result.message });
        }
        [HttpGet("AGVOffline")]
        public async Task<IActionResult> AGVOffline(string agv_name)
        {
            (bool success, string message) result = await VMSManager.AGVOffline(agv_name);
            return Ok(new { Success = result.success, Message = result.message });
        }


        [HttpGet("/ws/VMSStatus")]
        public async Task GetVMSStatus()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var websocket_client = await HttpContext.WebSockets.AcceptWebSocketAsync();
                while (websocket_client.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    Thread.Sleep(1000);

                    byte[] rev_buffer = new byte[4096];

                    websocket_client.ReceiveAsync(new ArraySegment<byte>(rev_buffer), CancellationToken.None);

                    var data = VMSManager.GetVMSViewData();
                    await websocket_client.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data))), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

    }
}
