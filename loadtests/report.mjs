import { createReadStream } from "node:fs";
import { createInterface } from "node:readline";

const resultsFilePath = process.argv[2];
const largeBusinessTag = "businessSize=large";
const durationsByEndpoint = new Map();
const failuresByEndpoint = new Map();
let columns;

for await (const line of createInterface({ input: createReadStream(resultsFilePath) })) {
  const values = line.split(",");
  if (!columns) {
    columns = Object.fromEntries(values.map((name, index) => [name, index]));
    continue;
  }
  const metric = values[columns.metric_name];
  const isLargeBusiness = values[columns.extra_tags]?.includes(largeBusinessTag);
  const endpoint = isLargeBusiness ? `${values[columns.name]} [large]` : values[columns.name];
  if (metric === "http_req_duration") {
    const durations = durationsByEndpoint.get(endpoint) ?? [];
    durations.push(Number(values[columns.metric_value]));
    durationsByEndpoint.set(endpoint, durations);
  } else if (metric === "http_req_failed" && values[columns.metric_value] === "1") {
    failuresByEndpoint.set(endpoint, (failuresByEndpoint.get(endpoint) ?? 0) + 1);
  }
}

function percentile(sortedValues, fraction) {
  return sortedValues[
    Math.min(sortedValues.length - 1, Math.floor(sortedValues.length * fraction))
  ];
}

const rows = [...durationsByEndpoint].map(([endpoint, durations]) => {
  const sorted = durations.sort((first, second) => first - second);
  return {
    endpoint,
    requests: sorted.length,
    failed: failuresByEndpoint.get(endpoint) ?? 0,
    median: percentile(sorted, 0.5),
    p95: percentile(sorted, 0.95),
    p99: percentile(sorted, 0.99),
    max: sorted[sorted.length - 1],
  };
});
rows.sort((first, second) => second.p95 - first.p95);

const milliseconds = (value) => `${value.toFixed(0)} ms`.padStart(8);
console.log(`| Endpoint | Requests | Failed | Median | p95 | p99 | Max |`);
console.log(`|---|---:|---:|---:|---:|---:|---:|`);
for (const row of rows) {
  console.log(
    `| \`${row.endpoint}\` | ${row.requests} | ${row.failed} | ${milliseconds(row.median)} | ${milliseconds(row.p95)} | ${milliseconds(row.p99)} | ${milliseconds(row.max)} |`,
  );
}
