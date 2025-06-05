```

BenchmarkDotNet v0.15.0, Windows 11 (10.0.26100.4202/24H2/2024Update/HudsonValley)
12th Gen Intel Core i7-12700H 2.30GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method        | Mean     | Error   | StdDev   | Median   | Allocated |
|-------------- |---------:|--------:|---------:|---------:|----------:|
| MediatR_Send  | 119.1 μs | 7.35 μs | 20.84 μs | 108.9 μs |     562 B |
| FlowPipe_Send | 105.7 μs | 2.10 μs |  3.21 μs | 106.2 μs |     930 B |
