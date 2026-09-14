# Evidence Ledger

> [!IMPORTANT]
> This file is a centrally maintained empty seed template. After provisioning at `project-backlog/evidence-ledger.md`, the adopting project owns its live copy. This default live ledger is the sole mutable authority for pass lifecycle status and accepted execution/review evidence in its lane.

Use the same stable pass identifiers and governed order as the project backlog. Apply all state transitions, SHA population, review, finalization, and start-gate rules from `evidence-ledger-protocol.md` and `review-protocol.md`.

One independently governed evidence ledger defines one strictly ordered lifecycle lane. This default ledger uses that model: at most one pass may be `Pending` within this ledger, and zero pending passes is valid. Parallel execution requires explicitly independent lanes with separately governed ledger state; the project backlog must identify every additional ledger path, authority boundary, dependency, and integration/merge order. No generic filename for additional ledgers is required, and this seed's table schema remains unchanged.

| Pass | Status | Pre-pass baseline SHA | Result SHA | PR # | Review-gate closure SHA | Tests/runs | Reviewer |
| --- | --- | --- | --- | --- | --- | --- | --- |

<!-- The reusable seed intentionally contains no ledger rows. Add project passes only in the provisioned project-owned copy. -->

Allowed lifecycle states are `Locked`, `Pending`, and `Completed`. Do not duplicate these mutable values in the project backlog.
