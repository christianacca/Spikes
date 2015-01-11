using System.ComponentModel.DataAnnotations;
using Spikes.Migrations.BaseModel;

namespace Spike.Migrations.Model
{
    public class CustomUserRole : UserRole
    {
        [StringLength(150)]
        public string CustomRoleProp { get; set; }
    }
}