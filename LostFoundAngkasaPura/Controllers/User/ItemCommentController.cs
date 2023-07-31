using LostFound.Authorize;
using LostFoundAngkasaPura.DAL.Model;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.ItemCategory;
using LostFoundAngkasaPura.DTO.ItemClaim;
using LostFoundAngkasaPura.DTO.ItemFound;
using LostFoundAngkasaPura.Service.ItemCategory;
using LostFoundAngkasaPura.Service.ItemClaim;
using LostFoundAngkasaPura.Service.ItemFound;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Xml.Linq;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Controllers
{
    [Route("Item-Claim")]
    [ApiController]
    public class ItemCommentController : ControllerBase
    {

        public ItemCommentController()
        {
        }

    }
}
