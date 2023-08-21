namespace LostFoundAngkasaPura.DTO.ItemClaim
{
    public class ItemClaimResponseDTO
    {
        public string Id { get; set; }
        public string ItemFoundId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public string IdentityNumber { get; set; }
        public string IdentityType { get; set; }
        public string UserName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string ProofImage { get; set; }
        public string ProofDescription { get; set; }
        public string? ClaimLocation { get; set; }
        public string? ClaimDate { get; set; }
        public string? RejectReason { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
