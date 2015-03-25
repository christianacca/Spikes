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

    public static class DbContextExtensions
    {
        public static IDictionary<Type, String> GetTables(this DbContext ctx)
        {
            ObjectContext octx = (ctx as IObjectContextAdapter).ObjectContext;
            IEnumerable<EntityType> entities =
                octx.MetadataWorkspace.GetItemCollection(DataSpace.OSpace).GetItems<EntityType>().ToList();

            return
                (entities.ToDictionary(x => Type.GetType(x.FullName), x => GetTableName(ctx, Type.GetType(x.FullName))));
        }

        public static String GetTableName(this DbContext ctx, Type entityType)
        {
            ObjectContext octx = (ctx as IObjectContextAdapter).ObjectContext;
            EntitySetBase et =
                octx.MetadataWorkspace.GetItemCollection(DataSpace.SSpace)
                    .GetItems<EntityContainer>()
                    .Single()
                    .BaseEntitySets.Single(x => x.Name == entityType.Name);

            String tableName = String.Concat(
                et.MetadataProperties["Schema"].Value,
                ".",
                et.MetadataProperties["Table"].Value);

            return (tableName);
        }

        public static IEnumerable<PropertyInfo> OneToMany(this DbContext ctx, Type entityType)
        {
            ObjectContext octx = (ctx as IObjectContextAdapter).ObjectContext;
            EntityType et =
                octx.MetadataWorkspace.GetItems(DataSpace.OSpace)
                    .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                    .OfType<EntityType>()
                    .Single(x => x.Name == entityType.Name);

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
            EntityType et = ctx.GetEntityType(entityType, DataSpace.OSpace);

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
            ObjectContext octx = (ctx as IObjectContextAdapter).ObjectContext;
            EntityType storageEntityType =
                octx.MetadataWorkspace.GetItems(DataSpace.SSpace)
                    .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                    .OfType<EntityType>()
                    .Single(x => x.Name == entityType.Name);
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

        public static IDictionary<String, PropertyInfo> GetTableColumns(this DbContext ctx, Type entityType)
        {
            EntityType storageEntityType = ctx.GetEntityType(entityType, DataSpace.SSpace);
            EntityType objectEntityType = ctx.GetEntityType(entityType);

            return
                (storageEntityType.Properties.Select(
                    (elm, index) =>
                        new { elm.Name, Property = entityType.GetProperty(objectEntityType.Members[index].Name) })
                    .ToDictionary(x => x.Name, x => x.Property));
        }

        public static Dictionary<PropertyInfo, IEnumerable<PropertyInfo>> GetNavigationProperties(this DbContext ctx, Type entityType)
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
    }
}