@rem run_db_migrations.cmd
SET CurrentPath=%CD%
SET ConfigFile=%CurrentPath%\bin\Debug\Spikes.Migrations.Tests.dll.config
SET MigrateExe=..\..\lib\packages\EntityFramework.6.1.1\tools\migrate.exe

%MigrateExe% Spikes.Migrations.BaseDataMigrations.dll /StartUpDirectory:%CurrentPath%\bin\Debug\ /startUpConfigurationFile:"%ConfigFile%"
%MigrateExe% Spikes.Migrations.DataMigrations.dll /StartUpDirectory:%CurrentPath%\bin\Debug\ /startUpConfigurationFile:"%ConfigFile%"
PAUSE