using LostFound.Authorize;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.ItemCategory;
using LostFoundAngkasaPura.DTO.ItemFound;
using LostFoundAngkasaPura.Service.ItemCategory;
using LostFoundAngkasaPura.Service.ItemFound;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LostFoundAngkasaPura.Controllers.Admin
{
    [Route("Admin/Item-Found")]
    [ApiController]
    public class AdminItemFoundController : ControllerBase
    {
        private readonly IItemCategoryService _itemCategory;
        private readonly IItemFoundService _itemFound;
        public AdminItemFoundController(IItemFoundService itemFound, IItemCategoryService itemCategory)
        {
            _itemCategory = itemCategory;
            _itemFound = itemFound;
        }

        [HttpGet("category")]
        [ProducesResponseType(typeof(DefaultResponse<List<ItemCategoryResponseDTO>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetListCategory()
        {
            var result = await _itemCategory.GetListCategory();
            return new OkObjectResult(new DefaultResponse<List<ItemCategoryResponseDTO>>(result));
        }

        [HttpGet]
        [ProducesResponseType(typeof(DefaultResponse<Pagination<ItemFoundResponseDTO>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, true)]
        public async Task<IActionResult> GetListItemFound(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? category = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? foundDate = null)
        {
            var result = await _itemFound.GetListItemFound(page, size, name, category, status, foundDate);
            return new OkObjectResult(new DefaultResponse<Pagination<ItemFoundResponseDTO>>(result));
        }


        [HttpPost]
        [ProducesResponseType(typeof(DefaultResponse<ItemFoundResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, true)]
        public async Task<IActionResult> CreateItemFound(
            [FromBody] ItemFoundCreateRequestDTO request)
        {
            var userId = User.Claims.FirstOrDefault(t => t.Type.Equals("Id")).Value;
            var result = await _itemFound.CreateItemFound(request, userId);
            return new OkObjectResult(new DefaultResponse<ItemFoundResponseDTO>(result));
        }

        [HttpGet("{itemId}")]
        [ProducesResponseType(typeof(DefaultResponse<ItemFoundResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [CustomAuthorize(true, true)]
        public async Task<IActionResult> GetDetailItem(
            [FromRoute] string itemId)
        {
            var result = await _itemFound.GetDetailItemFound(itemId);
            return new OkObjectResult(new DefaultResponse<ItemFoundResponseDTO>(result));
        }

    }
}
