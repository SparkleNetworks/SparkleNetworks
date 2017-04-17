
namespace Sparkle.Services.Networks.Tags.EntityWithTag
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Data;
    using Sparkle.Data.Entity.Networks.Sql;
    using Sparkle.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EntityWithTagSqlRepository : IEntityWithTagRepository
    {
        private const string logPath = "EntityWithTagSqlRepository.";
        private static readonly char[] forbiddenChars = { '"', '\'', '\\', '`', };
        private readonly string entityTagTableName;
        private readonly string entityForeignKeyToTable;

        public EntityWithTagSqlRepository(string entityTagTableName, string entityForeignKeyToTable)
        {
            this.CheckIdentifier(entityTagTableName);
            this.CheckIdentifier(entityForeignKeyToTable);

            this.entityTagTableName = entityTagTableName;
            this.entityForeignKeyToTable = entityForeignKeyToTable;
        }

        private void CheckIdentifier(string identifier)
        {
            foreach (var ch in identifier)
            {
                if (forbiddenChars.Contains(ch))
                    throw new InvalidOperationException(logPath + "CheckIdentifier: Found forbidden char in identifier '" + identifier + "'");
            }
        }

        private string MakeTagAlias(IServiceFactory services, int networkId, string name)
        {
            string alias = name.Trim().MakeUrlFriendly(true);
            if (services.Repositories.TagDefinitions.GetByAlias(networkId, alias) != null)
            {
                alias = alias.GetIncrementedString(a => services.Repositories.TagDefinitions.GetByAlias(networkId, a) == null);
            }

            return alias;
        }

        private TagDefinition GetOrAddTagDefinition(IServiceFactory services, IEntityWithTag item)
        {
            if (item.TagId.HasValue)
            {
                var tag = services.Repositories.TagDefinitions.GetById(item.TagId.Value);
                return tag;
            }
            else
            {
                var tag = new TagDefinition
                {
                    NetworkId = item.NetworkId,
                    CategoryId = item.TagCategoryId,
                    Name = item.TagName,
                    Alias = this.MakeTagAlias(services, item.NetworkId, item.TagName),
                    CreatedDateUtc = DateTime.UtcNow,
                    CreatedByUserId = item.ActingUserId.Value,
                };
                tag = services.Repositories.TagDefinitions.Insert(tag);
                services.Logger.Info(logPath + "GetOrAddTagDefinition", ErrorLevel.Success, "Created " + tag.ToString() + ".");
                item.TagId = tag.Id;

                return tag;
            }
        }

        private SqlEntityWithTag GetEntityTag(IServiceFactory services, IEntityWithTag item)
        {
            var sql = "SELECT * FROM dbo." + this.entityTagTableName + " WHERE " + this.entityForeignKeyToTable + " = @EntityId AND TagId = @TagId";
            var cmd = services.Repositories
                .CreateStoreCommand()
                .SetText(sql)
                .AddParameter("@EntityId", item.EntityId)
                .AddParameter("@TagId", item.TagId.Value);

            SqlEntityWithTag entityTag = null;
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    entityTag = new SqlEntityWithTag
                    {
                        EntityTagId = reader.GetInt32(reader.GetOrdinal("Id")),
                        EntityTagCreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                    };
                }
            }

            return entityTag;
        }

        public void Add(IServiceFactory services, IEntityWithTag item)
        {
            // Get/Add the TagDefinition in database
            var tag = this.GetOrAddTagDefinition(services, item);

            // Get/Add the EntityTag
            SqlEntityWithTag entityTag = null;
            if (!item.TagId.HasValue || (entityTag = this.GetEntityTag(services, item)) == null)
            {
                this.Insert(services, this.entityForeignKeyToTable, item.EntityId, tag.Id, item.ActingUserId.Value);
            }
            else
            {
                this.Update(services, entityTag.EntityTagId, null, null, null);
            }
        }

        public void Remove(IServiceFactory services, IEntityWithTag item)
        {
            TagDefinition tag;
            if (item.TagId.HasValue && (tag = services.Repositories.TagDefinitions.GetById(item.TagId.Value)) != null)
            {
                var entityTag = this.GetEntityTag(services, item);
                if (entityTag != null)
                {
                    var deleteReason = entityTag.EntityTagCreatedByUserId == item.ActingUserId.Value ? WallItemDeleteReason.AuthorDelete : WallItemDeleteReason.None;
                    this.Update(services, entityTag.EntityTagId, DateTime.UtcNow, item.ActingUserId.Value, deleteReason);
                }
            }
        }

        public void Insert(IServiceFactory services, string foreignKeyToEntityName, int entityId, int tagId, int createdByUserId)
        {
            var sql = string.Format(
                "INSERT INTO dbo.{0} ({1}, TagId, DateCreatedUtc, CreatedByUserId) VALUES (@EntityId, @TagId, @DateCreatedUtc, @CreatedByUserId)",
                this.entityTagTableName,
                foreignKeyToEntityName);
            var cmd = services.Repositories
                .CreateStoreCommand()
                .SetText(sql)
                .AddParameter("@EntityId", entityId)
                .AddParameter("@TagId", tagId)
                .AddParameter("@DateCreatedUtc", DateTime.UtcNow)
                .AddParameter("@CreatedByUserId", createdByUserId);
            cmd.ExecuteNonQuery();
            services.Logger.Info(logPath + "Insert", ErrorLevel.Success, "Created item " + foreignKeyToEntityName + ":" + entityId + " TagId:" + tagId + " into table " + this.entityTagTableName);
        }

        public void Update(IServiceFactory services, int entityTagId, DateTime? dateDeletedUtc, int? deletedByUserId, WallItemDeleteReason? deleteReason)
        {
            var sql = string.Format(
                "UPDATE dbo.{0} SET DateDeletedUtc = @DateDeletedUtc, DeletedByUserId = @DeletedByUserId, DeleteReason = @DeleteReason WHERE Id = @EntityTagId",
                this.entityTagTableName);
            var cmd = services.Repositories
                .CreateStoreCommand()
                .SetText(sql)
                .AddParameter("@DateDeletedUtc", dateDeletedUtc ?? (object)DBNull.Value)
                .AddParameter("@DeletedByUserId", deletedByUserId ?? (object)DBNull.Value)
                .AddParameter("@DeleteReason", deleteReason ?? (object)DBNull.Value)
                .AddParameter("@EntityTagId", entityTagId);
            cmd.ExecuteNonQuery();
            services.Logger.Info(logPath + "Update", ErrorLevel.Success, "Updated item " + entityTagId + " into table " + this.entityTagTableName);
        }

        public IList<Tag2Model> GetAllTagsForCategory(IServiceFactory services, int categoryId, int entityId)
        {
            return services.Tags.GetTagsByCategoryId(categoryId);
        }

        public IList<Tag2Model> GetEntityTagsForCategory(IServiceFactory services, int categoryId, int entityId)
        {
            var sql = string.Format(
                "SELECT TagId FROM dbo.{0} WHERE {1} = @EntityId AND DateDeletedUtc IS NULL",
                this.entityTagTableName,
                this.entityForeignKeyToTable);
            var cmd = services.Repositories
                .CreateStoreCommand()
                .SetText(sql)
                .AddParameter("@EntityId", entityId);

            List<int> tagIds = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tagIds.Add(reader.GetInt32(reader.GetOrdinal("TagId")));
                }
            }

            return services.Repositories.TagDefinitions.GetByIdsAndCategoryId(services.NetworkId, tagIds.ToArray(), categoryId).Select(o => new Tag2Model(o)).ToList();
        }

        public IList<Tag2Model> GetUsedEntityTags(IServiceFactory services)
        {
            var sql = string.Format(
                "SELECT DISTINCT TagId FROM dbo.{0} WHERE DateDeletedUtc IS NULL",
                this.entityTagTableName);
            var cmd = services.Repositories
                .CreateStoreCommand()
                .SetText(sql);

            var tagIds = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tagIds.Add(reader.GetInt32(reader.GetOrdinal("TagId")));
                }
            }

            return services.Tags.GetTagsById(tagIds.ToArray());
        }

        public IDictionary<int, int[]> GetEntitiesIdsByUsedTagsIds(IServiceFactory services, int[] tagIds)
        {
            var sql = string.Format(
                "SELECT {0}, TagId FROM dbo.{1} WHERE TagId IN ({2}) AND DateDeletedUtc IS NULL ORDER BY {0}",
                this.entityForeignKeyToTable,
                this.entityTagTableName,
                string.Join(",", tagIds));
            var cmd = services.Repositories
                .CreateStoreCommand()
                .SetText(sql);

            var entitiesIds = new Dictionary<int, int[]>();
            using (var reader = cmd.ExecuteReader())
            {
                int? currentEntityId = null;
                var ids = new List<int>();
                while (reader.Read())
                {
                    var entityId = reader.GetInt32(reader.GetOrdinal(this.entityForeignKeyToTable));
                    if (currentEntityId.HasValue && currentEntityId != entityId)
                    {
                        entitiesIds.Add(currentEntityId.Value, ids.ToArray());
                        ids.Clear();
                    }

                    currentEntityId = entityId;
                    ids.Add(reader.GetInt32(reader.GetOrdinal("TagId")));
                }

                entitiesIds.Add(currentEntityId.Value, ids.ToArray());
            }

            return entitiesIds;
        }
    }

    public class SqlEntityWithTag : IEntityWithTag
    {
        public int EntityTagId { get; set; }

        public int EntityTagCreatedByUserId { get; set; }

        // Above: present in IEntityWithTag

        public int? TagId { get; set; }

        public string TagName { get; set; }

        public int TagCategoryId { get; set; }

        public int NetworkId { get; set; }

        public int? ActingUserId { get; set; }

        public int EntityId { get; set; }
    }
}
