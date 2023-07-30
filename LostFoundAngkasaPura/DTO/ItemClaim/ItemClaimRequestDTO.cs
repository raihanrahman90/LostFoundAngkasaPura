namespace LostFoundAngkasaPura.DTO.ItemClaim
{
    public class ItemClaimRequestDTO
    {
        public string ItemFoundId { get; set; }
        public string IdentityNumber { get; set; }
        public string IdentityType { get; set; }
        public string ProofImageBase64 { get; set; }
        public string ProofDescription { get; set; }

    }
}
