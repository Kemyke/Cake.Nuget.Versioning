using Cake.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Cake.Core.Configuration;

namespace Cake.Nuget.Versioning.Test
{
    public class TestContext : ICakeContext
    {
        public IFileSystem FileSystem => throw new NotImplementedException();

        public ICakeEnvironment Environment => throw new NotImplementedException();

        public IGlobber Globber => throw new NotImplementedException();

        public ICakeLog Log => throw new NotImplementedException();

        public ICakeArguments Arguments => throw new NotImplementedException();

        public IProcessRunner ProcessRunner => throw new NotImplementedException();

        public IRegistry Registry => throw new NotImplementedException();

        public IToolLocator Tools => throw new NotImplementedException();

        public ICakeDataResolver Data => throw new NotImplementedException();

        public ICakeConfiguration Configuration => throw new NotImplementedException();
    }
}
