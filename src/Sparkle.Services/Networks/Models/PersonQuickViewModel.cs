
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PersonQuickViewModel
    {
        public PersonQuickViewModel()
        {
        }

        public PersonQuickViewModel(Entities.Networks.User user)
        {
            this.Id = user.Id;
            this.Login = user.Login;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Picture = "";

            if(user.JobReference.IsLoaded) {
                this.JobName = user.JobReference.Value.Libelle;
            }

            if (user.CompanyReference.IsLoaded)
            {
                this.CompanyName = user.CompanyReference.Value.Name;
            }
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Login { get; set; }

        public string Picture { get; set; }

        public string JobName { get; set; }

        public string CompanyName { get; set; }
    }
}
