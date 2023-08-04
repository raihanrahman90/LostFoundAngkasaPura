using AutoMapper;
using LostFoundAngkasaPura.DAL.Model;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO.ItemComment;
using LostFoundAngkasaPura.Utils;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.Service.ItemComment
{
    public class ItemCommentService : IItemCommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UploadLocation _uploadLocation;
        private IMapper _mapper;

        public ItemCommentService(IUnitOfWork uow, UploadLocation uploadLocation)
        {
            _unitOfWork = uow;    
            _uploadLocation = uploadLocation;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.ItemComment, ItemCommentResponseDTO>();
            }));
        }

        public async Task<ItemCommentResponseDTO> AddComment(ItemCommentCreateRequestDTO request, string? userId, string? adminId)
        {
            var comment = new DAL.Model.ItemComment()
            {
                UserId = userId,
                AdminId = adminId,
                Value = request.Value,
                CreatedBy = userId??adminId,
                ItemClaimId = request.ItemClaimId,
            };
            if (!String.IsNullOrWhiteSpace(request.ImageBase64))
            {
                var (extension, image) = Utils.GeneralUtils.GetDetailImageBase64(request.ImageBase64);
                var fileName = $"{userId}-{DateTime.Now.ToString("yyyy-MM-dd")}.{extension}";
                var fileLocation = _uploadLocation.ComentarLocation(fileName);
                Utils.GeneralUtils.UploadFile(image, _uploadLocation.FolderLocation(fileLocation));
               comment.ImageLocation = fileLocation;
            }
            await _unitOfWork.ItemCommentRepository.AddAsync(comment);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ItemCommentResponseDTO>(comment);
        }

        public async Task<List<ItemCommentResponseDTO>> GetComment(string itemClaimId)
        {
            var result = await _unitOfWork.ItemCommentRepository.Where(t => t.ItemClaimId.Equals(itemClaimId) && t.ActiveFlag)
                .Select(t => _mapper.Map<ItemCommentResponseDTO>(t)).ToListAsync();
            return result;
        }
    }
}
