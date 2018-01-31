namespace NDecred.Core
{
    public enum TxSerializeType : uint
    {
        // TxSerializeFull indicates a transaction be serialized with the prefix
        // and all witness data.
        TxSerializeFull = 0,

        // TxSerializeNoWitness indicates a transaction be serialized with only
        // the prefix.
        TxSerializeNoWitness,

        // TxSerializeOnlyWitness indicates a transaction be serialized with
        // only the witness data.
        TxSerializeOnlyWitness,

        // TxSerializeWitnessSigning indicates a transaction be serialized with
        // only the witness scripts.
        TxSerializeWitnessSigning,

        // TxSerializeWitnessValueSigning indicates a transaction be serialized
        // with only the witness input values and scripts.
        TxSerializeWitnessValueSigning
    }
}