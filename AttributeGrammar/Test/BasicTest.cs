global using System;
global using System.Collections.Generic;

global using Xunit;
global using Xunit.Abstractions;

namespace TestAttributeGrammar
{
    public class BasicTest
    {
        protected readonly ITestOutputHelper output;

        public BasicTest(ITestOutputHelper output) => this.output = output;
    }
}