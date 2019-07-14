﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization.Formatters.Binary
{
    internal sealed class ReadObjectInfo
    {
        // Fields
        internal bool bSimpleAssembly;
        internal SerObjectInfoCache cache;
        internal StreamingContext context;
        internal int count;
        internal IFormatterConverter formatterConverter;
        internal bool isNamed;
        internal bool isSi;
        internal bool isTyped;
        private int lastPosition;
        internal List<Type> memberTypesList;
        internal int objectInfoId;
        internal ObjectManager objectManager;
        internal Type objectType;
        internal static int readObjectInfoCounter;
        internal ISerializationSurrogate serializationSurrogate;
        internal SerObjectInfoInit serObjectInfoInit;
        internal ISurrogateSelector surrogateSelector;
        internal string[] wireMemberNames;
        internal Type[] wireMemberTypes;

        // Methods
        internal ReadObjectInfo()
        {
        }

        internal void AddValue(string name, object value, ref SerializationInfo si, ref object[] memberData)
        {
            if (this.isSi)
            {
                si.AddValue(name, value);
            }
            else
            {
                int index = this.Position(name);
                if (index != -1)
                {
                    memberData[index] = value;
                }
            }
        }

        
        internal static ReadObjectInfo Create(Type objectType, ISurrogateSelector surrogateSelector, StreamingContext context, ObjectManager objectManager, SerObjectInfoInit serObjectInfoInit, IFormatterConverter converter, bool bSimpleAssembly)
        {
            ReadObjectInfo objectInfo = GetObjectInfo(serObjectInfoInit);
            objectInfo.Init(objectType, surrogateSelector, context, objectManager, serObjectInfoInit, converter, bSimpleAssembly);
            return objectInfo;
        }

        
        internal static ReadObjectInfo Create(Type objectType, string[] memberNames, Type[] memberTypes, ISurrogateSelector surrogateSelector, StreamingContext context, ObjectManager objectManager, SerObjectInfoInit serObjectInfoInit, IFormatterConverter converter, bool bSimpleAssembly)
        {
            ReadObjectInfo objectInfo = GetObjectInfo(serObjectInfoInit);
            objectInfo.Init(objectType, memberNames, memberTypes, surrogateSelector, context, objectManager, serObjectInfoInit, converter, bSimpleAssembly);
            return objectInfo;
        }

        // [Conditional("SER_LOGGING")]
        private void DumpPopulate(MemberInfo[] memberInfos, object[] memberData)
        {
            for (int i = 0; i < memberInfos.Length; i++)
            {
            }
        }

        // [Conditional("SER_LOGGING")]
        private void DumpPopulateSi()
        {
        }

        internal MemberInfo GetMemberInfo(string name)
        {
            if (this.cache != null)
            {
                if (this.isSi)
                {
                    object[] values = new object[] { this.objectType + " " + name };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_MemberInfo", values));
                }
                if (this.cache.memberInfos == null)
                {
                    object[] objArray2 = new object[] { this.objectType + " " + name };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_NoMemberInfo", objArray2));
                }
                if (this.Position(name) != -1)
                {
                    return this.cache.memberInfos[this.Position(name)];
                }
            }
            return null;
        }

        internal Type GetMemberType(MemberInfo objMember)
        {
            if (objMember is FieldInfo)
            {
                return ((FieldInfo)objMember).FieldType;
            }
            if (objMember is PropertyInfo)
            {
                return ((PropertyInfo)objMember).PropertyType;
            }
            object[] values = new object[] { objMember.GetType() };
            throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_SerMemberInfo", values));
        }

        internal Type[] GetMemberTypes(string[] inMemberNames, Type objectType)
        {
            if (this.isSi)
            {
                object[] values = new object[] { objectType };
                throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_ISerializableTypes", values));
            }
            if (this.cache == null)
            {
                return null;
            }
            if (this.cache.memberTypes == null)
            {
                this.cache.memberTypes = new Type[this.count];
                for (int j = 0; j < this.count; j++)
                {
                    this.cache.memberTypes[j] = this.GetMemberType(this.cache.memberInfos[j]);
                }
            }
            bool flag = false;
            if (inMemberNames.Length < this.cache.memberInfos.Length)
            {
                flag = true;
            }
            Type[] typeArray = new Type[this.cache.memberInfos.Length];
            bool flag2 = false;
            for (int i = 0; i < this.cache.memberInfos.Length; i++)
            {
                if (!flag && inMemberNames[i].Equals(this.cache.memberInfos[i].Name))
                {
                    typeArray[i] = this.cache.memberTypes[i];
                    continue;
                }
                flag2 = false;
                for (int k = 0; k < inMemberNames.Length; k++)
                {
                    if (this.cache.memberInfos[i].Name.Equals(inMemberNames[k]))
                    {
                        typeArray[i] = this.cache.memberTypes[i];
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    //TODO:
                    //object[] customAttributes = this.cache.memberInfos[i].GetCustomAttributes(typeof(OptionalFieldAttribute), false);
                    //if (((customAttributes == null) || (customAttributes.Length == 0)) && !this.bSimpleAssembly)
                    //{
                    //    object[] objArray2 = new object[] { this.cache.memberNames[i], objectType, typeof(OptionalFieldAttribute).FullName };
                    //    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_MissingMember", objArray2));
                    //}
                }
            }
            return typeArray;
        }

        private static ReadObjectInfo GetObjectInfo(SerObjectInfoInit serObjectInfoInit) =>
            new ReadObjectInfo { objectInfoId = readObjectInfoCounter++ };

        internal Type GetType(string name)
        {
            Type type = null;
            int index = this.Position(name);
            if (index == -1)
            {
                return null;
            }
            if (this.isTyped)
            {
                type = this.cache.memberTypes[index];
            }
            else
            {
                type = this.memberTypesList[index];
            }
            if (type == null)
            {
                object[] values = new object[] { this.objectType + " " + name };
                throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_ISerializableTypes", values));
            }
            return type;
        }

        internal void Init(Type objectType, ISurrogateSelector surrogateSelector, StreamingContext context, ObjectManager objectManager, SerObjectInfoInit serObjectInfoInit, IFormatterConverter converter, bool bSimpleAssembly)
        {
            this.objectType = objectType;
            this.objectManager = objectManager;
            this.context = context;
            this.serObjectInfoInit = serObjectInfoInit;
            this.formatterConverter = converter;
            this.bSimpleAssembly = bSimpleAssembly;
            this.InitReadConstructor(objectType, surrogateSelector, context);
        }

        internal void Init(Type objectType, string[] memberNames, Type[] memberTypes, ISurrogateSelector surrogateSelector, StreamingContext context, ObjectManager objectManager, SerObjectInfoInit serObjectInfoInit, IFormatterConverter converter, bool bSimpleAssembly)
        {
            this.objectType = objectType;
            this.objectManager = objectManager;
            this.wireMemberNames = memberNames;
            this.wireMemberTypes = memberTypes;
            this.context = context;
            this.serObjectInfoInit = serObjectInfoInit;
            this.formatterConverter = converter;
            this.bSimpleAssembly = bSimpleAssembly;
            if (memberNames != null)
            {
                this.isNamed = true;
            }
            if (memberTypes != null)
            {
                this.isTyped = true;
            }
            if (objectType != null)
            {
                this.InitReadConstructor(objectType, surrogateSelector, context);
            }
        }

        internal void InitDataStore(ref SerializationInfo si, ref object[] memberData)
        {
            if (this.isSi)
            {
                if (si == null)
                {
                    si = new SerializationInfo(this.objectType, this.formatterConverter);
                }
            }
            else if ((memberData == null) && (this.cache != null))
            {
                memberData = new object[this.cache.memberNames.Length];
            }
        }

        private void InitMemberInfo()
        {
            this.cache = new SerObjectInfoCache(this.objectType);
            //this.cache.memberInfos = FormatterServices.GetSerializableMembers(this.objectType, this.context);
            this.count = this.cache.memberInfos.Length;
            this.cache.memberNames = new string[this.count];
            this.cache.memberTypes = new Type[this.count];
            for (int i = 0; i < this.count; i++)
            {
                this.cache.memberNames[i] = this.cache.memberInfos[i].Name;
                this.cache.memberTypes[i] = this.GetMemberType(this.cache.memberInfos[i]);
            }
            this.isTyped = true;
            this.isNamed = true;
        }

        private void InitNoMembers()
        {
            this.cache = new SerObjectInfoCache(this.objectType);
        }

        private void InitReadConstructor(Type objectType, ISurrogateSelector surrogateSelector, StreamingContext context)
        {
            if (objectType.IsArray)
            {
                this.InitNoMembers();
            }
            else
            {
                ISurrogateSelector selector = null;
                if (surrogateSelector != null)
                {
                    this.serializationSurrogate = surrogateSelector.GetSurrogate(objectType, context, out selector);
                }
                if (this.serializationSurrogate != null)
                {
                    this.isSi = true;
                }
                else if ((objectType != Converter.typeofObject) && Converter.typeofISerializable.IsAssignableFrom(objectType))
                {
                    this.isSi = true;
                }
                if (this.isSi)
                {
                    this.InitSiRead();
                }
                else
                {
                    this.InitMemberInfo();
                }
            }
        }

        private void InitSiRead()
        {
            if (this.memberTypesList != null)
            {
                this.memberTypesList = new List<Type>(20);
            }
        }

        internal void ObjectEnd()
        {
        }

        internal void PopulateObjectMembers(object obj, object[] memberData)
        {
            if (!this.isSi && (memberData != null))
            {
                //FormatterServices.PopulateObjectMembers(obj, this.cache.memberInfos, memberData);
            }
        }

        private int Position(string name)
        {
            if (this.cache != null)
            {
                if ((this.cache.memberNames.Length != 0) && this.cache.memberNames[this.lastPosition].Equals(name))
                {
                    return this.lastPosition;
                }
                int num = this.lastPosition + 1;
                this.lastPosition = num;
                if ((num < this.cache.memberNames.Length) && this.cache.memberNames[this.lastPosition].Equals(name))
                {
                    return this.lastPosition;
                }
                for (int i = 0; i < this.cache.memberNames.Length; i++)
                {
                    if (this.cache.memberNames[i].Equals(name))
                    {
                        this.lastPosition = i;
                        return this.lastPosition;
                    }
                }
                this.lastPosition = 0;
            }
            return -1;
        }

        internal void PrepareForReuse()
        {
            this.lastPosition = 0;
        }

        internal void RecordFixup(long objectId, string name, long idRef)
        {
            if (this.isSi)
            {
                this.objectManager.RecordDelayedFixup(objectId, name, idRef);
            }
            else
            {
                int index = this.Position(name);
                if (index != -1)
                {
                    this.objectManager.RecordFixup(objectId, this.cache.memberInfos[index], idRef);
                }
            }
        }
    }
}
