using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Eca.Commons.Testing;
using Eca.Spikes.DotNet.Properties;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class ConfigurationExamples
    {
        #region Test helpers

        private string CreateCloneConfigFileFrom(string dllConfigFilePath)
        {
            string filePathOfClone = Path.Combine(TempDir.RootPath, Path.GetRandomFileName());
            File.Copy(dllConfigFilePath, filePathOfClone);
            return filePathOfClone;
        }


        private SimpleConfigs GetConfigSection(string sectionName)
        {
            return (SimpleConfigs) ConfigurationManager.GetSection(sectionName);
        }

        #endregion


        /// <summary>
        /// Shows a technique whereby a dll can manage its own configuration
        /// file rather than having to rely on the executing application to
        /// provide these settings in the executing app config file
        /// <remarks>
        /// Sometimes is convenient for a dll to define its own application
        /// settings rather than rely on the executing exe to define it for
        /// them. This code shows how its possible for an assembly to reference
        /// it own settings directly. 
        /// <para>
        /// This method of returning a specific configuration file will not
        /// "take advantage" of the way that config files at various levels
        /// (machine, exe, roaming user, user) are merged using the other
        /// methods of retrieving configs
        /// </para>
        /// </remarks>
        /// </summary>
        [Test]
        public void CanRetrieveAppSettingsForThisDll()
        {
            string thisAssemblyName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            Configuration configs = ConfigurationManager.OpenExeConfiguration(thisAssemblyName);


            string expectedConfigFilePath =
                Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, thisAssemblyName) + ".config";
            Assert.That(configs.FilePath, Is.EqualTo(expectedConfigFilePath).IgnoreCase);
            Assert.That(configs.HasFile,
                        Is.True,
                        "Requested config file actually exists - we haven't just been returned configs from machine.config");

            string settingValue = configs.AppSettings.Settings["MyVeryOwnAppSetting"].Value;
            Assert.That(settingValue, Is.EqualTo("SomeValue"));
        }


        /// <summary>
        /// Shows the conventional way of retreiving the configs for the
        /// executing code. Ordinarily this dll (if it had production code
        /// rather than tests) would be called by an exe. It would be the
        /// settings from CallingApp.exe.config file that would be returned, any
        /// config file for this assembly would be ignored.
        /// <remarks>
        /// The configs returned will be the result of merging various levels of
        /// config files. In the case, a level of <see
        /// cref="ConfigurationUserLevel.None"/> is requested and will result in
        /// a merged set of configs from this assembly (or the calling exe if
        /// there was one) and the machine.config
        /// </remarks>
        /// </summary>
        [Test]
        public void NormalWayOfRetreivingConfiguration()
        {
            Configuration configs = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string settingValue = configs.AppSettings.Settings["MyVeryOwnAppSetting"].Value;
            Assert.That(settingValue, Is.EqualTo("SomeValue"));

            //or the shorthand way of accessing the same setting...
            Assert.That(ConfigurationManager.AppSettings["MyVeryOwnAppSetting"], Is.EqualTo(settingValue));
        }


        [Test]
        public void HowToSpecifyTheConfigFilesThatAreMerged()
        {
            TempDir.CreateNew();

            ExeConfigurationFileMap map = new ExeConfigurationFileMap();

            string dllConfigFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            ;
            map.ExeConfigFilename = dllConfigFilePath;

            //we're not setting MachineConfigFilename so we end up merging the default machine.config
            //map.MachineConfigFilename

            string roamingUserConfigFilePath = CreateCloneConfigFileFrom(dllConfigFilePath);
            map.RoamingUserConfigFilename = roamingUserConfigFilePath;

            string localUserConfigFilePath = CreateCloneConfigFileFrom(dllConfigFilePath);
            map.LocalUserConfigFilename = localUserConfigFilePath;

            Configuration userConfigs =
                ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.PerUserRoamingAndLocal);

            Assert.That(userConfigs.HasFile, Is.True, "Requested user config file actually exists");
            Assert.That(userConfigs.FilePath, Is.EqualTo(localUserConfigFilePath));

            Configuration appConfigs = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            Assert.That(appConfigs.HasFile, Is.True, "Requested app config file actually exists");
            Assert.That(appConfigs.FilePath, Is.EqualTo(dllConfigFilePath));
        }


        [Test]
        public void UserSettingsAreWritable()
        {
            //problem with this is that the settings get saved to a version specific subfolder in the Local Settings
            //folder. This is a problem for two reasons:
            //1) requires that app has permissions to write to the folder
            //2) when you increment the version number the previous settings will now be in the wrong folder (ie "lost")
            //for the later problem there is a workaround see here: http://tinyurl.com/346zqk

            Settings.Default.SimpleUserSetting = "ChangedValue";
            Settings.Default.Save();

            Settings.Default.Reload();
            Assert.That(Settings.Default.SimpleUserSetting, Is.EqualTo("ChangedValue"));
        }


        [Test]
        public void CanAddYourOwnValidationCodeToSettingsClass()
        {
            Assert.That(Settings.Default.IsValid, Is.True);
        }


        [Test]
        public void CanLoadSimpleCustomConfigObject()
        {
            SimpleConfigs configSection = GetConfigSection("SimpleConfigsEg1");
            Assert.That(configSection.SomeStringConfig, Is.EqualTo("StringValue"));
            Assert.That(configSection.SomeIntConfig, Is.EqualTo(1));
        }


        [Test]
        public void CanLoadMultipleInstancesOfSimpleCustomConfigObject()
        {
            SimpleConfigs simpleConfigs1 = GetConfigSection("SimpleConfigsEg1");
            Assert.That(simpleConfigs1.SomeStringConfig, Is.EqualTo("StringValue"));
            Assert.That(simpleConfigs1.SomeIntConfig, Is.EqualTo(1));

            SimpleConfigs simpleConfigs2 = GetConfigSection("SimpleConfigsEg2");
            Assert.That(simpleConfigs2.SomeStringConfig, Is.EqualTo("AnotherStringValue"));
            Assert.That(simpleConfigs2.SomeIntConfig, Is.EqualTo(2));
        }


        [Test]
        public void CanUpdateConfigsInConfigFile_UseWithCaution()
        {
            Configuration configs = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            SimpleConfigs simpleConfigs = (SimpleConfigs)configs.GetSection("SimpleConfigs_Scratch");
            simpleConfigs.SomeIntConfig = 22;

            configs.Save();
        }


        [Test]
        public void CustomConfigSectionsCanBeMadeToValidateProperties()
        {
            Configuration configs = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            SimpleConfigs simpleConfigs = (SimpleConfigs) configs.GetSection("SimpleConfigs_Scratch");

            ConfigurationErrorsException ex = Assert.Throws<ConfigurationErrorsException>(delegate {
                simpleConfigs.SomeStringConfig = "This value is to long for the property";
            });
            Assert.That(ex.Message, Text.Contains("The value for the property 'SomeStringConfig' is not valid."));
        }
    }



    public class SimpleConfigs : ConfigurationSection
    {
        private const string SomeIntConfigPropName = "SomeIntConfig";
        private const string SomeStringConfigPropName = "SomeStringConfig";

        [ConfigurationProperty(SomeIntConfigPropName)]
        [IntegerValidator]
        public int SomeIntConfig
        {
            get { return (int) this[SomeIntConfigPropName]; }
            set { this[SomeIntConfigPropName] = value; }
        }

        [ConfigurationProperty(SomeStringConfigPropName)]
        [StringValidator(MaxLength = 20)]
        public string SomeStringConfig
        {
            get { return (string) this[SomeStringConfigPropName]; }
            set { this[SomeStringConfigPropName] = value; }
        }
    }
}