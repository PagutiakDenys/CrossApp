\# CrossApp

Наскрізний проєкт з крос-платформного програмування.

\*\*Студент:\*\* Пагутяк Денис

\*\*Група:\*\* ФЕІ-33

\## Предметна область

\*\*Склад\*\*

\### Сутності

\* `Product` — товар

\* `StockBatch` — партія товару

\* `Warehouse` — склад

\* `Movement` — переміщення товару


\### Призначення

Застосунок призначений для обліку залишків товарів по партіях, складам та переміщенням.

\## Структура проєкту

```text

CrossApp/

├── .gitignore

├── CrossApp.slnx

├── README.md

└── src/

&#x20;   └── Cli/

&#x20;       ├── Cli.csproj

&#x20;       └── Program.cs

```


\## Запуск

Для збирання проєкту:

```bash

dotnet build

```

Для запуску консольного застосунку:

```bash

dotnet run --project src/Cli

```

\## Середовище


\* .NET SDK 10.0

\* C#

\* Git

\* GitHub

\* Windows x64


\## Git


Перший коміт лабораторної роботи:

```text

lab01: solution CrossApp, Cli, вибір домену

```
## Додаткове завдання

### Self-contained публікація

Проєкт було опубліковано у self-contained режимі для двох RID:

* `win-x64` — Windows x64
* `linux-x64` — Linux x64

Команди:

```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
dotnet publish src/Cli -c Release -r linux-x64 --self-contained true
```

Розмір каталогів `publish`:

| RID       |                          Розмір |
| --------- | ------------------------------: |
| win-x64   |  Cli net10.0 win-x64 succeeded (4,7s) → src\Cli\bin\Release\net10.0\win-x64\publish\ |
| linux-x64 | Cli net10.0 linux-x64 succeeded (1,9s) → src\Cli\bin\Release\net10.0\linux-x64\publish\ |

### JSON-режим

До консольного застосунку додано прапорець `--json`.

Звичайний запуск:

```bash
dotnet run --project src/Cli
```

Запуск у JSON-режимі:

```bash
dotnet run --project src/Cli -- --json
```

У режимі `--json` інформація про середовище виводиться одним JSON-рядком.