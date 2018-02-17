using System.Threading.Tasks;
using LittleCuteBlockchain.Controllers.Requests;
using LittleCuteBlockchain.Core;
using Microsoft.AspNetCore.Mvc;

namespace LittleCuteBlockchain.Controllers
{
    [Route("/peers")]
    public class PeerController : Controller
    {
        private readonly P2PService _p2PService;

        public PeerController(P2PService p2PService)
        {
            _p2PService = p2PService;
        }

        [HttpGet]
        public ObjectResult GetEndpoints()
        {
            return Ok(_p2PService.Endpoints);
        }

        [HttpPost]
        public async Task Add([FromBody]AddPeerRequest request)
        {
            await _p2PService.AddNewEndpoint(request.Endpoint);
        }

    }
}
