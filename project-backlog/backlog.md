# Project Backlog

> [!IMPORTANT]
> This file is a centrally maintained seed template. After provisioning, the adopting project owns its live copy and may evolve it through the backlog workflow's governance process.

## 1. Purpose and authority

This backlog is the stable project-owned roadmap and pass-contract authority for:

- pass identity and outcome;
- pass order and dependencies;
- project scope and boundaries;
- required outputs and invariants;
- acceptance criteria, validation, review gates, and stop conditions;
- project-specific decisions and exceptions.

Stable does not mean immutable. The project may add, refine, split, reorder, or retire future work through explicit workflow governance while preserving stable identities and accepted history.

This backlog does not:

- grant write authority;
- own mutable lifecycle status;
- record accepted baseline, result, or review SHAs;
- record accepted tests, runs, or reviewer state;
- replace repository instructions or technical specifications.

The project evidence ledger, normally `evidence-ledger.md`, is the sole mutable lifecycle and accepted-evidence authority. Reusable orchestration, review, and ledger semantics are defined by `backlog-workflow.md`, `review-protocol.md`, and `evidence-ledger-protocol.md`.

## 2. Program outcomes and completion definition

Describe the observable project outcomes that this roadmap must produce:

- `<PROGRAM-OUTCOME>`

Define program completion in terms of the accepted system state, not merely execution of every listed pass:

- `<COMPLETION-CONDITION>`

## 3. Boundaries and non-goals

### In scope

- `<IN-SCOPE-REPOSITORY-OR-SURFACE>`

### Out of scope

- `<OUT-OF-SCOPE-REPOSITORY-OR-SURFACE>`

### Deferred work

- `<DEFERRED-CAPABILITY-OR-DECISION>`

## 4. Decisions and supporting specifications

Record or link the decisions that constrain multiple passes:

- `<DECISION-OR-SPECIFICATION>`

Keep detailed architecture, schemas, migration plans, and trust models in their owning documents. Reference them here instead of duplicating conflicting versions.

## 5. Dependency and milestone model

Describe the ordering model, parallel lanes, cross-repository gates, and milestones:

- `<DEPENDENCY-OR-MILESTONE-RULE>`

If parallel lanes are permitted, define their independent authority, ledger, dependency, and merge boundaries explicitly.

## 6. Ordered pass index

This index owns pass identity, outcome, mode, and dependencies. Lifecycle status belongs only in the evidence ledger.

| Pass | Outcome | Mode | Dependencies |
| --- | --- | --- | --- |

<!-- Add one row for each admitted pass. Do not add lifecycle status or completion-checkbox columns. -->

## 7. Pass specifications

Define each admitted pass using the following structure. Copy the template below outside the code fence and replace every placeholder.

```markdown
## <PASS-ID> — <Outcome-oriented title>

**Mode:** <planning | implementation | review-only | validation-only | migration | coordination>

**Objective:**
<Observable result produced by this pass>

**Rationale:**
<Why this pass exists and why it occurs here>

**Dependencies:**

- <DEPENDENCY-ID or explicitly stated prerequisite>

**Scope:**

- repositories: <authorized repositories>
- allowed surfaces: <files, directories, APIs, or artifact classes>
- prohibited surfaces: <nearby work that remains out of scope>

**Required outputs:**

- <artifact, decision, report, or executable result>

**Required invariants:**

- <behavior, authority, compatibility, or safety property that must remain true>

**Non-goals:**

- <explicitly excluded work>

**Acceptance criteria:**

- <observable condition that must be true for acceptance>

**Validation:**

- `<command or check>` — <required evidence>

**Stop conditions:**

- <condition requiring work to stop rather than expand scope or authority>

**Review gate:**
<review authority and evidence required for acceptance>

**Follow-up effect:**
<later pass or milestone that becomes eligible after acceptance; eligibility is not activation>
```

Omit conditional fields only when they genuinely do not apply. Never omit objective, dependencies, scope, acceptance criteria, validation, review gate, or stop conditions.

## 8. Project-specific exceptions

Record approved deviations from the reusable workflow without weakening the reusable protocol documents:

- `<EXCEPTION-AND-AUTHORITY>`

Historical exceptions must be explicit, bounded, and must not silently redefine current behavior.

## 9. Backlog maintenance rules

- Never reuse or silently renumber an established pass identity.
- Preserve accepted pass history; later defects normally receive a new bounded follow-up pass.
- Apply changes to order, dependencies, scope, or acceptance criteria through an explicit governance review.
- Re-review the active handoff when its owning pass contract changes materially.
- Reconcile admitted pass identities with the evidence ledger without copying ledger lifecycle fields into this file.
- Keep current, planned, historical, deferred, and rejected behavior clearly distinguished.
- Do not treat a backlog entry, dependency, or follow-up effect as write authority or automatic activation.
