# ADR-0003: Human-readable live tuning

Status: **design choice; implementation pending T-002** (2026-09-24).

Use readable external configuration, provisionally JSON, for profiles and test locations. The parser and live reload are implemented only after the runtime spike. Config changes must validate before activation; the last valid profile remains active on parse failure. The exact serializer and deployment layout are T-002 decisions.
