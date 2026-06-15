# Security checklist

- Protected endpoints require authentication.
- Authorization is enforced for the specific object/action.
- Anonymous endpoints are intentionally anonymous.
- Request DTOs prevent over-posting.
- Inputs have length, range, format, and enum validation.
- Dynamic queries are parameterized and constrained.
- File paths are normalized and restricted when file handling exists.
- Redirects allow only trusted destinations.
- External URLs are allowlisted or otherwise SSRF-protected.
- Logs do not include secrets, tokens, passwords, connection strings, or sensitive payloads.
- Error responses do not leak stack traces or internal details.
- CORS is not opened broadly by accident.
- New dependencies are necessary and checked for vulnerabilities.
