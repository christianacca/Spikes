@rem run_db_migrations.cmd
SET CurrentPath=%CD%
SET ConfigFile=%CurrentPath%\bin\Debug\Spikes.Migrations.Tests.dll.config
SET MigrateExe=..\..\lib\packages\EntityFramework.6.1.1\tools\migrate.exe

%MigrateExe% Spikes.Migrations.BaseDataMigrations.dll /startUpDirectory:%CurrentPath%\bin\Debug\ /startUpConfigurationFile:"%ConfigFile%" /connectionStringName:SpikesMigrationsDb-Compare
%MigrateExe% Spikes.Migrations.DataMigrations.dll Configuration /startUpDirectory:%CurrentPath%\bin\Debug\ /startUpConfigurationFile:"%ConfigFile%" /connectionStringName:SpikesMigrationsDb-Compare
PAUSE