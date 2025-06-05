// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using FlowPipe.Benchmarks;

Console.WriteLine("Hello, World!");


BenchmarkRunner.Run<MediatRvsFlowPipeBenchmark>();