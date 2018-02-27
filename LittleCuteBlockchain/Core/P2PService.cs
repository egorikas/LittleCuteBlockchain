using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using LittleCuteBlockchain.P2P;
using Newtonsoft.Json;

namespace LittleCuteBlockchain.Core
{
    public class P2PService : WebSocketHandler
    {
        private readonly BlockService _blockService;
        public List<string> Endpoints { get; }
        public List<WebSocketSharp.WebSocket> Peers { get; }

        public P2PService(
            WebSocketConnectionManager webSocketConnectionManager,
            BlockService blockService
        ) : base(webSocketConnectionManager)
        {
            _blockService = blockService;
            Peers = new List<WebSocketSharp.WebSocket>();
            Endpoints = new List<string>();
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            await HandleMessage(socketId, null, message);
        }


        public async Task AddNewEndpoint(string endpoint)
        {
            Endpoints.Add(endpoint);
            var ws = new WebSocketSharp.WebSocket(endpoint);

            ws.OnMessage += async (sender, e) =>
            {
                await HandleMessage(null, sender as WebSocketSharp.WebSocket, e.Data);
            };

            ws.Connect();
            await SendMessage(null, ws, new P2PMessage()
            {
                Type = MessageType.GetLastBlock
            }.ToJson());
            Peers.Add(ws);
        }

        public void NotififyAboutMinedBlock(Block block)
        {
            var message = new P2PMessage
            {
                Chain = new List<Block> { block },
                Type = MessageType.BlockMined
            };

            foreach (var peer in Peers)
            {
                if (!peer.IsAlive)
                    peer.Connect();
                peer.Send(message.ToJson());
            }
        }

        private async Task SendMessage(string soketId, WebSocketSharp.WebSocket soket, string message)
        {
            if (!string.IsNullOrEmpty(soketId))
            {
                await SendMessageAsync(soketId, message);
                return;
            }

            soket?.Send(message);
        }


        public async Task HandleMessage(string soketId, WebSocketSharp.WebSocket soket, string message)
        {
            var parsedMessage = JsonConvert.DeserializeObject<P2PMessage>(message);

            if (parsedMessage.Type == MessageType.BlockMined && parsedMessage.Chain.LastOrDefault() != null)
                _blockService.AddNewBlock(parsedMessage.Chain.LastOrDefault());

            if (parsedMessage.Type == MessageType.ReplaceChain && parsedMessage.Chain.Count > 0)
                _blockService.ReplaceChain(parsedMessage.Chain);

            if (parsedMessage.Type == MessageType.GetLastBlock)
            {
                var blocks = _blockService.GetBlocks();
                var compareLastBlock = new P2PMessage
                {
                    Type = MessageType.CompareLastBlock,
                    Chain = new List<Block> { blocks.Last() }
                };
                await SendMessage(soketId, soket, compareLastBlock.ToJson());
            }

            if (parsedMessage.Type == MessageType.CompareLastBlock)
            {
                var blocks = _blockService.GetBlocks();
                var lastIndex = blocks.Last().Index;
                var anotherLastIndex = parsedMessage.Chain.Last().Index;
                if (lastIndex != anotherLastIndex)
                {
                    if (lastIndex > anotherLastIndex)
                        await SendMessage(soketId, soket, new P2PMessage
                        {
                            Type = MessageType.ReplaceChain,
                            Chain = _blockService.GetBlocks()
                        }.ToJson());
                    else
                        await SendMessage(soketId, soket, new P2PMessage
                        {
                            Type = MessageType.FetchChain
                        }.ToJson());
                }
            }

            if (parsedMessage.Type == MessageType.FetchChain)
            {
                var compareLastBlock = new P2PMessage()
                {
                    Type = MessageType.ReplaceChain,
                    Chain = _blockService.GetBlocks()
                };
                await SendMessage(soketId, soket, compareLastBlock.ToJson());
            }
        }
    }
}
