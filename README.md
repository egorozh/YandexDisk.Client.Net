# Yandex Disk API Client for .NET

[Yandex Disk Rest API](https://tech.yandex.ru/disk/rest/) client library for .NET Standard

[![Nuget](https://img.shields.io/nuget/v/Egorozh.YandexDisk.Client?label=Egorozh.YandexDisk.Client)](https://www.nuget.org/packages/Egorozh.YandexDisk.Client)

This is a fork [of this library](https://github.com/raidenyn/yandexdisk.client)

This version is optimized for Native Aot published

BenchmarkDotNet v0.13.10, Windows 11 (10.0.22621.2715/22H2/2022Update/SunValley2)
AMD Ryzen 5 7600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


| Method   | Mean         | Error      | StdDev     | Gen0    | Gen1    | Allocated |
|--------- |-------------:|-----------:|-----------:|--------:|--------:|----------:|
| Original | 2,248.342 us | 12.7236 us | 11.9017 us | 15.6250 | 11.7188 | 265.48 KB |
| Fork     |     4.178 us |  0.0350 us |  0.0327 us |  0.5493 |  0.0076 |   9.07 KB |

