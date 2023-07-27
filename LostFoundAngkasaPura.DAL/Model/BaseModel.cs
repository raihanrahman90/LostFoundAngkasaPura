
using System.ComponentModel.DataAnnotations;

namespace LostFoundAngkasaPura.DAL.Model
{
    public class BaseModel
    {
        [MaxLength(36)]
        public string Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? LastUpdatedDate { get; set; }

        public string? LastUpdatedBy { get; set; }

        public bool ActiveFlag { get; set; } = true;

        public BaseModel()
        {
            CreatedDate = DateTime.Now;
        }

        public static string GenerateClassName(Type t)
        {
            while (t.BaseType!.Name != "ModelBase")
            {
                t = t.BaseType;
            }

            return t.Name;
        }
    }
}
