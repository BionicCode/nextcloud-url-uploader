# Evidence Ledger

> [!IMPORTANT]
> This file is a centrally maintained empty seed template. After provisioning, the adopting project owns its live copy. The live evidence ledger is the sole mutable authority for pass lifecycle status and accepted execution/review evidence.

Use the same stable pass identifiers and governed order as the project backlog. Apply all state transitions, SHA population, review, finalization, and start-gate rules from `evidence-ledger-protocol.md` and `review-protocol.md`.

| Pass | Status | Pre-pass baseline SHA | Result SHA | PR # | Review-gate closure SHA | Tests/runs | Reviewer |
| --- | --- | --- | --- | --- | --- | --- | --- |

<!-- The reusable seed intentionally contains no ledger rows. Add project passes only in the provisioned project-owned copy. -->

Allowed lifecycle states are `Locked`, `Pending`, and `Completed`. Do not duplicate these mutable values in the project backlog.
