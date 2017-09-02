
namespace Sparkle.Data.Networks.Objects
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public class LivePerson
    {
        public LivePerson()
        {
        }

        public LivePerson(Live item)
        {
            this.Id = item.Id;
            this.DateTime = item.DateTime;
            this.UserId = item.UserId;
            this.Status = item.Status;

            if (item.User != null)
            {
                this.FirstName = item.User.FirstName;
                this.LastName = item.User.LastName;

                this.Login = item.User.Login;
            }
        }

        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public int UserId { get; set; }

        public int Status { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Login { get; set; }

        public string PictureUrl { get; set; }

        public IList<string> Messages { get; set; }

        public bool IsContact { get; set; }
    }
}
