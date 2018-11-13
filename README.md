# ndecred
A *WIP* implementation the Decred protocol in C#
--

* Not all peer messages are handled
* Transaction script engine does not currently support OP_CHECKSIGALT or OP_CHECKSIGALTVERIFY opcodes.
* Messages from peers are not indexed
* Stake transactions are not interpreted as such
* ... and so much more

Based on code from https://github.com/decred/dcrd

If you want to help, just create a branch and open a PR.  It's early in the project so the only rules are:

* New code should follow consensus rules from the first block up to the current tip
* The feature shouldn't be done already.  Refactoring to support a new block of code is OK.
* Tests are important around the consensus layer and more robust tests are welcome.
* KISS
