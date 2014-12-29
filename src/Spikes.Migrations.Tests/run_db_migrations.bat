@rem run_db_migrations.cmd
SET CurrentPath=%CD%
SET ConfigFile=%CurrentPath%\bin\Debug\Spikes.Migrations.Tests.dll.config
SET MigrateExe=..\..\lib\packages\EntityFramework.6.1.1\tools\migrate.exe

%MigrateExe% Spikes.Migrations.BaseData.dll /StartUpDirectory:%CurrentPath%\bin\Debug\ /startUpConfigurationFile:"%ConfigFile%"
%MigrateExe% Spikes.Migrations.Data.dll /StartUpDirectory:%CurrentPath%\bin\Debug\ /startUpConfigurationFile:"%ConfigFile%"
PAUSE