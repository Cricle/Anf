﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Anf.ChannelModel.Entity;
using Anf.ChannelModel.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Anf.WebService
{
    internal partial class KvComicChapterEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Anf.ChannelModel.Entity.KvComicChapter",
                typeof(KvComicChapter),
                baseEntityType);

            var targetUrl = runtimeEntityType.AddProperty(
                "TargetUrl",
                typeof(string),
                propertyInfo: typeof(ComicRef).GetProperty("TargetUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ComicRef).GetField("<TargetUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);

            var enitityId = runtimeEntityType.AddProperty(
                "EnitityId",
                typeof(long),
                propertyInfo: typeof(KvComicChapter).GetProperty("EnitityId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(KvComicChapter).GetField("<EnitityId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);

            var createTime = runtimeEntityType.AddProperty(
                "CreateTime",
                typeof(long),
                propertyInfo: typeof(WithPageChapterInfoOnly).GetProperty("CreateTime", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WithPageChapterInfoOnly).GetField("<CreateTime>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var order = runtimeEntityType.AddProperty(
                "Order",
                typeof(int),
                propertyInfo: typeof(KvComicChapter).GetProperty("Order", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(KvComicChapter).GetField("<Order>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var pages = runtimeEntityType.AddProperty(
                "Pages",
                typeof(string),
                propertyInfo: typeof(KvComicChapter).GetProperty("Pages", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(KvComicChapter).GetField("<Pages>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var refCount = runtimeEntityType.AddProperty(
                "RefCount",
                typeof(long),
                propertyInfo: typeof(WithPageChapterInfoOnly).GetProperty("RefCount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WithPageChapterInfoOnly).GetField("<RefCount>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var title = runtimeEntityType.AddProperty(
                "Title",
                typeof(string),
                propertyInfo: typeof(ComicChapter).GetProperty("Title", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ComicChapter).GetField("<Title>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var updateTime = runtimeEntityType.AddProperty(
                "UpdateTime",
                typeof(long),
                propertyInfo: typeof(WithPageChapterInfoOnly).GetProperty("UpdateTime", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(WithPageChapterInfoOnly).GetField("<UpdateTime>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { targetUrl, enitityId });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { enitityId });

            var index0 = runtimeEntityType.AddIndex(
                new[] { updateTime });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("EnitityId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var entity = declaringEntityType.AddNavigation("Entity",
                runtimeForeignKey,
                onDependent: true,
                typeof(KvComicEntity),
                propertyInfo: typeof(KvComicChapter).GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(KvComicChapter).GetField("<Entity>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var chapters = principalEntityType.AddNavigation("Chapters",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<KvComicChapter>),
                propertyInfo: typeof(KvComicEntity).GetProperty("Chapters", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(KvComicEntity).GetField("<Chapters>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}