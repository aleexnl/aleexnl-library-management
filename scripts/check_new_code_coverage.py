#!/usr/bin/env python3
from __future__ import annotations

import argparse
import glob
import re
import subprocess
import sys
import xml.etree.ElementTree as ET
from collections import defaultdict


HUNK_PATTERN = re.compile(r"^@@ -\d+(?:,\d+)? \+(\d+)(?:,(\d+))? @@")


def run_git(*args: str) -> str:
    result = subprocess.run(
        ["git", *args],
        check=True,
        capture_output=True,
        text=True,
    )
    return result.stdout


def normalize_path(path: str) -> str:
    normalized = path.replace("\\", "/")
    while normalized.startswith("./"):
        normalized = normalized[2:]
    src_index = normalized.find("src/")
    if src_index >= 0:
        normalized = normalized[src_index:]
    return normalized


def collect_added_lines(base_sha: str, head_sha: str) -> dict[str, set[int]]:
    merge_base = run_git("merge-base", base_sha, head_sha).strip()
    diff = run_git("diff", "--unified=0", "--no-color", merge_base, head_sha, "--", "src")

    added_lines: dict[str, set[int]] = defaultdict(set)
    current_file: str | None = None

    for line in diff.splitlines():
        if line.startswith("+++ "):
            if line == "+++ /dev/null":
                current_file = None
                continue

            file_path = normalize_path(line[6:])
            if file_path.endswith(".cs") and file_path.startswith("src/"):
                current_file = file_path
            else:
                current_file = None
            continue

        if current_file is None:
            continue

        match = HUNK_PATTERN.match(line)
        if not match:
            continue

        start_line = int(match.group(1))
        line_count = int(match.group(2) or "1")

        if line_count == 0:
            continue

        for line_number in range(start_line, start_line + line_count):
            added_lines[current_file].add(line_number)

    return dict(added_lines)


def collect_coverage(coverage_glob: str) -> dict[str, dict[int, int]]:
    coverage_by_file: dict[str, dict[int, int]] = defaultdict(dict)
    report_paths = glob.glob(coverage_glob, recursive=True)

    if not report_paths:
        raise FileNotFoundError(f"No coverage reports found for pattern: {coverage_glob}")

    for report_path in report_paths:
        root = ET.parse(report_path).getroot()
        for class_node in root.findall(".//class"):
            file_name = class_node.attrib.get("filename")
            if not file_name:
                continue

            normalized_file_name = normalize_path(file_name)
            file_coverage = coverage_by_file[normalized_file_name]

            for line_node in class_node.findall("./lines/line"):
                line_number = int(line_node.attrib["number"])
                hits = int(line_node.attrib["hits"])
                file_coverage[line_number] = max(file_coverage.get(line_number, 0), hits)

    return dict(coverage_by_file)


def emit_uncovered_annotations(uncovered_lines: dict[str, list[int]]) -> None:
    for file_path, line_numbers in uncovered_lines.items():
        joined_lines = ", ".join(str(number) for number in line_numbers[:20])
        suffix = "" if len(line_numbers) <= 20 else ", ..."
        print(
            f"::error file={file_path},line={line_numbers[0]}::"
            f"Added lines without coverage: {joined_lines}{suffix}"
        )


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Fail when added source lines are not covered above the configured threshold."
    )
    parser.add_argument("--base-sha", required=True)
    parser.add_argument("--head-sha", required=True)
    parser.add_argument("--coverage-glob", required=True)
    parser.add_argument("--threshold", type=float, required=True)
    args = parser.parse_args()

    added_lines = collect_added_lines(args.base_sha, args.head_sha)
    total_added_lines = sum(len(lines) for lines in added_lines.values())
    if total_added_lines == 0:
        print("No added C# lines found under src/. Skipping new-code coverage gate.")
        return 0

    coverage_by_file = collect_coverage(args.coverage_glob)
    uncovered_lines: dict[str, list[int]] = defaultdict(list)
    covered_lines = 0
    total_coverable_lines = 0
    skipped_non_coverable_lines = 0

    for file_path, line_numbers in sorted(added_lines.items()):
        file_coverage = coverage_by_file.get(file_path, {})

        for line_number in sorted(line_numbers):
            if not file_coverage:
                total_coverable_lines += 1
                uncovered_lines[file_path].append(line_number)
                continue

            if line_number not in file_coverage:
                skipped_non_coverable_lines += 1
                continue

            total_coverable_lines += 1

            if file_coverage.get(line_number, 0) > 0:
                covered_lines += 1
            else:
                uncovered_lines[file_path].append(line_number)

    if total_coverable_lines == 0:
        print("No coverable added C# lines found under src/. Skipping new-code coverage gate.")
        return 0

    coverage_percent = covered_lines / total_coverable_lines * 100

    print(
        f"New-code coverage: {coverage_percent:.2f}% "
        f"({covered_lines}/{total_coverable_lines} coverable added lines covered)"
    )
    if skipped_non_coverable_lines > 0:
        print(f"Skipped {skipped_non_coverable_lines} non-coverable added lines.")

    if coverage_percent <= args.threshold:
        emit_uncovered_annotations(uncovered_lines)
        print(
            f"Coverage gate failed: new code must be greater than {args.threshold:.2f}% coverage."
        )
        return 1

    print(f"Coverage gate passed: new code is above {args.threshold:.2f}% coverage.")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except subprocess.CalledProcessError as error:
        if error.stdout:
            sys.stdout.write(error.stdout)
        if error.stderr:
            sys.stderr.write(error.stderr)
        raise
    except FileNotFoundError as error:
        print(str(error))
        raise SystemExit(1)
