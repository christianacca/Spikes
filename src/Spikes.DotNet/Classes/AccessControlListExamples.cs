using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Eca.Commons.Testing;
using Microsoft.Win32;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class AccessControlListExamples : FileSystemTestsBase
    {
        [Test]
        public void CanDisplayDaclForFile_FromFileInfo()
        {
            FileInfo fileInfo = TempFile.Create();
            FileSecurity security = fileInfo.GetAccessControl();
            AuthorizationRuleCollection rules = security.GetAccessRules(true, true, typeof(NTAccount));

            PrintFileAccessRights(rules);
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
        }


        [Test]
        public void CanDisplayDaclForFile_FromFilePath()
        {
            string filePath = TempFile.Create().FullName;
            FileSecurity security = new FileSecurity(filePath, AccessControlSections.Access);
            //Alternatively:
//            FileSecurity security = File.GetAccessControl(filePath);
            AuthorizationRuleCollection accessRules = security.GetAccessRules(true, true, typeof(NTAccount));

            PrintFileAccessRights(accessRules);
        }


        [Test]
        public void CanDiaplayDaclForRegistryKey()
        {
            RegistrySecurity security = Registry.CurrentUser.GetAccessControl();
            AuthorizationRuleCollection rules = security.GetAccessRules(true, true, typeof(NTAccount));

            PrintRegistryAccessRights(rules);
        }


        [Test]
        public void CanDiaplaySaclForRegistryKey()
        {
            RegistrySecurity security = Registry.CurrentUser.GetAccessControl();
            AuthorizationRuleCollection rules = security.GetAuditRules(true, true, typeof(NTAccount));

            PrintRegistryAuditEvents(rules);
        }



        private void PrintFileAccessRights(string filePath)
        {
            PrintFileAccessRights(File.GetAccessControl(filePath).GetAccessRules(true, true, typeof(NTAccount)));
        }


        [Test]
        public void CanAddUserToExistingFile()
        {
            string existingFile = TempFile.Create().FullName;

            FileSecurity security = new FileSecurity();
            FileSystemAccessRule denyAppend =
                new FileSystemAccessRule("INTERACTIVE", FileSystemRights.AppendData, AccessControlType.Deny);
            security.AddAccessRule(denyAppend);

            File.SetAccessControl(existingFile, security);

            //notice that the file has inherited permissions from its container in addition to the user we're adding
            PrintFileAccessRights(existingFile);
        }


        [Test]
        public void CanCreateFileWithFileAccessPermissions()
        {
            string fileToCreate = Path.Combine(TempDir.RootPath, Path.GetRandomFileName());
            FileSecurity security = new FileSecurity();

            FileSystemAccessRule denyAppend =
                new FileSystemAccessRule(AccountOfCurrentUser, FileSystemRights.AppendData, AccessControlType.Deny);
            security.AddAccessRule(denyAppend);
            security.SetAccessRule(denyAppend);

            FileSystemAccessRule allowDelete =
                new FileSystemAccessRule(AccountOfCurrentUser, FileSystemRights.FullControl, AccessControlType.Allow);
            security.AddAccessRule(allowDelete);
            security.SetAccessRule(allowDelete);

            using (File.Create(fileToCreate, 1, FileOptions.RandomAccess, security))

            //notice that the file has not inherited the file permissions of its container
            PrintFileAccessRights(fileToCreate);
        }


        private NTAccount AccountOfCurrentUser
        {
            get { return new NTAccount(Environment.UserDomainName, Environment.UserName); }
        }


        private void PrintRegistryAccessRights(AuthorizationRuleCollection rules)
        {
            foreach (RegistryAccessRule rule in rules)
            {
                Console.WriteLine("User: {0}", rule.IdentityReference.Value);
                Console.WriteLine("\tRights: {0} - {1}", rule.RegistryRights, rule.AccessControlType);
            }

        }


        private void PrintRegistryAuditEvents(AuthorizationRuleCollection rules)
        {
            foreach (RegistryAuditRule rule in rules)
            {
                Console.WriteLine("User: {0}", rule.IdentityReference.Value);
                Console.WriteLine("\tRights: {0} - {1}", rule.RegistryRights, rule.AuditFlags);
            }

        }


        private void PrintFileAccessRights(AuthorizationRuleCollection rules)
        {
            foreach (FileSystemAccessRule rule in rules)
            {
                Console.WriteLine("User: {0}", rule.IdentityReference.Value);
                Console.WriteLine("\tRights: {0} - {1}", rule.FileSystemRights, rule.AccessControlType);
            }
        }


        public override void ReleaseFileLocksIfAnyHeld() {}
    }
}