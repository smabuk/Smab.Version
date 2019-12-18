using System;
using System.Reflection;

namespace Smab.Version
{
    public class VersionInfo
    {
        public enum VersionType
        {
            FileVersion = 1,
            ProductVersion = 2,
            InformationalVersion = 2,
            AssemblyVersion = 3,

            Location = 6,
            CodeBase = 7,

            Company = 11,
            Configuration = 12,
            Copyright = 13,
            Description = 14,
            Product = 15,
            Title = 16,
            Trademark = 17
        }

        /// <summary>
        /// Assembly represents the type of the assembly to be versioned.
        /// </summary>
        public System.Type AssemblyType { get; set; }

        /// <summary>
        /// Type represents the type of version to be returned
        /// One of FileVersion, ProductVersion or AssemblyVersion
        ///   or any of the extra Custom Attributes
        /// Defaults to ProductVersion (InformationalVersion)
        /// </summary>
        public VersionType Type { get; set; } = VersionType.ProductVersion;

        public string FileVersion => AssemblyType
                        .GetTypeInfo().Assembly
                        .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                        .Version ?? "";

        public string ProductVersion => AssemblyType
                        .GetTypeInfo().Assembly
                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                        .InformationalVersion ?? "";

        public string AssemblyVersion => AssemblyType.Assembly.GetName().Version.ToString();

        public string CodeBase => AssemblyType.Assembly.CodeBase ?? "";

        public string GetVersion(VersionType versionType)
        {
            string versionString = versionType switch
            {
                VersionType.FileVersion => FileVersion,
                VersionType.ProductVersion => ProductVersion,
                VersionType.AssemblyVersion => AssemblyVersion,
                VersionType.CodeBase => CodeBase,
                VersionType.Location => AssemblyType.Assembly.IsDynamic ? "" : (AssemblyType.Assembly.Location ?? ""),
                _ => ""
            };

            if (string.IsNullOrEmpty(versionString))
            {
                foreach (var ca in AssemblyType.Assembly.CustomAttributes)
                {
                    if (versionType.ToString().ToLowerInvariant() == ca.AttributeType.ToString().Replace("System.Reflection.Assembly", "").Replace("Attribute", "").ToLowerInvariant())
                    {
                        versionString = ca.ConstructorArguments[0].Value.ToString();
                    }
                }
            }

            return versionString;
        }

        public VersionInfo(Type assemblyType)
        {
            if (assemblyType == null)
            {
                AssemblyType = GetType();
            } else
            {
                AssemblyType = assemblyType;
            }

            return;
        }
    }
}
