using System.Collections.Generic;

namespace LittleCuteBlockchain.Core
{
    public enum MessageType
    {
        System,
        BlockMined,
        GetLastBlock,
        CompareLastBlock,
        ReplaceChain,
        FetchChain
    }

    public class P2PMessage
    {
        public MessageType Type { get; set; }
        public List<Block> Chain { get; set; }
    }
}
