using System.ComponentModel.DataAnnotations;

namespace Spikes.Migrations.BaseModel
{
    public class FakeEntity
    {
        public int Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }
    }
}