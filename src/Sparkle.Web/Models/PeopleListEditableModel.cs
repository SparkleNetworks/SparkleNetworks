using System.Collections.Generic;

namespace Sparkle.Models
{
    public class PeopleListEditableModel
    {
        public string RemoveMethod { get; set; }
        public List<PeopleModel> Peoples { get; set; }
        public int Id { get; set; }
    }
}