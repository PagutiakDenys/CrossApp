# CrossApp
Наскрізний проєкт з крос-платформного програмування.
Студент: Пагутяк Денис
Група: ФЕІ-33
# Предметна область
Склад
## Сутності
 `Product` — товар
 `StockBatch` — партія товару
 `Warehouse` — склад
 `Movement` — переміщення товару
## Призначення
Застосунок призначений для обліку залишків товарів по партіях, складам та переміщенням.

# Структура проєкту
rossApp/ 
├── CrossApp.slnx 
├── README.md 
└── src/
   ├── Cli/ 
   │ ├── Cli.csproj 
   │ └── Program.cs 
   └── Core/ 
      ├── Core.csproj
      ├── EnvironmentInfo.cs
      └── EnvironmentReport.cs
# Запуск
Збірка проєкту:

dotnet build

Запуск:

dotnet run --project src/Cli

Публікація:

dotnet publish src/Cli -c Release -r win-x64 --self-contained true -o publish/self-contained

dotnet publish src/Cli -c Release -r win-x64 --self-contained false -o publish/framework-dependent

### Порівняння публікацій

| RID       | Режим               | Розмір publish | Потрібен runtime |

| `win-x64` | self-contained      |      71 Мб     | Ні               |

| `win-x64` | framework-dependent |    0.209 Мб    | Так, .NET 10     |

| `win-x64` | single-file         |      71 Мб     | Ні               |

| `win-x64` | trimmed             |      20 Мб     | Ні               |


# Середовище
.NET SDK 10.0

C#

Git

GitHub

Windows 11 x64


# Коміти
lab2 бібліотека Core + Cli
(origin/main) оновлення Program.cs для укр,мови
lab01: additional self-contained and JSON mode
lab01: add README
lab01: solution CrossApp, Cli, вибір домену

# Додаткові завдання
## lab2
Single-file:

dotnet publish src/Cli -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/single-file

Запуск:

./publish/single-file/Cli.exe

Trimming:

dotnet publish src/Cli -c Release -r win-x64 --self-contained true -p:PublishTrimmed=true -o publish/trimmed

Multi-targeting:

<TargetFrameworks>net8.0;net10.0</TargetFrameworks>

У коді:

#if NET10_0_OR_GREATER
private const string BuildNote = "build for net10.0";
#else
private const string BuildNote = "build for net8.0";
#endif

Результат показується в консолі через:

Console.WriteLine($"Build Note: {report.BuildNote}");
## lab1
### Self-contained публікація
Проєкт було опубліковано у self-contained режимі для двох RID:
* `win-x64` — Windows x64
* `linux-x64` — Linux x64
Команди:
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
dotnet publish src/Cli -c Release -r linux-x64 --self-contained true
Розмір каталогів `publish`:

| RID       |                          Розмір |
| --------- | ------------------------------: |
| win-x64   |  Cli net10.0 win-x64 succeeded (4,7s) → src\Cli\bin\Release\net10.0\win-x64\publish\ |
| linux-x64 | Cli net10.0 linux-x64 succeeded (1,9s) → src\Cli\bin\Release\net10.0\linux-x64\publish\ |

### JSON-режим
 До консольного застосунку додано прапорець `--json`.
Звичайний запуск:

dotnet run --project src/Cli

 Запуск у JSON-режимі:

dotnet run --project src/Cli -- --json

У режимі `--json` інформація про середовище виводиться одним JSON-рядком.