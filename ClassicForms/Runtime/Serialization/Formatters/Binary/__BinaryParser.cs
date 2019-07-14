﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization.Formatters.Binary
{
    internal sealed class __BinaryParser
    {
        // Fields
        internal SizedArray assemIdToAssemblyTable;
        private BinaryObject binaryObject;
        private BinaryObjectWithMap bowm;
        private BinaryObjectWithMapTyped bowmt;
        private byte[] byteBuffer;
        private const int chunkSize = 0x1000;
        internal BinaryCrossAppDomainString crossAppDomainString;
        private BinaryReader dataReader;
        private static Encoding encoding = new UTF8Encoding(false, true);
        internal BinaryTypeEnum expectedType = BinaryTypeEnum.ObjectUrt;
        internal object expectedTypeInformation;
        internal long headerId;
        internal Stream input;
        internal MemberPrimitiveTyped memberPrimitiveTyped;
        internal MemberPrimitiveUnTyped memberPrimitiveUnTyped;
        internal MemberReference memberReference;
        internal static volatile MessageEnd messageEnd;
        internal SizedArray objectMapIdTable;
        internal ObjectNull objectNull;
        internal ObjectReader objectReader;
        internal BinaryObjectString objectString;
        private SerStack opPool;
        internal ParseRecord PRS;
        internal SerStack stack = new SerStack("ObjectProgressStack");
        private BinaryAssemblyInfo systemAssemblyInfo;
        internal long topId;

        // Methods
        internal __BinaryParser(Stream stream, ObjectReader objectReader)
        {
            this.input = stream;
            this.objectReader = objectReader;
            this.dataReader = new BinaryReader(this.input, encoding);
        }

        private ObjectProgress GetOp()
        {
            ObjectProgress progress = null;
            if ((this.opPool != null) && !this.opPool.IsEmpty())
            {
                progress = (ObjectProgress)this.opPool.Pop();
                progress.Init();
                return progress;
            }
            return new ObjectProgress();
        }

        private void PutOp(ObjectProgress op)
        {
            if (this.opPool == null)
            {
                this.opPool = new SerStack("opPool");
            }
            this.opPool.Push(op);
        }

        
        private void ReadArray(BinaryHeaderEnum binaryHeaderEnum)
        {
            BinaryAssemblyInfo assemblyInfo = null;
            BinaryArray array = new BinaryArray(binaryHeaderEnum);
            array.Read(this);
            if (array.binaryTypeEnum == BinaryTypeEnum.ObjectUser)
            {
                if (array.assemId < 1)
                {
                    object[] values = new object[] { array.typeInformation };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_AssemblyId", values));
                }
                assemblyInfo = (BinaryAssemblyInfo)this.AssemIdToAssemblyTable[array.assemId];
            }
            else
            {
                assemblyInfo = this.SystemAssemblyInfo;
            }
            ObjectProgress op = this.GetOp();
            ParseRecord pr = op.pr;
            op.objectTypeEnum = InternalObjectTypeE.Array;
            op.binaryTypeEnum = array.binaryTypeEnum;
            op.typeInformation = array.typeInformation;
            ObjectProgress progress2 = (ObjectProgress)this.stack.PeekPeek();
            if ((progress2 == null) || (array.objectId > 0))
            {
                op.name = "System.Array";
                pr.PRparseTypeEnum = InternalParseTypeE.Object;
                op.memberValueEnum = InternalMemberValueE.Empty;
            }
            else
            {
                pr.PRparseTypeEnum = InternalParseTypeE.Member;
                pr.PRmemberValueEnum = InternalMemberValueE.Nested;
                op.memberValueEnum = InternalMemberValueE.Nested;
                InternalObjectTypeE objectTypeEnum = progress2.objectTypeEnum;
                if (objectTypeEnum != InternalObjectTypeE.Object)
                {
                    if (objectTypeEnum != InternalObjectTypeE.Array)
                    {
                        object[] objArray2 = new object[] { progress2.objectTypeEnum.ToString() };
                        throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_ObjectTypeEnum", objArray2));
                    }
                    pr.PRmemberTypeEnum = InternalMemberTypeE.Item;
                    op.memberTypeEnum = InternalMemberTypeE.Item;
                }
                else
                {
                    pr.PRname = progress2.name;
                    pr.PRmemberTypeEnum = InternalMemberTypeE.Field;
                    op.memberTypeEnum = InternalMemberTypeE.Field;
                    pr.PRkeyDt = progress2.name;
                    pr.PRdtType = progress2.dtType;
                }
            }
            pr.PRobjectId = this.objectReader.GetId((long)array.objectId);
            if (pr.PRobjectId == this.topId)
            {
                pr.PRobjectPositionEnum = InternalObjectPositionE.Top;
            }
            else if ((this.headerId > 0L) && (pr.PRobjectId == this.headerId))
            {
                pr.PRobjectPositionEnum = InternalObjectPositionE.Headers;
            }
            else
            {
                pr.PRobjectPositionEnum = InternalObjectPositionE.Child;
            }
            pr.PRobjectTypeEnum = InternalObjectTypeE.Array;
            BinaryConverter.TypeFromInfo(array.binaryTypeEnum, array.typeInformation, this.objectReader, assemblyInfo, out pr.PRarrayElementTypeCode, out pr.PRarrayElementTypeString, out pr.PRarrayElementType, out pr.PRisArrayVariant);
            pr.PRdtTypeCode = InternalPrimitiveTypeE.Invalid;
            pr.PRrank = array.rank;
            pr.PRlengthA = array.lengthA;
            pr.PRlowerBoundA = array.lowerBoundA;
            bool flag = false;
            switch (array.binaryArrayTypeEnum)
            {
                case BinaryArrayTypeEnum.Single:
                case BinaryArrayTypeEnum.SingleOffset:
                    op.numItems = array.lengthA[0];
                    pr.PRarrayTypeEnum = InternalArrayTypeE.Single;
                    if (Converter.IsWriteAsByteArray(pr.PRarrayElementTypeCode) && (array.lowerBoundA[0] == 0))
                    {
                        flag = true;
                        this.ReadArrayAsBytes(pr);
                    }
                    break;

                case BinaryArrayTypeEnum.Jagged:
                case BinaryArrayTypeEnum.JaggedOffset:
                    op.numItems = array.lengthA[0];
                    pr.PRarrayTypeEnum = InternalArrayTypeE.Jagged;
                    break;

                case BinaryArrayTypeEnum.Rectangular:
                case BinaryArrayTypeEnum.RectangularOffset:
                    {
                        int num = 1;
                        for (int i = 0; i < array.rank; i++)
                        {
                            num *= array.lengthA[i];
                        }
                        op.numItems = num;
                        pr.PRarrayTypeEnum = InternalArrayTypeE.Rectangular;
                        break;
                    }
                default:
                    {
                        object[] objArray3 = new object[] { array.binaryArrayTypeEnum.ToString() };
                        throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_ArrayType", objArray3));
                    }
            }
            if (!flag)
            {
                this.stack.Push(op);
            }
            else
            {
                this.PutOp(op);
            }
            this.objectReader.Parse(pr);
            if (flag)
            {
                pr.PRparseTypeEnum = InternalParseTypeE.ObjectEnd;
                this.objectReader.Parse(pr);
            }
        }

        
        private void ReadArrayAsBytes(ParseRecord pr)
        {
            if (pr.PRarrayElementTypeCode == InternalPrimitiveTypeE.Byte)
            {
                pr.PRnewObj = this.ReadBytes(pr.PRlengthA[0]);
            }
            else if (pr.PRarrayElementTypeCode == InternalPrimitiveTypeE.Char)
            {
                pr.PRnewObj = this.ReadChars(pr.PRlengthA[0]);
            }
            else
            {
                int num = Converter.TypeLength(pr.PRarrayElementTypeCode);
                pr.PRnewObj = Converter.CreatePrimitiveArray(pr.PRarrayElementTypeCode, pr.PRlengthA[0]);
                Array pRnewObj = (Array)pr.PRnewObj;
                int num2 = 0;
                if (this.byteBuffer == null)
                {
                    this.byteBuffer = new byte[0x1000];
                }
                while (num2 < pRnewObj.Length)
                {
                    int num3 = Math.Min((int)(0x1000 / num), (int)(pRnewObj.Length - num2));
                    int size = num3 * num;
                    this.ReadBytes(this.byteBuffer, 0, size);
                    //TODO PROB DONT WORK
                    this.byteBuffer.CopyTo(pRnewObj, 0);

                    //Buffer.InternalBlockCopy(this.byteBuffer, 0, pRnewObj, num2 * num, size);
                    num2 += num3;
                }
            }
        }

        
        internal void ReadAssembly(BinaryHeaderEnum binaryHeaderEnum)
        {
            BinaryAssembly assembly = new BinaryAssembly();
            if (binaryHeaderEnum == BinaryHeaderEnum.CrossAppDomainAssembly)
            {
                BinaryCrossAppDomainAssembly assembly2 = new BinaryCrossAppDomainAssembly();
                assembly2.Read(this);
                assembly2.Dump();
                assembly.assemId = assembly2.assemId;
                assembly.assemblyString = this.objectReader.CrossAppDomainArray(assembly2.assemblyIndex) as string;
                if (assembly.assemblyString == null)
                {
                    object[] values = new object[] { "String", assembly2.assemblyIndex };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_CrossAppDomainError", values));
                }
            }
            else
            {
                assembly.Read(this);
                assembly.Dump();
            }
            this.AssemIdToAssemblyTable[assembly.assemId] = new BinaryAssemblyInfo(assembly.assemblyString);
        }

        internal void ReadBegin()
        {
        }

        internal bool ReadBoolean() =>
            this.dataReader.ReadBoolean();

        internal byte ReadByte() =>
            this.dataReader.ReadByte();

        internal byte[] ReadBytes(int length) =>
            this.dataReader.ReadBytes(length);

        internal void ReadBytes(byte[] byteA, int offset, int size)
        {
            while (size > 0)
            {
                int num = this.dataReader.Read(byteA, offset, size);
                if (num == 0)
                {
                    throw new EndOfStreamException(EnvironmentV2.GetResourceString("IO.EOF_ReadBeyondEOF"));
                    //__Error.EndOfFile();
                }
                offset += num;
                size -= num;
            }
        }

        internal char ReadChar() =>
            this.dataReader.ReadChar();

        internal char[] ReadChars(int length) =>
            this.dataReader.ReadChars(length);

        
        internal void ReadCrossAppDomainMap()
        {
            BinaryCrossAppDomainMap map = new BinaryCrossAppDomainMap();
            map.Read(this);
            map.Dump();
            object obj2 = this.objectReader.CrossAppDomainArray(map.crossAppDomainArrayIndex);
            BinaryObjectWithMap record = obj2 as BinaryObjectWithMap;
            if (record != null)
            {
                record.Dump();
                this.ReadObjectWithMap(record);
            }
            else
            {
                BinaryObjectWithMapTyped typed = obj2 as BinaryObjectWithMapTyped;
                if (typed != null)
                {
                    this.ReadObjectWithMapTyped(typed);
                }
                else
                {
                    object[] values = new object[] { "BinaryObjectMap", obj2 };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_CrossAppDomainError", values));
                }
            }
        }

        internal DateTime ReadDateTime() =>
            DateHelper.FromBinaryRaw(this.ReadInt64());

        internal decimal ReadDecimal() =>
            decimal.Parse(this.dataReader.ReadString(), CultureInfo.InvariantCulture);

        internal double ReadDouble() =>
            this.dataReader.ReadDouble();

        internal void ReadEnd()
        {
        }

        internal short ReadInt16() =>
            this.dataReader.ReadInt16();

        internal int ReadInt32() =>
            this.dataReader.ReadInt32();

        internal long ReadInt64() =>
            this.dataReader.ReadInt64();

        private void ReadMemberPrimitiveTyped()
        {
            if (this.memberPrimitiveTyped == null)
            {
                this.memberPrimitiveTyped = new MemberPrimitiveTyped();
            }
            this.memberPrimitiveTyped.Read(this);
            this.memberPrimitiveTyped.Dump();
            this.prs.PRobjectTypeEnum = InternalObjectTypeE.Object;
            ObjectProgress progress = (ObjectProgress)this.stack.Peek();
            this.prs.Init();
            this.prs.PRvarValue = this.memberPrimitiveTyped.value;
            this.prs.PRkeyDt = Converter.ToComType(this.memberPrimitiveTyped.primitiveTypeEnum);
            this.prs.PRdtType = Converter.ToType(this.memberPrimitiveTyped.primitiveTypeEnum);
            this.prs.PRdtTypeCode = this.memberPrimitiveTyped.primitiveTypeEnum;
            if (progress == null)
            {
                this.prs.PRparseTypeEnum = InternalParseTypeE.Object;
                this.prs.PRname = "System.Variant";
            }
            else
            {
                this.prs.PRparseTypeEnum = InternalParseTypeE.Member;
                this.prs.PRmemberValueEnum = InternalMemberValueE.InlineValue;
                InternalObjectTypeE objectTypeEnum = progress.objectTypeEnum;
                if (objectTypeEnum != InternalObjectTypeE.Object)
                {
                    if (objectTypeEnum != InternalObjectTypeE.Array)
                    {
                        object[] values = new object[] { progress.objectTypeEnum.ToString() };
                        throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_ObjectTypeEnum", values));
                    }
                    this.prs.PRmemberTypeEnum = InternalMemberTypeE.Item;
                }
                else
                {
                    this.prs.PRname = progress.name;
                    this.prs.PRmemberTypeEnum = InternalMemberTypeE.Field;
                }
            }
            this.objectReader.Parse(this.prs);
        }

        
        private void ReadMemberPrimitiveUnTyped()
        {
            ObjectProgress progress = (ObjectProgress)this.stack.Peek();
            if (this.memberPrimitiveUnTyped == null)
            {
                this.memberPrimitiveUnTyped = new MemberPrimitiveUnTyped();
            }
            this.memberPrimitiveUnTyped.Set((InternalPrimitiveTypeE)this.expectedTypeInformation);
            this.memberPrimitiveUnTyped.Read(this);
            this.memberPrimitiveUnTyped.Dump();
            this.prs.Init();
            this.prs.PRvarValue = this.memberPrimitiveUnTyped.value;
            this.prs.PRdtTypeCode = (InternalPrimitiveTypeE)this.expectedTypeInformation;
            this.prs.PRdtType = Converter.ToType(this.prs.PRdtTypeCode);
            this.prs.PRparseTypeEnum = InternalParseTypeE.Member;
            this.prs.PRmemberValueEnum = InternalMemberValueE.InlineValue;
            if (progress.objectTypeEnum == InternalObjectTypeE.Object)
            {
                this.prs.PRmemberTypeEnum = InternalMemberTypeE.Field;
                this.prs.PRname = progress.name;
            }
            else
            {
                this.prs.PRmemberTypeEnum = InternalMemberTypeE.Item;
            }
            this.objectReader.Parse(this.prs);
        }

        private void ReadMemberReference()
        {
            if (this.memberReference == null)
            {
                this.memberReference = new MemberReference();
            }
            this.memberReference.Read(this);
            this.memberReference.Dump();
            ObjectProgress progress = (ObjectProgress)this.stack.Peek();
            this.prs.Init();
            this.prs.PRidRef = this.objectReader.GetId((long)this.memberReference.idRef);
            this.prs.PRparseTypeEnum = InternalParseTypeE.Member;
            this.prs.PRmemberValueEnum = InternalMemberValueE.Reference;
            if (progress.objectTypeEnum == InternalObjectTypeE.Object)
            {
                this.prs.PRmemberTypeEnum = InternalMemberTypeE.Field;
                this.prs.PRname = progress.name;
                this.prs.PRdtType = progress.dtType;
            }
            else
            {
                this.prs.PRmemberTypeEnum = InternalMemberTypeE.Item;
            }
            this.objectReader.Parse(this.prs);
        }

        private void ReadMessageEnd()
        {
            if (messageEnd == null)
            {
                messageEnd = new MessageEnd();
            }
            messageEnd.Read(this);
            messageEnd.Dump();
            if (!this.stack.IsEmpty())
            {
                throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_StreamEnd"));
            }
        }

        internal void ReadMethodObject(BinaryHeaderEnum binaryHeaderEnum)
        {
            if (binaryHeaderEnum == BinaryHeaderEnum.MethodCall)
            {
                BinaryMethodCall binaryMethodCall = new BinaryMethodCall();
                binaryMethodCall.Read(this);
                binaryMethodCall.Dump();
                this.objectReader.SetMethodCall(binaryMethodCall);
            }
            else
            {
                BinaryMethodReturn binaryMethodReturn = new BinaryMethodReturn();
                binaryMethodReturn.Read(this);
                binaryMethodReturn.Dump();
                this.objectReader.SetMethodReturn(binaryMethodReturn);
            }
        }

        private void ReadObject()
        {
            if (this.binaryObject == null)
            {
                this.binaryObject = new BinaryObject();
            }
            this.binaryObject.Read(this);
            this.binaryObject.Dump();
            ObjectMap map = (ObjectMap)this.ObjectMapIdTable[this.binaryObject.mapId];
            if (map == null)
            {
                object[] values = new object[] { this.binaryObject.mapId };
                throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_Map", values));
            }
            ObjectProgress op = this.GetOp();
            ParseRecord pr = op.pr;
            this.stack.Push(op);
            op.objectTypeEnum = InternalObjectTypeE.Object;
            op.binaryTypeEnumA = map.binaryTypeEnumA;
            op.memberNames = map.memberNames;
            op.memberTypes = map.memberTypes;
            op.typeInformationA = map.typeInformationA;
            op.memberLength = op.binaryTypeEnumA.Length;
            ObjectProgress progress2 = (ObjectProgress)this.stack.PeekPeek();
            if ((progress2 == null) || progress2.isInitial)
            {
                op.name = map.objectName;
                pr.PRparseTypeEnum = InternalParseTypeE.Object;
                op.memberValueEnum = InternalMemberValueE.Empty;
            }
            else
            {
                pr.PRparseTypeEnum = InternalParseTypeE.Member;
                pr.PRmemberValueEnum = InternalMemberValueE.Nested;
                op.memberValueEnum = InternalMemberValueE.Nested;
                InternalObjectTypeE objectTypeEnum = progress2.objectTypeEnum;
                if (objectTypeEnum != InternalObjectTypeE.Object)
                {
                    if (objectTypeEnum != InternalObjectTypeE.Array)
                    {
                        object[] objArray2 = new object[] { progress2.objectTypeEnum.ToString() };
                        throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_Map", objArray2));
                    }
                    pr.PRmemberTypeEnum = InternalMemberTypeE.Item;
                    op.memberTypeEnum = InternalMemberTypeE.Item;
                }
                else
                {
                    pr.PRname = progress2.name;
                    pr.PRmemberTypeEnum = InternalMemberTypeE.Field;
                    op.memberTypeEnum = InternalMemberTypeE.Field;
                }
            }
            pr.PRobjectId = this.objectReader.GetId((long)this.binaryObject.objectId);
            pr.PRobjectInfo = map.CreateObjectInfo(ref pr.PRsi, ref pr.PRmemberData);
            if (pr.PRobjectId == this.topId)
            {
                pr.PRobjectPositionEnum = InternalObjectPositionE.Top;
            }
            pr.PRobjectTypeEnum = InternalObjectTypeE.Object;
            pr.PRkeyDt = map.objectName;
            pr.PRdtType = map.objectType;
            pr.PRdtTypeCode = InternalPrimitiveTypeE.Invalid;
            this.objectReader.Parse(pr);
        }

        private void ReadObjectNull(BinaryHeaderEnum binaryHeaderEnum)
        {
            if (this.objectNull == null)
            {
                this.objectNull = new ObjectNull();
            }
            this.objectNull.Read(this, binaryHeaderEnum);
            this.objectNull.Dump();
            ObjectProgress progress = (ObjectProgress)this.stack.Peek();
            this.prs.Init();
            this.prs.PRparseTypeEnum = InternalParseTypeE.Member;
            this.prs.PRmemberValueEnum = InternalMemberValueE.Null;
            if (progress.objectTypeEnum == InternalObjectTypeE.Object)
            {
                this.prs.PRmemberTypeEnum = InternalMemberTypeE.Field;
                this.prs.PRname = progress.name;
                this.prs.PRdtType = progress.dtType;
            }
            else
            {
                this.prs.PRmemberTypeEnum = InternalMemberTypeE.Item;
                this.prs.PRnullCount = this.objectNull.nullCount;
                progress.ArrayCountIncrement(this.objectNull.nullCount - 1);
            }
            this.objectReader.Parse(this.prs);
        }

        private void ReadObjectString(BinaryHeaderEnum binaryHeaderEnum)
        {
            if (this.objectString == null)
            {
                this.objectString = new BinaryObjectString();
            }
            if (binaryHeaderEnum == BinaryHeaderEnum.ObjectString)
            {
                this.objectString.Read(this);
                this.objectString.Dump();
            }
            else
            {
                if (this.crossAppDomainString == null)
                {
                    this.crossAppDomainString = new BinaryCrossAppDomainString();
                }
                this.crossAppDomainString.Read(this);
                this.crossAppDomainString.Dump();
                this.objectString.value = this.objectReader.CrossAppDomainArray(this.crossAppDomainString.value) as string;
                if (this.objectString.value == null)
                {
                    object[] values = new object[] { "String", this.crossAppDomainString.value };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_CrossAppDomainError", values));
                }
                this.objectString.objectId = this.crossAppDomainString.objectId;
            }
            this.prs.Init();
            this.prs.PRparseTypeEnum = InternalParseTypeE.Object;
            this.prs.PRobjectId = this.objectReader.GetId((long)this.objectString.objectId);
            if (this.prs.PRobjectId == this.topId)
            {
                this.prs.PRobjectPositionEnum = InternalObjectPositionE.Top;
            }
            this.prs.PRobjectTypeEnum = InternalObjectTypeE.Object;
            ObjectProgress progress = (ObjectProgress)this.stack.Peek();
            this.prs.PRvalue = this.objectString.value;
            this.prs.PRkeyDt = "System.String";
            this.prs.PRdtType = Converter.typeofString;
            this.prs.PRdtTypeCode = InternalPrimitiveTypeE.Invalid;
            this.prs.PRvarValue = this.objectString.value;
            if (progress == null)
            {
                this.prs.PRparseTypeEnum = InternalParseTypeE.Object;
                this.prs.PRname = "System.String";
            }
            else
            {
                this.prs.PRparseTypeEnum = InternalParseTypeE.Member;
                this.prs.PRmemberValueEnum = InternalMemberValueE.InlineValue;
                InternalObjectTypeE objectTypeEnum = progress.objectTypeEnum;
                if (objectTypeEnum != InternalObjectTypeE.Object)
                {
                    if (objectTypeEnum != InternalObjectTypeE.Array)
                    {
                        object[] objArray2 = new object[] { progress.objectTypeEnum.ToString() };
                        throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_ObjectTypeEnum", objArray2));
                    }
                    this.prs.PRmemberTypeEnum = InternalMemberTypeE.Item;
                }
                else
                {
                    this.prs.PRname = progress.name;
                    this.prs.PRmemberTypeEnum = InternalMemberTypeE.Field;
                }
            }
            this.objectReader.Parse(this.prs);
        }

        internal void ReadObjectWithMap(BinaryHeaderEnum binaryHeaderEnum)
        {
            if (this.bowm == null)
            {
                this.bowm = new BinaryObjectWithMap(binaryHeaderEnum);
            }
            else
            {
                this.bowm.binaryHeaderEnum = binaryHeaderEnum;
            }
            this.bowm.Read(this);
            this.bowm.Dump();
            this.ReadObjectWithMap(this.bowm);
        }

        private void ReadObjectWithMap(BinaryObjectWithMap record)
        {
            BinaryAssemblyInfo assemblyInfo = null;
            ObjectProgress op = this.GetOp();
            ParseRecord pr = op.pr;
            this.stack.Push(op);
            if (record.binaryHeaderEnum == BinaryHeaderEnum.ObjectWithMapAssemId)
            {
                if (record.assemId < 1)
                {
                    object[] values = new object[] { record.name };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_Assembly", values));
                }
                assemblyInfo = (BinaryAssemblyInfo)this.AssemIdToAssemblyTable[record.assemId];
                if (assemblyInfo == null)
                {
                    object[] objArray2 = new object[] { record.assemId + " " + record.name };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_Assembly", objArray2));
                }
            }
            else if (record.binaryHeaderEnum == BinaryHeaderEnum.ObjectWithMap)
            {
                assemblyInfo = this.SystemAssemblyInfo;
            }
            Type objectType = this.objectReader.GetType(assemblyInfo, record.name);
            ObjectMap map = ObjectMap.Create(record.name, objectType, record.memberNames, this.objectReader, record.objectId, assemblyInfo);
            this.ObjectMapIdTable[record.objectId] = map;
            op.objectTypeEnum = InternalObjectTypeE.Object;
            op.binaryTypeEnumA = map.binaryTypeEnumA;
            op.typeInformationA = map.typeInformationA;
            op.memberLength = op.binaryTypeEnumA.Length;
            op.memberNames = map.memberNames;
            op.memberTypes = map.memberTypes;
            ObjectProgress progress2 = (ObjectProgress)this.stack.PeekPeek();
            if ((progress2 == null) || progress2.isInitial)
            {
                op.name = record.name;
                pr.PRparseTypeEnum = InternalParseTypeE.Object;
                op.memberValueEnum = InternalMemberValueE.Empty;
            }
            else
            {
                pr.PRparseTypeEnum = InternalParseTypeE.Member;
                pr.PRmemberValueEnum = InternalMemberValueE.Nested;
                op.memberValueEnum = InternalMemberValueE.Nested;
                InternalObjectTypeE objectTypeEnum = progress2.objectTypeEnum;
                if (objectTypeEnum != InternalObjectTypeE.Object)
                {
                    if (objectTypeEnum != InternalObjectTypeE.Array)
                    {
                        object[] objArray3 = new object[] { progress2.objectTypeEnum.ToString() };
                        throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_ObjectTypeEnum", objArray3));
                    }
                    pr.PRmemberTypeEnum = InternalMemberTypeE.Item;
                    op.memberTypeEnum = InternalMemberTypeE.Field;
                }
                else
                {
                    pr.PRname = progress2.name;
                    pr.PRmemberTypeEnum = InternalMemberTypeE.Field;
                    op.memberTypeEnum = InternalMemberTypeE.Field;
                }
            }
            pr.PRobjectTypeEnum = InternalObjectTypeE.Object;
            pr.PRobjectId = this.objectReader.GetId((long)record.objectId);
            pr.PRobjectInfo = map.CreateObjectInfo(ref pr.PRsi, ref pr.PRmemberData);
            if (pr.PRobjectId == this.topId)
            {
                pr.PRobjectPositionEnum = InternalObjectPositionE.Top;
            }
            pr.PRkeyDt = record.name;
            pr.PRdtType = map.objectType;
            pr.PRdtTypeCode = InternalPrimitiveTypeE.Invalid;
            this.objectReader.Parse(pr);
        }
        internal void ReadObjectWithMapTyped(BinaryHeaderEnum binaryHeaderEnum)
        {
            if (this.bowmt == null)
            {
                this.bowmt = new BinaryObjectWithMapTyped(binaryHeaderEnum);
            }
            else
            {
                this.bowmt.binaryHeaderEnum = binaryHeaderEnum;
            }
            this.bowmt.Read(this);
            this.ReadObjectWithMapTyped(this.bowmt);
        }

        private void ReadObjectWithMapTyped(BinaryObjectWithMapTyped record)
        {
            BinaryAssemblyInfo assemblyInfo = null;
            ObjectProgress op = this.GetOp();
            ParseRecord pr = op.pr;
            this.stack.Push(op);
            if (record.binaryHeaderEnum == BinaryHeaderEnum.ObjectWithMapTypedAssemId)
            {
                if (record.assemId < 1)
                {
                    object[] values = new object[] { record.name };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_AssemblyId", values));
                }
                assemblyInfo = (BinaryAssemblyInfo)this.AssemIdToAssemblyTable[record.assemId];
                if (assemblyInfo == null)
                {
                    object[] objArray2 = new object[] { record.assemId + " " + record.name };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_AssemblyId", objArray2));
                }
            }
            else if (record.binaryHeaderEnum == BinaryHeaderEnum.ObjectWithMapTyped)
            {
                assemblyInfo = this.SystemAssemblyInfo;
            }
            ObjectMap map = ObjectMap.Create(record.name, record.memberNames, record.binaryTypeEnumA, record.typeInformationA, record.memberAssemIds, this.objectReader, record.objectId, assemblyInfo, this.AssemIdToAssemblyTable);
            this.ObjectMapIdTable[record.objectId] = map;
            op.objectTypeEnum = InternalObjectTypeE.Object;
            op.binaryTypeEnumA = map.binaryTypeEnumA;
            op.typeInformationA = map.typeInformationA;
            op.memberLength = op.binaryTypeEnumA.Length;
            op.memberNames = map.memberNames;
            op.memberTypes = map.memberTypes;
            ObjectProgress progress2 = (ObjectProgress)this.stack.PeekPeek();
            if ((progress2 == null) || progress2.isInitial)
            {
                op.name = record.name;
                pr.PRparseTypeEnum = InternalParseTypeE.Object;
                op.memberValueEnum = InternalMemberValueE.Empty;
            }
            else
            {
                pr.PRparseTypeEnum = InternalParseTypeE.Member;
                pr.PRmemberValueEnum = InternalMemberValueE.Nested;
                op.memberValueEnum = InternalMemberValueE.Nested;
                InternalObjectTypeE objectTypeEnum = progress2.objectTypeEnum;
                if (objectTypeEnum != InternalObjectTypeE.Object)
                {
                    if (objectTypeEnum != InternalObjectTypeE.Array)
                    {
                        object[] objArray3 = new object[] { progress2.objectTypeEnum.ToString() };
                        throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_ObjectTypeEnum", objArray3));
                    }
                    pr.PRmemberTypeEnum = InternalMemberTypeE.Item;
                    op.memberTypeEnum = InternalMemberTypeE.Item;
                }
                else
                {
                    pr.PRname = progress2.name;
                    pr.PRmemberTypeEnum = InternalMemberTypeE.Field;
                    op.memberTypeEnum = InternalMemberTypeE.Field;
                }
            }
            pr.PRobjectTypeEnum = InternalObjectTypeE.Object;
            pr.PRobjectInfo = map.CreateObjectInfo(ref pr.PRsi, ref pr.PRmemberData);
            pr.PRobjectId = this.objectReader.GetId((long)record.objectId);
            if (pr.PRobjectId == this.topId)
            {
                pr.PRobjectPositionEnum = InternalObjectPositionE.Top;
            }
            pr.PRkeyDt = record.name;
            pr.PRdtType = map.objectType;
            pr.PRdtTypeCode = InternalPrimitiveTypeE.Invalid;
            this.objectReader.Parse(pr);
        }

        internal sbyte ReadSByte() =>
            ((sbyte)this.ReadByte());

        internal void ReadSerializationHeaderRecord()
        {
            SerializationHeaderRecord record = new SerializationHeaderRecord();
            record.Read(this);
            record.Dump();
            this.topId = (record.topId > 0) ? this.objectReader.GetId((long)record.topId) : ((long)record.topId);
            this.headerId = (record.headerId > 0) ? this.objectReader.GetId((long)record.headerId) : ((long)record.headerId);
        }

        internal float ReadSingle() =>
            this.dataReader.ReadSingle();

        internal string ReadString() =>
            this.dataReader.ReadString();

        internal TimeSpan ReadTimeSpan() =>
            new TimeSpan(this.ReadInt64());

        internal ushort ReadUInt16() =>
            this.dataReader.ReadUInt16();

        internal uint ReadUInt32() =>
            this.dataReader.ReadUInt32();

        internal ulong ReadUInt64() =>
            this.dataReader.ReadUInt64();

        internal object ReadValue(InternalPrimitiveTypeE code)
        {
            switch (code)
            {
                case InternalPrimitiveTypeE.Boolean:
                    return this.ReadBoolean();

                case InternalPrimitiveTypeE.Byte:
                    return this.ReadByte();

                case InternalPrimitiveTypeE.Char:
                    return this.ReadChar();

                case InternalPrimitiveTypeE.Decimal:
                    return this.ReadDecimal();

                case InternalPrimitiveTypeE.Double:
                    return this.ReadDouble();

                case InternalPrimitiveTypeE.Int16:
                    return this.ReadInt16();

                case InternalPrimitiveTypeE.Int32:
                    return this.ReadInt32();

                case InternalPrimitiveTypeE.Int64:
                    return this.ReadInt64();

                case InternalPrimitiveTypeE.SByte:
                    return this.ReadSByte();

                case InternalPrimitiveTypeE.Single:
                    return this.ReadSingle();

                case InternalPrimitiveTypeE.TimeSpan:
                    return this.ReadTimeSpan();

                case InternalPrimitiveTypeE.DateTime:
                    return this.ReadDateTime();

                case InternalPrimitiveTypeE.UInt16:
                    return this.ReadUInt16();

                case InternalPrimitiveTypeE.UInt32:
                    return this.ReadUInt32();

                case InternalPrimitiveTypeE.UInt64:
                    return this.ReadUInt64();
            }
            object[] values = new object[] { code.ToString() };
            throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_TypeCode", values));
        }

        
        internal void Run()
        {
            try
            {
                bool flag = true;
                this.ReadBegin();
                this.ReadSerializationHeaderRecord();
                while (flag)
                {
                    object[] objArray1;
                    byte num;
                    BinaryHeaderEnum binaryHeaderEnum = BinaryHeaderEnum.Object;
                    switch (this.expectedType)
                    {
                        case BinaryTypeEnum.Primitive:
                            this.ReadMemberPrimitiveUnTyped();
                            goto Label_0169;

                        case BinaryTypeEnum.String:
                        case BinaryTypeEnum.Object:
                        case BinaryTypeEnum.ObjectUrt:
                        case BinaryTypeEnum.ObjectUser:
                        case BinaryTypeEnum.ObjectArray:
                        case BinaryTypeEnum.StringArray:
                        case BinaryTypeEnum.PrimitiveArray:
                            num = this.dataReader.ReadByte();
                            binaryHeaderEnum = (BinaryHeaderEnum)num;
                            switch (binaryHeaderEnum)
                            {
                                case BinaryHeaderEnum.Object:
                                    goto Label_00C3;

                                case BinaryHeaderEnum.ObjectWithMap:
                                case BinaryHeaderEnum.ObjectWithMapAssemId:
                                    goto Label_00D9;

                                case BinaryHeaderEnum.ObjectWithMapTyped:
                                case BinaryHeaderEnum.ObjectWithMapTypedAssemId:
                                    goto Label_00E5;

                                case BinaryHeaderEnum.ObjectString:
                                case BinaryHeaderEnum.CrossAppDomainString:
                                    goto Label_00F7;

                                case BinaryHeaderEnum.Array:
                                case BinaryHeaderEnum.ArraySinglePrimitive:
                                case BinaryHeaderEnum.ArraySingleObject:
                                case BinaryHeaderEnum.ArraySingleString:
                                    goto Label_0100;

                                case BinaryHeaderEnum.MemberPrimitiveTyped:
                                    goto Label_0109;

                                case BinaryHeaderEnum.MemberReference:
                                    goto Label_0111;

                                case BinaryHeaderEnum.ObjectNull:
                                case BinaryHeaderEnum.ObjectNullMultiple256:
                                case BinaryHeaderEnum.ObjectNullMultiple:
                                    goto Label_0119;

                                case BinaryHeaderEnum.MessageEnd:
                                    goto Label_0122;

                                case BinaryHeaderEnum.CrossAppDomainMap:
                                    goto Label_00CE;

                                case BinaryHeaderEnum.MethodCall:
                                case BinaryHeaderEnum.MethodReturn:
                                    goto Label_00EE;
                            }
                            goto Label_0132;

                        default:
                            throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_TypeExpected"));
                    }
                    this.ReadAssembly(binaryHeaderEnum);
                    goto Label_0169;
                Label_00C3:
                    this.ReadObject();
                    goto Label_0169;
                Label_00CE:
                    this.ReadCrossAppDomainMap();
                    goto Label_0169;
                Label_00D9:
                    this.ReadObjectWithMap(binaryHeaderEnum);
                    goto Label_0169;
                Label_00E5:
                    this.ReadObjectWithMapTyped(binaryHeaderEnum);
                    goto Label_0169;
                Label_00EE:
                    this.ReadMethodObject(binaryHeaderEnum);
                    goto Label_0169;
                Label_00F7:
                    this.ReadObjectString(binaryHeaderEnum);
                    goto Label_0169;
                Label_0100:
                    this.ReadArray(binaryHeaderEnum);
                    goto Label_0169;
                Label_0109:
                    this.ReadMemberPrimitiveTyped();
                    goto Label_0169;
                Label_0111:
                    this.ReadMemberReference();
                    goto Label_0169;
                Label_0119:
                    this.ReadObjectNull(binaryHeaderEnum);
                    goto Label_0169;
                Label_0122:
                    flag = false;
                    this.ReadMessageEnd();
                    this.ReadEnd();
                    goto Label_0169;
                Label_0132:
                    objArray1 = new object[] { num };
                    throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_BinaryHeader", objArray1));
                Label_0169:
                    if (binaryHeaderEnum != BinaryHeaderEnum.Assembly)
                    {
                        bool next = false;
                        while (!next)
                        {
                            ObjectProgress op = (ObjectProgress)this.stack.Peek();
                            if (op == null)
                            {
                                this.expectedType = BinaryTypeEnum.ObjectUrt;
                                this.expectedTypeInformation = null;
                                next = true;
                            }
                            else
                            {
                                next = op.GetNext(out op.expectedType, out op.expectedTypeInformation);
                                this.expectedType = op.expectedType;
                                this.expectedTypeInformation = op.expectedTypeInformation;
                                if (!next)
                                {
                                    this.prs.Init();
                                    if (op.memberValueEnum == InternalMemberValueE.Nested)
                                    {
                                        this.prs.PRparseTypeEnum = InternalParseTypeE.MemberEnd;
                                        this.prs.PRmemberTypeEnum = op.memberTypeEnum;
                                        this.prs.PRmemberValueEnum = op.memberValueEnum;
                                        this.objectReader.Parse(this.prs);
                                    }
                                    else
                                    {
                                        this.prs.PRparseTypeEnum = InternalParseTypeE.ObjectEnd;
                                        this.prs.PRmemberTypeEnum = op.memberTypeEnum;
                                        this.prs.PRmemberValueEnum = op.memberValueEnum;
                                        this.objectReader.Parse(this.prs);
                                    }
                                    this.stack.Pop();
                                    this.PutOp(op);
                                }
                            }
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
                throw new SerializationException(EnvironmentV2.GetResourceString("Serialization_StreamEnd"));
            }
        }

        // Properties
        internal SizedArray AssemIdToAssemblyTable
        {
            get
            {
                if (this.assemIdToAssemblyTable == null)
                {
                    this.assemIdToAssemblyTable = new SizedArray(2);
                }
                return this.assemIdToAssemblyTable;
            }
        }

        internal SizedArray ObjectMapIdTable
        {
            get
            {
                if (this.objectMapIdTable == null)
                {
                    this.objectMapIdTable = new SizedArray();
                }
                return this.objectMapIdTable;
            }
        }

        internal ParseRecord prs
        {
            get
            {
                if (this.PRS == null)
                {
                    this.PRS = new ParseRecord();
                }
                return this.PRS;
            }
        }

        internal BinaryAssemblyInfo SystemAssemblyInfo
        {
            get
            {
                if (this.systemAssemblyInfo == null)
                {
                    this.systemAssemblyInfo = new BinaryAssemblyInfo(Converter.urtAssemblyString, Converter.urtAssembly);
                }
                return this.systemAssemblyInfo;
            }
        }
    }

}
