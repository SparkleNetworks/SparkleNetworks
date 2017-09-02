
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class BasicUsersService : BaseService<BasicUserModel>
    {
        public BasicUsersService(ServiceConfiguration configuration)
            : base(configuration, "BasicUsers")
        {
        }

        public Dictionary<Guid, BasicUserModel> GetAll()
        {
            return this.Read();
        }
    
        internal BasicUserModel CreateDefaultUser()
        {
            using (var transaction = this.Write())
            {
                if (transaction.Items.Count > 0)
                {
                    throw new InvalidOperationException("Cannot create default user: there are already users.");
                }
                else
                {
                    var id = Guid.NewGuid();
                    var item = new BasicUserModel();
                    item.Guid = id;
                    item.Password = Guid.NewGuid();
                    item.DateCreatedUtc = DateTime.UtcNow;
                    transaction.Items.Add(id, item);
                    transaction.Save();
                    return item;
                }
            }
        }
    
        internal BasicUserModel CreateUser()
        {
            var id = Guid.NewGuid();
            var item = new BasicUserModel();
            item.Guid = id;
            item.Password = Guid.NewGuid();
            item.DateCreatedUtc = DateTime.UtcNow;

            using (var transaction = this.Write())
            {
                transaction.Items.Add(id, item);
                transaction.Save();
                return item;
            }
        }

        internal bool Exists(Guid id)
        {
            return this.Read().ContainsKey(id);
        }

        internal BasicUserModel Authenticate(Guid password)
        {
            return this.Read().Values.Where(x => x.Password.Equals(password)).SingleOrDefault();
        }

        internal BasicUserModel Get(Guid guid)
        {
            var items = this.Read();
            return items.ContainsKey(guid) ? items[guid] : null;
        }
    }
}
