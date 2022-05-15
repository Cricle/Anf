﻿// <auto-generated />
using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Anf.WebService
{
    internal partial class IdentityUserLoginlongEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Microsoft.AspNetCore.Identity.IdentityUserLogin<long>",
                typeof(IdentityUserLogin<long>),
                baseEntityType);

            var loginProvider = runtimeEntityType.AddProperty(
                "LoginProvider",
                typeof(string),
                propertyInfo: typeof(IdentityUserLogin<long>).GetProperty("LoginProvider", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUserLogin<long>).GetField("<LoginProvider>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);

            var providerKey = runtimeEntityType.AddProperty(
                "ProviderKey",
                typeof(string),
                propertyInfo: typeof(IdentityUserLogin<long>).GetProperty("ProviderKey", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUserLogin<long>).GetField("<ProviderKey>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);

            var providerDisplayName = runtimeEntityType.AddProperty(
                "ProviderDisplayName",
                typeof(string),
                propertyInfo: typeof(IdentityUserLogin<long>).GetProperty("ProviderDisplayName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUserLogin<long>).GetField("<ProviderDisplayName>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var userId = runtimeEntityType.AddProperty(
                "UserId",
                typeof(long),
                propertyInfo: typeof(IdentityUserLogin<long>).GetProperty("UserId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUserLogin<long>).GetField("<UserId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { loginProvider, providerKey });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { userId });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("UserId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "AspNetUserLogins");

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
