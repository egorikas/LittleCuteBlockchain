using LittleCuteBlockchain.Controllers.Requests;
using LittleCuteBlockchain.Core;
using Microsoft.AspNetCore.Mvc;

namespace LittleCuteBlockchain.Controllers
{
    [Route("/blocks")]
    public class BlockController : Controller
    {
        private readonly BlockService _blockChain;
        private readonly P2PService _p2PService;

        public BlockController(BlockService blockChain, P2PService p2PService)
        {
            _blockChain = blockChain;
            _p2PService = p2PService;
        }

        [HttpGet]
        public ObjectResult GetBlocks()
        {
            return Ok(_blockChain.GetBlocks());
        }

        [HttpPost]
        [Route("mine")]
        public ObjectResult MineBlock([FromBody]MineBlockRequest data)
        {
            var block = _blockChain.GenerateNewBlock(data.Data);
            if (block != null)
                _p2PService.NotififyAboutMinedBlock(block);
            return Ok(block);
        }
    }
}