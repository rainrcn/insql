﻿using Insql.Resolvers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Insql.Providers.DirectoryXml
{
    public class DirectoryDescriptorProvider : IInsqlDescriptorProvider
    {
        private readonly IOptions<DirectoryDescriptorOptions> options;

        public DirectoryDescriptorProvider(IOptions<DirectoryDescriptorOptions> options)
        {
            this.options = options;
        }

        public IEnumerable<InsqlDescriptor> GetDescriptors()
        {
            var optionsValue = this.options.Value;

            if (!optionsValue.Enabled)
            {
                return new List<InsqlDescriptor>();
            }

            if (string.IsNullOrWhiteSpace(optionsValue.Matches))
            {
                throw new ArgumentNullException(nameof(optionsValue.Matches), $"{nameof(DirectoryDescriptorOptions)} `Matches` is null!");
            }

            GlobMatcher globMatcher = new GlobMatcher(optionsValue.Matches);

            var directory = optionsValue.Directory;

            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = AppDomain.CurrentDomain.BaseDirectory;
            }

            if (!Path.IsPathRooted(directory))
            {
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
            }

            var fileNames = Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories).Where(path => globMatcher.IsMatch(path)).ToList();

            return fileNames.Select(path =>
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                using (var stream = File.OpenRead(path))
                {
                    return InsqlDescriptorXmlParser.Instance.ParseDescriptor(stream, optionsValue.Namespace);
                }
            }).Where(item => item != null).ToList();
        }
    }
}
