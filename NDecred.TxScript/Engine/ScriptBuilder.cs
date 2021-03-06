﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using NDecred.Common;
using NDecred.Cryptography;
using NDecred.Wire;

namespace NDecred.TxScript
{
    public class ScriptBuilder
    {
        private readonly List<byte> _scriptBytes = new List<byte>();

        public ScriptBuilder()
        {
        }

        public ScriptBuilder(params object[] data)
        {
            Add(data);
        }

        public void AddRawScriptBytes(byte[] data)
        {
            _scriptBytes.AddRange(data);
        }

        public void Add(params object[] elements)
        {
            foreach (var element in elements)
            {
                var type = element.GetType();
                if(type == typeof(string))
                    AddData((string) element);
                else if(type == typeof(byte[]))
                    AddData((byte[]) element);
                else if (type == typeof(OpCode))
                    AddOpCode((OpCode) element);
                else if(type == typeof(long) || type == typeof(int) || type == typeof(short))
                    AddInt64(long.Parse(element.ToString()));
                else if(type == typeof(ParsedOpCode))
                    AddOpCode((ParsedOpCode) element);
                else
                    throw new InvalidOperationException($"Cannot add object of type {type} to script");
            }
        }

        public void AddData(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            AddData(bytes);
        }

        public void AddData(byte[] data)
        {
            var opCode = OpCodeUtil.CanonicalOpCodeForData(data);
            var parsedOpCode = new ParsedOpCode(opCode, data);
            AddOpCode(parsedOpCode);
        }

        public void AddInt64(long number)
        {
            var integer = new ScriptInteger(number);
            var bytes = integer.ToBytes();
            AddData(bytes);
        }

        public void AddOpCode(OpCode opCode)
        {
            _scriptBytes.Add((byte) opCode);
        }

        public void AddOpCode(ParsedOpCode opCode)
        {
            _scriptBytes.AddRange(opCode.Serialize());
        }

        public Script ToScript()
        {
            return new Script(_scriptBytes.ToArray());
        }

        public static Script P2PKHUnlockingScript(ECDSAType signatureType, byte[] signature, byte[] publicKey)
        {
            if(signatureType != ECDSAType.ECTypeSecp256k1)
                throw new NotImplementedException();

            using (var ms = new MemoryStream(signature.Length + publicKey.Length + 2))
            using (var bw = new BinaryWriter(ms))
            {
                bw.WriteVariableLengthBytes(signature);
                bw.WriteVariableLengthBytes(publicKey);
                return new Script(ms.ToArray());
            }
        }

        public static Script P2PKHLockingScript(ECDSAType signatureType, byte[] publicKeyHash)
        {
            switch (signatureType)
            {
                case ECDSAType.ECTypeSecp256k1:
                    var bytes = new byte[25];
                    bytes[0] = (byte) OpCode.OP_DUP;
                    bytes[1] = (byte) OpCode.OP_HASH160;
                    bytes[2] = (byte) OpCode.OP_DATA_20;
                    Array.Copy(publicKeyHash, 0, bytes, 3, 20);
                    bytes[23] = (byte) OpCode.OP_EQUALVERIFY;
                    bytes[24] = (byte) OpCode.OP_CHECKSIG;
                    return new Script(bytes);
            }

            throw new NotImplementedException();
        }
    }
}
