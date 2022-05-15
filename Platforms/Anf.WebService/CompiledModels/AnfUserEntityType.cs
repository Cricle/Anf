﻿// <auto-generated />
using System;
using System.Reflection;
using Anf.ChannelModel.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Anf.WebService
{
    internal partial class AnfUserEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Anf.ChannelModel.Entity.AnfUser",
                typeof(AnfUser),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(long),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw);

            var accessFailedCount = runtimeEntityType.AddProperty(
                "AccessFailedCount",
                typeof(int),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("AccessFailedCount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<AccessFailedCount>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var concurrencyStamp = runtimeEntityType.AddProperty(
                "ConcurrencyStamp",
                typeof(string),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("ConcurrencyStamp", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<ConcurrencyStamp>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                concurrencyToken: true);

            var email = runtimeEntityType.AddProperty(
                "Email",
                typeof(string),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("Email", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<Email>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 256);

            var emailConfirmed = runtimeEntityType.AddProperty(
                "EmailConfirmed",
                typeof(bool),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("EmailConfirmed", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<EmailConfirmed>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var lockoutEnabled = runtimeEntityType.AddProperty(
                "LockoutEnabled",
                typeof(bool),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("LockoutEnabled", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<LockoutEnabled>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var lockoutEnd = runtimeEntityType.AddProperty(
                "LockoutEnd",
                typeof(DateTimeOffset?),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("LockoutEnd", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<LockoutEnd>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var normalizedEmail = runtimeEntityType.AddProperty(
                "NormalizedEmail",
                typeof(string),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("NormalizedEmail", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<NormalizedEmail>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 256);

            var normalizedUserName = runtimeEntityType.AddProperty(
                "NormalizedUserName",
                typeof(string),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("NormalizedUserName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<NormalizedUserName>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 256);

            var passwordHash = runtimeEntityType.AddProperty(
                "PasswordHash",
                typeof(string),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("PasswordHash", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<PasswordHash>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var phoneNumber = runtimeEntityType.AddProperty(
                "PhoneNumber",
                typeof(string),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("PhoneNumber", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<PhoneNumber>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var phoneNumberConfirmed = runtimeEntityType.AddProperty(
                "PhoneNumberConfirmed",
                typeof(bool),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("PhoneNumberConfirmed", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<PhoneNumberConfirmed>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var securityStamp = runtimeEntityType.AddProperty(
                "SecurityStamp",
                typeof(string),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("SecurityStamp", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<SecurityStamp>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var twoFactorEnabled = runtimeEntityType.AddProperty(
                "TwoFactorEnabled",
                typeof(bool),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("TwoFactorEnabled", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<TwoFactorEnabled>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var userName = runtimeEntityType.AddProperty(
                "UserName",
                typeof(string),
                propertyInfo: typeof(IdentityUser<long>).GetProperty("UserName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(IdentityUser<long>).GetField("<UserName>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 256);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { normalizedEmail });
            index.AddAnnotation("Relational:Name", "EmailIndex");

            var index0 = runtimeEntityType.AddIndex(
                new[] { normalizedUserName },
                unique: true);
            index0.AddAnnotation("Relational:Name", "UserNameIndex");

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "AspNetUsers");

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
