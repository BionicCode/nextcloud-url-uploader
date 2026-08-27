# Backlog-Driven Engineering Review Protocol

## Purpose

This protocol defines reusable evidence, review, authorization, integrity, and completion gates for work governed by a project backlog and evidence ledger.

The default live artifact names are `backlog.md`, `evidence-ledger.md`, `backlog-workflow.md`, `evidence-ledger-protocol.md`, and `review-protocol.md`. These names describe the projected adopter files, not their physical locations in a distribution-source repository.

The protocol does not grant write authority. Current explicit user authorization and applicable repository instructions define what may be read or changed.

## Authority model

Keep these responsibilities separate:

| Authority | Responsibility |
| --- | --- |
| Current authorization | Grants task-specific read and write authority |
| Repository instructions | Define repository scope, validation, protected files, and local conventions |
| Project backlog | Owns stable project roadmap and pass-contract data: identity, order, dependencies, scope, and acceptance criteria |
| Project evidence ledger | Sole mutable authority for lifecycle status, accepted SHAs, test/run evidence, reviewer state, and associated lifecycle metadata |
| Backlog workflow protocol | Defines reusable orchestration and handoff semantics |
| Evidence-ledger protocol | Defines reusable state, transition, and evidence-field semantics |
| Review protocol | Defines the review method and mandatory gates |
| Technical specifications | Define product or system design where the pass contract needs more detail |

The project backlog is stable but not immutable. It may evolve through explicit workflow governance. Do not use that evolution to rewrite accepted evidence, reuse pass identities, or introduce mutable lifecycle status into the backlog.

A downstream artifact archive may preserve reports or logs, but it is not backlog, ledger, review, or workflow-state authority.

If authorities conflict, stop the affected work and identify the exact conflicting files, fields, or instructions. Resume only after an explicitly authorized governance correction.

## Evidence precedence

Evaluate evidence in this order unless a repository defines a stricter order:

1. current explicit authorization;
2. applicable system, safety, and repository instructions;
3. exact repository content at the resolved commit and current worktree state;
4. executable implementation, parser, schema, configuration, and tests tied to that state;
5. hosted jobs, logs, or artifacts tied to the same state;
6. current authoritative product or platform documentation;
7. project backlog, evidence ledger, handoffs, execution reports, and review reports;
8. historical notes, chat history, and commit messages.

Planned or historical Markdown does not override current executable evidence. Passing tests do not override an explicitly documented contract merely because the tests encode current behavior.

Clearly distinguish verified facts, static conclusions, assumptions, and unverified behavior.

## Review state header

Before planning, editing, or reviewing, resolve and record what is relevant:

- repository identity and authority role;
- branch, HEAD, target branch, and worktree state;
- exact task-supplied baseline or execution lease;
- applicable instructions and protected files;
- selected pass identity and backlog revision;
- evidence-ledger state;
- allowed and prohibited surfaces;
- required runtime, tools, fixtures, credentials, or external systems;
- unavailable validation or unresolved dependencies;
- Git and external mutations that are or are not authorized.

Recheck drift-prone state immediately before final review or handoff.

## Backlog and ledger integrity

### Correlation

Confirm that:

1. every ledger row correlates to exactly one admitted backlog pass;
2. pass identifiers are stable, unique, and not reused;
3. backlog order and dependencies are internally coherent;
4. the backlog contains no mutable lifecycle status or completion checkbox;
5. the ledger uses the states and columns defined by `evidence-ledger-protocol.md`;
6. mutable lifecycle and accepted evidence appear only in the ledger;
7. a governed backlog revision has been reconciled with required ledger identities without rewriting accepted history.

### Start gate

Before pass-specific work begins:

1. the selected pass exists exactly once in the backlog and ledger;
2. every required predecessor has `Completed` ledger status and a fully finalized evidence row;
3. the selected pass has `Pending` ledger status;
4. no other pass is `Pending` unless the backlog explicitly defines an independently governed parallel lane;
5. ineligible later or dependent passes remain `Locked`;
6. the active row contains no premature result or review evidence;
7. the handoff identifies the exact activation commit and execution lease;
8. branch ancestry and working-tree state match that lease;
9. current authorization covers the intended work.

If any gate fails, stop before pass-specific work and report the exact defect.

### Transition integrity

The normal transitions are:

```text
Locked → Pending → Completed
```

Confirm that:

- at most one pass is `Pending` in a strictly ordered lane;
- zero pending passes is valid during maintenance or ledger finalization;
- a pass never skips `Pending`;
- a completed pass does not return to `Pending` without an explicit project reopening procedure that preserves its accepted evidence;
- completion does not activate the next pass;
- next-pass activation occurs only after predecessor evidence is fully finalized and required maintenance is complete.

### Evidence integrity

For each completed row, require:

- historical pre-pass baseline SHA;
- accepted result SHA, or the protocol's explicit review-only value;
- delivering pull-request identifier or explicit `N/A`;
- review-gate closure SHA;
- accepted tests or runs;
- accepting reviewer.

Each SHA must have one documented semantic role. Do not use an activation, bookkeeping, closure, merge, or finalization commit as the result SHA unless it actually contains the accepted pass result.

No operation may require a commit to contain its own SHA. Populate self-referential or post-merge values in a later finalization commit.

### Authority and commit separation

Only the maintainer or an explicitly authorized coordination task may change ledger lifecycle status or accepted evidence.

Normally keep separate:

1. pass-result changes;
2. review-gate closure and accepted evidence;
3. post-merge ledger finalization;
4. next-pass activation.

A coordination-only commit must not modify pass-result files. Combined commits require explicit authorization and must preserve every state, evidence, ancestry, and review invariant.

## Execution lease and ancestry

The pre-pass baseline is the exact reviewed snapshot after prerequisites and maintenance and immediately before pass-specific work.

Before work and again in the final report:

- resolve the literal task-supplied baseline;
- confirm the pass branch was created from the approved activation commit;
- confirm the baseline is an ancestor of every pass-specific commit;
- inspect every post-baseline commit;
- identify unexpected changes or branch movement;
- confirm the worktree contains no unrelated changes;
- verify that the baseline recorded at closure matches the executed handoff.

Do not infer the lease from a branch name, latest commit, merge-base alone, automation branch, pull request, or conversation.

## Task-mode review

### Review-only

- Make no repository or external-state changes unless separately authorized.
- Trace real implementation and data paths.
- Report confirmed defects, risks, and unverified suspicions distinctly.
- State the exact evidence boundary where verification stops.

### Planning

- Resolve product intent, authority, interfaces, failure behavior, migration, validation, and completion criteria before implementation.
- Record unresolved product or governance choices as decisions for the maintainer rather than hiding them in implementation.
- Do not silently proceed from planning into edits.

### Implementation

- Edit only the authorized scope.
- Preserve unrelated work and target-owned content.
- Update implementation, tests, schemas, configuration, examples, and documentation together when they form one public contract.
- Stop instead of broadening repositories, permissions, dependencies, or authority.
- Do not stage, commit, push, create or update pull requests, merge, dispatch workflows, or alter settings unless the current authorization explicitly permits that action.

### Validation or migration

- Validation-only work establishes evidence without changing the implementation contract.
- Migration work must define compatibility, sequencing, rollback, and ownership before mutation.

## Public contract and migration review

Treat paths, filenames, schemas, manifests, configuration, generated files, command-line interfaces, workflow inputs/outputs, and documented behavior as public contracts when consumers rely on them.

For a path or filename migration:

1. approve an exact old-to-new map;
2. inventory every literal reference before editing;
3. update implementation, invocations, scripts, tests, fixtures, manifests, templates, generated definitions, and documentation;
4. preserve triggers, permissions, inputs, outputs, dependencies, conditions, concurrency, and execution behavior unless the pass explicitly changes them;
5. provide compatibility when external consumers cannot migrate atomically;
6. define the compatibility-removal gate;
7. scan for unexplained stale references after editing;
8. review the complete rename and behavior diff independently.

Do not label a contract migration as cosmetic merely because implementation logic is unchanged.

## Trust and generated artifacts

Classify repositories and checkouts by authority, not by name or ancestry. Common roles include implementation authority, canonical content source, caller, target, and disposable fixture.

Keep trusted base/default state separate from candidate branch or pull-request state. Never execute candidate or untrusted repository code as though it were trusted merely to inspect or validate it.

A generated artifact must have:

- an identified owning source;
- a documented generated-file boundary;
- a controlled regeneration path;
- a version or source identity consistent with coupled artifacts;
- validation that detects manual drift;
- a rule for preserving target-owned content outside its managed boundary.

Do not disguise generated dependency metadata as hand-maintained implementation or silently regenerate it during unrelated work.

## Initialization, upgrade, and configuration review

An installer, initializer, upgrader, or provisioner must be:

- explicit;
- idempotent;
- reviewable;
- all-or-nothing for coupled managed surfaces;
- non-destructive to target-owned content;
- version-consistent;
- fail-closed on ambiguous ownership or malformed input.

Test at least missing, existing, no-op, upgrade, conflict, malformed input, protected path, path-safety, partial-write, and stale-version cases when they apply.

Modify structured configuration structurally:

1. parse and validate input;
2. preserve unrelated entries;
3. identify exact managed identities;
4. reject ambiguous or conflicting entries;
5. produce deterministic output;
6. validate the result before writing;
7. ensure planning failure performs no writes.

Never use blind text insertion for JSON, YAML, XML, or another structured public format.

## Version and ownership authority

Coupled artifacts must resolve to one coherent version. Examples include generated wrappers, schema copies, managed documentation, package metadata, and source references.

For a cross-component plan or ownership contract:

- define an explicit versioned schema;
- define compatibility and migration rules for breaking changes;
- update producer, consumer, tests, and documentation together;
- preserve one write and version authority for each managed surface;
- perform no writes when the required authority plan is invalid;
- identify the governing source in diagnostics;
- never use an incidental `Version` field to choose a content winner.

Changing an internal implementation does not break a consumer unless the shared contract or semantics change. Changing that shared contract requires version, migration, tests, examples, and documentation review.

## Documentation review

Documentation is a required product surface. Confirm that it:

- describes current behavior separately from planned, historical, deferred, or unsupported behavior;
- explains concepts and ownership before implementation details;
- uses exact public names and valid examples;
- records important caveats, failure behavior, and trust boundaries;
- links or refers to artifacts in a way that remains valid in their intended context;
- agrees with schemas, configuration, tests, and implementation;
- remains understandable to a maintainer without prior conversational context.

Code or tests passing while documentation, schema, examples, or ownership rules disagree is an incomplete result.

## Validation layers

Choose the smallest combination that proves the pass contract:

1. **Text and syntax** — whitespace, encoding, JSON/YAML/XML parsing, language parsers, and Markdown references.
2. **Semantic static checks** — schema validation, linters, type checks, workflow semantics, permissions, and contract matrices.
3. **Unit and acceptance tests** — executable behavior, failure paths, and diagnostics.
4. **Fixtures and integration** — multi-file ownership, init/upgrade, rollback, caller/target behavior, and clean temporary environments.
5. **Hosted or external execution** — real platform behavior, permissions, logs, outputs, and artifacts tied to exact revisions.
6. **Cross-repository certification** — real external consumers where a local fixture cannot prove access or integration.
7. **Convergence** — repeat applicable operations and require the second run to be a no-op.

Record exact commands, relevant runtime versions, revisions, results, timeouts, skipped cases, and limitations. A syntax parse is not semantic platform validation.

## Test-quality review

Inspect whether tests:

- prove the external contract rather than mirror the implementation;
- include positive, negative, boundary, and failure cases;
- cover false positives and false negatives;
- preserve unrelated target-owned data;
- prove no-write-on-failure and idempotence;
- prove coupled-artifact and version coherence;
- exercise authority and lifecycle boundaries;
- avoid timing races, shared mutable state, and weakened assertions;
- localize hangs and bound child processes.

An unrelated defect should be reported separately. It does not expand the current pass.

## Security and reliability review

Where relevant, review:

- minimum permissions and credential authority;
- trusted versus untrusted inputs and checkouts;
- secret propagation and command/script injection;
- immutable versus mutable dependency references;
- path traversal, absolute paths, symlinks, and junctions;
- UTF-8, Unicode normalization, newline, casing, and culture assumptions;
- partial writes, rollback, and all-or-nothing claims;
- concurrency, retries, duplicate work, and stale-state races;
- branch protection and force-with-lease behavior;
- generated-file and workflow-file write permissions.

Do not weaken security or validation controls to make a check pass.

## Review output

A result review should include:

1. verdict;
2. resolved repositories, roles, branches, and revisions;
3. pass identity and scope check;
4. authority and contract check;
5. findings with precise locations and evidence;
6. validation commands, results, and limitations;
7. test-quality assessment;
8. documentation, schema, configuration, and generated-artifact assessment;
9. trust, security, ownership, idempotence, and rollback assessment where applicable;
10. complete diff self-review;
11. stop-condition and plan-deviation status;
12. files and external state changed;
13. remaining risks and unverified behavior.

Do not claim that a check passed when it was not run. Do not rely solely on the executor's summary when the underlying diff, state, or evidence is available.

## Completion standard

A pass is complete only when:

- its stable backlog contract is satisfied;
- scope and authority are exact;
- required implementation, tests, schemas, configuration, examples, and documentation agree;
- relevant validation has passed or an exact blocking limitation is reported;
- the complete result has received the required independent review;
- trust and ownership boundaries have been reviewed;
- no stop condition remains unresolved;
- the maintainer or designated authority accepts the review gate;
- ledger lifecycle and accepted evidence are updated only through separately authorized coordination.

Implementation completion, tests, or self-review alone never close a pass or activate later work.

## Protected governance

Treat repository instructions, review protocols, workflow governance, project backlog, project evidence ledger, ownership manifests, and equivalent control-plane files as protected when repository policy designates them so.

A task may modify a protected file only when current authorization identifies the path and intended governance or coordination change. Broad requests to update documentation, references, or implementation do not automatically authorize control-plane mutation.

If an implementation makes protected governance inaccurate but that file is outside scope:

1. leave it unchanged;
2. report the exact inconsistency;
3. propose the required correction;
4. stop if continuing would create a materially contradictory contract.
