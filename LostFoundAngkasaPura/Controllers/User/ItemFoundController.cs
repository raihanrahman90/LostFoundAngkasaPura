using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.ItemCategory;
using LostFoundAngkasaPura.DTO.ItemFound;
using LostFoundAngkasaPura.Service.ItemCategory;
using LostFoundAngkasaPura.Service.ItemClaim;
using LostFoundAngkasaPura.Service.ItemFound;
using Microsoft.AspNetCore.Mvc;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Controllers.User
{
    [Route("Item-Found")]
    [ApiController]
    public class ItemFoundController : ControllerBase
    {
        private readonly IItemCategoryService _itemCategory;
        private readonly IItemFoundService _itemFound;
        private readonly IItemClaimService _itemClaim;
        public ItemFoundController(IItemFoundService itemFound, IItemCategoryService itemCategory, IItemClaimService itemClaim)
        {
            _itemCategory = itemCategory;
            _itemFound = itemFound;
            _itemClaim = itemClaim; 
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
        public async Task<IActionResult> GetListItemFound(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? category = null,
            [FromQuery] DateTime? foundDate = null)
        {
            var result = await _itemFound.GetListItemFound(page, size, name, category, ItemFoundStatus.Found, foundDate);
            return new OkObjectResult(new DefaultResponse<Pagination<ItemFoundResponseDTO>>(result));
        }

        [HttpGet("{itemId}")]
        [ProducesResponseType(typeof(DefaultResponse<ItemFoundResponseDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetDetailItem(
            [FromRoute] string itemId)
        {
            var result = await _itemFound.GetDetailItemFound(itemId);
            return new OkObjectResult(new DefaultResponse<ItemFoundResponseDTO>(result));
        }

    }
}
