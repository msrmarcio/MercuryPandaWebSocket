using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using Mercury.PandaSerial;
using System.Runtime.CompilerServices;

namespace WebSocketsSample.Controllers;

#region snippet_Controller_Connect
public class WebSocketController : ControllerBase
{
    static SerialComm serial;

    public WebSocketController()
    {
        serial = new SerialComm();
        //serial.OpenCloseCom("COM1");
    }

    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    #endregion

    private static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            /* CHAMADA DO SOCKET SERIAL,, PARA EXECUÇÃO DOS COMANDOS */
            var res = Teste(buffer);
            serial.OpenCloseCom("COM1");

            serial.SendDataBySerial(res);


            var bufferRetorno = new byte[1024 * 4];

            byte[] MainArray = new byte[4096];
            string resposta = "Isso é um Retorno";
            byte[] String = Encoding.ASCII.GetBytes(resposta);
            Array.Copy(String, 0, MainArray, 0, String.Length);
            MainArray = MainArray.Where(v => v > 0).ToArray();




            await webSocket.SendAsync(
                new ArraySegment<byte>(MainArray, 0, resposta.Count()),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(MainArray), CancellationToken.None);
            /*
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            */

        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    private static string Teste(byte[] buffer)
    {
        serial.SendData(System.Text.Encoding.ASCII.GetString(buffer).ToUpper());

        return System.Text.Encoding.ASCII.GetString(buffer);
    }
}
