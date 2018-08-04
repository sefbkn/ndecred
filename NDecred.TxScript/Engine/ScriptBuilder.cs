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
        private readonly List<ParsedOpCode> _opCodes = new List<ParsedOpCode>();

        public void AddData(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            AddData(bytes);
        }
        
        public void AddData(byte[] data)
        {
            var opCode = OpCodeUtil.CanonicalOpCodeForData(data);
            var includeData = opCode.IsOpData() || opCode.IsPushDataOpCode();
            var parsedOpCode = new ParsedOpCode(opCode, includeData ? data : null);
            AddOpCode(parsedOpCode);
        }

        public void AddInt64(ulong number)
        {
            if (number == 0)
            {
                
            }
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.WriteVariableLengthInteger(number);
                var bytes = ms.ToArray();
                AddData(bytes);
            }
        }

        public void AddOpCode(ParsedOpCode opCode)
        {
            _opCodes.Add(opCode);
        }

        public Script ToScript()
        {
            return new Script(_opCodes);
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