using System;
using System.Collections.Generic;
using System.Linq;

namespace LittleCuteBlockchain.Core
{
    public class BlockService
    {
        private List<Block> _blocks;

        public BlockService()
        {
            var genesisBlock = new Block(
                0,
                "64AC7FC01D90D297FE7975E29178BD8764481B6D5C12A8692A0BCD0B11094C87",
                "the genesis block",
                new DateTime(2018,2,7)
            );
            _blocks = new List<Block> { genesisBlock };
        }

        public List<Block> GetBlocks()
        {
            return _blocks;
        }

        public Block GenerateNewBlock(string data)
        {
            var lastBlock = _blocks.Last();
            var newBlock = new Block(_blocks.Count, lastBlock.Hash, data, DateTime.UtcNow);
                   
            return AddNewBlock(newBlock);
        }


        public Block AddNewBlock(Block block)
        {
            if (CheckBlockValidity(block, _blocks.Last()))
            {
                _blocks.Add(block);
                return block;
            }

            return null;
        }

        public void ReplaceChain(IList<Block> chain)
        {
            if (CheckChainValidity(chain) && chain.Count > _blocks.Count)
                _blocks = chain.ToList();
        }
             
        public bool CheckBlockValidity(Block newBlock, Block previousBlock)
        {
            if (previousBlock.Index != newBlock.Index - 1)
                return false;
            if (previousBlock.Hash != newBlock.PreviousHash)
                return false;
            if (newBlock.CalculateHash() != newBlock.Hash)
                return false;

            return true;
        }

        public bool CheckChainValidity(IList<Block> chain)
        {
            var genesis = _blocks[0].ToJson();
            var anotherGenesis = chain[0].ToJson();
            if (genesis != anotherGenesis)
                return false;

            for (var i = 1; i < chain.Count; i++)
            {
                if (!CheckBlockValidity(chain[i], chain[i - 1]))
                    return false;
            }

            return true;
        }
    }
}
