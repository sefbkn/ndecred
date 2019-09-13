# ndecred
A *WIP* implementation the Decred protocol in C#
--

* Not all peer messages are handled
* Transaction script engine does not currently support OP_CHECKSIGALT or OP_CHECKSIGALTVERIFY opcodes.
* Messages from peers are not indexed
* Stake transactions are not interpreted as such
* There is no validation, storage, or relay of transactions
* ... and so much more

Based on code from https://github.com/decred/dcrd
