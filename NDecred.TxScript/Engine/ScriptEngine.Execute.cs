using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NDecred.Common;
using NDecred.Cryptography;
using NDecred.Wire;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.EC;

namespace NDecred.TxScript
{
    public partial class ScriptEngine
    {
        private void OpNop(ParsedOpCode op)
        {
            if (op.Code.IsUpgradableNop())
                throw new ReservedOpCodeException(op.Code);
        }

        private void OpDisabled(ParsedOpCode op)
        {
            throw new DisabledOpCodeException(op.Code);
        }
        
        private void OpFalse()
        {
            MainStack.Push(new byte[0]);
        }

        private void OpPushData(ParsedOpCode op)
        {
            if (op.Code.IsOpN())
            {
                var value = (op.Code - OpCode.OP_1) + 1;
                MainStack.Push(value);
            }
            else
            {
                MainStack.Push(op.Data);                
            }
        }

        // Push an integer onto the stack.
        // Value is converted to ScriptInteger so consensus rules are followed.
        private void OpPush(int value)
        {
            var scriptInteger = new ScriptInteger(value);
            MainStack.Push(scriptInteger.ToBytes());
        }

        private void OpIf()
        {
            switch (BranchStack.Peek())
            {
                case BranchOption.Skip:
                case BranchOption.False:
                    BranchStack.Push(BranchOption.Skip);
                    break;
                case BranchOption.True:
                    BranchStack.Push(MainStack.PopBool() ? BranchOption.True : BranchOption.False);
                    break;
            }
        }

        private void OpNotIf()
        {
            switch (BranchStack.Peek())
            {
                case BranchOption.Skip:
                case BranchOption.False:
                    BranchStack.Push(BranchOption.Skip);
                    break;
                case BranchOption.True:
                    BranchStack.Push(!MainStack.PopBool() ? BranchOption.True : BranchOption.False);
                    break;
            }
        }

        private void OpElse()
        {
            switch (BranchStack.Peek())
            {
                case BranchOption.Skip:
                    break;
                case BranchOption.False:
                    BranchStack.Replace(BranchOption.True);
                    break;
                case BranchOption.True:
                    BranchStack.Replace(BranchOption.False);
                    break;
            }
        }
        
        private void OpEndIf()
        {
            BranchStack.Discard();
        }

        private void OpVerify()
        {
            if (!MainStack.PopBool())
                throw new VerifyFailedException();
        }
        
        private void OpReturn()
        {
            throw new EarlyReturnException();
        }

        private void OpReserved(ParsedOpCode op)
        {
            throw new ReservedOpCodeException(op.Code);
        }

        private void OpToAltStack()
        {
            AltStack.Push(MainStack.Pop());
        }
        
        private void OpFromAltStack()
        {
            MainStack.Push(AltStack.Pop());
        }

        private void OpIfDup()
        {
            var bytes = MainStack.Peek();
            
            var scriptInt = new ScriptInteger(bytes, false, bytes.Length);
            if ((int) scriptInt == 0)
                return;
            
            MainStack.Push(bytes);
        }

        private void OpDepth()
        {
            var bytes = new ScriptInteger(MainStack.Size()).ToBytes();
            MainStack.Push(bytes);
        }

        private void OpDrop()
        {
            MainStack.Pop();
        }

        private void OpDup()
        {
            var value = MainStack.Peek();
            MainStack.Push(value);
        }

        private void OpNip()
        {
            MainStack.Pop(1);
        }
        
        private void OpOver()
        {
            MainStack.Push(MainStack.Peek(1));
        }

        private void OpPick()
        {
            var n = MainStack.PopInt32();
            MainStack.Push(MainStack.Peek(n));
        }

        private void OpRoll()
        {
            var n = MainStack.PopInt32();
            var val = MainStack.Pop(n);
            MainStack.Push(val);
        }

        private void OpRot()
        {
            var a = MainStack.Pop();
            var b = MainStack.Pop();
            var c = MainStack.Pop();
            
            MainStack.Push(b);
            MainStack.Push(c);
            MainStack.Push(a);
        }
        
        private void OpSwap()
        {
            var a = MainStack.Pop();
            var b = MainStack.Pop();
            
            MainStack.Push(a);
            MainStack.Push(b);
        }

        private void OpTuck()
        {
            var a = MainStack.Peek();
            MainStack.Push(a, 2);
        }

        private void Op2Drop()
        {
            MainStack.Pop();
            MainStack.Pop();
        }

        private void Op2Dup()
        {
            var a = MainStack.Peek(0);
            var b = MainStack.Peek(1);
            
            MainStack.Push(b);
            MainStack.Push(a);
        }

        private void Op3Dup()
        {
            var a = MainStack.Peek(0);
            var b = MainStack.Peek(1);
            var c = MainStack.Peek(2);
            
            MainStack.Push(c);
            MainStack.Push(b);
            MainStack.Push(a);
        }

        private void Op2Over()
        {
            var a = MainStack.Peek(2);
            var b = MainStack.Peek(3);
            
            MainStack.Push(b);
            MainStack.Push(a);
        }

        private void Op2Rot()
        {
            var a = MainStack.Pop(4);
            var b = MainStack.Pop(4);
            
            MainStack.Push(b);
            MainStack.Push(a);
        }

        private void Op2Swap()
        {
            var a = MainStack.Pop();
            var b = MainStack.Pop();
            var c = MainStack.Pop();
            var d = MainStack.Pop();
            
            MainStack.Push(b);
            MainStack.Push(a);
            MainStack.Push(d);
            MainStack.Push(c);
        }

        private void OpCat(ParsedOpCode op)
        {
            var first = MainStack.Pop();
            var second = MainStack.Pop();
                        
            var bytes = second.Concat(first).ToArray();
            MainStack.Push(bytes);
        }

        private void OpSubStr(ParsedOpCode op)
        {
            var startIndex = MainStack.PopInt32();
            var endIndexExclusive = MainStack.PopInt32();
            var array = MainStack.Pop();
            
            if(array.Length == 0)
                MainStack.Push(new byte[0]);
            else if(startIndex < 0 || endIndexExclusive < 0)
                throw new ScriptIndexOutOfRangeException(op.Code, "Negative substring index");
            else if(startIndex > array.Length || endIndexExclusive > array.Length)
                throw new ScriptIndexOutOfRangeException(op.Code, "Substring index out of bounds");
            else if(startIndex > endIndexExclusive)
                throw new ScriptIndexOutOfRangeException(op.Code, "Start index is greater than end index");
            else if(startIndex == endIndexExclusive)
                MainStack.Push(new byte[0]);
            else
            {
                var substr = array.Skip(startIndex).Take(endIndexExclusive - startIndex).ToArray();
                MainStack.Push(substr);
            }
        }

        private void OpLeft(ParsedOpCode op)
        {
            var high = MainStack.PopInt32();
            var data = MainStack.Pop();
            
            if(data.Length == 0 || high == 0)
                MainStack.Push(new byte[0]);
            else if(high < 0)
                throw new ScriptIndexOutOfRangeException(op.Code, "upper boundary less than zero");
            else if(high > data.Length)
                throw new ScriptIndexOutOfRangeException(op.Code, "upper boundary greater than array length");
            else if(high < 0)
                throw new ScriptIndexOutOfRangeException(op.Code, "upper boundary less than zero");
            else
            {
                var bytes = data.Take(high).ToArray();
                MainStack.Push(bytes);
            }
        }
        
        private void OpRight(ParsedOpCode op)
        {
            var index = MainStack.PopInt32();
            var data = MainStack.Pop();

            if (data.Length == 0 || index == data.Length)
                MainStack.Push(new byte[0]);
            else if(index < 0)
                throw new ScriptIndexOutOfRangeException(op.Code, "upper boundary less than zero");
            else if(index > data.Length)
                throw new ScriptIndexOutOfRangeException(op.Code, "upper boundary greater than array length");
            else
                MainStack.Push(data.Skip(index).ToArray());
        }

        private void OpSize(ParsedOpCode op)
        {
            var length = MainStack.Peek().Length;
            MainStack.Push(length);
        }

        private void OpInvert(ParsedOpCode op)
        {
            var value = MainStack.PopInt32();
            MainStack.Push(~value);
        }

        private void OpAnd(ParsedOpCode op)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(a & b);
        }

        private void OpOr(ParsedOpCode op)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(a | b);
        }

        private void OpXor(ParsedOpCode op)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(a ^ b);
        }

        private void OpEqual(ParsedOpCode op)
        {
            var a = MainStack.Pop();
            var b = MainStack.Pop();            
            MainStack.Push(a.SequenceEqual(b));
        }

        private void OpEqualVerify(ParsedOpCode op)
        {
            OpEqual(op);
            OpVerify();
        }

        private void OpRotr(ParsedOpCode op)
        {
            var rotate = MainStack.PopInt32();
            var value = MainStack.PopInt32();
            
            if(rotate < 0)
                throw new ScriptIndexOutOfRangeException(op.Code, "attempted to rotate by negative value");
            if(rotate > 31)
                throw new ScriptIndexOutOfRangeException(op.Code, "attempted to rotate by value > 31");

            var rotatedValue = (value >> rotate) | (value << (32 - rotate));
            MainStack.Push(rotatedValue);
        }

        private void OpRotl(ParsedOpCode op)
        {
            var rotate = MainStack.PopInt32();
            var value = MainStack.PopInt32();
            
            if(rotate < 0)
                throw new ScriptIndexOutOfRangeException(op.Code, "attempted to rotate by negative value");
            if(rotate > 31)
                throw new ScriptIndexOutOfRangeException(op.Code, "attempted to rotate by value > 31");

            var rotatedValue = (value << rotate) | (value >> (32 - rotate));
            MainStack.Push(rotatedValue);
        }

        private void Op1Add(ParsedOpCode op)
        {
            MainStack.Push(MainStack.PopInt32() + 1);
        }

        private void Op1Sub(ParsedOpCode op)
        {
            MainStack.Push(MainStack.PopInt32() - 1);
        }

        private void OpNegate(ParsedOpCode obj)
        {
            MainStack.Push(-MainStack.PopInt32());
        }

        private void OpAbs(ParsedOpCode obj)
        {
            var value = MainStack.PopInt32();
            MainStack.Push(Math.Abs(value));
        }

        private void OpNot(ParsedOpCode obj)
        {
            var value = MainStack.PopInt32();
            MainStack.Push(value == 0 ? 1 : 0);
        }

        private void Op0NotEqual(ParsedOpCode obj)
        {
            var value = MainStack.PopInt32();
            MainStack.Push(value == 0 ? 0 : 1);
        }

        private void OpAdd(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(a + b);
        }

        private void OpSub(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(b - a);
        }

        /// <summary>
        /// Multiplies top two numbers on the stack.
        /// </summary>
        /// <param name="obj"></param>
        private void OpMul(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(a * b);
        }

        private void OpDiv(ParsedOpCode op)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            
            if(a == 0)
                throw new ArithemeticException(op.Code, "Division by zero");
            
            MainStack.Push(b/a);
        }

        private void OpMod(ParsedOpCode op)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            
            if(a == 0)
                throw new ArithemeticException(op.Code, "Division by zero");
            
            MainStack.Push(b % a);
        }

        private void OpLShift(ParsedOpCode op)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            
            if(a < 0)
                throw new ScriptIndexOutOfRangeException(op.Code, "Negative shift");
            if(a > 32)
                throw new ScriptIndexOutOfRangeException(op.Code, "Shift overflow");

            MainStack.Push(b << a);
        }

        private void OpRShift(ParsedOpCode op)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            
            if(a < 0)
                throw new ScriptIndexOutOfRangeException(op.Code, "Negative shift");
            if(a > 32)
                throw new ScriptIndexOutOfRangeException(op.Code, "Shift overflow");

            MainStack.Push(b >> a);
        }

        private void OpBoolAnd(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(a != 0 && b != 0 ? 1 : 0);
        }

        private void OpBoolOr(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(a != 0 || b != 0 ? 1 : 0);
        }

        private void OpNumEqual(ParsedOpCode op)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(a == b ? 1 : 0);
        }

        private void OpNumEqualVerify(ParsedOpCode op)
        {
            OpNumEqual(op);
            OpVerify();
        }

        private void OpNumNotEqual(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(a != b ? 1 : 0);
        }
        
        private void OpLessThan(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(b < a ? 1 : 0);
        }

        private void OpGreaterThan(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(b > a ? 1 : 0);
        }
        
        private void OpLessThanOrEqual(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(b < a ? 1 : 0);
        }

        private void OpGreaterThanOrEqual(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(b > a ? 1 : 0);
        }

        private void OpMin(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(Math.Min(a, b));
        }

        private void OpMax(ParsedOpCode obj)
        {
            var a = MainStack.PopInt32();
            var b = MainStack.PopInt32();
            MainStack.Push(Math.Max(a, b));
        }

        private void OpWithin(ParsedOpCode obj)
        {
            var max = MainStack.PopInt32();
            var min = MainStack.PopInt32();
            var test = MainStack.PopInt32();
            
            MainStack.Push(min <= test && test < max ? 1 : 0);
        }

        private void OpRipemd160(ParsedOpCode obj)
        {
            var value = MainStack.Pop();
            MainStack.Push(HashUtil.Ripemd160(value));
        }

        private void OpSha1(ParsedOpCode obj)
        {
            var value = MainStack.Pop();
            MainStack.Push(HashUtil.Sha1(value));
        }

        private void OpBlake256(ParsedOpCode obj)
        {
            var value = MainStack.Pop();
            MainStack.Push(HashUtil.Blake256(value));
        }

        private void OpHash160(ParsedOpCode obj)
        {
            var value = MainStack.Pop();
            MainStack.Push(HashUtil.Ripemd160(HashUtil.Blake256(value)));
        }

        private void OpHash256(ParsedOpCode obj)
        {
            var value = MainStack.Pop();
            MainStack.Push(HashUtil.Blake256D(value));
        }

        private void OpInvalid(ParsedOpCode op)
        {
            throw new InvalidOpCodeException(op.Code);
        }
        
        private void OpSha256(ParsedOpCode op)
        {
            if (!Options.EnableSha256)
                throw new ReservedOpCodeException(OpCode.OP_UNKNOWN192);
            
            var data = MainStack.Pop();
            var hash = HashUtil.Sha256(data);
            MainStack.Push(hash);
        }

        public void OpCheckSig(ParsedOpCode op, MsgTx transaction)
        {
            try
            {
                var rawPublicKey = MainStack.Pop();
                var rawSignature = MainStack.Pop();

                if (rawSignature.Length < 1)
                {
                    MainStack.Push(false);
                    return;
                }

                var signature = rawSignature.Take(rawSignature.Length - 1).ToArray();
                var signatureType = (SignatureHashType) rawSignature.Last();

                AssertSignatureHashType(signatureType);
                AssertSignatureEncoding(signature);
                AssertPublicKeyEncoding(rawPublicKey);

                var subScript = Script.GetOpCodesWithoutData(rawSignature);            
                var hash = CalculateSignatureHash(subScript, signatureType, (MsgTx) transaction.Clone(), _index);
                
                var ecSignature = new ECSignature(signature);
                var securityService = new ECPublicSecurityService(rawPublicKey);
                var isValidSignature = securityService.VerifySignature(hash, ecSignature);
                
                MainStack.Push(isValidSignature);
            }
            catch (ScriptException)
            {
                MainStack.Push(false);
            }
        }
        
        private void OpCheckSigVerify(ParsedOpCode op, MsgTx transaction)
        {
            OpCheckSig(op, transaction);
            OpVerify();
        }

        public static byte[] CalculateSignatureHash(ParsedOpCode[] subScript, SignatureHashType hashType, MsgTx transaction, int index)
        {
            const SignatureHashType mask = (SignatureHashType) 0x1f;
            
            if ((hashType & mask) == SignatureHashType.Single && index >= transaction.TxOut.Length)
                throw new InvalidSignatureException("SignatureHashType.Single index out of range");

            // Clear out signature scripts for input transactions not at index
            // transactionIndex
            for (var i = 0; i < transaction.TxIn.Length; i++)
                transaction.TxIn[i].SignatureScript = 
                    i == index ? 
                        subScript.SelectMany(s => s.Serialize()).ToArray() 
                        : new byte[0];

            switch (hashType & mask)
            {
                case SignatureHashType.None:
                    transaction.TxOut = new TxOut[0];
                    for(var i = 0; i < transaction.TxIn.Length; i++)
                        if (i != index)
                            transaction.TxIn[i].Sequence = 0;
                    break;
                case SignatureHashType.Single:
                    transaction.TxOut = new TxOut[index];

                    for (var i = 0; i < index; i++)
                    {
                        transaction.TxOut[i].Value = -1;
                        transaction.TxOut[i].PkScript = null;
                    }

                    for (var i = 0; i < transaction.TxIn.Length; i++)
                        if (i != index)
                            transaction.TxIn[i].Sequence = 0;
                    break;
                case SignatureHashType.Old:
                    break;
                case SignatureHashType.All:
                    break;
                case SignatureHashType.AllValue:
                    break;
                case SignatureHashType.AnyOneCanPay:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if ((hashType & SignatureHashType.AnyOneCanPay) != 0)
            {
                transaction.TxIn = transaction.TxIn
                    .Skip(index)
                    .Take(1)
                    .ToArray();
            }
            
            var wbuf = new List<byte>(32 * 2 + 4);
            wbuf.AddRange(BitConverter.GetBytes((uint) hashType));

            var prefixHash = transaction.GetHash(TxSerializeType.NoWitness);
            var witnessHash = transaction.GetHash(
                (hashType & mask) != SignatureHashType.All ? 
                TxSerializeType.WitnessSigning : 
                TxSerializeType.WitnessValueSigning
            );

            wbuf.AddRange(prefixHash);
            wbuf.AddRange(witnessHash);

            return HashUtil.Blake256(wbuf.ToArray());
        }

        private static void AssertPublicKeyEncoding(byte[] publicKey)
        {
            switch (publicKey.Length)
            {
                // Checks for a compressed key
                case 33 when publicKey[0] == 0x02 || publicKey[0] == 0x03:
                // Checks for an uncompressed key
                case 65 when publicKey[0] == 0x04:
                    return;
            }

            throw new InvalidSignatureException("Unsupported public key format.");
        }
        
        private void AssertSignatureHashType(SignatureHashType type)
        {
            var t = type & ~SignatureHashType.AnyOneCanPay;
            if(t < SignatureHashType.All || t > SignatureHashType.Single)
                throw new InvalidSignatureException($"Invalid hash type {(byte)type:X}");
        }
        
        private void AssertSignatureEncoding(byte[] signature)
        {   
            if(signature.Length < 8)
                throw new InvalidSignatureException("Signature length too short");

            var signatureType = signature[0];
            var expectedSignatureLength = signature[1];
            var signatureIntegerMarker = signature[2];
            var rLen = signature[3];
            var sLen = signature[rLen + 5];

            if(signature.Length > 72)
                throw new InvalidSignatureException("Signature length too long");
            
            if(signatureType != 0x30)
                throw new InvalidSignatureException("Signature has wrong type");
        
            if(expectedSignatureLength != signature.Length - 2)
                throw new InvalidSignatureException("Expected signature length does not match actual length");

            if(rLen + 5 > signature.Length)
                throw new InvalidSignatureException("Signature 'S' parameter out of bounds");

            if(rLen + sLen + 6 > signature.Length)
                throw new InvalidSignatureException("Invalid R length");
            
            if(signatureIntegerMarker != 0x02)
                throw new InvalidSignatureException("Signature missing first integer marker");
            
            if(rLen == 0)
                throw new InvalidSignatureException("Signature 'R' length is zero");
            
            if((signature[4] & 0x80) != 0)
                throw new InvalidSignatureException("Signature 'R' value is negative");
            
            if(rLen > 1 && signature[4] == 0x00 && (signature[5]&0x80) == 0)
                throw new InvalidSignatureException("Signature 'R' value is invalid");
            
            if(signature[rLen + 4] != 0x02)
                throw new InvalidSignatureException("Missing second integer marker");
            
            if(sLen == 0)
                throw new InvalidSignatureException("Signature 'S' value length is zero");
            
            if((signature[rLen + 6] & 0x80) != 0)
                throw new InvalidSignatureException("Signature 'S' value is negative");
            
            if(sLen > 1 && signature[rLen+6] == 0x00 && (signature[rLen+7]&0x80) == 0)
                throw new InvalidSignatureException("Signature 'S' value is invalid");

            // TODO: Verify the behavior of this branch extensively.
            // halforder is used to tame ECDSA malleability (see BIP0062).
            // var halfOrder = new(big.Int).Rsh(chainec.Secp256k1.GetN(), 1)
            //                 if sValue.Cmp(halfOrder) > 0 {

            var sValue = new BigInteger(signature.Skip(rLen + 6).Take(rLen+6+sLen).ToArray());
            var n = CustomNamedCurves.GetByOid(SecObjectIdentifiers.SecP256k1).N.ToByteArray();
            var halfOrder = new BigInteger(n) >> 1;
            if (sValue.CompareTo(halfOrder) > 0)
                throw new InvalidSignatureException("Failed VerifyLowS validation.");
        }
    }
}


