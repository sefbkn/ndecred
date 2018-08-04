using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NDecred.Common;
using NDecred.Wire;
using Newtonsoft.Json;
using Xunit;

namespace NDecred.TxScript.Tests
{
    public class ScriptEngineAltTest
    {
        [Fact]
        public void ReadsTest()
        {            
            var rawTests = ParseScriptValid("./data/script_valid.json");     
            var testCases = ParseTestCases(rawTests);

            for (var index = 0; index < testCases.Length; index++)
            {                
                var test = testCases[index];
                if (test.IsComment)
                    continue;

                try
                {
                    var spendingTx = test.SpendingTx();
                    var engine = new ScriptEngine(spendingTx, 0, test.PublicKeyScript);
                    engine.Run();
                }
                
                catch (Exception e)
                {
                    throw new Exception($"Failed test {index}.  {test.RawTest}", e);
                }
            }
        }

        private string[][] ParseScriptValid(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<string[][]>(json);
        }

        private TestCase[] ParseTestCases(string[][] rawTestCases)
        {
            return rawTestCases.Select(c => new TestCase(c)).ToArray();
        }

        // scriptSig, scriptPubKey, flags, ... comments
        private class TestCase
        {
            private static Regex _hexRegex = new Regex(
                @"^(?<prefix>0x)(?<value>([\d]|[a-fA-F])+)|(?<value>-?\d+)$", 
                RegexOptions.Compiled
            );
            
            private static Regex _pushLiteralRegex = new Regex(
                @"^'(?<data>.*?)'$", 
                RegexOptions.Compiled
            );

            public bool IsComment { get; }
            
            public Script SignatureScript { get; }
            public Script PublicKeyScript { get; }
            public string Flags { get; }
            public string Comments { get; }
            public string RawTest { get; }
            
            public TestCase(string[] raw)
            {
                var validLengths = new[] { 1, 3, 4 };
                if(!validLengths.Contains(raw.Length))
                    throw new ArgumentException(
                        "Error parsing test: " + string.Join(", ", raw));
                
                IsComment = raw.Length == 1;
                if (IsComment) return;
                
                if (raw.Length == 4)
                    Comments = raw[3];

                if (raw[0] == "-1 0 1 2")
                {
                }

                SignatureScript = new Script(ParseOpcodes(raw[0]));
                PublicKeyScript = new Script(ParseOpcodes(raw[1]));
                Flags = raw[2];
                

                RawTest = string.Join(',', raw);
                
            }

            private ParsedOpCode[] ParseOpcodes(string raw)
            {
                try
                {
                    var tokens = raw.Split(' ', '\t', '\n')
                        .Select(s => s.Trim())
                        .Where(s => s.Length > 0)
                        .ToArray();

                    var builder = new ScriptBuilder();

                    for(int i = 0; i < tokens.Length; i++)
                    {
                        var token = tokens[i];
                        try
                        {
                            if (_hexRegex.IsMatch(token))
                            {
                                var match = _hexRegex.Match(token);
                                var isHex = match.Groups["prefix"]?.Value == "0x";
                                var value = match.Groups["value"].Value.Trim();

                                if (isHex)
                                    builder.AddRawScriptBytes(Hex.ToByteArray(value));
                                else
                                    builder.AddInt64(long.Parse(value));                            
                            }
                    
                            else if (_pushLiteralRegex.IsMatch(token))
                            {
                                var data = _pushLiteralRegex.Match(token).Groups["data"].Value.Trim();
                                builder.AddData(data);
                            }

                            else
                            {
                                var opCode = ParseOpcode(token);
                                builder.AddOpCode(opCode);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw new Exception("Token: " + token, e);
                        }
                    }

                    return builder.ToScript().ParsedOpCodes;
                }
                catch (Exception e)
                {
                    throw new Exception("ParseOpCode fail.  Raw: " + raw, e);
                }
            }

            public MsgTx SpendingTx()
            {
                var coinbaseTx = new MsgTx
                {
                    TxIn = new[]
                    {
                        new TxIn
                        {
                            PreviousOutPoint = new OutPoint(new byte[32], 0, TxTree.TxTreeRegular),
                            SignatureScript = new[]{ (byte)OpCode.OP_0, (byte)OpCode.OP_0 }
                        }
                    },
                    TxOut = new[]
                    {
                        new TxOut
                        {
                            Value = 0,
                            PkScript = PublicKeyScript.Bytes
                        }
                    }
                };

                var coinbaseTxHash = coinbaseTx.GetHash();
                var spendingTx = new MsgTx
                {
                    TxIn = new[]
                    {
                        new TxIn
                        {
                            PreviousOutPoint = new OutPoint(coinbaseTxHash, 0, TxTree.TxTreeRegular),
                            SignatureScript = SignatureScript.Bytes
                        }
                    },
                    TxOut = new[]
                    {
                        new TxOut() 
                    }
                };

                return spendingTx;
            }
            
            private ParsedOpCode ParseOpcode(string token)
            {
                token = token.ToUpper().Trim();
                if (!token.StartsWith("OP"))
                    token = $"OP_{token}";
                var code = OpCodeUtil.FromString(token);
                return new ParsedOpCode(code);
            }
        }
    }
}