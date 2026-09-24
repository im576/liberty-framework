# T-002 — Configuration and logging skeleton

Status: **BLOCKED on T-001**. Scope: load/validate/reload one harmless JSON sample profile, retain last valid data on malformed input, and write structured context to a bounded log. No gameplay tuning yet.

Deliverables: schema documented in [CONFIG_SCHEMA.md](../architecture/CONFIG_SCHEMA.md), one sample file, deployment instructions, exact build output, and errors that do not crash the script loop. Human test: edit valid sample, reload, observe new value in log; then make invalid JSON, reload, see error and retained prior value; restore valid file. Agent records exact keystrokes/path once runtime is known and sets `NEEDS-PLAYTEST`.
