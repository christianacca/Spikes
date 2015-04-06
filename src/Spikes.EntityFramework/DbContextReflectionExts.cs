using System.Data.Entity.Core.Mapping;

namespace Spikes.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;

    public static class DbContextReflectionExts
    {
        public static string GetTableName(this DbContext context, Type type)
        {
            MetadataWorkspace metadata = ((IObjectContextAdapter) context).ObjectContext.MetadataWorkspace;
            var objectItemCollection = ((ObjectItemCollection) metadata.GetItemCollection(DataSpace.OSpace));
            EntityType entityType = metadata.GetItems<EntityType>(DataSpace.OSpace)
                .Single(e => objectItemCollection.GetClrType(e) == type);
            EntityType entitySet = metadata.GetItems(DataSpace.CSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType).OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);
            List<EntitySetMapping> entitySetMappings = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                .Single().EntitySetMappings.ToList();

            EntitySet table;
            EntitySetMapping mapping = entitySetMappings.SingleOrDefault(x => x.EntitySet.Name == entitySet.Name);
            if (mapping != null)
            {
                table = mapping.EntityTypeMappings.Single().Fragments.Single().StoreEntitySet;
            }
            else
            {
                mapping =
                    entitySetMappings.SingleOrDefault(
                        x =>
                            x.EntityTypeMappings.Where(y => y.EntityType != null)
                                .Any(y => y.EntityType.Name == entitySet.Name));
                if (mapping != null)
                {
                    table = mapping.EntityTypeMappings.Where(x => x.EntityType != null)
                        .Single(x => x.EntityType.Name == entityType.Name)
                        .Fragments.Single()
                        .StoreEntitySet;
                }
                else
                {
                    EntitySetMapping entitySetMapping = entitySetMappings
                        .Single(x => x.EntityTypeMappings.Any(y => y.IsOfEntityTypes.Any(z => z.Name == entitySet.Name)));
                    table = entitySetMapping.EntityTypeMappings
                        .First(x => x.IsOfEntityTypes.Any(y => y.Name == entitySet.Name))
                        .Fragments.Single().StoreEntitySet;
                }
            }

            return String.Format("{0}.{1}", table.MetadataProperties["Schema"].Value ?? table.Schema,
                table.MetadataProperties["Table"].Value ?? table.Name);
        }

        public static IDictionary<Type, string> GetTableMappings(this DbContext ctx)
        {
            return ctx.GetClrEntityTypes().ToDictionary(t => t, ctx.GetTableName);
        }

        public static IEnumerable<PropertyInfo> OneToMany(this DbContext ctx, Type entityType)
        {
            EntityType et = ctx.GetEntityType(entityType);

            return
                (et.NavigationProperties.Where(
                    x =>
                        x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One
                        && x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                    .Select(
                        x =>
                            entityType.GetProperty(
                                x.Name,
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty
                                | BindingFlags.SetProperty))
                    .ToList());
        }

        public static IEnumerable<PropertyInfo> OneToOne(this DbContext ctx, Type entityType)
        {
            EntityType et = ctx.GetEntityType(entityType);

            return
                (et.NavigationProperties.Where(
                    x =>
                        (x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One
                         || x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne)
                        && (x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One
                            || x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne))
                    .Select(
                        x =>
                            entityType.GetProperty(
                                x.Name,
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty
                                | BindingFlags.SetProperty))
                    .ToList());
        }

        public static EntityType GetEntityType(this DbContext ctx, Type entityType, DataSpace dataSpace = DataSpace.OSpace)
        {
            EntityType et = ctx.GetEntityTypes(dataSpace).Single(x => x.Name == entityType.Name);
            return et;
        }

        public static IEnumerable<EntityType> GetEntityTypes(this DbContext ctx, DataSpace dataSpace = DataSpace.OSpace)
        {
            ObjectContext octx = (ctx as IObjectContextAdapter).ObjectContext;
            return octx.MetadataWorkspace.GetItems(dataSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>();
        }

        public static IEnumerable<PropertyInfo> ManyToOne(this DbContext ctx, Type entityType)
        {
            EntityType et = ctx.GetEntityType(entityType);
            return
                (et.NavigationProperties.Where(
                    x =>
                        x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many
                        && x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)
                    .Select(
                        x =>
                            entityType.GetProperty(
                                x.Name,
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty
                                | BindingFlags.SetProperty))
                    .ToList());
        }

        public static IEnumerable<PropertyInfo> ManyToMany(this DbContext ctx, Type entityType)
        {
            EntityType et = ctx.GetEntityType(entityType);

            return
                (et.NavigationProperties.Where(
                    x =>
                        x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many
                        && x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                    .Select(
                        x =>
                            entityType.GetProperty(
                                x.Name,
                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty
                                | BindingFlags.SetProperty))
                    .ToList());
        }

        public static IEnumerable<PropertyInfo> GetIdProperties(this DbContext ctx, Type entityType)
        {
            EntityType et = ctx.GetEntityType(entityType);

            return (et.KeyMembers.Select(x => entityType.GetProperty(x.Name)).ToList());
        }

        public static IDictionary<String, PropertyInfo> GetTableKeyColumns(this DbContext ctx, Type entityType)
        {
            EntityType storageEntityType = ctx.GetEntityType(entityType, DataSpace.SSpace);
            EntityType objectEntityType = ctx.GetEntityType(entityType);
            return
                (storageEntityType.KeyMembers.Select(
                    (elm, index) =>
                        new
                        {
                            elm.Name,
                            Property =
                                entityType.GetProperty(
                                    (objectEntityType.MetadataProperties["Members"].Value as IEnumerable<EdmMember>)
                                        .ElementAt(index).Name)
                        }).ToDictionary(x => x.Name, x => x.Property));
        }

        public static ICollection<PropertyInfo> GetTableColumns(this DbContext ctx, Type entityType)
        {
            EntityType conceptualEntityType = ctx.GetEntityType(entityType, DataSpace.CSpace);
            return
                conceptualEntityType.Properties.Select(elm => entityType.GetProperty(elm.Name)).ToList();
        }

        public static Dictionary<PropertyInfo, IEnumerable<PropertyInfo>> GetForeignKeyColumns(this DbContext ctx, Type entityType)
        {
            EntityType conceptualEntityType = ctx.GetEntityType(entityType, DataSpace.CSpace);
            var mapping = conceptualEntityType.NavigationProperties
                .Select(navProp => new
                {
                    Property = entityType.GetProperty(navProp.Name),
                    Properties = navProp.GetDependentProperties().Select(dp => entityType.GetProperty(dp.Name))
                })
                .ToDictionary(x => x.Property, x => x.Properties);

            return mapping;
        }

        public static IEnumerable<Type> GetClrEntityTypes<T>(this T dbContext) where T : DbContext
        {
            ObjectContext octx = (dbContext as IObjectContextAdapter).ObjectContext;
            // Get the mapping between CLR types and metadata OSpace
            var objectItemCollection = ((ObjectItemCollection)octx.MetadataWorkspace.GetItemCollection(DataSpace.OSpace));

            var entityTypes = octx.MetadataWorkspace.GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>();

            return entityTypes.Select(objectItemCollection.GetClrType);
        }
    }
}