# SchoolApp

A Windows desktop app for a school database, made for the Database Systems I course (BDB501) at the Syrian Virtual University (SVU), semester F23. Written in C# with WPF on .NET 6; it reads a SQL Server database named `BDB501_F23`.

## Features

- One tab per table: Staff, Staff Role, Study Years, Students, Classrooms, Courses, Timetable, Report Cards and Assessments. Opening a tab loads the table into a grid.
- A **Statistics** tab with three queries:
  - students who scored 100 in Mathematics and failed Arabic in 2023-2024,
  - courses taught by more than one teacher, with the teachers' names,
  - the top student of each course.

The **Save Change** buttons are not finished: they don't write changes back to the database.

## Requirements

- Windows with the [.NET 6 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/6.0).
- SQL Server with the `BDB501_F23` database (tables `Staff`, `Staff_Roles`, `StudyYears`, `Students`, `Classrooms`, `Courses`, `Timetable`, `ReportCards`, `Assessments`). The database scripts are not in this repository.

## Building

```
git clone https://github.com/Mohammad-Diab/SchoolApp-SVU-F23-BDB501.git
cd SchoolApp-SVU-F23-BDB501
dotnet build SchoolApp\SchoolApp.sln -c Release
```

Or open `SchoolApp\SchoolApp.sln` in Visual Studio. The exe is written to `SchoolApp\SchoolApp\bin\Release\net6.0-windows`.

The connection string is in `SchoolApp\SchoolApp\MainWindow.xaml.cs` (`connectionString`); change it to match your SQL Server.

## Author

Mohammad Diab

## License

[Apache 2.0](LICENSE)
